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
        private List<Product> _cachedProducts;

        public async Task<Product[]> GetProducts(bool allowCache = true)
        {
            if (allowCache && _cachedProducts != null)
                return _cachedProducts.ToArray();

            var request = new RestRequest("products", HttpMethod.Get, ContentTypes.Json);

            var result = await _httpClient.ExecuteWithPolicyAsync<Product[]>(this, request);

            if (allowCache || _cachedProducts != null)
                _cachedProducts = new List<Product>(result);
            return result;
        }

        public async Task<Product> GetProduct(int id, bool allowCache = true)
        {
            if (allowCache && _cachedProducts != null && _cachedProducts.Any(p=>p.Id == id))
                return _cachedProducts.FirstOrDefault(p=>p.Id == id);

            var request = new RestRequest($"products/{id}", HttpMethod.Get, ContentTypes.Json);
            

            var result = await _httpClient.ExecuteWithPolicyAsync<Product>(this, request);
            if (result != null && allowCache)
            {
                StoreInCache(result);
            }
            return result;
        }

        public async Task<Product> CreateProduct(Product product, bool storeInCache = true)
        {
            var request = new RestRequest("products", HttpMethod.Post, ContentTypes.Json)
            {
                ContentType = ContentTypes.Json
            };


            request.AddParameter(new[] { product});
            var response = await _httpClient.SendWithPolicyAsync<object>(this, request);
            if (response.HttpResponseMessage.IsSuccessStatusCode)
            {
                product.Id = Convert.ToInt32(response.Content);
                if (storeInCache)
                {
                    StoreInCache(product);
                }
                return product;
            }
            throw new RequestFailedLibException(response.HttpResponseMessage.StatusCode);
        }

        public async Task<Product> UpdateProduct(Product product, bool storeInCache = true)
        {
            var request = new RestRequest($"products/{product.Id}", HttpMethod.Put, ContentTypes.Json)
            {
                ContentType = ContentTypes.Json,
            };

            request.AddParameter(new[] { product });
            var response = await _httpClient.SendWithPolicyAsync<string>(this, request);
            if (response.HttpResponseMessage.IsSuccessStatusCode)
            {
                if (storeInCache)
                {
                    StoreInCache(product);
                }
                return product;
            }
            throw new RequestFailedLibException(response.HttpResponseMessage.StatusCode);
        }

        public async Task DeleteProduct(Product product, bool updateInCache = true)
        {
            var request = new RestRequest($"products/{product.Id}", HttpMethod.Delete, ContentTypes.Json)
            {
                ContentType = ContentTypes.Json,
            };

            var response = await _httpClient.SendWithPolicyAsync<string>(this, request);
            if (response.HttpResponseMessage.IsSuccessStatusCode)
            {
                if (updateInCache)
                {
                    RemoveFromCache(product);
                }
            }
            throw new RequestFailedLibException(response.HttpResponseMessage.StatusCode);
        }

        private void StoreInCache(Product product)
        {
            if (_cachedProducts == null)
                _cachedProducts = new List<Product>();
            _cachedProducts.Add(product);
        }

        private void RemoveFromCache(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            lock (_cachedProducts)
            {
                if (_cachedProducts != null && _cachedProducts.Any(p => p.Id == product.Id))
                    _cachedProducts.Remove(_cachedProducts.First(p => p.Id == product.Id));
            }
        }
    }
}
