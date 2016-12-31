using System.Collections.Generic;
using CrmCommandLineHelpersCore.CommandLine.ParameterDefinition;

namespace CrmCommandLineHelpersCore.CommandLine.Parameters
{
    public class Automate : BooleanParameter
    {
        protected override IEnumerable<string> Tags => new [] { "automate", "a" };
        public override string Description => "Using this causes the confirmation dialog to be bypassed. Actions are run only if all resources pass validation checks - otherwise the program exits with an error. This is useful in CI situations.";
    }
}