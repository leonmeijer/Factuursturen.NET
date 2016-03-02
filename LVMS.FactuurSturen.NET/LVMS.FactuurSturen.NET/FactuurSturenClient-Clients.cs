using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using LVMS.FactuurSturen.Exceptions;
using LVMS.FactuurSturen.Model;
using PortableRest;

namespace LVMS.FactuurSturen
{
    public partial class FactuurSturenClient
    {
        List<Client> _cachedClients;

        public async Task<Client[]> GetClients(bool? allowCache = true)
        {
            if (!allowCache.HasValue)
                allowCache = _allowResponseCaching;
            if ((bool)allowCache && _cachedClients != null)
                return _cachedClients.ToArray();

            var request = new RestRequest("clients", HttpMethod.Get, ContentTypes.Json);

            var result = await _httpClient.ExecuteWithPolicyAsync<Client[]>(this, request);

            if ((bool)allowCache || _cachedClients != null)
                _cachedClients = new List<Client>(result);
            return result;
        }

        public async Task<Client> GetClient(int clientNr, bool? allowCache = true)
        {
            if (!allowCache.HasValue)
                allowCache = _allowResponseCaching;
            if ((bool)allowCache && _cachedClients != null && _cachedClients.Any(p => p.ClientNr == clientNr))
                return _cachedClients.FirstOrDefault(p => p.ClientNr == clientNr);

            var request = new RestRequest($"clients/{clientNr}", HttpMethod.Get, ContentTypes.Json);

            var result = await _httpClient.ExecuteWithPolicyAsync<Client>(this, request);
            if (result != null && (bool)allowCache)
            {
                StoreInCache(result);
            }
            return result;
        }

        /// <summary>
        /// Gets a client by company name. Finds the client by searching through ALL clients.
        /// </summary>
        /// <param name="companyName">Company name</param>
        /// <param name="allowCache">Whether or not to allow cache</param>
        /// <returns></returns>
        public async Task<Client> GetClient(string companyName, bool? allowCache = true)
        {
            var allClients = await GetClients(allowCache);
            return
                allClients.FirstOrDefault(
                    c => c.Company != null && c.Company.Equals(companyName, StringComparison.CurrentCultureIgnoreCase));
        }

        public async Task<Client> CreateClient(Client client, bool? storeInCache = true)
        {
            if (!storeInCache.HasValue)
                storeInCache = _allowResponseCaching;
            var request = new RestRequest("clients", HttpMethod.Post, ContentTypes.Json)
            {
                ContentType = ContentTypes.Json
            };

            request.AddParameter(new[] { client });
            var response = await _httpClient.SendWithPolicyAsync<object>(this, request);
            if (response.HttpResponseMessage.IsSuccessStatusCode)
            {
                client.ClientNr = Convert.ToInt32(response.Content);
                if ((bool)storeInCache)
                {
                    StoreInCache(client);
                }
                return client;
            }
            throw new RequestFailedLibException(response.HttpResponseMessage.StatusCode);
        }

        public async Task<Client> UpdateClient(Client client, bool? storeInCache = true)
        {
            if (!storeInCache.HasValue)
                storeInCache = _allowResponseCaching;
            var request = new RestRequest($"clients/{client.ClientNr}", HttpMethod.Put, ContentTypes.Json)
            {
                ContentType = ContentTypes.Json,
            };

            request.AddParameter(new[] { client });
            var response = await _httpClient.SendWithPolicyAsync<string>(this, request);
            if (response.HttpResponseMessage.IsSuccessStatusCode)
            {
                if ((bool)storeInCache)
                {
                    StoreInCache(client);
                }
                return client;
            }
            throw new RequestFailedLibException(response.HttpResponseMessage.StatusCode);
        }

        public async Task DeleteClient(Client client)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));
            var clientNr = client.ClientNr;
            await DeleteClient(clientNr);
           
            RemoveClientFromCache(client);
        }

        public async Task DeleteClient(int clientNr)
        {
            var request = new RestRequest($"clients/{clientNr}", HttpMethod.Delete, ContentTypes.Json)
            {
                ContentType = ContentTypes.Json,
            };

            var response = await _httpClient.SendWithPolicyAsync<string>(this, request);
            if (!response.HttpResponseMessage.IsSuccessStatusCode)
            {
                throw new RequestFailedLibException(response.HttpResponseMessage.StatusCode);
            }
            
            RemoveClientFromCache(clientNr);
        }

        private void StoreInCache(Client client)
        {
            if (_cachedClients == null)
                _cachedClients = new List<Client>();
            _cachedClients.Add(client);
        }

        private void RemoveClientFromCache(Client client)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            var clientNr = client.ClientNr;
            RemoveClientFromCache(clientNr);
        }

        private void RemoveClientFromCache(int clientNr)
        {
            if (_cachedClients == null)
                return;
            lock (_cachedClients)
            {
                if (_cachedClients != null && _cachedClients.Any(p => p.ClientNr == clientNr))
                    _cachedClients.Remove(_cachedClients.First(p => p.ClientNr == clientNr));
            }
        }
    }
}
