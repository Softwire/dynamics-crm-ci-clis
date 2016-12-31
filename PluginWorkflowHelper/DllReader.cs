using System;
using System.Activities;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Xrm.Sdk;

namespace PluginWorkflowHelper
{
    class DllReader
    {
        private readonly PluginWorkflowHelperOptions options;

        public DllReader(PluginWorkflowHelperOptions options)
        {
            this.options = options;
        }

        public PluginAssemblyAndTypes ReadDll()
        {
            if (options.DllPath == null)
            {
                throw new Exception("The assembly path must be provided as a command line argument");
            }
            var assemblyFile = new FileInfo(options.DllPath);
            var bytes = File.ReadAllBytes(assemblyFile.FullName);

            // TODO - preload all of the ones we expect CRM to have (?)
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += CurrentDomain_ReflectionOnlyAssemblyResolve;
            //Assembly.ReflectionOnlyLoadFrom("Microsoft.Xrm.Sdk.dll");
            var crmAssembly = Assembly.ReflectionOnlyLoad(bytes);
            var allTypes = crmAssembly.ExportedTypes.ToList();

//            var pluginTypesOneOld = allTypes.Where(t => t.IsSubclassOf(typeof(IPlugin))).ToList();
//            var pluginTypes2 = allTypes.Where(t => typeof(IPlugin).IsAssignableFrom(t)).ToList();
//            var pluginTypes3 = allTypes.Where(t => t.GetInterfaces().Contains(typeof(IPlugin))).ToList();
//            var pluginTypes4 = allTypes.SelectMany(t => t.GetInterfaces().ToList()).ToList();
//            var x = pluginTypes4.First().Assembly.Location;
//            var y = typeof(IPlugin).Assembly.Location;

            // Can't just use typeof(IPlugin) because they're different types as the assemblies are loaded in reflection only mode
            var pluginTypes = allTypes.Where(t => t.GetInterfaces().Any(i => i.Name == "IPlugin" && i.Namespace == "Microsoft.Xrm.Sdk")).ToList();
            //TODO - Do recursively for non-parent types
            var customWorkflowTypes = allTypes.Where(t => t.BaseType?.Name == "CodeActivity" && t.BaseType?.Namespace == "System.Activities").ToList();

            var newPluginAssembly = new PluginAssembly
            {
                Name = assemblyFile.Name,
                Content = Convert.ToBase64String(bytes),
                Version = crmAssembly.GetName().Version.ToString(),
                IsolationMode = new OptionSetValue((int)(options.IsolationMode ?? PluginAssemblyIsolationMode.Sandbox)),
                SourceType = new OptionSetValue((int)(options.SourceType ?? PluginAssemblySourceType.Database))
            };

            return new PluginAssemblyAndTypes
            {
                PluginAssembly = newPluginAssembly,
                Types = new PluginAssemblyTypes
                {
                    PluginTypes = pluginTypes,
                    CustomWorkflowTypes = customWorkflowTypes
                },
                Assembly = crmAssembly
            };
        }

        static Assembly CurrentDomain_ReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
        {
            return Assembly.ReflectionOnlyLoad(args.Name);
        }
    }
}