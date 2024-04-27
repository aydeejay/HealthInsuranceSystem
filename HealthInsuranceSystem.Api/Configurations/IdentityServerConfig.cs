using IdentityServer4.Models;

using System.Security.Cryptography.X509Certificates;

namespace HealthInsuranceSystem.Api.Configurations
{
    public static class IdentityServerConfig
    {
        public const string RoleIdentityResource = "role";
        public const string ApiResourceName = "ERPAPI";

        public static void ConfigureServices(IServiceCollection services, IConfiguration config)
        {
            var identityServer = services
                .AddIdentityServer()
                .AddInMemoryClients(GetClients(config))
                .AddInMemoryIdentityResources(GetIdentityResources())
                .AddInMemoryApiResources(GetApiResources(config));

            var certThumbprint = config["Identity:Certificate"];

            if (string.IsNullOrWhiteSpace(certThumbprint))
            {
                identityServer.AddDeveloperSigningCredential();
            }
            else
            {
                var certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                certStore.Open(OpenFlags.ReadOnly);

                var certs = certStore.Certificates.Find(X509FindType.FindByThumbprint, certThumbprint, false);
                if (certs.Count > 0)
                {
                    identityServer.AddSigningCredential(certs[0]);
                }
            }
        }

        public static void Configure(IApplicationBuilder app)
        {
            app.UseIdentityServer();
        }

        private static IEnumerable<Client> GetClients(IConfiguration config)
        {
            var clients = config
                .GetSection("Identity:Clients")
                .Get<List<Client>>();

            var clientSecret = new Secret(config["Identity:ClientSecret"].Sha256());

            clients.ForEach(x =>
            {
                x.AllowedGrantTypes = GrantTypes.ResourceOwnerPassword;
                x.ClientSecrets = new List<Secret> { clientSecret };
                x.AllowOfflineAccess = true;
                x.RefreshTokenUsage = TokenUsage.OneTimeOnly;
                x.UpdateAccessTokenClaimsOnRefresh = true;
                //x.AccessTokenLifetime = int.Parse(config["Identity:Clients:TokenLife"]);
                //x.AccessTokenLifetime = 2592000;
                x.IncludeJwtId = true;
            });

            return clients;
        }

        private static IEnumerable<IdentityResource> GetIdentityResources()
        {
            yield return new IdentityResources.OpenId();
            yield return new IdentityResources.Profile();
            yield return new IdentityResources.Email();

            yield return new IdentityResource
            {
                Name = RoleIdentityResource,
                DisplayName = RoleIdentityResource,
                UserClaims = { RoleIdentityResource }
            };
        }

        private static IEnumerable<ApiResource> GetApiResources(IConfiguration config)
        {
            var scopes = config
                .GetSection("Identity:Scopes")
                .Get<IEnumerable<string>>()
                //.Select(x => new Scopes(x))
                .ToArray();

            var secret = config["Identity:ApiSecret"];

            yield return new ApiResource
            {
                Name = ApiResourceName,
                DisplayName = ApiResourceName,
                Description = $"{ApiResourceName} Access",
                UserClaims = { RoleIdentityResource },
                ApiSecrets = { new Secret(config["Identity:ApiSecret"].Sha256()) },
                Scopes = scopes
            };
        }
    }
}
