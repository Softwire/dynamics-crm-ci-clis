using CrmCommandLineHelpersCore.CommandLine.ParameterDefinition;
using CrmCommandLineHelpersCore.Exceptions;

namespace WebResourceHelper.CommandLine.Parameters
{
    public class PublisherPrefixOverride : StringParameter
    {
        protected override string Tag => "publisherprefixoverride";
        public override string Description => "Including this option means that the solution's publisher prefix (eg new_) is replaced. Don't include the underscore in this parameter. Example usage - /publisherprefixoverride:pub which will result in file names such as (eg) pub_/abc.htm";

        public override string ParseValue(bool parameterExists, string value)
        {
            var prefix = base.ParseValue(parameterExists, value);
            if (prefix == null) return null;
            if (prefix.Length <= 1) throw new ParameterException("the prefix must have length 2 or greater", this);
            return prefix;
        }
    }
}