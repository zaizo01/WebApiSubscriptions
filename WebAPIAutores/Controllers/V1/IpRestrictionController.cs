using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPIAutores.Controllers.V1;
using WebAPIAutores.DTOs;
using WebAPIAutores.Entities;

namespace WebAPIAutores.Controllers
{
    [Route("api/IpRestriction")]
    [ApiController]
    public class IpRestrictionController : CustomBaseController
    {
        private readonly ApplicationDbContext context;

        public IpRestrictionController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpPost]
        public async Task<ActionResult> PostIpRestriction(PostIpRestrictionDTO postIpRestrictionDTO)
        {
            var keyDB = await context.KeysAPI.FirstOrDefaultAsync(key => key.Id == postIpRestrictionDTO.KeyId);
            if (keyDB == null) return NotFound();

            var userId = GetUserId();
            if (keyDB.UserId != userId) return Forbid();

            var ipRestriction = new IPRestriction()
            {
                KeyId = postIpRestrictionDTO.KeyId,
                IP = postIpRestrictionDTO.IP
            };

            context.Add(ipRestriction);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> PutIPRestriction(int id, PutIPRestrictionDTO putIPRestrictionDTO)
        {
            var restrictionDB = await context.IPRestrictions.Include(x => x.Key)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (restrictionDB == null) return NotFound();

            var userId = GetUserId();
            if (restrictionDB.Key.UserId != userId) return Forbid();

            restrictionDB.IP = putIPRestrictionDTO.IP;
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteIPRestriction(int id)
        {
            var restrictionDB = await context.IPRestrictions.Include(x => x.Key)
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
