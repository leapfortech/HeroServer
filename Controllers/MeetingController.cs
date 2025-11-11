using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace HeroServer.Controllers
{
    [Route("services/meeting")]
    [Authorize("FirebaseAccess")]
    [ApiController]
    public class MeetingController : Controller
    {
        // GET services/meeting/ById?id=1
        [HttpGet("ById")]
        public async Task<ActionResult<Meeting>> GetById([FromQuery]String id)
        {
            return Ok(await MeetingFunctions.GetById(Convert.ToInt32(id)));
        }

        // GET services/meeting/ByBoardUserId?appUserId=1
        [HttpGet("ByBoardUserId")]
        public async Task<ActionResult<IEnumerable<Meeting>>> GetByBaordUserId([FromQuery]String boardUserId)
        {
            return Ok(await MeetingFunctions.GetByBoardUserId(Convert.ToInt32(boardUserId)));
        }

        // GET services/meeting/ByDates?startDateTime=&endDateTime=
        [HttpGet("ByDates")]
        public async Task<ActionResult<IEnumerable<Meeting>>> GetByDates([FromQuery]String startDateTime, [FromQuery]String endDateTime)
        {
            return Ok(await MeetingFunctions.GetByDates(Convert.ToDateTime(startDateTime), Convert.ToDateTime(endDateTime)));
        }

        // GET services/meeting/Infos?
        [HttpGet("Infos")]
        public async Task<ActionResult<IEnumerable<MeetingInfo>>> GetInfos()
        {
            try
            {
                return Ok(await MeetingFunctions.GetInfos());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET services/meeting/InfosByDates?startDateTime=&endDateTime=
        [HttpGet("InfosByDates")]
        public async Task<ActionResult<IEnumerable<MeetingInfo>>> GetInfosByDates([FromQuery]String startDateTime, [FromQuery]String endDateTime)
        {
            return Ok(await MeetingFunctions.GetInfosByDates(Convert.ToDateTime(startDateTime), Convert.ToDateTime(endDateTime)));
        }

        // POST services/meeting/Register
        [HttpPost("Register")]
        public async Task<ActionResult<int>> Register([FromBody]Meeting meeting)
        {
            try
            {
                return Ok(await MeetingFunctions.Register(meeting));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST services/meeting/RegisterAppointment
        [HttpPost("RegisterAppointment")]
        public async Task<ActionResult<int>> RegisterAppointment([FromBody]Appointment appointment)
        {
            try
            {
                return Ok(await AppointmentFunctions.Register(appointment));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/meeting
        [HttpPut]
        public async Task<ActionResult<bool>> Update([FromBody]Meeting meeting)
        {
            try
            {
                return Ok(await MeetingFunctions.Update(meeting));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/meeting/UpdateAppointmentStatus?id=5&status=1
        [HttpPut("UpdateAppointmentStatus")]
        public async Task<ActionResult> UpdateAppointmentStatus([FromQuery]String id, [FromQuery]String status)
        {
            try
            {
                return Ok(await AppointmentFunctions.UpdateStatus(Convert.ToInt32(id), Convert.ToInt32(status)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}