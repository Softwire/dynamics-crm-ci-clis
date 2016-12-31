using CrmCommandLineHelpersCore.CommandLine.ParameterDefinition;

namespace WebResourceHelper.CommandLine.Parameters
{
    public class NoUpdates : BooleanParameter
    {
        protected override string Tag => "noupdates";
        public override string Description => "Using this parameter means that local web resources are ignored if there is currently a web resource with the same name in the solution on the CRM.";
    }
}