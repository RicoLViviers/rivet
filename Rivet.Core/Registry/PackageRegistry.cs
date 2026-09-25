using System.Text.Json;

namespace Rivet.Core.Registry
{
    public class PackageRegistry
    {
        private readonly string _registryUrl =
            "https://raw.githubusercontent.com/RicoLViviers/rivet/main/packages.json";

        private readonly HttpClient _client = new HttpClient();

        public async Task<Package> FindPackageAsync(string packageName)
        {
            string registryJson = await _client.GetStringAsync(_registryUrl);

            JsonSerializerOptions options = new()
            {
                PropertyNameCaseInsensitive = true,
                IncludeFields = true
            };

            List<Package> packages =
                JsonSerializer.Deserialize<List<Package>>(registryJson, options);

            foreach (Package package in packages)
            {
                if (package.Name == packageName)
                {
                    return package;
                }
            }

            return null;
        }
    }
}