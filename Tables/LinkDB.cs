using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class LinkDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[D-Link]";

        private static Link GetLink(SqlDataReader reader)
        {
            return new Link(Convert.ToInt64(reader["Id"]),
                            Convert.ToInt64(reader["LinkTypeId"]),
                            Convert.ToInt64(reader["PostId"]),
                            reader["Url"].ToString(),
                            Convert.ToDateTime(reader["CreateDateTime"]),
                            Convert.ToDateTime(reader["UpdateDateTime"]),
                            Convert.ToInt32(reader["Status"]));
        }

        public static LinkFull GetLinkFull(SqlDataReader reader)
        {
            return new LinkFull(Convert.ToInt64(reader["Id"]),
                                Convert.ToInt64(reader["LinkTypeId"]),
                                reader["Url"].ToString(),
                                Convert.ToInt32(reader["Status"]));
        }

        // GET
        public async Task<List<Link>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<Link> links = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         Link link = GetLink(reader);
                         links.Add(link);
                    }
                }
            }
            return links;
        }

        public async Task<Link> GetById(long id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

            Link link = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         link = GetLink(reader);
                    }
                }
            }
            return link;
        }

        // INSERT
        public async Task<long> Add(Link link)
        {
            String strCmd = $"INSERT INTO {table}(LinkTypeId, PostId, Url, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@LinkTypeId, @PostId, @Url, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@LinkTypeId", SqlDbType.BigInt, link.LinkTypeId);
            DBHelper.AddParam(command, "@PostId", SqlDbType.BigInt, link.PostId);
            DBHelper.AddParam(command, "@Url", SqlDbType.VarChar, link.Url);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, link.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (long)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(Link link)
        {
            String strCmd = $"UPDATE {table} SET LinkTypeId = @LinkTypeId, PostId = @PostId, Url = @Url, UpdateDateTime = @UpdateDateTime, Status = @Status WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@LinkTypeId", SqlDbType.BigInt, link.LinkTypeId);
            DBHelper.AddParam(command, "@PostId", SqlDbType.BigInt, link.PostId);
            DBHelper.AddParam(command, "@Url", SqlDbType.VarChar, link.Url);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, link.Status);
            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, link.Id);

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
