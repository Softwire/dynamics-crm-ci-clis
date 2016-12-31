using CrmCommandLineHelpersCore.CommandLine.ParameterDefinition;

namespace WebResourceHelper.CommandLine.Parameters
{
    public class RemoveMissing : BooleanParameter
    {
        protected override string Tag => "removemissing";
        public override string Description => "Using this parameter deletes web resources off the CRM where they are present in the solution but not in the specified root folder. This is useful for where a CRM solution and build folder should be in 1:1 correspondance.";
    }
}