using PluginBase;
using System;

namespace HelloWorldPlugin
{
    public class HelloWorldPlugin : IWebsitePlugin
    {
        public PluginMeta Meta => new PluginMeta(
            "Hello World", new Uri("https://github.com/phillippelevidad/"),
            "Plugin example for a Proof of Concept", "1.0.0",
            "Phillippe Santana", new Uri("https://github.com/phillippelevidad/"), "GNUv2");

        public void Activate(IPluginActivationContext context)
        {
        }

        public void Deactivate(IPluginDeactivationContext context)
        {
        }
    }
}
