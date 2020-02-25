using System;

namespace ZMM.App.Clients.Repo
{
    public static class Constants
    {
        public static readonly string RepoURL = "https://hub.umoya.ai/v3/search?semVerLevel=2.0.&prerelease=true";
        public static readonly string RepoURLQueryByResourceType = "https://hub.umoya.ai/v3/search?semVerLevel=2.0.&prerelease=true&packageType=";
        public static readonly string RepoURLQuery = "https://hub.umoya.ai/v3/search?semVerLevel=2.0.&prerelease=true&q=";

        public static readonly string RepoURLQueryByResourceTypeAndQueryString = "https://hub.umoya.ai/v3/search?semVerLevel=2.0.&prerelease=true&packageType=ResourceType&q=QueryString";

        public static readonly string RepoURLByResourceId = "https://hub.umoya.ai/v3/registration/ResourceId/index.json";

   }
}
