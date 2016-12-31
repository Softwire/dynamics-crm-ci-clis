using System;
using System.Collections.Generic;
using System.Linq;
using CrmCommandLineHelpersCore.CommandLine;
using CrmCommandLineHelpersCore.CommandLine.ParameterDefinition;
using CrmCommandLineHelpersCore.CommandLine.Parameters;
using CrmCommandLineHelpersCore.Exceptions;
using WebResourceHelper.CommandLine.Parameters;

namespace WebResourceHelper
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                return InnerMain(args);
            }
            catch (Exception ex)
            {
                PrintException(ex);
                return 1;
            }
        }

        static int InnerMain(string[] args)
        {
            List<ParameterException> argParseExceptions;
            var options = ParseArgs(args, out argParseExceptions);
            if (options.ShouldShowHelp)
            {
                PrintHelp();
                return 0;
            }
            if (argParseExceptions.Any())
            {
                ConsoleHelper.WriteLine("=== ERROR ===", foregroundColor: ConsoleColor.DarkRed);
                ConsoleHelper.WriteLine();
                var requiredExceptions = argParseExceptions.Select(e => e as MissingRequiredParameterException).Where(e => e != null).ToList();
                var otherExceptions = argParseExceptions.Where(e => !(e is MissingRequiredParameterException)).ToList();
                if (requiredExceptions.Any())
                {
                    ConsoleHelper.WriteLine($"The following required parameter{(requiredExceptions.Count > 1 ? "s are" : " is")} missing:", indent: 1);
                    requiredExceptions.ForEach(e => ConsoleHelper.WriteLine($"- {e.Parameter.GetType().Name}", indent: 2));
                    ConsoleHelper.WriteLine();
                }
                if (otherExceptions.Any())
                {
                    ConsoleHelper.WriteLine("There are the following issues with your provided parameters:", indent: 1);
                    otherExceptions.ForEach(e => ConsoleHelper.WriteLine($"- {e.Parameter.GetType().Name} - {e.Message}", indent: 1));
                    ConsoleHelper.WriteLine();
                }
                ConsoleHelper.WriteLine("Run the utility with /help for more information on the correct form the parameters should take.", indent: 1);
                return 1;
            }
            ConsoleHelper.LogLevel = options.LogLevel;
            return new WebResourceUploader(options).Run();
        }

        public static WebResourceHelperOptions ParseArgs(string[] args, out List<ParameterException> allExceptions)
        {
            allExceptions = new List<ParameterException>();
            var pathSeparator = ArgsUtility.ExtractArgValueCatchingErrors(args, new PathSeparator(), allExceptions);
            return new WebResourceHelperOptions
            {
                ConnectionString = ArgsUtility.ExtractArgValueCatchingErrors(args, new ConnectionString(), allExceptions),
                RootPath = ArgsUtility.ExtractArgValueCatchingErrors(args, new RootFolderPath(), allExceptions),
                ShouldShowHelp = ArgsUtility.ExtractArgValueCatchingErrors(args, new ShouldShowHelp(), allExceptions),
                SolutionName = ArgsUtility.ExtractArgValueCatchingErrors(args, new SolutionName(), allExceptions),
                PathSeparator = pathSeparator,
                PublisherPrefixOverride = ArgsUtility.ExtractArgValueCatchingErrors(args, new PublisherPrefixOverride(), allExceptions),
                Prefix = ArgsUtility.ExtractArgValueCatchingErrors(args, new Prefix(), allExceptions) ?? pathSeparator,
                LogLevel = ArgsUtility.ExtractArgValueCatchingErrors(args, new InformationLevel(), allExceptions),
                StatusOnly = ArgsUtility.ExtractArgValueCatchingErrors(args, new StatusOnly(), allExceptions),
                Force = ArgsUtility.ExtractArgValueCatchingErrors(args, new Force(), allExceptions),
                Automate = ArgsUtility.ExtractArgValueCatchingErrors(args, new Automate(), allExceptions),
                AddNewResources = !ArgsUtility.ExtractArgValueCatchingErrors(args, new NoNew(), allExceptions),
                UpdateResources = !ArgsUtility.ExtractArgValueCatchingErrors(args, new NoUpdates(), allExceptions),
                RemoveMissingResources = ArgsUtility.ExtractArgValueCatchingErrors(args, new RemoveMissing(), allExceptions),
                ShouldPublish = !ArgsUtility.ExtractArgValueCatchingErrors(args, new NoPublish(), allExceptions),
                FileSizeLimitKb = ArgsUtility.ExtractArgValueCatchingErrors(args, new FileSizeLimit(), allExceptions),
                PublishAllOnly = ArgsUtility.ExtractArgValueCatchingErrors(args, new PublishAllOnly(), allExceptions)
            };
        }

        private readonly static List<IParameter> ParametersToShowHelpFor = new List<IParameter>
                                                                           {
                                                                               new ShouldShowHelp(),
                                                                               new InformationLevel(),
                                                                               new ConnectionString(),
                                                                               new RootFolderPath(),
                                                                               new SolutionName(),
                                                                               new PathSeparator(),
                                                                               new PublisherPrefixOverride(),
                                                                               new Prefix(),
                                                                               new FileSizeLimit(),
                                                                               new StatusOnly(),
                                                                               new Force(),
                                                                               new Automate(),
                                                                               new NoNew(),
                                                                               new NoUpdates(),
                                                                               new RemoveMissing(),
                                                                               new NoPublish(),
                                                                               new PublishAllOnly()
                                                                           };

        public static void PrintHelp()
        {
            ConsoleHelper.LogLevel = LogLevel.Trace; // Show all messages
            HelpUtility.PrintParameterHelp(ParametersToShowHelpFor);
            ConsoleHelper.WriteLine();
            ConsoleHelper.WriteLine("==== General Instructions ====", foregroundColor: ConsoleColor.DarkCyan);
            ConsoleHelper.WriteLine();
            ConsoleHelper.WriteLine("This is a command line tool to upload all web resources from a folder to a CRM instance. It discovers all files with valid web resource file extensions. Initially, you should use this from a console. As long as you don't use /automate or /force, this is perfectly safe, and will explain which actions it wishes to perform, followed by a prompt whether to continue or not. Feel free to experiment with the optional parameters in this mode. Example usage (line breaks should naturally be omitted):", indent: 1);
            ConsoleHelper.WriteLine();

            ConsoleHelper.Write("CrmResourceUploader.exe", indent: 1);
            ConsoleHelper.Write("/solutionname:", indent: 1, foregroundColor: ConsoleColor.Yellow);
            ConsoleHelper.WriteLine("Default");
            ConsoleHelper.Write("/rootpath:", indent: 25, foregroundColor: ConsoleColor.Yellow);
            ConsoleHelper.WriteLine("\"../../WebResources/build/release\"", wrappedIndent: 29);
            ConsoleHelper.Write("/crmconnectionstring:", indent: 25, foregroundColor: ConsoleColor.Yellow);
            ConsoleHelper.WriteLine("'Url=https://contoso.crm2.dynamics.com/Contoso; Username=john.doe@contoso.com; Password=password'", wrappedIndent: 29);
            ConsoleHelper.WriteLine("/nonew /nopublish", indent: 25, foregroundColor: ConsoleColor.Magenta);

            ConsoleHelper.WriteLine();
            ConsoleHelper.WriteLine("Once you are happy with your options, you can save them (in, say, a batch file) for running automatically. You should add /automate or /force to ensure the tool proceeds without interruption.", indent: 1);
            ConsoleHelper.WriteLine();
        }

        private static void PrintException(Exception ex)
        {
            ConsoleHelper.WriteLine();
            ConsoleHelper.PrintFatalError(ex);
            ConsoleHelper.WriteLine("Use the flag /h to get help with how to use this utility.", indent: 1, foregroundColor: ConsoleColor.White);
        }
    }
}
