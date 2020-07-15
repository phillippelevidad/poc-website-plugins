namespace PluginBase
{
    public interface IWebsitePlugin
    {
        PluginMeta Meta { get; }
        void Activate(IPluginActivationContext context);
        void Deactivate(IPluginDeactivationContext context);
    }
}
