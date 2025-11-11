using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace HeroServer.Controllers
{
    [Route("services/onboarding")]
    [Authorize("FirebaseAccess")]
    [ApiController]
    public class OnboardingController : Controller
    {
        // GET services/onboarding?appUserId=1
        [HttpGet]
        public async Task<ActionResult<List<Onboarding>>> GetByAppUserId([FromQuery]String appUserId)
        {
            return Ok(await OnboardingFunctions.GetByAppUserId(Convert.ToInt32(appUserId)));
        }

        // GET services/onboarding/All?appUserId=1
        [HttpGet("All")]
        public async Task<ActionResult<List<Onboarding>>> GetAllByAppUserId([FromQuery]String appUserId)
        {
            return Ok(await OnboardingFunctions.GetAllByAppUserId(Convert.ToInt32(appUserId)));
        }

        // ADD

        // POST services/onboarding
        [HttpPost]
        public async Task<ActionResult<int>> Add([FromBody]Onboarding onboarding)
        {
            try
            {
                return Ok(await OnboardingFunctions.Add(onboarding));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // UPDATE

        // PUT services/onboarding
        [HttpPut]
        public async Task<ActionResult<int>> UpdateOnboarding([FromBody]Onboarding onboarding)
        {
            try
            {
                return Ok(await OnboardingFunctions.UpdateOnboarding(onboarding));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/onboarding/DpiFront?appUserId=1
        [HttpPut("DpiFront")]
        public async Task<ActionResult> UpdateDpiFront([FromQuery]String appUserId, [FromBody]String dpiPhotos)
        {
            try
            {
                await OnboardingFunctions.UpdateDpiFront(Convert.ToInt32(appUserId), dpiPhotos);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/onboarding/DpiBack?appUserId=2
        [HttpPut("DpiBack")]
        public async Task<ActionResult> UpdateDpiBack([FromQuery]String appUserId, [FromBody]String dpiBack)
        {
            try
            {
                await OnboardingFunctions.UpdateDpiBack(Convert.ToInt32(appUserId), dpiBack);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/onboarding/IdentityInfo
        [HttpPut("IdentityInfo")]
        public async Task<ActionResult<int>> UpdateIdentityInfo([FromBody]IdentityInfo identityInfo)
        {
            try
            {
                return Ok(await OnboardingFunctions.UpdateIdentityInfo(identityInfo));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/onboarding/Portrait?appUserId=3
        [HttpPut("Portrait")]
        public async Task<ActionResult> UpdatePortrait([FromQuery]String appUserId, [FromBody]String portrait)
        {
            try
            {
                await OnboardingFunctions.UpdatePortrait(Convert.ToInt32(appUserId), portrait);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/onboarding/HouseholdBills?appUserId=4
        [HttpPut("HouseholdBills")]
        public async Task<ActionResult> UpdateHouseholdBills([FromQuery]String appUserId, [FromBody]String[] householdBills)
        {
            try
            {
                await OnboardingFunctions.UpdateHouseholdBills(Convert.ToInt32(appUserId), householdBills);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // VALIDATE

        // PUT services/onboarding/Authorize?onboardingId=1&appUserId=2
        [HttpPut("Authorize")]
        public async Task<ActionResult> Authorize([FromQuery]String onboardingId, [FromQuery]String appUserId)
        {
            try
            {
                await OnboardingFunctions.Authorize(Convert.ToInt32(onboardingId), Convert.ToInt32(appUserId));
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT services/onboarding/Reject?onboardingId=1&appUserId=2
        [HttpPut("Reject")]
        public async Task<ActionResult> Reject([FromQuery]String onboardingId, [FromQuery]String appUserId)
        {
            try
            {
                await OnboardingFunctions.Reject(Convert.ToInt32(onboardingId), Convert.ToInt32(appUserId));
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
