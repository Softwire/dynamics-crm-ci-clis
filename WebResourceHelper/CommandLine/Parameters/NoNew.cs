using CrmCommandLineHelpersCore.CommandLine.ParameterDefinition;

namespace WebResourceHelper.CommandLine.Parameters
{
    public class NoNew : BooleanParameter
    {
        protected override string Tag => "nonew";
        public override string Description => "Using this parameter means that no new files not currently present in the CRM are uploaded to the CRM. This could be useful if you don't have a build process, or your build process outputs web resources you do not wish to be uploaded. In this case, the solution defines the file names you are allowed to upload.";
    }
}