using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class PepIdentityDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[DD-PepIdentity]";

        private static PepIdentity GetPepIdentity(SqlDataReader reader)
        {
            return new PepIdentity(Convert.ToInt32(reader["Id"]),
                                   Convert.ToInt32(reader["IdentityId"]),
                                   Convert.ToInt32(reader["RelationshipTypeId"]),
                                   reader["RelatioshipTypeDescription"].ToString(),
                                   Convert.ToInt32(reader["PartnershipMotiveId"]),
                                   reader["PartnershipMotiveDescription"].ToString(),
                                   reader["InstitutionName"].ToString(),
                                   Convert.ToInt32(reader["InstitutionCountryId"]),
                                   reader["JobTitle"].ToString(),
                                   Convert.ToInt32(reader["IsForeigner"]),
                                   Convert.ToDateTime(reader["CreateDatetime"]),
                                   Convert.ToDateTime(reader["UpdateDateTime"]));
        }

        // GET
        public async Task<IEnumerable<PepIdentity>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<PepIdentity> pepIdentities = new List<PepIdentity>();
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         PepIdentity pepIdentity = GetPepIdentity(reader);
                         pepIdentities.Add(pepIdentity);
                    }
                }
            }
            return pepIdentities;
        }

        public async Task<PepIdentity> GetById(int id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            PepIdentity pepIdentity = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         pepIdentity = GetPepIdentity(reader);
                    }
                }
            }
            return pepIdentity;
        }

        // INSERT
        public async Task<int> Add(PepIdentity pepIdentity)
        {
            String strCmd = $"INSERT INTO {table}(IdentityId, RelationshipTypeId, RelatioshipTypeDescription, PartnershipMotiveId, PartnershipMotiveDescription, InstitutionName, InstitutionCountryId, JobTitle, IsForeigner, CreateDatetime, UpdateDateTime)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@IdentityId, @RelationshipTypeId, @RelatioshipTypeDescription, @PartnershipMotiveId, @PartnershipMotiveDescription, @InstitutionName, @InstitutionCountryId, @JobTitle, @IsForeigner, @CreateDatetime, @UpdateDateTime)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@IdentityId", SqlDbType.Int, pepIdentity.IdentityId);
            DBHelper.AddParam(command, "@RelationshipTypeId", SqlDbType.Int, pepIdentity.RelationshipTypeId);
            DBHelper.AddParam(command, "@RelatioshipTypeDescription", SqlDbType.VarChar, pepIdentity.RelatioshipTypeDescription);
            DBHelper.AddParam(command, "@PartnershipMotiveId", SqlDbType.Int, pepIdentity.PartnershipMotiveId);
            DBHelper.AddParam(command, "@PartnershipMotiveDescription", SqlDbType.VarChar, pepIdentity.PartnershipMotiveDescription);
            DBHelper.AddParam(command, "@InstitutionName", SqlDbType.VarChar, pepIdentity.InstitutionName);
            DBHelper.AddParam(command, "@InstitutionCountryId", SqlDbType.Int, pepIdentity.InstitutionCountryId);
            DBHelper.AddParam(command, "@JobTitle", SqlDbType.VarChar, pepIdentity.JobTitle);
            DBHelper.AddParam(command, "@IsForeigner", SqlDbType.Int, pepIdentity.IsForeigner);
            DBHelper.AddParam(command, "@CreateDatetime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);

            using (conn)
            {
                await conn.OpenAsync();
                return (int)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(PepIdentity pepIdentity)
        {
            String strCmd = $"UPDATE {table} SET IdentityId = @IdentityId, RelationshipTypeId = @RelationshipTypeId, RelatioshipTypeDescription = @RelatioshipTypeDescription, PartnershipMotiveId = @PartnershipMotiveId, PartnershipMotiveDescription = @PartnershipMotiveDescription, InstitutionName = @InstitutionName, InstitutionCountryId = @InstitutionCountryId, JobTitle = @JobTitle, IsForeigner = @IsForeigner, CreateDatetime = @CreateDatetime, UpdateDateTime = @UpdateDateTime WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@IdentityId", SqlDbType.Int, pepIdentity.IdentityId);
            DBHelper.AddParam(command, "@RelationshipTypeId", SqlDbType.Int, pepIdentity.RelationshipTypeId);
            DBHelper.AddParam(command, "@RelatioshipTypeDescription", SqlDbType.VarChar, pepIdentity.RelatioshipTypeDescription);
            DBHelper.AddParam(command, "@PartnershipMotiveId", SqlDbType.Int, pepIdentity.PartnershipMotiveId);
            DBHelper.AddParam(command, "@PartnershipMotiveDescription", SqlDbType.VarChar, pepIdentity.PartnershipMotiveDescription);
            DBHelper.AddParam(command, "@InstitutionName", SqlDbType.VarChar, pepIdentity.InstitutionName);
            DBHelper.AddParam(command, "@InstitutionCountryId", SqlDbType.Int, pepIdentity.InstitutionCountryId);
            DBHelper.AddParam(command, "@JobTitle", SqlDbType.VarChar, pepIdentity.JobTitle);
            DBHelper.AddParam(command, "@IsForeigner", SqlDbType.Int, pepIdentity.IsForeigner);
            DBHelper.AddParam(command, "@CreateDatetime", SqlDbType.DateTime2, pepIdentity.CreateDatetime);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, pepIdentity.Id);

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
