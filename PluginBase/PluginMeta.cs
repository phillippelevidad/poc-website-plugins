using System;

namespace PluginBase
{
    public class PluginMeta
    {
        public PluginMeta(
            string name, Uri uri, string description, string version,
            string author, Uri authorUri, string license)
        {
            Name = name;
            Uri = uri;
            Description = description;
            Version = version;
            Author = author;
            AuthorUri = authorUri;
            License = license;
        }

        public string Name { get; }
        public Uri Uri { get; }
        public string Description { get; }
        public string Version { get; }
        public string Author { get; }
        public Uri AuthorUri { get; }
        public string License { get; }
    }
}
