using System.Collections.Generic;
using JetBrains.Annotations;

namespace CrmCommandLineHelpersCore.CommandLine.ParameterDefinition
{
    public interface IParameter<out T> : IParameter
    {
        T ParseValue(bool parameterExists, [CanBeNull] string value);
    }

    public interface IParameter
    {
        IEnumerable<CommandLineTag> CommandLineTags { get; }
        string Description { get; }
        bool Required { get; }
    }

    public class CommandLineTag
    {
        public string Name { get; set; }
        public bool TakesValue { get; set; }
    }
}