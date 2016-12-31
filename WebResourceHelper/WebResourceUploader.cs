using System;
using System.Collections.Generic;
using System.Linq;
using CrmCommandLineHelpersCore;
using CrmCommandLineHelpersCore.CommandLine;

namespace WebResourceHelper
{
    class WebResourceUploader : ProgramFlowBase
    {
        private readonly WebResourceQueryingUtilities webResourceQueryingUtils;
        private readonly FileDiscoverer fileDiscoverer;
        private readonly WebResourceConverter resourceConverter;
        private readonly WebResourceHelperOptions options;

        private List<WebResource> newWebResources;
        private List<WebResource> changedWebResources;
        private List<WebResource> unchangedWebResources;
        private List<WebResource> missingWebResources;
        private List<InvalidWebResource> invalidWebResources;

        public WebResourceUploader(WebResourceHelperOptions options) : base(options)
        {
            this.options = options;
            webResourceQueryingUtils = new WebResourceQueryingUtilities(CrmConnector);
            resourceConverter = new WebResourceConverter(options);
            fileDiscoverer = new FileDiscoverer(options);
        }

        protected override void PreStatusProcessing()
        {
            if (options.PublishAllOnly) return;
            var localWebResources = fileDiscoverer.DiscoverFiles().Select(w => resourceConverter.FileInfoToWebResource(w, SolutionWithPublisher.Publisher)).Where(w => w != null).ToList();
            var crmWebResources = webResourceQueryingUtils.RetrieveWebResourcesForSolution(SolutionWithPublisher.Solution.Id).ToList();

            var partitionedWebResources = new WebResourcePartitioner(options, crmWebResources, localWebResources).Partition();
            newWebResources = partitionedWebResources.NewWebResources;
            changedWebResources = partitionedWebResources.ChangedWebResources;
            unchangedWebResources = partitionedWebResources.UnchangedWebResources;
            missingWebResources = partitionedWebResources.MissingWebResources;
            invalidWebResources = partitionedWebResources.InvalidWebResources;
        }

        protected override void PrintStatus()
        {
            if (options.PublishAllOnly) return;
            ConsoleHelper.WriteLine("===== STATUS =====", logLevel: LogLevel.Warn);
            ConsoleHelper.WriteLine(logLevel: LogLevel.Warn);
            PrintResources(newWebResources, "new", titleColor: ConsoleColor.Yellow);
            PrintResources(changedWebResources, "changed", titleColor: ConsoleColor.Green);
            PrintResources(missingWebResources, "missing", existsLogLevel: LogLevel.Warn, noneLogLevel: LogLevel.Trace, itemLogLevel: LogLevel.Info, titleColor: ConsoleColor.Red);
            PrintResources(unchangedWebResources, "unchanged", existsLogLevel: LogLevel.Debug, noneLogLevel: LogLevel.Trace, itemLogLevel: LogLevel.Trace, titleColor: ConsoleColor.White);
            PrintInvalidResources(existsLogLevel: LogLevel.Error, noneLogLevel: LogLevel.Trace, itemLogLevel: LogLevel.Error, titleColor: ConsoleColor.DarkRed);
            ConsoleHelper.WriteLine(logLevel: LogLevel.Warn);
            ConsoleHelper.WriteLine("=== STATUS END ===", logLevel: LogLevel.Warn);
            ConsoleHelper.WriteLine(logLevel: LogLevel.Warn);
        }

        private static void PrintResources(List<WebResource> webResources, string type, LogLevel existsLogLevel = LogLevel.Info, LogLevel noneLogLevel = LogLevel.Debug, LogLevel itemLogLevel = LogLevel.Info, ConsoleColor? titleColor = null)
        {
            if (!webResources.Any())
            {
                PrintBulletPoint($"No {type} web resources", noneLogLevel, ConsoleColor.White);
                return;
            }
            PrintBulletPoint($"{webResources.Count} {type} web {PluralizeResource(webResources)}", existsLogLevel, titleColor);
            webResources.ForEach(wr => ConsoleHelper.WriteLine(wr.Name, indent: 2, wrappedIndent: 3, logLevel: itemLogLevel));
        }

        private static string PluralizeResource(IReadOnlyCollection<WebResource> webResources)
        {
            return webResources.Count == 1 ? "resource" : "resources";
        }

        private void PrintInvalidResources(LogLevel existsLogLevel = LogLevel.Info, LogLevel noneLogLevel = LogLevel.Debug, LogLevel itemLogLevel = LogLevel.Info, ConsoleColor? titleColor = null)
        {
            if (!invalidWebResources.Any())
            {
                PrintBulletPoint("No invalid web resource files were found", noneLogLevel, ConsoleColor.White);
                return;
            }
            PrintBulletPoint($"{invalidWebResources.Count} invalid web {PluralizeResource(invalidWebResources)} found. {(invalidWebResources.Count == 1 ? "It" : "they")} will be ignored.", existsLogLevel, titleColor);
            invalidWebResources.ForEach(wr =>
            {
                ConsoleHelper.WriteLine($"{wr.Name}", indent: 2, wrappedIndent: 3, logLevel: itemLogLevel);
                wr.ReasonsForInvalidity.ForEach(r => ConsoleHelper.WriteLine(r, indent: 4, logLevel: itemLogLevel, foregroundColor: titleColor));
            });
        }

        protected override int? PostStatusExitCode()
        {
            if (options.PublishAllOnly || !options.Automate || !invalidWebResources.Any())
            {
                return null;
            }
            ConsoleHelper.WriteLine("ERROR: Invalid web resources detected. Program ending.", logLevel: LogLevel.Warn);
            return 1;
        }

        protected override void CreateActionCommitments()
        {
            ActionCommitments = new List<ActionCommitment>();
            if (!options.PublishAllOnly && options.AddNewResources && newWebResources.Any())
            {
                ActionCommitments.Add(new ActionCommitment
                {
                    Action = AddNewWebResources,
                    Description = $"{newWebResources.Count} {PluralizeResource(newWebResources)} will be added",
                    TextColor = ConsoleColor.Yellow
                });
            }
            if (!options.PublishAllOnly && options.UpdateResources && changedWebResources.Any())
            {
                ActionCommitments.Add(new ActionCommitment
                {
                    Action = UpdateChangedWebResources,
                    Description = $"{changedWebResources.Count} {PluralizeResource(changedWebResources)} will be updated",
                    TextColor = ConsoleColor.Green
                });
            }
            if (!options.PublishAllOnly && options.RemoveMissingResources && missingWebResources.Any())
            {
                ActionCommitments.Add(new ActionCommitment
                {
                    Action = DeleteMissingWebResources,
                    Description = $"{missingWebResources.Count} {PluralizeResource(missingWebResources)} will be deleted",
                    TextColor = ConsoleColor.Red
                });
            }
            if (options.PublishAllOnly || options.ShouldPublish && ActionCommitments.Any())
            {
                ActionCommitments.Add(new ActionCommitment
                {
                    Action = PublishCustomizations,
                    Description = "Publish all customizations",
                    TextColor = ConsoleColor.White
                });
            }
        }

        private int AddNewWebResources()
        {
            PrintBulletPoint("Adding Web Resources...", LogLevel.Info, ConsoleColor.Yellow);
            return RunResourceActions(newWebResources, wr => webResourceQueryingUtils.AddWebResource(wr, SolutionWithPublisher.Solution));
        }

        private int UpdateChangedWebResources()
        {
            PrintBulletPoint("Updating Web Resources...", LogLevel.Info, ConsoleColor.Green);
            return RunResourceActions(changedWebResources, wr => webResourceQueryingUtils.UpdateWebResourceContent(wr));
        }

        private int DeleteMissingWebResources()
        {
            PrintBulletPoint("Deleting Web Resources...", LogLevel.Info, ConsoleColor.Red);
            return RunResourceActions(missingWebResources, wr => webResourceQueryingUtils.DeleteWebResource(wr));
        }

        private int PublishCustomizations()
        {
            PrintBulletPoint("Publishing all customizations...", LogLevel.Info, ConsoleColor.White);
            return RunActionWithErrorHandling(() => webResourceQueryingUtils.PublishAll(), "Publishing");
        }

        private static int RunResourceActions(List<WebResource> resources, Action<WebResource> action)
        {
            var actions = resources.Select<WebResource, Func<int>>(resource => () => RunActionWithErrorHandling(() => action.Invoke(resource), resource.Name));
            return RunActions(actions);
        }
    }
}