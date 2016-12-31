using System;
using System.Collections.Generic;
using System.Linq;
using CrmCommandLineHelpersCore;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Client.Services;
using Microsoft.Xrm.Sdk.Query;

namespace WebResourceHelper
{
    class WebResourceQueryingUtilities
    {
        private readonly OrganizationService orgService;

        public WebResourceQueryingUtilities(CrmConnector crmConnector)
        {
            orgService = crmConnector.GetConnection().OrgService;
        }

        public IEnumerable<WebResource> RetrieveWebResourcesForSolution(Guid solutionId)
        {
            var query = new QueryExpression(WebResource.EntityLogicalName) { ColumnSet = new ColumnSet("webresourceid", "webresourcetype", "silverlightversion", "displayname", "name", "description", "content") };

            var link = query.AddLink(SolutionComponent.EntityLogicalName, "webresourceid", "objectid", JoinOperator.Inner);
            link.EntityAlias = "sc";

            query.Criteria = new FilterExpression();
            query.Criteria.AddCondition("sc", "solutionid", ConditionOperator.Equal, solutionId);
            query.Criteria.AddCondition("ismanaged", ConditionOperator.Equal, false);
            query.Criteria.AddCondition("iscustomizable", ConditionOperator.Equal, true);

            var response = (RetrieveUnpublishedMultipleResponse)orgService.Execute(new RetrieveUnpublishedMultipleRequest { Query = query });

            return response.EntityCollection.Entities.Cast<WebResource>();
        }

        public void AddWebResource(WebResource webResource, Solution solution)
        {
            var id = orgService.Create(webResource);
            if (solution.UniqueName != "Default")
            {
                orgService.Execute(new AddSolutionComponentRequest
                                   {
                                       SolutionUniqueName = solution.UniqueName,
                                       ComponentId = id,
                                       ComponentType = (int)componenttype.WebResource
                                   });
            }
        }

        public void UpdateWebResourceContent(WebResource webResource)
        {
            orgService.Update(new WebResource
                              {
                                  Id = webResource.Id,
                                  Content = webResource.Content
                              });
        }

        public void DeleteWebResource(WebResource webResource)
        {
            orgService.Delete(webResource.LogicalName, webResource.Id);
        }

        public void PublishAll()
        {
            orgService.Execute(new PublishAllXmlRequest());
        }
    }
}
