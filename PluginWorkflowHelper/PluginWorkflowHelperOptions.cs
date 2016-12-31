using CrmCommandLineHelpersCore;

namespace PluginWorkflowHelper
{
    class PluginWorkflowHelperOptions : CoreOptions
    {
        public string DllPath;
        public PluginAssemblyIsolationMode? IsolationMode;
        public PluginAssemblySourceType? SourceType;
    }
}