using System;
using System.Collections.Generic;
using System.Linq;
using CrmCommandLineHelpersCore.CommandLine;
using CrmCommandLineHelpersCore.DataAccess;
using JetBrains.Annotations;

namespace CrmCommandLineHelpersCore
{
    public class ProgramFlowBase
    {
        private readonly CoreQueryingUtilities coreQueryingUtils;
        private readonly CoreOptions options;

        protected readonly CrmConnector CrmConnector;

        protected SolutionWithPublisher SolutionWithPublisher;
        protected List<ActionCommitment> ActionCommitments = new List<ActionCommitment>();

        public class ActionCommitment
        {
            public Func<int> Action;
            public string Description;
            public ConsoleColor? TextColor;
        }

        public ProgramFlowBase(CoreOptions options)
        {
            this.options = options;
            CrmConnector = new CrmConnector(options);
            coreQueryingUtils = new CoreQueryingUtilities(CrmConnector);
        }

        public int Run()
        {
            SolutionWithPublisher = GetChosenSolution();

            PreStatusProcessing();

            PrintStatus();

            if (options.StatusOnly) return 0;
            var exitCode = PostStatusExitCode();
            if (exitCode.HasValue) return exitCode.Value;

            CreateActionCommitments();
            PrintActionCommitments();

            if (!AllowedToProceed()) return 0;

            var errorCode = RunActionCommitments();

            if (errorCode == 0)
            {
                ConsoleHelper.WriteLine();
                ConsoleHelper.WriteLine("All actions completed successfully", foregroundColor: ConsoleColor.Green);
            }

            return errorCode;
        }

        [NotNull]
        public SolutionWithPublisher GetChosenSolution()
        {
            var solutions = coreQueryingUtils.GetUnmanagedSolutions().ToList();

            var chosenSolution = solutions.FirstOrDefault(s => s.Solution.UniqueName == options.SolutionName);

            if (chosenSolution == null)
            {
                throw new Exception($"A solution should be provided that's one of: {string.Join(", ", solutions.Select(s => s.Solution.UniqueName))}");
            }
            return chosenSolution;
        }

        protected virtual void PreStatusProcessing() { }

        protected virtual void PrintStatus() { }

        protected virtual int? PostStatusExitCode()
        {
            return null;
        }

        protected static void PrintBulletPoint(string message, LogLevel logLevel, ConsoleColor? textColor = null)
        {
            ConsoleHelper.Write(">", foregroundColor: ConsoleColor.White, indent: 1, logLevel: logLevel);
            ConsoleHelper.WriteLine($" {message}", foregroundColor: textColor, wrappedIndent: 2, logLevel: logLevel);
        }

        protected virtual void CreateActionCommitments() { }

        private void PrintActionCommitments()
        {
            if (!ActionCommitments.Any())
            {
                ConsoleHelper.WriteLine("No actions will be taken", foregroundColor: ConsoleColor.Yellow);
                ConsoleHelper.WriteLine();
                return;
            }
            ConsoleHelper.WriteLine($"The following {(ActionCommitments.Count == 1 ? "action" : "actions")} will be taken:", logLevel: LogLevel.Warn);
            ConsoleHelper.WriteLine(logLevel: LogLevel.Warn);
            ActionCommitments.ForEach(a =>
            {
                PrintBulletPoint(a.Description, LogLevel.Warn, a.TextColor);
            });
            ConsoleHelper.WriteLine(logLevel: LogLevel.Warn);
        }

        protected virtual bool AllowedToProceed()
        {
            if (!ActionCommitments.Any()) return false;
            if (options.Force || options.Automate) return true;
            while (true)
            {
                ConsoleHelper.Write("Proceed? (Y/N) ", foregroundColor: ConsoleColor.White, logLevel: LogLevel.Error); // Has to be displayed
                var input = Console.ReadLine()?.ToUpper();
                switch (input)
                {
                    case "Y":
                        Console.WriteLine();
                        return true;
                    case "N":
                        Console.WriteLine();
                        return false;
                }
            }
        }

        private int RunActionCommitments()
        {
            ConsoleHelper.WriteLine("===== ACTIONS =====");
            ConsoleHelper.WriteLine();

            var errorCode = RunActions(ActionCommitments.Select(a => a.Action));

            ConsoleHelper.WriteLine();
            ConsoleHelper.WriteLine("=== ACTIONS END ===");
            ConsoleHelper.WriteLine();

            return errorCode;
        }

        protected static int RunActions(IEnumerable<Func<int>> actionsReturningErrorCodes)
        {
            var errorCodes = actionsReturningErrorCodes.Select(a => a.Invoke()).ToList();
            return errorCodes.Any(c => c != 0) ? 1 : 0;
        }

        protected static int RunActionWithErrorHandling(Action action, string contextName)
        {
            try
            {
                action.Invoke();
                ConsoleHelper.WriteLine($"{contextName} done", indent: 2);
            }
            catch (Exception ex)
            {
                ConsoleHelper.Write($"Error with {contextName}: ", indent: 2, foregroundColor: ConsoleColor.Red, logLevel: LogLevel.Error);
                ConsoleHelper.Write(" " + ex.GetType().Name);
                ConsoleHelper.WriteLine(" - " + ex.Message, wrappedIndent: 4);
                return 1;
            }
            return 0;
        }
    }
}