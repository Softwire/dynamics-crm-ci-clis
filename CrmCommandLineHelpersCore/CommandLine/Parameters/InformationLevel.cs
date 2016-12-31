using System.Collections.Generic;
using CrmCommandLineHelpersCore.CommandLine.ParameterDefinition;

namespace CrmCommandLineHelpersCore.CommandLine.Parameters
{
    public class InformationLevel : ParameterBase<LogLevel>
    {
        public override IEnumerable<CommandLineTag> CommandLineTags => new [] { new CommandLineTag { Name = "loglevel", TakesValue = true } };
        public override string Description => "How verbose the output should be. Allowed values: trace, debug, info, warn, error. Default: debug";

        public override LogLevel ParseValue(bool parameterExists, string value)
        {
            if (!parameterExists || value == null) return LogLevel.Debug;
            var level = value.ToLower();
            switch (level)
            {
                case "trace":
                    return LogLevel.Trace;
                case "debug":
                    return LogLevel.Debug;
                case "info":
                    return LogLevel.Info;
                case "warn":
                    return LogLevel.Warn;
                case "error":
                    return LogLevel.Error;
                default:
                    return LogLevel.Debug;
            }
        }
    }
}