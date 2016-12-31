using System;
using CrmCommandLineHelpersCore.CommandLine;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Client;
using Microsoft.Xrm.Client.Messages;
using Microsoft.Xrm.Client.Services;
using Microsoft.Xrm.Sdk.Client;

namespace CrmCommandLineHelpersCore
{
    public class CrmConnector
    {
        private readonly string crmConnectionString;
        private OpenCrmConnection openCrmConnection;

        public CrmConnector(CoreOptions options)
        {
            crmConnectionString = options.ConnectionString;
        }

        public OpenCrmConnection GetConnection()
        {
            if (openCrmConnection != null)
            {
                return openCrmConnection;
            }

            ConsoleHelper.WriteLine();
            ConsoleHelper.Write("Connecting to the CRM...");

            var crmConnection = CrmConnection.Parse(crmConnectionString);
            crmConnection.Timeout = TimeSpan.FromMinutes(10); // Defaults to 2 minutes. This can timeout large pluginassembly uploads over slow connections.

            var orgService = new OrganizationService(crmConnection);

            openCrmConnection = new OpenCrmConnection
            {
                OrgService = orgService,
                OrgServiceContext = new OrganizationServiceContext(orgService)
            };

            TestConnection();

            ConsoleHelper.WriteLine("Done!", indent: 1, foregroundColor: ConsoleColor.Green);
            ConsoleHelper.WriteLine();

            return openCrmConnection;
        }

        private void TestConnection()
        {
            openCrmConnection.OrgServiceContext.WhoAmI<WhoAmIResponse>();
        }
    }
}
