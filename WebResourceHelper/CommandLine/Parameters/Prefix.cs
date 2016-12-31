using CrmCommandLineHelpersCore.CommandLine.ParameterDefinition;

namespace WebResourceHelper.CommandLine.Parameters
{
    public class Prefix : StringParameter
    {
        protected override string Tag => "prefix";
        public override string Description => @"The prefix after the publisher prefix, but before the relative paths are used. Defaults to the path separator (which defaults to /). Example use - /prefix:/projecta/ which would result in a file at widgetb/index.htm being uploaded (say) as new_/projecta/widgetb/index.htm. A blank string may also be used by writing: /prefix:";
    }
}