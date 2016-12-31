using System.Collections.Generic;
using System.Linq;

namespace CrmCommandLineHelpersCore.CommandLine.ParameterDefinition
{
    public abstract class StringParameter : ParameterBase<string>
    {
        public sealed override IEnumerable<CommandLineTag> CommandLineTags => Tags.Select(t => new CommandLineTag { Name = t, TakesValue = true });
        protected virtual string Default => null;

        public override string ParseValue(bool parameterExists, string value)
        {
            return parameterExists ? value : Default;
        }
    }
}