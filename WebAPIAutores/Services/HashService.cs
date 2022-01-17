using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using WebAPIAutores.DTOs;

namespace WebAPIAutores.Services
{
    public class HashService
    {
        public HashResult Hash(string plainText)
        {
            var sal = new byte[16];
            using (var random = RandomNumberGenerator.Create())
            {
                random.GetBytes(sal);
            }

            return Hash(plainText, sal);
        }

        public HashResult Hash(string plainText, byte[] sal)
        {
            var deliveratedKey = KeyDerivation.Pbkdf2(password: plainText,
                salt: sal, prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 32);

            var hash = Convert.ToBase64String(deliveratedKey);

            return new HashResult()
            {
                Hash = hash,
                Sal = sal
            };
        }
    }
}
