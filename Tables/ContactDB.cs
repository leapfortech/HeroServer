using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class ContactDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[D-Contact]";

        private static Contact GetContact(SqlDataReader reader)
        {
            return new Contact(Convert.ToInt64(reader["Id"]),
                              Convert.ToInt64(reader["ProductId"]),
                              reader["Name"].ToString(),
                              Convert.ToInt64(reader["PhoneCountryId"]),
                              reader["Phone"].ToString(),
                              reader["Email"].ToString(),
                              Convert.ToDateTime(reader["CreateDateTime"]),
                              Convert.ToDateTime(reader["UpdateDateTime"]),
                              Convert.ToInt32(reader["Status"]));
        }

        // GET
        public async Task<IEnumerable<Contact>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<Contact> contacts = new List<Contact>();
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         Contact contact = GetContact(reader);
                         contacts.Add(contact);
                    }
                }
            }
            return contacts;
        }

        public async Task<Contact> GetById(long id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

            Contact contact = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         contact = GetContact(reader);
                    }
                }
            }
            return contact;
        }

        // INSERT
        public async Task<long> Add(Contact contact)
        {
            String strCmd = $"INSERT INTO {table}(Id, ProductId, Name, PhoneCountryId, Phone, Email, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@Id, @ProductId, @Name, @PhoneCountryId, @Phone, @Email, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, SecurityFunctions.GetUid('~'));
            DBHelper.AddParam(command, "@ProductId", SqlDbType.BigInt, contact.ProductId);
            DBHelper.AddParam(command, "@Name", SqlDbType.VarChar, contact.Name);
            DBHelper.AddParam(command, "@PhoneCountryId", SqlDbType.BigInt, contact.PhoneCountryId);
            DBHelper.AddParam(command, "@Phone", SqlDbType.VarChar, contact.Phone);
            DBHelper.AddParam(command, "@Email", SqlDbType.VarChar, contact.Email);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, contact.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (long)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(Contact contact)
        {
            String strCmd = $"UPDATE {table} SET ProductId = @ProductId, Name = @Name, PhoneCountryId = @PhoneCountryId, Phone = @Phone, Email = @Email, UpdateDateTime = @UpdateDateTime, Status = @Status WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@ProductId", SqlDbType.BigInt, contact.ProductId);
            DBHelper.AddParam(command, "@Name", SqlDbType.VarChar, contact.Name);
            DBHelper.AddParam(command, "@PhoneCountryId", SqlDbType.BigInt, contact.PhoneCountryId);
            DBHelper.AddParam(command, "@Phone", SqlDbType.VarChar, contact.Phone);
            DBHelper.AddParam(command, "@Email", SqlDbType.VarChar, contact.Email);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, contact.Status);
            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, contact.Id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateStatus(long id, int status)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, Status = @Status" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);
            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        // DELETE
        public async Task<int> DeleteAll()
        {
            String strCmd = $"DELETE {table}";
            SqlCommand command = new SqlCommand(strCmd, conn);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<bool> DeleteById(long id)
        {
            String strCmd = $"DELETE {table} WHERE Id = @Id";
            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }
    }
}
