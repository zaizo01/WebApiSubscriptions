using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WebAPIAutores.Controllers.V1;
using WebAPIAutores.DTOs;
using WebAPIAutores.Entities;

namespace WebAPIAutores.Controllers
{
    [Route("api/DomainRestriction")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class DomainRestrictionController : CustomBaseController
    {
        private readonly ApplicationDbContext context;

        public DomainRestrictionController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpPost]
        public async Task<ActionResult> PostDomainRestriction(PostRestrictionDomainDTO postRestrictionDomainDTO)
        {
            var keyDB = await context.KeysAPI.FirstOrDefaultAsync(key => key.Id == postRestrictionDomainDTO.KeyId);
            if (keyDB == null) return NotFound();

            var userId = GetUserId();
            if (keyDB.UserId != userId) return Forbid();

            var domainRestriction = new DomainRestriction()
            {
                KeyId = postRestrictionDomainDTO.KeyId,
                Domain = postRestrictionDomainDTO.Domain
            };

            context.Add(domainRestriction);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> PutDomainRestriction(int id, PutDomainRestrictionDTO putDomainRestrictionDTO)
        {
            var restrictionDB = await context.DomainRestrictions.Include(x => x.Key)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (restrictionDB == null) return NotFound();

            var userId = GetUserId();
            if (restrictionDB.Key.UserId != userId) return Forbid();

            restrictionDB.Domain = putDomainRestrictionDTO.Domain;
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteDomainRestriction(int id)
        {
            var restrictionDB = await context.DomainRestrictions.Include(x => x.Key)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (restrictionDB == null) return NotFound();

            var userId = GetUserId();
            if (restrictionDB.Key.UserId != userId) return Forbid();

            context.Remove(restrictionDB);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
