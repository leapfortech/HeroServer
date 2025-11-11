using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class CpeDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[DD-Cpe]";

        private static Cpe GetCpe(SqlDataReader reader)
        {
            return new Cpe(Convert.ToInt32(reader["Id"]),
                           reader["ServiceType"].ToString(),
                           reader["InstitutionName"].ToString(),
                           reader["BeneficiaryName"].ToString(),
                           reader["PositionType"].ToString(),
                           Convert.ToDateTime(reader["CreateDatetime"]),
                           Convert.ToDateTime(reader["UpdateDateTime"]));
        }

        // GET
        public async Task<IEnumerable<Cpe>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<Cpe> cpes = new List<Cpe>();
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         Cpe cpe = GetCpe(reader);
                         cpes.Add(cpe);
                    }
                }
            }
            return cpes;
        }

        public async Task<Cpe> GetById(int id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            Cpe cpe = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         cpe = GetCpe(reader);
                    }
                }
            }
            return cpe;
        }

        // INSERT
        public async Task<int> Add(Cpe cpe)
        {
            String strCmd = $"INSERT INTO {table}(ServiceType, InstitutionName, BeneficiaryName, PositionType, CreateDatetime, UpdateDateTime)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@ServiceType, @InstitutionName, @BeneficiaryName, @PositionType, @CreateDatetime, @UpdateDateTime)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@ServiceType", SqlDbType.VarChar, cpe.ServiceType);
            DBHelper.AddParam(command, "@InstitutionName", SqlDbType.VarChar, cpe.InstitutionName);
            DBHelper.AddParam(command, "@BeneficiaryName", SqlDbType.VarChar, cpe.BeneficiaryName);
            DBHelper.AddParam(command, "@PositionType", SqlDbType.VarChar, cpe.PositionType);
            DBHelper.AddParam(command, "@CreateDatetime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);

            using (conn)
            {
                await conn.OpenAsync();
                return (int)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(Cpe cpe)
        {
            String strCmd = $"UPDATE {table} SET ServiceType = @ServiceType, InstitutionName = @InstitutionName, BeneficiaryName = @BeneficiaryName, PositionType = @PositionType, CreateDatetime = @CreateDatetime, UpdateDateTime = @UpdateDateTime WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@ServiceType", SqlDbType.VarChar, cpe.ServiceType);
            DBHelper.AddParam(command, "@InstitutionName", SqlDbType.VarChar, cpe.InstitutionName);
            DBHelper.AddParam(command, "@BeneficiaryName", SqlDbType.VarChar, cpe.BeneficiaryName);
            DBHelper.AddParam(command, "@PositionType", SqlDbType.VarChar, cpe.PositionType);
            DBHelper.AddParam(command, "@CreateDatetime", SqlDbType.DateTime2, cpe.CreateDatetime);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, cpe.Id);

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
