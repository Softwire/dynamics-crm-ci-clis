using System.Collections.Generic;
using CrmCommandLineHelpersCore.CommandLine.ParameterDefinition;

namespace CrmCommandLineHelpersCore.CommandLine.Parameters
{
    public class ConnectionString : StringParameter
    {
        protected override IEnumerable<string> Tags => new [] { "crmconnectionstring", "connectionstring" };
        public override string Description => "See: https://technet.microsoft.com/en-us/library/gg695810.aspx. Example use - /crmconnectionstring:'Url=http://server/org; Domain=dom; Username=user; Password=pass'";
        public override bool Required => true;
    }
}