using System;

namespace WebResourceHelper
{
    internal class WebResourceExtensionUtilities
    {
        public static string[] ValidExtensions = { ".css", ".xml", ".gif", ".htm", ".html", ".ico", ".jpg", ".jpeg", ".png", ".js", ".json", ".map", ".xap", ".xsl", ".xslt", ".eot", ".svg", ".ttf", ".woff", ".woff2" };

        public static WebResourceWebResourceType ConvertStringExtension(string extensionWithDot)
        {
            var extension = extensionWithDot.TrimStart('.').ToLower();
            switch (extension)
            {
                case "css":
                    return WebResourceWebResourceType.StyleSheet_CSS;
                case "xsl":
                case "xslt":
                    return WebResourceWebResourceType.StyleSheet_XSL;

                case "xml":
                    return WebResourceWebResourceType.Data_XML;
                case "htm":
                case "html":
                    return WebResourceWebResourceType.Webpage_HTML;

                case "gif":
                    return WebResourceWebResourceType.GIFformat;
                case "ico":
                    return WebResourceWebResourceType.ICOformat;
                case "jpg":
                case "jpeg":
                    return WebResourceWebResourceType.JPGformat;
                case "png":
                    return WebResourceWebResourceType.PNGformat;

                case "xap":
                    return WebResourceWebResourceType.Silverlight_XAP;

                case "js":
                case "json":
                // Allowed for sourcemaps (which will be in JSON format typically)
                case "map":
                // These are font files. The closest web resource type is JScript. Hopefully browsers will be able to understand this mime type correctly.
                case "eot":
                case "svg":
                case "ttf":
                case "woff":
                case "woff2":
                    return WebResourceWebResourceType.Script_JScript;

                default:
                    throw new ArgumentOutOfRangeException($"\"{extension.ToLower()}\" is not recognized as a valid file extension for a Web Resource.");
            }
        }
    }
}
