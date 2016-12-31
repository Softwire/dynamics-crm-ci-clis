using System;
using System.Collections.Generic;
using CrmCommandLineHelpersCore.CommandLine;
using CrmCommandLineHelpersCore.CommandLine.ParameterDefinition;
using CrmCommandLineHelpersCore.CommandLine.Parameters;
using CrmCommandLineHelpersCore.Exceptions;
using System.Linq;
using PluginWorkflowHelper.CommandLine.Parameters;

namespace PluginWorkflowHelper
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
            return new DllUploader(options).Run();
        }

        public static PluginWorkflowHelperOptions ParseArgs(string[] args, out List<ParameterException> allExceptions)
        {
            allExceptions = new List<ParameterException>();
            return new PluginWorkflowHelperOptions
            {
                ConnectionString = ArgsUtility.ExtractArgValueCatchingErrors(args, new ConnectionString(), allExceptions),
                ShouldShowHelp = ArgsUtility.ExtractArgValueCatchingErrors(args, new ShouldShowHelp(), allExceptions),
                SolutionName = ArgsUtility.ExtractArgValueCatchingErrors(args, new SolutionName(), allExceptions),
                LogLevel = ArgsUtility.ExtractArgValueCatchingErrors(args, new InformationLevel(), allExceptions),
                DllPath = ArgsUtility.ExtractArgValueCatchingErrors(args, new DllPath(), allExceptions),
                IsolationMode = ArgsUtility.ExtractArgValueCatchingErrors(args, new IsolationMode(), allExceptions),
                SourceType = ArgsUtility.ExtractArgValueCatchingErrors(args, new SourceType(), allExceptions)
            };
        }

        private readonly static List<IParameter> ParametersToShowHelpFor = new List<IParameter>
                                                                           {
                                                                               new ShouldShowHelp(),
                                                                               new InformationLevel(),
                                                                               new ConnectionString(),
                                                                               new SolutionName(),
                                                                               new DllPath(),
                                                                               new IsolationMode(),
                                                                               new SourceType()
                                                                           };

        public static void PrintHelp()
        {
            ConsoleHelper.LogLevel = LogLevel.Trace; // Show all messages
            HelpUtility.PrintParameterHelp(ParametersToShowHelpFor);
            ConsoleHelper.WriteLine();
            ConsoleHelper.WriteLine("==== General Instructions ====", foregroundColor: ConsoleColor.DarkCyan);
            ConsoleHelper.WriteLine();
            ConsoleHelper.WriteLine("This is a command line tool to upload custom plugin and workflow code from a dll to a CRM instance. The dll should already be registered with the SDK, and the appropriate steps created, this tool is simply for command-line updating. Initially, you should use this from a console. As long as you don't use /automate or /force, this is perfectly safe, and will explain which actions it wishes to perform, followed by a prompt whether to continue or not. Feel free to experiment with the optional parameters in this mode. Example usage (line breaks should naturally be omitted):", indent: 1);
            ConsoleHelper.WriteLine();

            ConsoleHelper.Write("PluginWorkflowHelper.exe", indent: 1);
            ConsoleHelper.Write("/solutionname:", indent: 1, foregroundColor: ConsoleColor.Yellow);
            ConsoleHelper.WriteLine("Default");
            ConsoleHelper.Write("/dllpath:", indent: 25, foregroundColor: ConsoleColor.Yellow);
            ConsoleHelper.WriteLine("\"../../DotNet/bin/Release/CrmPlugins.dll\"", wrappedIndent: 29);
            ConsoleHelper.Write("/crmconnectionstring:", indent: 25, foregroundColor: ConsoleColor.Yellow);
            ConsoleHelper.WriteLine("'Url=https://contoso.crm2.dynamics.com/Contoso; Username=john.doe@contoso.com; Password=password'", wrappedIndent: 29);
            ConsoleHelper.WriteLine("/nonew", indent: 25, foregroundColor: ConsoleColor.Magenta);

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
