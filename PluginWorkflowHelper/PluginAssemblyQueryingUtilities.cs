using System;
using System.Collections.Generic;
using System.Linq;
using CrmCommandLineHelpersCore;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Client.Services;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

namespace PluginWorkflowHelper
{
    class PluginAssemblyQueryingUtilities
    {
        private readonly OrganizationService orgService;

        public PluginAssemblyQueryingUtilities(CrmConnector crmConnector)
        {
            orgService = crmConnector.GetConnection().OrgService;
        }

        public IEnumerable<PluginAssembly> RetrievePluginAssembliesForSolution(Guid solutionId)
        {
            var query = new QueryExpression(PluginAssembly.EntityLogicalName) { ColumnSet = new ColumnSet("pluginassemblyid", "name", "isolationmode", "major", "minor", "sourcetype", "version") };

            var link = query.AddLink(SolutionComponent.EntityLogicalName, "pluginassemblyid", "objectid", JoinOperator.Inner);
            link.EntityAlias = "sc";

            query.Criteria = new FilterExpression();
            query.Criteria.AddCondition("sc", "solutionid", ConditionOperator.Equal, solutionId);
            query.Criteria.AddCondition("ismanaged", ConditionOperator.Equal, false);

            var response = (RetrieveMultipleResponse)orgService.Execute(new RetrieveMultipleRequest { Query = query });

            return response.EntityCollection.Entities.Cast<PluginAssembly>();
        }

        public void AddPluginAssembly(PluginAssemblyAndTypes pluginAssemblyAndTypes, Solution solution)
        {
            var id = orgService.Create(pluginAssemblyAndTypes.PluginAssembly);
            pluginAssemblyAndTypes.PluginAssembly.Id = id;
            AddComponentToSolution(solution, id, componenttype.PluginAssembly);
            UpsertAssemblyTypes(pluginAssemblyAndTypes, solution);
        }

        private void AddComponentToSolution(Solution solution, Guid componentId, componenttype componentTypeCode)
        {
            if (solution.UniqueName != "Default")
            {
                orgService.Execute(new AddSolutionComponentRequest
                {
                    SolutionUniqueName = solution.UniqueName,
                    ComponentId = componentId,
                    ComponentType = (int)componentTypeCode
                });
            }
        }

        public void UpsertAssemblyTypes(PluginAssemblyAndTypes pluginAssemblyAndTypes, Solution solution)
        {
            pluginAssemblyAndTypes.Types.PluginTypes.ForEach(pt => UpsertPluginType(pt, pluginAssemblyAndTypes.PluginAssembly, solution));
            pluginAssemblyAndTypes.Types.CustomWorkflowTypes.ForEach(cwat => UpsertCustomWorkflowActivityType(cwat, pluginAssemblyAndTypes.PluginAssembly, solution));
        }

        public void UpsertPluginType(Type type, PluginAssembly pluginAssembly, Solution solution)
        {
            var id = orgService.Create(new PluginType
            {
                PluginAssemblyId = new EntityReference(pluginAssembly.LogicalName, pluginAssembly.Id),
                TypeName = type.FullName,
                FriendlyName = type.GUID.ToString() // The Plugin Registration Tool uses this, but default dynamics plugin types do have friendly names here. Using the GUID to be safe though.
            });
            AddComponentToSolution(solution, id, componenttype.PluginType);
        }

        public void UpsertCustomWorkflowActivityType(Type type, PluginAssembly pluginAssembly, Solution solution)
        {
            var id = orgService.Create(new PluginType
            {
                PluginAssemblyId = new EntityReference(pluginAssembly.LogicalName, pluginAssembly.Id),
                TypeName = type.FullName,
                FriendlyName = type.GUID.ToString(), // The Plugin Registration Tool uses this, but default dynamics plugin types do have friendly names here. Using the GUID to be safe though.
                WorkflowActivityGroupName = $"{pluginAssembly.Name} ({pluginAssembly.Version})"
            });
            AddComponentToSolution(solution, id, componenttype.PluginType);
        }

        public void UpdatePluginAssemblyContent(PluginAssembly pluginAssembly)
        {
            orgService.Update(new PluginAssembly
                              {
                                  Id = pluginAssembly.Id,
                                  Content = pluginAssembly.Content
                              });
        }

        public void PublishAll()
        {
            orgService.Execute(new PublishAllXmlRequest());
        }
    }
}
