using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class PuzzleAnswerDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[D-PuzzleAnswer]";

        public static PuzzleAnswer GetPuzzleAnswer(SqlDataReader reader)
        {
            return new PuzzleAnswer(Convert.ToInt64(reader["Id"]),
                                    Convert.ToInt64(reader["PuzzleId"]),
                                    reader["Description"].ToString(),
                                    Convert.ToInt32(reader["IsCorrect"]),
                                    Convert.ToDateTime(reader["CreateDateTime"]),
                                    Convert.ToDateTime(reader["UpdateDateTime"]),
                                    Convert.ToInt32(reader["Status"]));
        }

        public static PuzzleAnswerFull GetPuzzleAnswerFull(SqlDataReader reader)
        {
            return new PuzzleAnswerFull(Convert.ToInt64(reader["Id"]),
                                        reader["Description"].ToString(),
                                        Convert.ToInt32(reader["IsCorrect"]),
                                        Convert.ToInt32(reader["Status"]));
        }

        // GET
        public async Task<IEnumerable<PuzzleAnswer>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<PuzzleAnswer> puzzleAnswers = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         PuzzleAnswer puzzleAnswer = GetPuzzleAnswer(reader);
                         puzzleAnswers.Add(puzzleAnswer);
                    }
                }
            }
            return puzzleAnswers;
        }

        public async Task<PuzzleAnswer> GetById(long id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Id", SqlDbType.BigInt, id);

            PuzzleAnswer puzzleAnswer = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         puzzleAnswer = GetPuzzleAnswer(reader);
                    }
                }
            }
            return puzzleAnswer;
        }

        public async Task<PuzzleAnswer> GetCorrectByPuzzleId(long puzzleId)
        {
            String strCmd = $"SELECT TOP 1 * " +
                            $"FROM {table} " +
                            $"WHERE PuzzleId = @PuzzleId " +
                            $"AND IsCorrect = 1 " +
                            $"AND Status = 1";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@PuzzleId", SqlDbType.BigInt, puzzleId);

            PuzzleAnswer puzzleAnswer = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        puzzleAnswer = GetPuzzleAnswer(reader);
                    }
                }
            }
            return puzzleAnswer;
        }



        // INSERT
        public async Task<long> Add(PuzzleAnswer puzzleAnswer)
        {
            String strCmd = $"INSERT INTO {table}(Id, PuzzleId, Description, IsCorrect, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@Id, @PuzzleId, @Description, @IsCorrect, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@Id", SqlDbType.BigInt, SecurityFunctions.GetUid('z'));
            command.AddParam("@PuzzleId", SqlDbType.BigInt, puzzleAnswer.PuzzleId);
            command.AddParam("@Description", SqlDbType.VarChar, puzzleAnswer.Description);
            command.AddParam("@IsCorrect", SqlDbType.Int, puzzleAnswer.IsCorrect);
            command.AddParam("@CreateDateTime", SqlDbType.DateTime, DateTime.Now);
            command.AddParam("@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            command.AddParam("@Status", SqlDbType.Int, puzzleAnswer.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (long)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(PuzzleAnswer puzzleAnswer)
        {
            String strCmd = $"UPDATE {table} SET PuzzleId = @PuzzleId, Description = @Description, IsCorrect = @IsCorrect, UpdateDateTime = @UpdateDateTime WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@PuzzleId", SqlDbType.BigInt, puzzleAnswer.PuzzleId);
            command.AddParam("@Description", SqlDbType.VarChar, puzzleAnswer.Description);
            command.AddParam("@IsCorrect", SqlDbType.Int, puzzleAnswer.IsCorrect);
            command.AddParam("@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);   
            command.AddParam("@Id", SqlDbType.BigInt, puzzleAnswer.Id);

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

            command.AddParam("@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            command.AddParam("@Status", SqlDbType.Int, status);
            command.AddParam("@Id", SqlDbType.BigInt, id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateStatusByPuzzleId(long puzzleId, int curStatus, int newStatus)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, Status = @NewStatus" +
                            " WHERE PuzzleId = @PuzzleId AND Status = @CurStatus";

            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            command.AddParam("@PuzzleId", SqlDbType.BigInt, puzzleId);
            command.AddParam("@CurStatus", SqlDbType.Int, curStatus);
            command.AddParam("@NewStatus", SqlDbType.Int, newStatus);

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

            command.AddParam("@Id", SqlDbType.BigInt, id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> DeleteByPuzzleId(long puzzleId)
        {
            String strCmd = $"DELETE {table} WHERE PuzzleId = @PuzzleId";
            SqlCommand command = new SqlCommand(strCmd, conn);

            command.AddParam("@PuzzleId", SqlDbType.BigInt, puzzleId);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }
    }
}
