using System.Collections.Generic;
using System.Linq;
using CrmCommandLineHelpersCore.Exceptions;

namespace CrmCommandLineHelpersCore.CommandLine.ParameterDefinition
{
    public abstract class BooleanParameter : ParameterBase<bool>
    {
        public sealed override IEnumerable<CommandLineTag> CommandLineTags => Tags.SelectMany(t => new[] { new CommandLineTag { Name = t, TakesValue = false }, new CommandLineTag { Name = t, TakesValue = true } });

        public override bool ParseValue(bool parameterExists, string value)
        {
            // ReSharper disable ConvertIfStatementToSwitchStatement
            if (!parameterExists) return false;
            if (value == null) return true; // Present as /name
            if (new [] { "true", "t", "1" }.Contains(value.ToLower())) return true; // Present as eg /name:true
            if (new [] { "false", "f", "0" }.Contains(value.ToLower())) return false; // Present as eg /name:true
            throw new ParameterException("invalid value", this);
            // ReSharper restore ConvertIfStatementToSwitchStatement
        }
    }
}