using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class FaqDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[D-Faq]";

        private static Faq GetFaq(SqlDataReader reader)
        {
            return new Faq(Convert.ToInt64(reader["Id"]),
                           Convert.ToInt64(reader["BoardUserId"]),
                           Convert.ToInt64(reader["FaqTypeId"]),
                           reader["Question"].ToString(),
                           reader["Answer"].ToString(),
                           Convert.ToDateTime(reader["CreateDateTime"]),
                           Convert.ToDateTime(reader["UpdateDateTime"]),
                           Convert.ToInt32(reader["Status"]));
        }

        // GET
        public async Task<IEnumerable<Faq>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<Faq> faqs = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         Faq faq = GetFaq(reader);
                         faqs.Add(faq);
                    }
                }
            }
            return faqs;
        }

        public async Task<List<Faq>> GetByType(long faqTypeId)
        {
            String strCmd = $"SELECT * FROM {table} WHERE FaqTypeId = @FaqTypeId";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@FaqTypeId", SqlDbType.BigInt, faqTypeId);

            List<Faq> faqs = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Faq faq = GetFaq(reader);
                        faqs.Add(faq);
                    }
                }
            }
            return faqs;
        }

        public async Task<Faq> GetById(long id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

            Faq faq = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         faq = GetFaq(reader);
                    }
                }
            }
            return faq;
        }

        // INSERT
        public async Task<long> Add(Faq faq)
        {
            String strCmd = $"INSERT INTO {table}(BoardUserId, FaqTypeId, Question, Answer, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@BoardUserId, @FaqTypeId, @Question, @Answer, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@BoardUserId", SqlDbType.BigInt, faq.BoardUserId);
            DBHelper.AddParam(command, "@FaqTypeId", SqlDbType.BigInt, faq.FaqTypeId);
            DBHelper.AddParam(command, "@Question", SqlDbType.VarChar, faq.Question);
            DBHelper.AddParam(command, "@Answer", SqlDbType.VarChar, faq.Answer);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, faq.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (long)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(Faq faq)
        {
            String strCmd = $"UPDATE {table} SET BoardUserId = @BoardUserId, FaqTypeId = @FaqTypeId, Question = @Question, Answer = @Answer, UpdateDateTime = @UpdateDateTime WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@BoardUserId", SqlDbType.BigInt, faq.BoardUserId);
            DBHelper.AddParam(command, "@FaqTypeId", SqlDbType.BigInt, faq.FaqTypeId);
            DBHelper.AddParam(command, "@Question", SqlDbType.VarChar, faq.Question);
            DBHelper.AddParam(command, "@Answer", SqlDbType.VarChar, faq.Answer);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, faq.Id);

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
