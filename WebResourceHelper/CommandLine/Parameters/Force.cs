using System.Collections.Generic;
using CrmCommandLineHelpersCore.CommandLine.ParameterDefinition;

namespace WebResourceHelper.CommandLine.Parameters
{
    public class Force : BooleanParameter
    {
        protected override IEnumerable<string> Tags => new [] { "force", "f" };
        public override string Description => "Using this bypasses the confirmation dialog, and all valid actions will be run. Invalid resources will be ignored.";
    }
}