using CrmCommandLineHelpersCore.CommandLine.ParameterDefinition;

namespace CrmCommandLineHelpersCore.CommandLine.Parameters
{
    public class SolutionName : StringParameter
    {
        protected override string Tag => "solutionname";
        public override string Description => "The solution to upload to. Example use - /solutionname:Contoso";
        public override bool Required => true;
    }
}