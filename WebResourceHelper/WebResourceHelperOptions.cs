using CrmCommandLineHelpersCore;
using JetBrains.Annotations;

namespace WebResourceHelper
{
    class WebResourceHelperOptions : CoreOptions
    {
        [CanBeNull]
        public string RootPath;
        [NotNull]
        public string PathSeparator = "";
        [NotNull]
        public string Prefix = "";
        [CanBeNull]
        public string PublisherPrefixOverride;
        public bool AddNewResources;
        public bool UpdateResources;
        public bool RemoveMissingResources;
        public bool ShouldPublish;
        public int FileSizeLimitKb;
        public bool PublishAllOnly;
    }
}