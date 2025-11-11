using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class MeetingDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[DD-Meeting]";

        private static Meeting GetMeeting(SqlDataReader reader)
        {
            return new Meeting(Convert.ToInt32(reader["Id"]),
                               Convert.ToInt32(reader["BoardUserId"]),
                               Convert.ToInt32(reader["MeetingTypeId"]),
                               reader["Subject"].ToString(),
                               Convert.ToDateTime(reader["StartDateTime"]),
                               Convert.ToDateTime(reader["EndDateTime"]),
                               reader["Description"].ToString(),
                               Convert.ToDateTime(reader["CreateDateTime"]),
                               Convert.ToDateTime(reader["UpdateDateTime"]),
                               Convert.ToInt32(reader["Status"]));
        }

        public static MeetingFull GetMeetingFull(SqlDataReader reader)
        {
            return new MeetingFull(Convert.ToInt32(reader["Id"]),
                                   reader["MeetingType"].ToString(),
                                   reader["Subject"].ToString(),
                                   Convert.ToDateTime(reader["StartDateTime"]),
                                   Convert.ToDateTime(reader["EndDateTime"]),
                                   reader["Description"].ToString(),
                                   Convert.ToInt32(reader["Status"]));
        }

        public static MeetingInfo GetMeetingInfo(SqlDataReader reader)
        {
            return new MeetingInfo(Convert.ToInt32(reader["Id"]),
                                   Convert.ToInt32(reader["BoardUserId"]),
                                   Convert.ToInt32(reader["MeetingTypeId"]),
                                   reader["Subject"].ToString(),
                                   Convert.ToDateTime(reader["StartDateTime"]),
                                   Convert.ToDateTime(reader["EndDateTime"]),
                                   reader["Description"].ToString(),
                                   Convert.ToInt32(reader["Status"]));
        }

        // GET
        public async Task<IEnumerable<Meeting>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<Meeting> meetings = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         Meeting meeting = GetMeeting(reader);
                         meetings.Add(meeting);
                    }
                }
            }
            return meetings;
        }

        public async Task<Meeting> GetById(int id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            Meeting meeting = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         meeting = GetMeeting(reader);
                    }
                }
            }
            return meeting;
        }

        public async Task<IEnumerable<Meeting>> GetByBoardUserId(int boardUserId, int status)
        {
            String strCmd = $"SELECT * FROM {table} WHERE BoardUserId = @BoardUserId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@BoardUserId", SqlDbType.Int, boardUserId);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            List<Meeting> meetings = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Meeting meeting = GetMeeting(reader);
                        meetings.Add(meeting);
                    }
                }
            }
            return meetings;
        }

        public async Task<IEnumerable<Meeting>> GetByStatus(int status)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            List<Meeting> meetings = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Meeting meeting = GetMeeting(reader);
                        meetings.Add(meeting);
                    }
                }
            }
            return meetings;
        }

        public async Task<IEnumerable<Meeting>> GetByDates(DateTime startDateTime, DateTime endDateTime, int status)
        {
            String strCmd = $"SELECT * FROM {table} WHERE StartDateTime >= @StartDateTime AND EndDateTime <= @EndDateTime AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@StartDateTime", SqlDbType.DateTime2, startDateTime);
            DBHelper.AddParam(command, "@EndDateTime", SqlDbType.DateTime2, endDateTime);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            List<Meeting> meetings = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Meeting meeting = GetMeeting(reader);
                        meetings.Add(meeting);
                    }
                }
            }
            return meetings;
        }

        public async Task<List<MeetingInfo>> GetInfos()
        {
            String strCmd = $"SELECT Id, BoardUserId, MeetingTypeId, Subject, StartDateTime, EndDateTime, Description, Status FROM {table}" +
                            //" WHERE Status = @Status";
                            " ORDER BY StartDateTime";

            SqlCommand command = new SqlCommand(strCmd, conn);

            //DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            List<MeetingInfo> meetingInfos = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        MeetingInfo meetingInfo = GetMeetingInfo(reader);
                        meetingInfos.Add(meetingInfo);
                    }
                }
            }
            return meetingInfos;
        }

        public async Task<IEnumerable<MeetingInfo>> GetInfosByDates(DateTime startDateTime, DateTime endDateTime, int status)
        {
            String strCmd = $"SELECT Id, BoardUserId, MeetingTypeId, Subject, StartDateTime, EndDateTime, Description, Status FROM {table}" +
                            " WHERE StartDateTime >= @StartDateTime AND EndDateTime <= @EndDateTime AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@StartDateTime", SqlDbType.DateTime2, startDateTime);
            DBHelper.AddParam(command, "@EndDateTime", SqlDbType.DateTime2, endDateTime);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            List<MeetingInfo> meetingFulls = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        MeetingInfo meetingFull = GetMeetingInfo(reader);
                        meetingFulls.Add(meetingFull);
                    }
                }
            }
            return meetingFulls;
        }

        // INSERT
        public async Task<int> Add(Meeting meeting)
        {
            String strCmd = $"INSERT INTO {table}(BoardUserId, MeetingTypeId, Subject, StartDateTime, EndDateTime, Description, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@BoardUserId, @MeetingTypeId, @Subject, @StartDateTime, @EndDateTime, @Description, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@BoardUserId", SqlDbType.Int, meeting.BoardUserId);
            DBHelper.AddParam(command, "@MeetingTypeId", SqlDbType.Int, meeting.MeetingTypeId);
            DBHelper.AddParam(command, "@Subject", SqlDbType.VarChar, meeting.Subject);
            DBHelper.AddParam(command, "@StartDateTime", SqlDbType.DateTime2, meeting.StartDateTime);
            DBHelper.AddParam(command, "@EndDateTime", SqlDbType.DateTime2, meeting.EndDateTime);
            DBHelper.AddParam(command, "@Description", SqlDbType.VarChar, meeting.Description);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, meeting.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (int)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(Meeting meeting)
        {
            String strCmd = $"UPDATE {table} SET BoardUserId = @BoardUserId, MeetingTypeId = @MeetingTypeId, Subject = @Subject, StartDateTime = @StartDateTime, EndDateTime = @EndDateTime, Description = @Description, UpdateDateTime = @UpdateDateTime WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@BoardUserId", SqlDbType.Int, meeting.BoardUserId);
            DBHelper.AddParam(command, "@MeetingTypeId", SqlDbType.Int, meeting.MeetingTypeId);
            DBHelper.AddParam(command, "@Subject", SqlDbType.VarChar, meeting.Subject);
            DBHelper.AddParam(command, "@StartDateTime", SqlDbType.DateTime2, meeting.StartDateTime);
            DBHelper.AddParam(command, "@EndDateTime", SqlDbType.DateTime2, meeting.EndDateTime);
            DBHelper.AddParam(command, "@Description", SqlDbType.VarChar, meeting.Description);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, meeting.Id);

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
