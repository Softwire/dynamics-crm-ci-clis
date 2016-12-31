using CrmCommandLineHelpersCore.CommandLine;
using JetBrains.Annotations;

namespace CrmCommandLineHelpersCore
{
    public class CoreOptions
    {
        public bool ShouldShowHelp;
        [CanBeNull]
        public string ConnectionString;
        [CanBeNull]
        public string SolutionName;
        public LogLevel LogLevel;
        public bool Force;
        public bool StatusOnly;
        public bool Automate;
    }
}