using System.Collections.Generic;
using CrmCommandLineHelpersCore.CommandLine.ParameterDefinition;

namespace WebResourceHelper.CommandLine.Parameters
{
    public class RootFolderPath : StringParameter
    {
        protected override IEnumerable<string> Tags => new [] { "folderpath", "rootpath", "rootfolderpath" };
        public override string Description => "Provide the path (relative or absolute) to the directory to upload files from. Example use - /folderpath:build/release/";
        public override bool Required => true;
    }
}