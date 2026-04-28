using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeroServer.Controllers
{
    [Route("services/wa")]
    [ApiController]
    public class WAController : Controller
    {
        private readonly String VERIFY_TOKEN = "AS12565E85D654AE856585DDDA69EHJP";

        // GET services/wa/webhook
        [AllowAnonymous]
        [HttpGet("webhook")]
        public IActionResult VerifyWebhook(
            [FromQuery(Name = "hub.mode")] String mode,
            [FromQuery(Name = "hub.verify_token")] String verifyToken,
            [FromQuery(Name = "hub.challenge")] String challenge)
        {
            try
            {
                if (mode == "subscribe" && verifyToken == VERIFY_TOKEN)
                {
                    return Ok(challenge);
                }

                return Forbid();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost("webhook")]
        public async Task<IActionResult> ReceiveWebhook([FromBody] JsonElement body)
        {
            try
            {
                JsonElement entry;

                if (body.TryGetProperty("entry", out entry))
                {
                    foreach (JsonElement e in entry.EnumerateArray())
                    {
                        JsonElement changes = e.GetProperty("changes");

                        foreach (JsonElement change in changes.EnumerateArray())
                        {
                            JsonElement value = change.GetProperty("value");

                            // STATUS
                            JsonElement statuses;
                            if (value.TryGetProperty("statuses", out statuses))
                            {
                                foreach (JsonElement status in statuses.EnumerateArray())
                                {
                                    String messageId = status.GetProperty("id").GetString();
                                    String state = status.GetProperty("status").GetString();

                                    String statusInfo = null;

                                    JsonElement errors;

                                    if (status.TryGetProperty("errors", out errors))
                                    {
                                        foreach (JsonElement err in errors.EnumerateArray())
                                        {
                                            String code = err.GetProperty("code").ToString();
                                            String title = err.GetProperty("title").GetString();

                                            statusInfo = state + "|" + code;// + "|" + title;
                                        }
                                    }
                                    else
                                    {
                                        statusInfo = state;
                                    }

                                    await PrecheckFunctions.ProcessWAStatus(messageId, statusInfo);
                                }
                            }

                            // MESSAGES
                            JsonElement messages;
                            if (value.TryGetProperty("messages", out messages))
                            {
                                foreach (JsonElement msg in messages.EnumerateArray())
                                {
                                    String from = msg.GetProperty("from").GetString();
                                    String type = msg.GetProperty("type").GetString();
                                }
                            }
                        }
                    }
                }

                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}