using System.Collections.Generic;
using CrmCommandLineHelpersCore.CommandLine.ParameterDefinition;
using CrmCommandLineHelpersCore.Exceptions;

namespace PluginWorkflowHelper.CommandLine.Parameters
{
    public class IsolationMode : ParameterBase<PluginAssemblyIsolationMode?>
    {
        public override IEnumerable<CommandLineTag> CommandLineTags => new [] { new CommandLineTag { Name = "isolationmode", TakesValue = true } };
        public override string Description => "Allowed values: sandbox or none. Specifying this parameter ensures the assembly is registered in that way. If no value is given, updated assemblies keep the same isolation mode, new assemblies are uploaded sandboxed.";

        public override PluginAssemblyIsolationMode? ParseValue(bool parameterExists, string value)
        {
            if (parameterExists || value == null) return null;
            switch (value.ToLower())
            {
                case "sandbox":
                    return PluginAssemblyIsolationMode.Sandbox;
                case "none":
                    return PluginAssemblyIsolationMode.None;
                default:
                    throw new ParameterException("the only valid values for this parameter are sandbox or none", this);
            }
        }
    }
}