using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace HeroServer
{
    public class AppointmentFunctions
    {
        // GET
        public static async Task<Appointment> GetById(int id)
        {
            return await new AppointmentDB().GetById(id);
        }

        public static async Task<List<Appointment>> GetByMeetingId(int meetingId, int status = 1)
        {
            return await new AppointmentDB().GetByMeetingId(meetingId, status);
        }

        public static async Task<List<(int, String)>> GetMailsByMeetingId(int meetingId)
        {
            return await new AppointmentDB().GetMailsByMeetingId(meetingId);
        }

        // ADD
        public static async Task<int> Register(Appointment appointment)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                int appointmentCount = await new AppointmentDB().GetCountByIds(appointment.MeetingId, appointment.AppUserId);
                
                if (appointmentCount != 0)
                    throw new Exception("Ya está registrado al evento.");

                appointment.Status = 1;
                appointment.Id = await new AppointmentDB().Add(appointment);

                await SendEmails(appointment);

                scope.Complete();
            }
            return appointment.Id;
        }

        public static async Task<int> Add(Appointment appointment)
        {
            return await new AppointmentDB().Add(appointment);
        }

        // UPDATE
        public static async Task<bool> Update(Appointment appointment)
        {
            return await new AppointmentDB().Update(appointment);
        }

        public static async Task<bool> UpdateStatus(int id, int status)
        {
            return await new AppointmentDB().UpdateStatus(id, status);
        }

        public static async Task SendEmails(Appointment appointment)
        {
            Meeting meeting = await MeetingFunctions.GetById(appointment.MeetingId);

            await SendAppUserEmail(appointment.AppUserId, meeting.Subject, meeting.StartDateTime);
            await SendBoardUserEmail(meeting.BoardUserId, meeting.Subject, meeting.StartDateTime);
        }

        // Email
        public static async Task<int> SendAppUserEmail(int appUserId, String subject, DateTime startDateTime)
        {
            //AppUserNamed appUserNamed = await new AppUserDB().GetNamedById(appointment.AppUserId);
            int webSysUserId = await new AppUserDB().GetWebSysUserId(appUserId);
            WebSysUser webSysUser = await new WebSysUserDB().GetById(webSysUserId);

            //String appUserName = appUserNamed.FirstName1 + " " + appUserNamed.LastName1;

            String body =  "Fuiste registrado satisfactoriamente al evento.<br><br>" + 
                          //$"Nombre: {webSysUser.}<br>" +
                          $"Fecha y hora: {startDateTime.AddHours(-6):dd/MM/yyyy HH:mm}<br>(hora de Guatemala)";  // JAD

            String message = HtmlHelper.GetConfirmResultHtml(subject, body, "#666666");
            if (message == null)
                return 3;

            try
            {
                await MailHelper.SendMail(webSysUser.Email, webSysUser.Email, $"Evento Expande: {subject}", message, true);
            }
            catch
            {
                return 2;
            }

            return 1;
        }

        public static async Task<int> SendBoardUserEmail(int boardUserId, String subject, DateTime startDateTime)
        {
            //AppUserNamed appUserNamed = await new AppUserDB().GetNamedById(appointment.AppUserId);
            int webSysUserId = await new BoardUserDB().GetWebSysUserId(boardUserId);
            WebSysUser webSysUser = await new WebSysUserDB().GetById(webSysUserId);

            //String appUserName = appUserNamed.FirstName1 + " " + appUserNamed.LastName1;

            String body =  "Un nuevo usuario se ha registrado al evento.<br><br>" +
                          //$"Nombre: {webSysUser.}<br>" +
                          $"Teléfono: {webSysUser.Phone}<br>" +
                          $"Email: {webSysUser.Email}<br><br>" +
                          $"Fecha y hora: {startDateTime.AddHours(-6):dd/MM/yyyy HH:mm}<br>(hora de Guatemala)";  // JAD

            String message = HtmlHelper.GetConfirmResultHtml(subject, body, "#666666");
            if (message == null)
                return 3;

            BoardUser boardUser = await BoardUserFunctions.GetById(boardUserId);
            String boardUserName = boardUser == null ? webSysUser.Email : boardUser.GetCompleteName();

            try
            {
                await MailHelper.SendMail(webSysUser.Email, boardUserName, $"Evento Expande: {subject}", message, true);
            }
            catch
            {
                return 2;
            }

            return 1;
        }

        // DELETE
        public static async Task DeleteById(int id)
        {
            await new AppointmentDB().DeleteById(id);
        }

        public static async Task DeleteByAppUserId(int appUserId)
        {
            await new AppointmentDB().DeleteByAppUserId(appUserId);
        }
    }
}
