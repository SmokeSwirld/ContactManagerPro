using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactManagerPro.Models
{
    internal class ContactDbContext : DbContext
    {
        // DbSet для контактів
        public DbSet<Contact> Contacts { get; set; }

        // Перевизначення методу OnConfiguring для вказівки конфігурації EF
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Використовуйте SQLite як провайдер бази даних
            optionsBuilder.UseSqlite("Data Source=contacts.db");
        }
    }
}
