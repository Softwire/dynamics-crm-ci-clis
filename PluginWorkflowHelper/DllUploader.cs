using System;
using System.Collections.Generic;
using System.Linq;
using CrmCommandLineHelpersCore;
using CrmCommandLineHelpersCore.CommandLine;

namespace PluginWorkflowHelper
{
    internal class DllUploader : ProgramFlowBase
    {
        private readonly PluginWorkflowHelperOptions options;
        private readonly DllReader dllReader;
        private readonly PluginAssemblyQueryingUtilities pluginAssemblyQueryingUtilities;

        private PluginAssemblyAndTypes localPluginAssemblyAndTypes;
        private PluginAssembly preExistingPluginAssembly;

        private readonly List<string> issues = new List<string>();

        public DllUploader(PluginWorkflowHelperOptions options) : base(options)
        {
            this.options = options;
            dllReader = new DllReader(options);
            pluginAssemblyQueryingUtilities = new PluginAssemblyQueryingUtilities(CrmConnector);
        }

        protected override void PreStatusProcessing()
        {
            localPluginAssemblyAndTypes = dllReader.ReadDll();
            var editablePluginAssembliesInSolution = pluginAssemblyQueryingUtilities.RetrievePluginAssembliesForSolution(SolutionWithPublisher.Solution.Id);
            preExistingPluginAssembly = FindMatchingAssembly(editablePluginAssembliesInSolution);
            if (preExistingPluginAssembly == null)
            {
                RunNewAssemblyChecks();
            }
            else
            {
                RunUpdatedAssemblyChecks();
            }
        }

        private PluginAssembly FindMatchingAssembly(IEnumerable<PluginAssembly> pluginAssemblies)
        {
            return pluginAssemblies.FirstOrDefault(pa => pa.Name == localPluginAssemblyAndTypes.PluginAssembly.Name);
        }

        private void RunNewAssemblyChecks() {}

        private void RunUpdatedAssemblyChecks()
        {
            var crmAssemblyVersion = localPluginAssemblyAndTypes.Assembly.GetName().Version;
            if (preExistingPluginAssembly.Major != crmAssemblyVersion.Major || preExistingPluginAssembly.Minor != crmAssemblyVersion.Minor)
            {
                issues.Add($"The local assembly is at version {crmAssemblyVersion}, but the assembly on the CRM has version starting {preExistingPluginAssembly.Major}.{preExistingPluginAssembly.Minor}. A change of assembly major or minor version requires re-registration with the plugin registration tool.");
            }
            if (options.IsolationMode.HasValue && options.IsolationMode.Value != (PluginAssemblyIsolationMode)preExistingPluginAssembly.IsolationMode.Value)
            {
                issues.Add($"The assembly was specified to be uploaded {IsolationModeToName(options.IsolationMode.Value)}, but the assembly on the CRM is {IsolationModeToName((PluginAssemblyIsolationMode)preExistingPluginAssembly.IsolationMode.Value)}.");
            }
            if (options.SourceType.HasValue && options.SourceType.Value != (PluginAssemblySourceType)preExistingPluginAssembly.SourceType.Value)
            {
                issues.Add($"The assembly was specified to be stored {SourceTypeToName(options.SourceType.Value)}, but the assembly on the CRM is stored {SourceTypeToName((PluginAssemblySourceType)preExistingPluginAssembly.SourceType.Value)}.");
            }
        }

        private static string IsolationModeToName(PluginAssemblyIsolationMode mode)
        {
            switch (mode)
            {
                case PluginAssemblyIsolationMode.None:
                    return "without isolation";
                case PluginAssemblyIsolationMode.Sandbox:
                    return "in sandboxed mode";
                default:
                    return "in an unknown mode";
            }
        }

        private static string SourceTypeToName(PluginAssemblySourceType sourceType)
        {
            switch (sourceType)
            {
                case PluginAssemblySourceType.Database:
                    return "in the database";
                case PluginAssemblySourceType.Disk:
                    return "on the disk";
                case PluginAssemblySourceType.Normal:
                    return "with a normal source type";
                default:
                    return "with an unknown source type";
            }
        }

        protected override void PrintStatus()
        {
            ConsoleHelper.WriteLine("===== STATUS =====", logLevel: LogLevel.Warn);
            ConsoleHelper.WriteLine(logLevel: LogLevel.Warn);
            if (preExistingPluginAssembly == null)
            {
                PrintBulletPoint($"No assembly matching the name {localPluginAssemblyAndTypes.Assembly.GetName().Name} has been found in the solution", logLevel: LogLevel.Warn);
                if (issues.Count > 0)
                {
                    PrintBulletPoint("But, it cannot be uploaded due to the following issue/s:", LogLevel.Warn);
                    issues.ForEach(i => ConsoleHelper.WriteLine(i, indent: 2, wrappedIndent: 3, logLevel: LogLevel.Warn));
                }
            }
            else
            {
                PrintBulletPoint($"An assembly matching the name {localPluginAssemblyAndTypes.Assembly.GetName().Name} has been found in the solution", logLevel: LogLevel.Warn);
                if (issues.Count > 0)
                {
                    PrintBulletPoint("But, it cannot be updated due to the following issue/s:", LogLevel.Warn);
                    issues.ForEach(i => ConsoleHelper.WriteLine(i, indent: 2, wrappedIndent: 3, logLevel: LogLevel.Warn));
                }
            }
            ConsoleHelper.WriteLine(logLevel: LogLevel.Warn);
            ConsoleHelper.WriteLine("=== STATUS END ===", logLevel: LogLevel.Warn);
            ConsoleHelper.WriteLine(logLevel: LogLevel.Warn);
        }

        protected override int? PostStatusExitCode()
        {
            return issues.Any() && !options.Force ? 1 : base.PostStatusExitCode();
        }

        protected override void CreateActionCommitments()
        {
            if (preExistingPluginAssembly == null)
            {
                ActionCommitments.Add(new ActionCommitment
                                      {
                                          Action = UploadNewAssembly, Description = $"A new assembly {localPluginAssemblyAndTypes.Assembly.GetName().Name} will be uploaded", TextColor = ConsoleColor.Yellow
                                      });
//                ActionCommitments.AddRange(localPluginAssemblyAndTypes.Types.PluginTypes.Select(t => new ActionCommitment
//                                                                                                     {
//                                                                                                         Action = UploadNewAssembly,
//                                                                                                         Description = $"A new assembly {localPluginAssemblyAndTypes.Assembly.GetName().Name} will be uploaded",
//                                                                                                       TextColor = ConsoleColor.Yellow
//                                                                                                     }));
            }
            else
            {
                ActionCommitments.Add(new ActionCommitment
                                      {
                                          Action = UpdateExistingAssembly, Description = $"The assembly {localPluginAssemblyAndTypes.Assembly.GetName().Name} will be updated", TextColor = ConsoleColor.Green
                                      });
            }
            base.CreateActionCommitments();
        }

        private int UploadNewAssembly()
        {
            pluginAssemblyQueryingUtilities.AddPluginAssembly(localPluginAssemblyAndTypes, SolutionWithPublisher.Solution);
            return 0;
        }

        private int UpdateExistingAssembly()
        {
            preExistingPluginAssembly.Content = localPluginAssemblyAndTypes.PluginAssembly.Content;
            pluginAssemblyQueryingUtilities.UpdatePluginAssemblyContent(preExistingPluginAssembly);
            return 0;
        }
    }
}