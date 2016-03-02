using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using LVMS.FactuurSturen.Model;
using PortableRest;

namespace LVMS.FactuurSturen
{
    public partial class FactuurSturenClient
    {
        List<Profile> _cachedProfiles;

        public async Task<Profile[]> GetProfiles(bool? allowCache = true)
        {
            if (!allowCache.HasValue)
                allowCache = _allowResponseCaching;
            if ((bool)allowCache && _cachedProfiles != null)
                return _cachedProfiles.ToArray();

            var request = new RestRequest("profiles", HttpMethod.Get, ContentTypes.Json);

            var result = await _httpClient.ExecuteWithPolicyAsync<Profile[]>(this, request);

            if ((bool)allowCache || _cachedProfiles != null)
                _cachedProfiles = new List<Profile>(result);
            return result;
        }
    }
}
