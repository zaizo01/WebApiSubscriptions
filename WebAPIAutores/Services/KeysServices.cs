using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPIAutores.Entities;

namespace WebAPIAutores.Services
{
    public class KeysServices
    {
        private readonly ApplicationDbContext context;

        public KeysServices(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task CreateKey(string userId, KeyType keyType)
        {
            var key = GenerateKey();

            var keyAPI = new KeyAPI
            {
                IsActive = true,
                Key = key,
                KeyType = keyType,
                UserId = userId
            };

            context.Add(keyAPI);
            await context.SaveChangesAsync();
        }

        public string GenerateKey()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }
    }
}
