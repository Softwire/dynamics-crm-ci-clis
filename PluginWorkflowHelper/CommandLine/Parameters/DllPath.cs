using System.Collections.Generic;
using CrmCommandLineHelpersCore.CommandLine.ParameterDefinition;

namespace PluginWorkflowHelper.CommandLine.Parameters
{
    public class DllPath : StringParameter
    {
        protected override IEnumerable<string> Tags => new [] { "dllpath", "assemblypath", "pluginassemblypath" };
        public override string Description => "Provide the path (relative or absolute) to the assembly to load. Example use - /dllpath:\"../../DotNet/bin/Release/CrmPlugins.dll\"";
        public override bool Required => true;
    }
}