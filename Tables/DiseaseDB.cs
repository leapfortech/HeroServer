using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class DiseaseDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[J-Disease]";

        private static Disease GetDisease(SqlDataReader reader)
        {
            return new Disease(Convert.ToInt64(reader["Id"]),
                               Convert.ToInt64(reader["TreatmentId"]),
                               Convert.ToInt64(reader["DiseaseTypeId"]),
                               Convert.ToDateTime(reader["CreateDateTime"]),
                               Convert.ToDateTime(reader["UpdateDateTime"]),
                               Convert.ToInt32(reader["Status"]));
        }

        public static DiseaseFull GetDiseaseFull(SqlDataReader reader)
        {
            return new DiseaseFull(Convert.ToInt64(reader["Id"]),
                                   Convert.ToInt64(reader["DiseaseTypeId"]),
                                   Convert.ToInt32(reader["Status"]));
        }

        // GET
        public async Task<IEnumerable<Disease>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<Disease> diseases = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         Disease disease = GetDisease(reader);
                         diseases.Add(disease);
                    }
                }
            }
            return diseases;
        }

        public async Task<IEnumerable<Disease>> GetAllByTreatmentId(long treatmentId, int status = 1)
        {
            String strCmd = $"SELECT * FROM {table} WHERE TreatmentId = @TreatmentId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@TreatmentId", SqlDbType.BigInt, treatmentId);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            List<Disease> diseases = new List<Disease>();
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Disease disease = GetDisease(reader);
                        diseases.Add(disease);
                    }
                }
            }
            return diseases;
        }

        public async Task<Disease> GetById(long id, int status = 1)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            Disease disease = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         disease = GetDisease(reader);
                    }
                }
            }
            return disease;
        }

        public async Task<Treatment> GetByTreatmentId(long treatmentId, int status = 1)
        {
            String strCmd = $"SELECT * FROM {table} WHERE PostId = @PostId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@TreatmentId", SqlDbType.BigInt, treatmentId);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            Treatment treatment = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        treatment = TreatmentDB.GetTreatment(reader);
                    }
                }
            }
            return treatment;
        }

        public async Task<long> GetIdByTreatmentId(long treatmentId, int status = 1)
        {
            String strCmd = $"SELECT Id FROM {table} WHERE TreatmentId = @TreatmentId";
            if (status != -1)
                strCmd += " AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@TreatmentId", SqlDbType.BigInt, treatmentId);
            if (status != -1)
                DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            long diseaseId = -1;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        diseaseId = Convert.ToInt64(reader["Id"]);
                    }
                }
            }
            return diseaseId;
        }

        // INSERT
        public async Task<long> Add(Disease disease)
        {
            String strCmd = $"INSERT INTO {table}(TreatmentId, DiseaseTypeId, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@TreatmentId, @DiseaseTypeId, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@TreatmentId", SqlDbType.BigInt, disease.TreatmentId);
            DBHelper.AddParam(command, "@DiseaseTypeId", SqlDbType.BigInt, disease.DiseaseTypeId);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, disease.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (long)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(Disease disease)
        {
            String strCmd = $"UPDATE {table} SET TreatmentId = @TreatmentId, DiseaseTypeId = @DiseaseTypeId, UpdateDateTime = @UpdateDateTime, Status = @Status WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@TreatmentId", SqlDbType.BigInt, disease.TreatmentId);
            DBHelper.AddParam(command, "@DiseaseTypeId", SqlDbType.BigInt, disease.DiseaseTypeId);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, disease.Status);
            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, disease.Id);

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

        public async Task<bool> UpdateStatus(long id, int curStatus, int newStatus)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, Status = @NewStatus" +
                            " WHERE Id = @Id AND Status = @CurStatus";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@CurStatus", SqlDbType.Int, curStatus);
            DBHelper.AddParam(command, "@NewStatus", SqlDbType.Int, newStatus);
            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateStatusByTreatmentId(long treatmentId, int curStatus, int newStatus)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, Status = @NewStatus" +
                            " WHERE TreatmentId = @TreatmentId AND Status = @CurStatus";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@TreatmentId", SqlDbType.BigInt, treatmentId);
            DBHelper.AddParam(command, "@CurStatus", SqlDbType.Int, curStatus);
            DBHelper.AddParam(command, "@NewStatus", SqlDbType.Int, newStatus);

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
