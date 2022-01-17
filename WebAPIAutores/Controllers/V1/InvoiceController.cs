using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPIAutores.DTOs;

namespace WebAPIAutores.Controllers
{
    [Route("api/invoice")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public InvoiceController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpPost]
        public async Task<ActionResult> Pagar(PayInvoiceDTO payInvoice)
        {
            var invoiceDB = await context.Commercialnvoices
                                .Include(x => x.User)
                                .FirstOrDefaultAsync(x => x.Id == payInvoice.InvoiceId);

            if (invoiceDB == null)
            {
                return NotFound();
            }

            if (invoiceDB.Paid)
            {
                return BadRequest("The invoice has already been paid");
            }

            // Lógica para pagar la factura
            // nosotros vamos a pretender que el pago fue exitoso

            invoiceDB.Paid = true;
            await context.SaveChangesAsync();

            var ThereareInvoicesPendingOverdue = await context.Commercialnvoices
                .AnyAsync(x => x.UserId == invoiceDB.UserId &&
                !x.Paid && x.PayDayLimit < DateTime.Today);

            if (!ThereareInvoicesPendingOverdue)
            {
                invoiceDB.User.PoorPay = false;
                await context.SaveChangesAsync();
            }

            return NoContent();
        }
    }
}
