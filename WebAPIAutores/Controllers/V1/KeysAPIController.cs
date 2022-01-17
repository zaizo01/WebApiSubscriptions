using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPIAutores.DTOs;
using WebAPIAutores.Services;

namespace WebAPIAutores.Controllers.V1
{
    [Route("api/KeysAPI")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class KeysAPIController : CustomBaseController
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly KeysServices keysServices;

        public KeysAPIController(ApplicationDbContext context, IMapper mapper, KeysServices keysServices)
        {
            this.context = context;
            this.mapper = mapper;
            this.keysServices = keysServices;
        }

        [HttpGet]
        public async Task<List<KeyDTO>> MyKeys()
        {
            var userId = GetUserId();
            var keys = await context.KeysAPI
                .Include(x => x.DomainRestriction)
                .Include(x => x.IPRestriction)
                .Where(user => user.UserId == userId).ToListAsync();
            return mapper.Map<List<KeyDTO>>(keys);
        }

        [HttpPost]
        public async Task<ActionResult> CreateKey([FromBody] KeyPostDTO keyPostDTO)
        {
            var userId = GetUserId();

            if (keyPostDTO.KeyType == Entities.KeyType.Free)
            {
                var haveAFreeKeyUser = await context.KeysAPI
                    .AnyAsync(x => x.UserId == userId && x.KeyType == Entities.KeyType.Free);

                if (haveAFreeKeyUser) return BadRequest("The user have a key free");
            }

            await keysServices.CreateKey(userId, keyPostDTO.KeyType);
            return NoContent();
        }

        [HttpPut]
        public async Task<ActionResult> PutKey(KeyPutDTO keyPutDTO)
        {
            var userId = GetUserId();

            var keyDB = await context.KeysAPI.FirstOrDefaultAsync(key => key.Id == keyPutDTO.KeyId);

            if (keyDB == null) return NotFound();

            if (userId != keyDB.UserId) return Forbid();

            if (keyPutDTO.UpdateKey)
            {
                keyDB.Key = keysServices.GenerateKey();
            }

            keyDB.IsActive = keyPutDTO.IsActive;
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
