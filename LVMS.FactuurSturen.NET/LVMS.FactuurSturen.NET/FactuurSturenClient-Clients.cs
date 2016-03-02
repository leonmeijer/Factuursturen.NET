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
        List<Client> _cachedClients;

        public async Task<Client[]> GetClients(bool allowCache = true)
        {
            if (allowCache && _cachedClients != null)
                return _cachedClients.ToArray();

            var request = new RestRequest("clients", HttpMethod.Get, ContentTypes.Json);

            var result = await _httpClient.ExecuteWithPolicyAsync<Client[]>(this, request);

            if (allowCache || _cachedClients != null)
                _cachedClients = new List<Client>(result);
            return result;
        }

        public async Task<Client> GetClient(int clientNr, bool allowCache = true)
        {
            if (allowCache && _cachedClients != null && _cachedClients.Any(p => p.ClientNr == clientNr))
                return _cachedClients.FirstOrDefault(p => p.ClientNr == clientNr);

            var request = new RestRequest($"clients/{clientNr}", HttpMethod.Get, ContentTypes.Json);

            var result = await _httpClient.ExecuteWithPolicyAsync<Client>(this, request);
            if (result != null && allowCache)
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
        public async Task<Client> GetClient(string companyName, bool allowCache = true)
        {
            var allClients = await GetClients(allowCache);
            return
                allClients.FirstOrDefault(
                    c => c.Company != null && c.Company.Equals(companyName, StringComparison.CurrentCultureIgnoreCase));
        }

        public async Task<Client> CreateClient(Client client, bool storeInCache = true)
        {
            var request = new RestRequest("clients", HttpMethod.Post, ContentTypes.Json)
            {
                ContentType = ContentTypes.Json
            };

            request.AddParameter(new[] { client });
            var response = await _httpClient.SendWithPolicyAsync<object>(this, request);
            if (response.HttpResponseMessage.IsSuccessStatusCode)
            {
                client.ClientNr = Convert.ToInt32(response.Content);
                if (storeInCache)
                {
                    StoreInCache(client);
                }
                return client;
            }
            throw new RequestFailedLibException(response.HttpResponseMessage.StatusCode);
        }

        public async Task<Client> UpdateClient(Client client, bool storeInCache = true)
        {
            var request = new RestRequest($"clients/{client.ClientNr}", HttpMethod.Put, ContentTypes.Json)
            {
                ContentType = ContentTypes.Json,
            };

            request.AddParameter(new[] { client });
            var response = await _httpClient.SendWithPolicyAsync<string>(this, request);
            if (response.HttpResponseMessage.IsSuccessStatusCode)
            {
                if (storeInCache)
                {
                    StoreInCache(client);
                }
                return client;
            }
            throw new RequestFailedLibException(response.HttpResponseMessage.StatusCode);
        }

        public async Task DeleteClient(Client client, bool updateInCache = true)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));
            var clientNr = client.ClientNr;
            await DeleteClient(clientNr, updateInCache);

            if (updateInCache)
            {
                RemoveFromCache(client);
            }
        }

        public async Task DeleteClient(int clientNr, bool updateInCache = true)
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
        }

        private void StoreInCache(Client client)
        {
            if (_cachedClients == null)
                _cachedClients = new List<Client>();
            _cachedClients.Add(client);
        }

        private void RemoveFromCache(Client client)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            lock (_cachedClients)
            {
                if (_cachedClients != null && _cachedClients.Any(p => p.ClientNr == client.ClientNr))
                    _cachedClients.Remove(_cachedClients.First(p => p.ClientNr == client.ClientNr));
            }
        }
    }
}
