using System;
using CrmCommandLineHelpersCore.CommandLine.ParameterDefinition;

namespace CrmCommandLineHelpersCore.Exceptions
{
    public class MissingRequiredParameterException : ParameterException
    {
        public MissingRequiredParameterException(IParameter parameter) : base("the argument is required", parameter) { }
    }
}
