using System;
using System.Collections.Generic;
using System.Linq;
using CrmCommandLineHelpersCore.CommandLine.ParameterDefinition;
using CrmCommandLineHelpersCore.Exceptions;

namespace CrmCommandLineHelpersCore.CommandLine
{
    public static class ArgsUtility
    {
        public static T ExtractArgValueCatchingErrors<T>(IEnumerable<string> args, IParameter<T> parameterDefinition, ICollection<ParameterException> exceptions)
        {
            try
            {
                return ExtractArgValue(args, parameterDefinition);
            }
            catch (ParameterException ex)
            {
                exceptions.Add(ex);
                return default(T);
            }
        }

        private static T ExtractArgValue<T>(IEnumerable<string> args, IParameter<T> parameterDefinition)
        {
            if (parameterDefinition.CommandLineTags == null)
            {
                throw new Exception(parameterDefinition.GetType().Name);
            }
            var matchingArguments = parameterDefinition.CommandLineTags.SelectMany(tag => tag.TakesValue ?
                    args.Where(a => a.ToLower().StartsWith("/" + tag.Name.ToLower() + ":")).Select(t => t.Remove(0, tag.Name.Length + 2)) :
                    args.Where(a => string.Equals(a, "/" + tag.Name, StringComparison.CurrentCultureIgnoreCase)).Select(t => (string)null)
                ).ToList();

            if (!matchingArguments.Any())
            {
                if (parameterDefinition.Required)
                {
                    throw new MissingRequiredParameterException(parameterDefinition);
                }
                return parameterDefinition.ParseValue(false, null);
            }

            if (matchingArguments.Count > 1)
            {
                throw new ParameterException("this parameter is given more than once.", parameterDefinition);
            }

            return parameterDefinition.ParseValue(true, matchingArguments.Single());
        }
    }
}
