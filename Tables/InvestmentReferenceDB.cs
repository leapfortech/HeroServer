using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class InvestmentReferenceDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[DD-InvestmentReference]";

        private static InvestmentReference GetInvestmentReference(SqlDataReader reader)
        {
            return new InvestmentReference(
                             Convert.ToInt32(reader["Id"]),
                             Convert.ToInt32(reader["AppUserId"]),
                             Convert.ToInt32(reader["InvestmentId"]),
                             Convert.ToInt32(reader["ReferenceTypeId"]),
                             reader["Name"].ToString(),
                             Convert.ToInt32(reader["PhoneCountryId"]),
                             reader["Phone"].ToString(),
                             reader["Email"].ToString(),
                             reader["Description"].ToString(),
                             Convert.ToDateTime(reader["CreateDateTime"]),
                             Convert.ToDateTime(reader["UpdateDateTime"]),
                             Convert.ToInt32(reader["Status"])
             );
        }

        // GET
        public async Task<IEnumerable<InvestmentReference>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<InvestmentReference> investmentReferences = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         InvestmentReference investmentReference = GetInvestmentReference(reader);
                         investmentReferences.Add(investmentReference);
                    }
                }
            }
            return investmentReferences;
        }

        public async Task<InvestmentReference> GetById(int id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            InvestmentReference investmentReference = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         investmentReference = GetInvestmentReference(reader);
                    }
                }
            }
            return investmentReference;
        }

        public async Task<IEnumerable<InvestmentReference>> GetByAppUserId(int appUserId, int status)
        {
            String strCmd = $"SELECT * FROM {table} WHERE AppUserId = @AppUserId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, appUserId);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            List<InvestmentReference> investmentReferences = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        InvestmentReference investmentReference = GetInvestmentReference(reader);
                        investmentReferences.Add(investmentReference);
                    }
                }
            }
            return investmentReferences;
        }

        public async Task<IEnumerable<InvestmentReference>> GetByInvestmentId(int investmentId, int status)
        {
            String strCmd = $"SELECT * FROM {table} WHERE InvestmentId = @InvestmentId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@InvestmentId", SqlDbType.Int, investmentId);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            List<InvestmentReference> investmentReferences = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        InvestmentReference investmentReference = GetInvestmentReference(reader);
                        investmentReferences.Add(investmentReference);
                    }
                }
            }
            return investmentReferences;
        }

        // INSERT
        public async Task<int> Add(InvestmentReference investmentReference)
        {
            String strCmd = $"INSERT INTO {table}(AppUserId, InvestmentId, ReferenceTypeId, Name, PhoneCountryId, Phone, Email, Description, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@AppUserId, @InvestmentId, @ReferenceTypeId, @Name, @PhoneCountryId, @Phone, @Email, @Description, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, investmentReference.AppUserId);
            DBHelper.AddParam(command, "@InvestmentId", SqlDbType.Int, investmentReference.InvestmentId);
            DBHelper.AddParam(command, "@ReferenceTypeId", SqlDbType.Int, investmentReference.ReferenceTypeId);
            DBHelper.AddParam(command, "@Name", SqlDbType.VarChar, investmentReference.Name);
            DBHelper.AddParam(command, "@PhoneCountryId", SqlDbType.Int, investmentReference.PhoneCountryId);
            DBHelper.AddParam(command, "@Phone", SqlDbType.VarChar, investmentReference.Phone);
            DBHelper.AddParam(command, "@Email", SqlDbType.VarChar, investmentReference.Email);
            DBHelper.AddParam(command, "@Description", SqlDbType.VarChar, investmentReference.Description);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, investmentReference.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (int)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(InvestmentReference investmentReference)
        {
            String strCmd = $"UPDATE {table} SET AppUserId = @AppUserId, InvestmentId = @InvestmentId, ReferenceTypeId = @ReferenceTypeId, Name = @Name, PhoneCountryId = @PhoneCountryId, Phone = @Phone, Email = @Email, Description = @Description, UpdateDateTime = @UpdateDateTime WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, investmentReference.AppUserId);
            DBHelper.AddParam(command, "@InvestmentId", SqlDbType.Int, investmentReference.InvestmentId);
            DBHelper.AddParam(command, "@ReferenceTypeId", SqlDbType.Int, investmentReference.ReferenceTypeId);
            DBHelper.AddParam(command, "@Name", SqlDbType.VarChar, investmentReference.Name);
            DBHelper.AddParam(command, "@PhoneCountryId", SqlDbType.Int, investmentReference.PhoneCountryId);
            DBHelper.AddParam(command, "@Phone", SqlDbType.VarChar, investmentReference.Phone);
            DBHelper.AddParam(command, "@Email", SqlDbType.VarChar, investmentReference.Email);
            DBHelper.AddParam(command, "@Description", SqlDbType.VarChar, investmentReference.Description);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, investmentReference.Id);

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

        public async Task<bool> DeleteByInvestmentId(int investmentId)
        {
            String strCmd = $"DELETE {table} WHERE InvestmentId = @InvestmentId";
            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@InvestmentId", SqlDbType.Int, investmentId);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }
    }
}
