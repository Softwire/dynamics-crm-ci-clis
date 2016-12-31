using System.Collections.Generic;
using System.Linq;
using CrmCommandLineHelpersCore.Exceptions;

namespace CrmCommandLineHelpersCore.CommandLine.ParameterDefinition
{
    public abstract class IntParameter : ParameterBase<int>
    {
        public sealed override IEnumerable<CommandLineTag> CommandLineTags => Tags.Select(t => new CommandLineTag { Name = t, TakesValue = true });
        public virtual int Default => 0;
        public virtual bool DisallowNegatives => false;
        public virtual bool DisallowZero => false;

        public override int ParseValue(bool parameterExists, string value)
        {
            if (!parameterExists) return Default;
            int parsedInt;
            if (!int.TryParse(value, out parsedInt)) throw new ParameterException("the provided value is not an integer", this);
            if (DisallowNegatives && parsedInt < 0) throw new ParameterException("the provided value is < 0", this);
            if (DisallowZero && parsedInt == 0) throw new ParameterException("the provided value is 0", this);
            return parsedInt;
        }
    }
}