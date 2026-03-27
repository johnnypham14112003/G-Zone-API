using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace GZone.Service.Services
{
    public class ProductServiceClient
    {
        private readonly HttpClient _http;

        public ProductServiceClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<bool> Prepare(Guid variantId, int quantity)
        {
            var res = await _http.PostAsJsonAsync("api/ProductVariants/reserve/prepare",
                new { variantId, quantity });

            return res.IsSuccessStatusCode;
        }

        public async Task<bool> Commit(Guid variantId, int quantity)
        {
            var res = await _http.PostAsJsonAsync("api/ProductVariants/reserve/commit",
                new { variantId, quantity });

            return res.IsSuccessStatusCode;
        }

        public async Task<bool> Rollback(Guid variantId, int quantity)
        {
            var res = await _http.PostAsJsonAsync("api/ProductVariants/reserve/rollback",
                new { variantId, quantity });

            return res.IsSuccessStatusCode;
        }
    }
}
