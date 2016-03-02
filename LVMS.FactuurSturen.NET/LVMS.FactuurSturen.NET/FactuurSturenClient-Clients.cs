using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using LVMS.FactuurSturen.Model;
using PortableRest;

namespace LVMS.FactuurSturen
{
    public partial class FactuurSturenClient
    {
        Client[] _cachedClients;

        public async Task<Client[]> GetClients(bool allowCache = true)
        {
            if (allowCache && _cachedClients != null)
                return _cachedClients;

            var request = new RestRequest("clients", HttpMethod.Get, ContentTypes.Json);

            var result = await _httpClient.ExecuteWithPolicyAsync<Client[]>(this, request);

            if (allowCache || _cachedClients != null)
                _cachedClients = result;
            return result;
        }
    }
}
