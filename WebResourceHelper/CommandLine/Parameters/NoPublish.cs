using CrmCommandLineHelpersCore.CommandLine.ParameterDefinition;

namespace WebResourceHelper.CommandLine.Parameters
{
    public class NoPublish : BooleanParameter
    {
        protected override string Tag => "nopublish";
        public override string Description => "Using this parameter means that a publish all isn't performed after upload.";
    }
}