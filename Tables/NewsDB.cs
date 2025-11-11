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
        readonly String table = "[DD-News]";

        public static News GetNews(SqlDataReader reader)
        {
            return new News(Convert.ToInt32(reader["Id"]),
                            reader["Title"].ToString(),
                            reader["Description"].ToString(),
                            reader["Link"].ToString(),
                            reader["Content"].ToString(),
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

        public async Task<News> GetById(int id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

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

        public async Task<List<News>> GetByStatus(int status)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

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

        // INSERT
        public async Task<int> Add(News news)
        {
            String strCmd = $"INSERT INTO {table}(Title, Description, Link, Content, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@Title, @Description, @Link, @Content, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Title", SqlDbType.VarChar, news.Title);
            DBHelper.AddParam(command, "@Description", SqlDbType.VarChar, news.Description);
            DBHelper.AddParam(command, "@Link", SqlDbType.VarChar, news.Link);
            DBHelper.AddParam(command, "@Content", SqlDbType.VarChar, news.Content);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, news.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (int)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(News news)
        {
            String strCmd = $"UPDATE {table} SET Title = @Title, Description = @Description, Link = @Link, Content = @Content, UpdateDateTime = @UpdateDateTime WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Title", SqlDbType.VarChar, news.Title);
            DBHelper.AddParam(command, "@Description", SqlDbType.VarChar, news.Description);
            DBHelper.AddParam(command, "@Link", SqlDbType.VarChar, news.Link);
            DBHelper.AddParam(command, "@Content", SqlDbType.VarChar, news.Content);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, news.Id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateStatus(int id, int status)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, Status = @Status" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

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

        public async Task<bool> DeleteById(int id)
        {
            String strCmd = $"DELETE {table} WHERE Id = @Id";
            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }
    }
}
