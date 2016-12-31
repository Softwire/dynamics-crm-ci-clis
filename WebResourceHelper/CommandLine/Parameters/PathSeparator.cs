using CrmCommandLineHelpersCore.CommandLine.ParameterDefinition;

namespace WebResourceHelper.CommandLine.Parameters
{
    public class PathSeparator : StringParameter
    {
        protected override string Tag => "pathseparator";
        public override string Description => @"The path separator. Defaults to /. Example use - /pathseparator:\";
        protected override string Default => "/";
    }
}