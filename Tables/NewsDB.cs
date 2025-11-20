using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class NewsDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[D-News]";

        private static News GetNews(SqlDataReader reader)
        {
            return new News(Convert.ToInt64(reader["Id"]),
                            Convert.ToInt64(reader["PostId"]),
                            Convert.ToInt64(reader["NewsTypeId"]),
                            Convert.ToInt64(reader["OriginCountryId"]),
                            Convert.ToInt64(reader["OriginStateId"]),
                            reader["NewsSource"].ToString(),
                            reader["NewsUrl"].ToString(),
                            reader["DateTime"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["DateTime"]),
                            Convert.ToDateTime(reader["CreateDateTime"]),
                            Convert.ToDateTime(reader["UpdateDateTime"]),
                            Convert.ToInt32(reader["Status"]));
        }

        // GET
        public async Task<IEnumerable<News>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<News> newss = new List<News>();
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         News news = GetNews(reader);
                         newss.Add(news);
                    }
                }
            }
            return newss;
        }

        public async Task<News> GetById(long id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

            News news = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         news = GetNews(reader);
                    }
                }
            }
            return news;
        }

        // INSERT
        public async Task<long> Add(News news)
        {
            String strCmd = $"INSERT INTO {table}(Id, PostId, NewsTypeId, OriginCountryId, OriginStateId, NewsSource, NewsUrl, DateTime, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@Id, @PostId, @NewsTypeId, @OriginCountryId, @OriginStateId, @NewsSource, @NewsUrl, @DateTime, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, SecurityFunctions.GetUid('~'));
            DBHelper.AddParam(command, "@PostId", SqlDbType.BigInt, news.PostId);
            DBHelper.AddParam(command, "@NewsTypeId", SqlDbType.BigInt, news.NewsTypeId);
            DBHelper.AddParam(command, "@OriginCountryId", SqlDbType.BigInt, news.OriginCountryId);
            DBHelper.AddParam(command, "@OriginStateId", SqlDbType.BigInt, news.OriginStateId);
            DBHelper.AddParam(command, "@NewsSource", SqlDbType.VarChar, news.NewsSource);
            DBHelper.AddParam(command, "@NewsUrl", SqlDbType.VarChar, news.NewsUrl);
            DBHelper.AddParam(command, "@DateTime", SqlDbType.DateTime, news.DateTime);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, news.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (long)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(News news)
        {
            String strCmd = $"UPDATE {table} SET PostId = @PostId, NewsTypeId = @NewsTypeId, OriginCountryId = @OriginCountryId, OriginStateId = @OriginStateId, NewsSource = @NewsSource, NewsUrl = @NewsUrl, DateTime = @DateTime, UpdateDateTime = @UpdateDateTime, Status = @Status WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@PostId", SqlDbType.BigInt, news.PostId);
            DBHelper.AddParam(command, "@NewsTypeId", SqlDbType.BigInt, news.NewsTypeId);
            DBHelper.AddParam(command, "@OriginCountryId", SqlDbType.BigInt, news.OriginCountryId);
            DBHelper.AddParam(command, "@OriginStateId", SqlDbType.BigInt, news.OriginStateId);
            DBHelper.AddParam(command, "@NewsSource", SqlDbType.VarChar, news.NewsSource);
            DBHelper.AddParam(command, "@NewsUrl", SqlDbType.VarChar, news.NewsUrl);
            DBHelper.AddParam(command, "@DateTime", SqlDbType.DateTime, news.DateTime);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, news.Status);
            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, news.Id);

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
