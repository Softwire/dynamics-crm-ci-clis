using CrmCommandLineHelpersCore.CommandLine.ParameterDefinition;

namespace WebResourceHelper.CommandLine.Parameters
{
    public class PublishAllOnly : BooleanParameter
    {
        protected override string Tag => "publishallonly";
        public override string Description => "Using this parameter just runs a publish all command. A valid rootpath and solution still have to be given, but these are ignored.";
    }
}