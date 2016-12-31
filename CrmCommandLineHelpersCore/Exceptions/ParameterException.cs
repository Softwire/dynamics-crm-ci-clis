using System;
using CrmCommandLineHelpersCore.CommandLine.ParameterDefinition;

namespace CrmCommandLineHelpersCore.Exceptions
{
    public class ParameterException : Exception
    {
        public readonly IParameter Parameter;

        public ParameterException(string message, IParameter parameter) : base(GetMessage(message, parameter))
        {
            Parameter = parameter;
        }

        private static string GetMessage(string message, IParameter parameter)
        {
            return $"{parameter.GetType().Name} - {message}";
        }
    }
}
