using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class AppointmentDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[DD-Appointment]";

        private static Appointment GetAppointment(SqlDataReader reader)
        {
            return new Appointment(Convert.ToInt32(reader["Id"]),
                                   Convert.ToInt32(reader["MeetingId"]),
                                   Convert.ToInt32(reader["AppUserId"]),
                                   Convert.ToDateTime(reader["CreateDateTime"]),
                                   Convert.ToDateTime(reader["UpdateDateTime"]),
                                   Convert.ToInt32(reader["Status"]));
        }

        // GET
        public async Task<IEnumerable<Appointment>> GetAll()
        {
            String strCmd = $"SELECT * FROM {table}";

            SqlCommand command = new SqlCommand(strCmd, conn);

            List<Appointment> appointments = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         Appointment appointment = GetAppointment(reader);
                         appointments.Add(appointment);
                    }
                }
            }
            return appointments;
        }

        public async Task<Appointment> GetById(int id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.Int, id);

            Appointment appointment = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         appointment = GetAppointment(reader);
                    }
                }
            }
            return appointment;
        }

        public async Task<int> GetCountByIds(int meetingId, int appUserId)
        {
            String strCmd = $"SELECT Count(1) AS Count FROM {table} WHERE MeetingId = @MeetingId AND AppUserId = @AppUserId";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@MeetingId", SqlDbType.Int, meetingId);
            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, appUserId);

            int count = -1;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        count = Convert.ToInt32(reader["Count"]);
                    }
                }
            }
            return count;
        }

        public async Task<List<Appointment>> GetByMeetingId(int meetingId, int status = 1)
        {
            String strCmd = $"SELECT * FROM {table} WHERE MeetingId = @MeetingId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@MeetingId", SqlDbType.Int, meetingId);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            List<Appointment> appointments = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Appointment appointment = GetAppointment(reader);
                        appointments.Add(appointment);
                    }
                }
            }
            return appointments;
        }

        public async Task<IEnumerable<Appointment>> GetByAppUserId(int appUserId, int status = 1)
        {
            String strCmd = $"SELECT * FROM {table} WHERE AppUserId = @AppUserId AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, appUserId);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            List<Appointment> appointments = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Appointment appointment = GetAppointment(reader);
                        appointments.Add(appointment);
                    }
                }
            }
            return appointments;
        }

        public async Task<List<(int, String)>> GetMailsByMeetingId(int meetingId)  // , int status = 1)
        {
            String strCmd = $"SELECT AppUserId, Email FROM {table}" +
                            " INNER JOIN [DD-AppUser] AS AppUser ON (AppUser.Id = AppUserId)" +
                            " INNER JOIN [DD-WebSysUser] AS WebSysUser ON (WebSysUser.Id = WebSysUserId)" +
                            " WHERE MeetingId = @MeetingId";
                            //" AND Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@MeetingId", SqlDbType.Int, meetingId);
            //DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            List<(int, String)> mails = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        mails.Add((Convert.ToInt32(reader["AppUserId"]), reader["Email"].ToString()));
                    }
                }
            }
            return mails;
        }

        // INSERT
        public async Task<int> Add(Appointment appointment)
        {
            String strCmd = $"INSERT INTO {table}(MeetingId, AppUserId, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@MeetingId, @AppUserId, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@MeetingId", SqlDbType.Int, appointment.MeetingId);
            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, appointment.AppUserId);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, appointment.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (int)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(Appointment appointment)
        {
            String strCmd = $"UPDATE {table} SET MeetingId = @MeetingId, AppUserId = @AppUserId, UpdateDateTime = @UpdateDateTime WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@MeetingId", SqlDbType.Int, appointment.MeetingId);
            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, appointment.AppUserId);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@Id", SqlDbType.Int, appointment.Id);

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

        public async Task<bool> DeleteByAppUserId(int appUserId)
        {
            String strCmd = $"DELETE {table} WHERE AppUserId = @AppUserId";
            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@AppUserId", SqlDbType.Int, appUserId);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }
    }
}
