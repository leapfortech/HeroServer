using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class ProjectInformationDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[DD-ProjectInformation]";

        public static ProjectInformation GetProjectInformation(SqlDataReader reader)
        {
            return new ProjectInformation(Convert.ToInt32(reader["Id"]),
                                          Convert.ToInt32(reader["ProjectId"]),
                                          Convert.ToInt32(reader["ProjectInformationTypeId"]),
                                          reader["Information"].ToString(),
                                          Convert.ToDateTime(reader["CreateDateTime"]),
                                          Convert.ToDateTime(reader["UpdateDateTime"]),
                                          Convert.ToInt32(reader["Status"]));
        }

        // GET
        public async Task<IEnumerable<ProjectInformation>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<ProjectInformation> projectInformations = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         ProjectInformation projectInformation = GetProjectInformation(reader);
                         projectInformations.Add(projectInformation);
                    }
                }
            }
            return projectInformations;
        }

        public async Task<ProjectInformation> GetById(int id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            ProjectInformation projectInformation = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         projectInformation = GetProjectInformation(reader);
                    }
                }
            }
            return projectInformation;
        }

        public async Task<List<ProjectInformation>> GetByProjectId(int projectId, int status = 1)
        {
            String strCmd = $"SELECT * FROM {table} WHERE ProjectId = @ProjectId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@ProjectId", SqlDbType.Int, projectId);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            List<ProjectInformation> projectInformations = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        ProjectInformation projectInformation = GetProjectInformation(reader);
                        projectInformations.Add(projectInformation);
                    }
                }
            }
            return projectInformations;
        }

        // INSERT
        public async Task<int> Add(ProjectInformation projectInformation)
        {
            String strCmd = $"INSERT INTO {table}(ProjectId, ProjectInformationTypeId, Information, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@ProjectId, @ProjectInformationTypeId, @Information, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@ProjectId", SqlDbType.Int, projectInformation.ProjectId);
            DBHelper.AddParam(command, "@ProjectInformationTypeId", SqlDbType.Int, projectInformation.ProjectInformationTypeId);
            DBHelper.AddParam(command, "@Information", SqlDbType.VarChar, projectInformation.Information);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, projectInformation.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (int)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(ProjectInformation projectInformation, bool status = false)
        {
            String strCmd = $"UPDATE {table} SET ProjectId = @ProjectId, ProjectInformationTypeId = @ProjectInformationTypeId, Information = @Information, UpdateDateTime = @UpdateDateTime" +
                             (status ? ", Status = @Status" : "") +
                             " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@ProjectId", SqlDbType.Int, projectInformation.ProjectId);
            DBHelper.AddParam(command, "@ProjectInformationTypeId", SqlDbType.Int, projectInformation.ProjectInformationTypeId);
            DBHelper.AddParam(command, "@Information", SqlDbType.VarChar, projectInformation.Information);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            if (status)
                DBHelper.AddParam(command, "@Status", SqlDbType.Int, projectInformation.Status);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, projectInformation.Id);

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

        public async Task<bool> UpdateStatusByProjectId(int projectId, int status)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, Status = @Status" +
                            " WHERE ProjectId = @ProjectId";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);
            DBHelper.AddParam(command, "@ProjectId", SqlDbType.Int, projectId);

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
