using Microsoft.Xrm.Client.Services;
using Microsoft.Xrm.Sdk.Client;

namespace CrmCommandLineHelpersCore
{
    public class OpenCrmConnection
    {
        public OrganizationService OrgService;
        public OrganizationServiceContext OrgServiceContext;
    }
}