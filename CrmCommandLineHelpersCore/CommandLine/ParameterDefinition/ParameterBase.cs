using System.Collections.Generic;

namespace CrmCommandLineHelpersCore.CommandLine.ParameterDefinition
{
    public abstract class ParameterBase<T> : IParameter<T>
    {
        // One of these two should be overriden in a subsubclass!
        // These should be turned into CommandLineTags appropriately by a general subclass
        protected virtual IEnumerable<string> Tags => new[] { Tag };
        protected virtual string Tag => null;

        public virtual IEnumerable<CommandLineTag> CommandLineTags => null;
        public abstract string Description { get; }
        public abstract T ParseValue(bool parameterExists, string value);
        public virtual bool Required => false;
    }
}