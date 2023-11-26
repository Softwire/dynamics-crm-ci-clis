using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CrmCommandLineHelpersCore.CommandLine;
using JetBrains.Annotations;

namespace WebResourceHelper
{
    class PartitionedWebResources
    {
        public List<WebResource> NewWebResources;
        public List<WebResource> UnchangedWebResources;
        public List<WebResource> ChangedWebResources;
        public List<WebResource> MissingWebResources;
        public List<InvalidWebResource> InvalidWebResources;
    }

    class InvalidWebResource : WebResource
    {
        public List<string> ReasonsForInvalidity;
    }

    class WebResourcePartitioner
    {
        private static readonly Regex InvalidWebResourceNameRegex = new Regex(@"[^a-z0-9A-Z-_\\./]|[/]{2,}", (RegexOptions.Compiled | RegexOptions.CultureInvariant));

        private readonly WebResourceHelperOptions options;

        private readonly List<WebResource> allCrmWebResources;
        private readonly List<WebResource> allLocalWebResources;

        private List<WebResource> unaccountedCrmResources;
        private List<WebResource> newResources;
        private List<WebResource> unchangedResources;
        private List<WebResource> changedResources;
        private List<InvalidWebResource> invalidResources;

        public WebResourcePartitioner(WebResourceHelperOptions options, List<WebResource> crmWebResources, List<WebResource> localWebResources)
        {
            this.options = options;
            allCrmWebResources = crmWebResources;
            allLocalWebResources = localWebResources;
        }

        private void Initialize()
        {
            unaccountedCrmResources = allCrmWebResources.Select(Clone).ToList();
            newResources = new List<WebResource>();
            unchangedResources = new List<WebResource>();
            changedResources = new List<WebResource>();
            invalidResources = new List<InvalidWebResource>();
        }

        public PartitionedWebResources Partition()
        {
            Initialize();
            PartitionResources();

            return new PartitionedWebResources
            {
                NewWebResources = newResources,
                UnchangedWebResources = unchangedResources,
                ChangedWebResources = changedResources,
                MissingWebResources = unaccountedCrmResources.Where(r => invalidResources.All(ir => ir.Name != r.Name)).ToList(),
                InvalidWebResources = invalidResources
            };
        }

        private void PartitionResources()
        {
            foreach (var localResource in allLocalWebResources)
            {
                var invalidityReasons = FindReasonsForInvalidity(localResource);
                var relatedCrmResource = unaccountedCrmResources.FirstOrDefault(cr => cr.Name == localResource.Name);

                if (invalidityReasons.Any())
                {
                    HandleInvalidResource(localResource, relatedCrmResource, invalidityReasons);
                    continue;
                }

                if (relatedCrmResource == null)
                {
                    AddNewResource(localResource);
                }
                else
                {
                    HandleExistingResource(relatedCrmResource, localResource);
                }
            }
        }

        private void HandleInvalidResource(WebResource localResource, [CanBeNull] WebResource relatedCrmResource, List<string> reasonsForInvalidity)
        {
            if (relatedCrmResource != null) unaccountedCrmResources.Remove(relatedCrmResource);

            var clonedResource = WebResourceConverter.CloneRelevantFields<InvalidWebResource>(localResource);
            clonedResource.ReasonsForInvalidity = reasonsForInvalidity;
            invalidResources.Add(clonedResource);
        }

        private void AddNewResource(WebResource resource)
        {
            newResources.Add(Clone(resource));
        }

        private void HandleExistingResource(WebResource crmResource, WebResource localResource)
        {
            unaccountedCrmResources.Remove(crmResource);
            if (crmResource.Content == localResource.Content)
            {
                AddUnchangedResource(crmResource);
            }
            else
            {
                AddChangedResource(crmResource, localResource.Content);
            }
        }

        private void AddUnchangedResource(WebResource resource)
        {
            unchangedResources.Add(Clone(resource));
        }

        private void AddChangedResource(WebResource resource, string newContent)
        {
            var changedCrmResource = Clone(resource);
            changedCrmResource.Content = newContent;
            changedResources.Add(changedCrmResource);
        }

        private List<string> FindReasonsForInvalidity(WebResource resource)
        {
            var invalidityReasons = new List<string>();
            if (!IsWebResourceNameValid(resource.Name))
            {
                invalidityReasons.Add("Invalid name - names must be <= 100 characters and may only contain a-z A-Z 0-9 _ . and non-consecutive / characters");
            }
            if (IsWebResourceOverSizeLimit(resource))
            {
                invalidityReasons.Add($"File size is {Math.Ceiling(resource.Content.Length * 3.0 / 4 / 1024)}KB > limit of {options.FileSizeLimitKb}KB");
            }
            return invalidityReasons;
        }

        private static bool IsWebResourceNameValid(string name)
        {
            return !(InvalidWebResourceNameRegex.IsMatch(name) || name.Length > 100);
        }

        private bool IsWebResourceOverSizeLimit(WebResource resource)
        {
            // Each letter is base-64, so 6 bits, ie (3/4) bytes
            return resource.Content.Length * 3 > options.FileSizeLimitKb * 1024 * 4;
        }

        private static WebResource Clone(WebResource resource)
        {
            return WebResourceConverter.CloneRelevantFields<WebResource>(resource);
        }
    }
}
