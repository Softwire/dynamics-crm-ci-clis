using System.Collections.Generic;
using CrmCommandLineHelpersCore.CommandLine.ParameterDefinition;
using CrmCommandLineHelpersCore.Exceptions;

namespace PluginWorkflowHelper.CommandLine.Parameters
{
    public class SourceType : ParameterBase<PluginAssemblySourceType?>
    {
        public override IEnumerable<CommandLineTag> CommandLineTags => new[] { new CommandLineTag { Name = "sourcetype", TakesValue = true } };
        public override string Description => "Allowed values: database, disk or normal. Specifying this parameter ensures the assembly is stored in that way. If no value is given, updated assemblies keep the same location, new assemblies are uploaded to the database.";

        public override PluginAssemblySourceType? ParseValue(bool parameterExists, string value)
        {
            if (parameterExists || value == null) return null;
            switch (value.ToLower())
            {
                case "database":
                    return PluginAssemblySourceType.Database;
                case "disk":
                case "ondisk":
                case "on-disk":
                    return PluginAssemblySourceType.Disk;
                case "normal":
                    return PluginAssemblySourceType.Normal;
                default:
                    throw new ParameterException("the only valid values for this parameter are database, disk or normal", this);
            }
        }
    }
}