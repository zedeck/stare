using BackendAPI.Database;
using BackendAPI.Services;
using Common.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace BackendAPI.Controllers
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    [MIEAuthorize]
    public class RemindersController : ControllerBase
    {
        private readonly ReminderService _reminderService;

        public RemindersController(ReminderService reminderService)
        {
            _reminderService = reminderService;
        }

        [HttpPost("SendReminder")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> SendReminder(int requestId)
        {
            try
            {
                bool sentWithinLast24Hours = await _reminderService.WasSentWithinLastXHoursForRequest(requestId);

                if (sentWithinLast24Hours)
                {
                    return Ok(new { error = "error", message = "Reminder was already sent within the last 24 hours.", date = DateTime.Now });
                }

                await _reminderService.SendReminders(requestId);

                return Ok(new { message = "Reminder sent" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending reminders: {ex.Message}");
                return BadRequest($"Error sending reminders: {ex.Message}");
            }
        }
        

        [HttpGet]
       //[Route("api/Referees/WasSentWithinLast24Hours")]
        public async Task<IActionResult> WasSentWithinLastXHours(int refereeId)
        {
            try
            {
                bool result = await _reminderService.WasSentWithinLastXHours(refereeId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking if reminder was sent within last 24 hours: {ex.Message}");
                return StatusCode(500, $"Error checking if reminder was sent within last 24 hours: {ex.Message}");
            }
        }

        [HttpPost("SendAllReminder")]
        [ProducesResponseType(typeof(string), 200)] // OK
        [ProducesResponseType(typeof(string), 400)] // Bad Request
        public async Task<IActionResult> SendAllReminder()
        {

            try
            {
                await _reminderService.SendAllReminders();

                return Ok();
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error sending reminders: {ex.Message}");
                return BadRequest($"Error sending reminders: {ex.Message}");
            }
        }


        [HttpPost("SendInternalNotifications")]
        [ProducesResponseType(typeof(string), 200)] // OK
        [ProducesResponseType(typeof(string), 400)] // Bad Request
        public async Task<IActionResult> SendInternalNotifications()
        {

            try
            {
                var authDetails = HttpContext.GetAuthDetails();
                await _reminderService.SendAllInternalNotifications(authDetails.Token);

                return Ok();
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error sending reminders: {ex.Message}");
                return BadRequest($"Error sending reminders: {ex.Message}");
            }
        }


    }
}
