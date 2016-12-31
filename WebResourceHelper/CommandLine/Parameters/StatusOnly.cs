using CrmCommandLineHelpersCore.CommandLine.ParameterDefinition;

namespace WebResourceHelper.CommandLine.Parameters
{
    public class StatusOnly : BooleanParameter
    {
        protected override string Tag => "status";
        public override string Description => "Using this parameter only gives a diff against the CRM solution, but will not run or ask permission to run any actions.";
    }
}