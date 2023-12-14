using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactManagerPro.Models
{
    internal class ContactManager
    {
        /*  public void AddContact(string name, string email)
          {
              using (var context = new ContactDbContext())
              {
                  var newContact = new Contact { Name = name, Email = email };
                  context.Contacts.Add(newContact);
                  context.SaveChanges();
              }
          }*/
        // використання асинхронного методу для збереження змін в базі даних:
        public async Task AddContactAsync(string name, string email)
        {
            using (var context = new ContactDbContext())
            {
                var newContact = new Contact { Name = name, Email = email };
                context.Contacts.Add(newContact);
                await context.SaveChangesAsync();
            }
        }
        public async Task DeleteContactAsync(int contactId)
        {
            using (var context = new ContactDbContext())
            {
                var contactToDelete = await context.Contacts.FindAsync(contactId);

                if (contactToDelete != null)
                {
                    context.Contacts.Remove(contactToDelete);
                    await context.SaveChangesAsync();
                }
            }
        }
        public async Task UpdateContactAsync(int contactId, string newName, string newEmail)
        {
            using (var context = new ContactDbContext())
            {
                var contactToUpdate = await context.Contacts.FindAsync(contactId);

                if (contactToUpdate != null)
                {
                    contactToUpdate.Name = newName;
                    contactToUpdate.Email = newEmail;

                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
