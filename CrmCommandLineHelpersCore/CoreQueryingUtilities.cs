using System.Collections.Generic;
using System.Linq;
using CrmCommandLineHelpersCore.DataAccess;
using Microsoft.Xrm.Sdk.Client;

namespace CrmCommandLineHelpersCore
{
    class CoreQueryingUtilities
    {
        private readonly OrganizationServiceContext orgContext;

        public CoreQueryingUtilities(CrmConnector crmConnector)
        {
            orgContext = crmConnector.GetConnection().OrgServiceContext;
        }

        public IEnumerable<SolutionWithPublisher> GetUnmanagedSolutions()
        {
            var solutions = from sol in orgContext.CreateQuery<Solution>()
                            join pub in orgContext.CreateQuery<Publisher>()
                                on sol.PublisherId.Id equals pub.Id
                            where sol.IsManaged == false
                            where sol.UniqueName != "Active"
                            where sol.UniqueName != "Basic"
                            select new SolutionWithPublisher
                            {
                                Publisher = pub,
                                Solution = sol
                            };

            return solutions.AsEnumerable();
        }
    }
}
