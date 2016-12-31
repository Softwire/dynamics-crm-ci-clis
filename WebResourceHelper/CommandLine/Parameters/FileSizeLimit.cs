using CrmCommandLineHelpersCore.CommandLine.ParameterDefinition;

namespace WebResourceHelper.CommandLine.Parameters
{
    public class FileSizeLimit : IntParameter
    {
        protected override string Tag => "filesizelimitkb";
        public override string Description => @"The file size limit in kilobytes at which a file is marked as invalid. By default, this uses 5120 (ie, 5MB), which is the default value on a fresh Dynamics install. Only change this command line option if you have changed the corresponding value on the CRM (the CRM setting is the same setting as the email attachment size limit).";
        public override int Default => 5120;
    }
}