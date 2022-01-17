using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WebAPIAutores.Services
{
    public class InvoiceHostedService : IHostedService
    {
        private readonly IServiceProvider serviceProvider;
        private Timer timer;

        public InvoiceHostedService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(ProcessInvoice, null, TimeSpan.Zero, TimeSpan.FromDays(1));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            timer.Dispose();
            return Task.CompletedTask;
        }

        public void ProcessInvoice(object state)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                SetUserPaidPoor(context);
                InvoiceIssueds(context);
            }
        }

        private static void SetUserPaidPoor(ApplicationDbContext context)
        {
            context.Database.ExecuteSqlRaw("exec SET_USER_POOR_PAY");
        }

        private static void InvoiceIssueds(ApplicationDbContext context)
        {
            var today = DateTime.Today;
            var dateComparasion = today.AddMonths(-1);
            var invoiceIssueds = context.InvoiceIssueds.Any(x => x.Year == dateComparasion.Year && x.Month == dateComparasion.Month);

            if (!invoiceIssueds)
            {
                var startDate = new DateTime(dateComparasion.Year, dateComparasion.Month, 1);
                var endDate = startDate.AddMonths(1);
                context.Database.ExecuteSqlInterpolated($"exec INVOICE_CREATION {startDate.ToString("yyyy-MM-dd")}, {endDate.ToString("yyyy-MM-dd")}");
            }
        }
    }
}
