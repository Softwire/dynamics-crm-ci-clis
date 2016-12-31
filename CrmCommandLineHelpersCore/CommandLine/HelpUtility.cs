using System;
using System.Collections.Generic;
using System.Linq;
using CrmCommandLineHelpersCore.CommandLine.ParameterDefinition;

namespace CrmCommandLineHelpersCore.CommandLine
{
    public static class HelpUtility
    {
        public static void PrintParameterHelp(IEnumerable<IParameter> parameters)
        {
            ConsoleHelper.WriteLine();
            ConsoleHelper.WriteLine("==== Help ====", foregroundColor: ConsoleColor.DarkCyan);

            foreach (var arg in parameters)
            {
                ConsoleHelper.WriteLine();
                ConsoleHelper.Write($" {arg.GetType().Name}", foregroundColor: ConsoleColor.DarkMagenta);
                if (arg.Required)
                {
                    ConsoleHelper.Write("(required)", indent: 1, foregroundColor: ConsoleColor.Yellow);
                }
                ConsoleHelper.Write(" - ");
                ConsoleHelper.Write($"{CommandLineParameterToInputString(arg.CommandLineTags.First())}", foregroundColor: ConsoleColor.Magenta);
                Console.WriteLine();
                ConsoleHelper.WriteLine(arg.Description, 3, 2);
            }
            ConsoleHelper.WriteLine();
        }

        private static string CommandLineParameterToInputString(CommandLineTag tag)
        {
            return tag.TakesValue ? $"/{tag.Name}:" : $"/{tag.Name}";
        }
    }
}