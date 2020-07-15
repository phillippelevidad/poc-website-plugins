using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Hosting;
using PluginBase;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace HostWebsite.Services
{
    public interface IPluginLoader
    {
        ReadOnlyCollection<Result<Plugin>> LoadPlugins();
    }

    public class PluginLoader : IPluginLoader
    {
        private readonly IWebHostEnvironment environment;

        public PluginLoader(IWebHostEnvironment environment)
        {
            this.environment = environment;
        }

        public ReadOnlyCollection<Result<Plugin>> LoadPlugins()
        {
            var pluginFolders = Directory.EnumerateDirectories(Path.Combine(environment.WebRootPath, "plugins"));
            return pluginFolders.Select(f => LoadPlugin(f)).ToList().AsReadOnly();
        }

        private Result<Plugin> LoadPlugin(string pluginFolder)
        {
            var assembly = LoadPluginAssembly(pluginFolder);
            if (assembly.IsFailure) return Result.Failure<Plugin>(assembly.Error);

            var entryPoint = LoadPluginEntryPoint(assembly.Value);
            if (entryPoint.IsFailure) return Result.Failure<Plugin>(entryPoint.Error);

            return Result.Success(new Plugin(assembly.Value, entryPoint.Value));
        }

        private Result<Assembly> LoadPluginAssembly(string pluginFolder)
        {
            try
            {
                var loadContext = new PluginLoadContext(pluginFolder);
                return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginFolder)));
            }
            catch (Exception ex)
            {
                return Result.Failure<Assembly>(ex.ToString());
            }
        }

        private Result<IWebsitePlugin> LoadPluginEntryPoint(Assembly assembly)
        {
            try
            {
                var type = assembly.GetTypes().FirstOrDefault(t => typeof(IWebsitePlugin).IsAssignableFrom(t));
                var plugin = Activator.CreateInstance(type) as IWebsitePlugin;
                return Result.Success(plugin);
            }
            catch (Exception ex)
            {
                return Result.Failure<IWebsitePlugin>(ex.ToString());
            }
        }
    }

    public class Plugin
    {
        public Plugin(Assembly assembly, IWebsitePlugin entryPoint)
        {
            Assembly = assembly;
            EntryPoint = entryPoint;
        }

        public Assembly Assembly { get; }
        public IWebsitePlugin EntryPoint { get; }
    }

    public class PluginLoadContext : AssemblyLoadContext
    {
        private readonly AssemblyDependencyResolver resolver;

        public PluginLoadContext(string pluginFolder)
        {
            var dllName = $"{Path.GetFileNameWithoutExtension(pluginFolder)}.dll";
            resolver = new AssemblyDependencyResolver(Path.Combine(pluginFolder, dllName));
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            var assemblyPath = resolver.ResolveAssemblyToPath(assemblyName);
            if (assemblyPath != null) return LoadFromAssemblyPath(assemblyPath);
            return null;
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            var libraryPath = resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            if (libraryPath != null) return LoadUnmanagedDllFromPath(libraryPath);
            return IntPtr.Zero;
        }
    }
}
