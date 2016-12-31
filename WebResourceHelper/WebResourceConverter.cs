using System.Text.RegularExpressions;
using Microsoft.Xrm.Sdk;

namespace WebResourceHelper
{
    class WebResourceConverter
    {
        private static readonly Regex PathSeparatorRegex = new Regex(@"[\\\/]", (RegexOptions.Compiled | RegexOptions.CultureInvariant));
        private readonly WebResourceHelperOptions webResourceHelperOptions;

        public WebResourceConverter(WebResourceHelperOptions webResourceHelperOptions)
        {
            this.webResourceHelperOptions = webResourceHelperOptions;
        }

        public WebResource FileInfoToWebResource(WebResourceFile file, Publisher publisher)
        {
            var name = ConvertRelativePathToWebResourceName(file.RelativePath, publisher);
            var webResource = new WebResource
            {
                Name = name,
                DisplayName = name,
                WebResourceType = new OptionSetValue((int)WebResourceExtensionUtilities.ConvertStringExtension(file.Extension)),
                Description = "",
                Content = file.Base64Content
            };
            if (webResource.WebResourceType.Value == (int)WebResourceWebResourceType.Silverlight_XAP)
            {
                webResource.SilverlightVersion = "4.0";
            }
            return webResource;
        }

        private string ConvertRelativePathToWebResourceName(string relativePath, Publisher publisher)
        {
            var pathWithCorrectSeparator = PathSeparatorRegex.Replace(relativePath, webResourceHelperOptions.PathSeparator);
            var solutionPrefix = (webResourceHelperOptions.PublisherPrefixOverride ?? publisher.CustomizationPrefix) + "_";
            return solutionPrefix + webResourceHelperOptions.Prefix + pathWithCorrectSeparator;
        }

        public static T CloneRelevantFields<T>(WebResource resource) where T : WebResource, new()
        {
            var clonedResource = new T
            {
                Name = resource.Name,
                DisplayName = resource.DisplayName,
                WebResourceType = resource.WebResourceType,
                SilverlightVersion = resource.SilverlightVersion,
                Description = resource.Description,
                Content = resource.Content
            };
            if (resource.WebResourceId != null) // If an id is set at all, then it will cause errors on creation
            {
                clonedResource.WebResourceId = resource.WebResourceId;
            }
            return clonedResource;
        }
    }
}
