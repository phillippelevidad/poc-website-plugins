using CSharpFunctionalExtensions;
using HostWebsite.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;

namespace HostWebsite.Pages.Plugins
{
    public class IndexModel : PageModel
    {
        private readonly IPluginLoader loader;

        public IndexModel(IPluginLoader loader)
        {
            this.loader = loader;
        }

        public List<Plugin> Plugins { get; set; }

        public List<string> LoadingErrors { get; set; }

        public void OnGet()
        {
            var result = loader.LoadPlugins();
            Plugins = result.Where(p => p.IsSuccess).Select(p => p.Value).ToList();
            LoadingErrors = result.Where(p => p.IsFailure).Select(p => p.Error).ToList();
        }
    }
}