using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class ProjectLikeDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[DD-ProjectLike]";

        private static ProjectLike GetProjectLike(SqlDataReader reader)
        {
            return new ProjectLike(Convert.ToInt32(reader["Id"]),
                                   Convert.ToInt32(reader["ProjectId"]),
                                   Convert.ToInt32(reader["AppUserId"]),
                                   Convert.ToDateTime(reader["CreateDateTime"]),
                                   Convert.ToDateTime(reader["UpdateDateTime"]),
                                   Convert.ToInt32(reader["Status"]));
        }

        // GET
        public async Task<IEnumerable<ProjectLike>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<ProjectLike> rojectLikes = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         ProjectLike rojectLike = GetProjectLike(reader);
                         rojectLikes.Add(rojectLike);
                    }
                }
            }
            return rojectLikes;
        }

        public async Task<ProjectLike> GetById(int id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            ProjectLike projectLike = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         projectLike = GetProjectLike(reader);
                    }
                }
            }
            return projectLike;
        }

        public async Task<IEnumerable<ProjectLike>> GetByProjectId(int projectId, int status)
        {
            String strCmd = $"SELECT * FROM {table} WHERE ProjectId = @ProjectId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@ProjectId", SqlDbType.Int, projectId);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            List<ProjectLike> projectLikes = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        ProjectLike projectLike = GetProjectLike(reader);
                        projectLikes.Add(projectLike);
                    }
                }
            }
            return projectLikes;
        }

        public async Task<List<int>> GetIdsByAppUserId(int appUserId, int status)
        {
            String strCmd = $"SELECT ProjectId FROM {table} WHERE AppUserId = @AppUserId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, appUserId);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            List<int> projectIds = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        int rojectId = Convert.ToInt32(reader["ProjectId"]);
                        projectIds.Add(rojectId);
                    }
                }
            }
            return projectIds;
        }

        public async Task<ProjectLike> GetByIds(int projectId, int appUserId, int status = 1)
        {
            String strCmd = $"SELECT * FROM {table} WHERE ProjectId = @ProjectId AND AppUserId = @AppUserId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@ProjectId", SqlDbType.Int, projectId);
            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, appUserId);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            ProjectLike projectLike = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        projectLike = GetProjectLike(reader);
                    }
                }
            }
            return projectLike;
        }

        // INSERT
        public async Task<int> Add(ProjectLike projectLike)
        {
            String strCmd = $"INSERT INTO {table}(ProjectId, AppUserId, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@ProjectId, @AppUserId, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@ProjectId", SqlDbType.Int, projectLike.ProjectId);
            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, projectLike.AppUserId);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, projectLike.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (int)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(ProjectLike projectLike)
        {
            String strCmd = $"UPDATE {table} SET ProjectId = @ProjectId, AppUserId = @AppUserId, UpdateDateTime = @UpdateDateTime WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@ProjectId", SqlDbType.Int, projectLike.ProjectId);
            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, projectLike.AppUserId);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, projectLike.Id);

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
