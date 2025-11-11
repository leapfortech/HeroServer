using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace HeroServer
{
    public class MeetingFunctions
    {
        // GET
        public static async Task<Meeting> GetById(int id)
        {
            return await new MeetingDB().GetById(id);
        }

        public static async Task<IEnumerable<Meeting>> GetByBoardUserId(int boardUserId, int status = 1)
        {
            return await new MeetingDB().GetByBoardUserId(boardUserId, status);
        }

        public static async Task<IEnumerable<Meeting>> GetByStatus(int status)
        {
            return await new MeetingDB().GetByStatus(status);
        }

        public static async Task<IEnumerable<Meeting>> GetByDates(DateTime startDateTime, DateTime endDateTime)
        {
            return await new MeetingDB().GetByDates(startDateTime, endDateTime, 1);
        }

        public static async Task<List<MeetingInfo>> GetInfos()
        {
            List<MeetingInfo> meetingsFull = await new MeetingDB().GetInfos();

            for (int i = 0; i < meetingsFull.Count; i++)
            {
                meetingsFull[i].Appointments = [];
                List<(int AppUserId, String Email)> appointments = await AppointmentFunctions.GetMailsByMeetingId(meetingsFull[i].Id);
                for (int k = 0; k < appointments.Count; k++)
                {
                    Identity identity = await new IdentityDB().GetByAppUserId(appointments[k].AppUserId);
                    if (identity == null)
                        meetingsFull[i].Appointments.Add(appointments[k].Email);
                    else
                        meetingsFull[i].Appointments.Add($"{identity.FirstName1}{(identity.FirstName2 == null ? "" : " " + identity.FirstName2)} {identity.LastName1}{(identity.LastName2 == null ? "" : " " + identity.LastName2)}");
                }
            }

            return meetingsFull;
        }

        public static async Task<IEnumerable<MeetingInfo>> GetInfosByDates(DateTime startDateTime, DateTime endDateTime)
        {
            return await new MeetingDB().GetInfosByDates(startDateTime, endDateTime, 1);
        }

        // Register
        public static async Task<int> Register(Meeting meeting)
        {
            meeting.Status = 1;
            return await Add(meeting);
        }

        // ADD
        public static async Task<int> Add(Meeting meeting)
        {
            return await new MeetingDB().Add(meeting);
        }

        // UPDATE
        public static async Task<bool> Update(Meeting meeting)
        {
            return await new MeetingDB().Update(meeting);
        }

        public static async Task<bool> UpdateStatus(int id, int status)
        {
            return await new MeetingDB().UpdateStatus(id, status);
        }
    }
}
