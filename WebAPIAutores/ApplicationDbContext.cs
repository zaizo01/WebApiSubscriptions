using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPIAutores.Entities;

namespace WebAPIAutores
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AutorLibro>()
                .HasKey(al => new { al.AutorId, al.LibroId });

            modelBuilder.Entity<Commercialnvoice>()
                .Property(x => x.Amount).HasColumnType("decimal(18,2)");

        }

        // Tables
        public DbSet<Autor> Autores { get; set; }
        public DbSet<Libro> Libros { get; set; }
        public DbSet<Comment> Comentarios { get; set; }
        public DbSet<AutorLibro> AutoresLibros { get; set; }
        public DbSet<KeyAPI> KeysAPI { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<DomainRestriction> DomainRestrictions { get; set; }
        public DbSet<IPRestriction> IPRestrictions { get; set; }
        public DbSet<Commercialnvoice> Commercialnvoices { get; set; }
        public DbSet<InvoiceIssued> InvoiceIssueds { get; set; }
        
    }
}
