using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HeroServer
{
    public class ContactFunctions
    {
        // GET
        public static async Task<List<Contact>> GetAll()
        {
            return await new ContactDB().GetAll();
        }

        public static async Task<Contact> GetById(long id)
        {
            return await new ContactDB().GetById(id);
        }

        // REGISTER
        public static async Task<long> Register(long postId, Contact contact)
        {
            contact.PostId = postId;
            contact.Status = 1;

            return await Add(contact);
        }

        // ADD
        public static async Task<long> Add(Contact contact)
        {
            return await new ContactDB().Add(contact);
        }

        // UPDATE
        public static async Task<bool> Update(Contact contact)
        {
            return await new ContactDB().Update(contact);
        }

        public static async Task<bool> UpdateStatus(long id, int status)
        {
            return await new ContactDB().UpdateStatus(id, status);
        }

        // DELETE

        public static async Task Delete(long id)
        {
            await new ContactDB().DeleteById(id);
        }
    }
}