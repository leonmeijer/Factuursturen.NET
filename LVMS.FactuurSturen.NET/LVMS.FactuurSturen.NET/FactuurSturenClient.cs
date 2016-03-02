using System;
using System.Net.Http;
using System.Threading.Tasks;
using LVMS.FactuurSturen.Exceptions;
using LVMS.FactuurSturen.Interfaces;
using LVMS.FactuurSturen.Model;
using Newtonsoft.Json;
using PortableRest;
using PortableRest.Authentication;
using Newtonsoft.Json.Converters;

namespace LVMS.FactuurSturen
{
    public partial class FactuurSturenClient : IFactuurSturenClient
    {
        protected string ApiUrl = "https://www.factuursturen.nl:443/api/v1/";
        private RestClient _httpClient;
        
        private bool _initialized;
        internal bool UsePollyTransientFaultHandling;


        /// <summary>
        /// Initializes a new FactuurSturenClient instance with a custom API url.
        /// </summary>
        /// <param name="apiUrl">Endpoint address of the FactuurSturen.nl REST API</param>
        /// <param name="usePollyTransientFaultHandling">Whether or not to use transient fault handling</param>
        public FactuurSturenClient(string apiUrl = null, bool usePollyTransientFaultHandling = true)
        {
            if (apiUrl != null)
                ApiUrl = apiUrl;
         
            UsePollyTransientFaultHandling = usePollyTransientFaultHandling;
        }

        /// <summary>
        /// Checks whether or not this library and the connection with FactuurSturen.nl is initialized.
        /// </summary>
        public void CheckInitialized()
        {
            if (!_initialized)
                throw new FactuurSturenLibException();
        }

        /// <summary>
        /// Authenticate this client with the FactuurSturen API.
        /// </summary>
        /// <param name="userName">User name (typically the email address)</param>
        /// <param name="apiKey">Password</param>
        public async Task LoginAsync(string userName, string apiKey)
        {
            _httpClient = new RestClient
            {
                BaseUrl = ApiUrl,
                Authenticator = new BasicAuthenticator(userName, apiKey)
            };
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new LowerCaseContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
            };
            
            _httpClient.JsonSerializerSettings = jsonSerializerSettings;

            //var loginInfo = new Login() {Username = userName, Password = apiKey};
            var initRequest = new RestRequest("clients", HttpMethod.Get, ContentTypes.Json);

            var response = await _httpClient.SendWithPolicyAsync<Client[]>(this, initRequest, byPassCheckInitialized: true);
            if (response.HttpResponseMessage.IsSuccessStatusCode)
            {
                _initialized = true;
            }
            else
            {
                throw new AuthenticationFailureLibException("API didn't return success code");
            }
        }

        public Task<bool> CheckConnection()
        {
            throw new NotImplementedException();
        }
    }
}
