using System.Collections.Generic;
using CrmCommandLineHelpersCore.CommandLine.ParameterDefinition;

namespace CrmCommandLineHelpersCore.CommandLine.Parameters
{
    public class ShouldShowHelp : BooleanParameter
    {
        protected override IEnumerable<string> Tags => new [] { "h", "help" };
        public override string Description => "Shows the help message.";
    }
}