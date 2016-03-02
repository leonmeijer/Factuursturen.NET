using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using LVMS.FactuurSturen.Exceptions;
using LVMS.FactuurSturen.Model;
using PortableRest;

namespace LVMS.FactuurSturen
{
    public partial class FactuurSturenClient
    {
        List<Profile> _cachedProfiles;

        public async Task<Profile[]> GetProfiles(bool allowCache = true)
        {
            if (allowCache && _cachedProfiles != null)
                return _cachedProfiles.ToArray();

            var request = new RestRequest("profiles", HttpMethod.Get, ContentTypes.Json);

            var result = await _httpClient.ExecuteWithPolicyAsync<Profile[]>(this, request);

            if (allowCache || _cachedProfiles != null)
                _cachedProfiles = new List<Profile>(result);
            return result;
        }
    }
}
