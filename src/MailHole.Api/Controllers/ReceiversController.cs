using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MailHole.Common.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace MailHole.Api.Controllers
{
    public class ReceiversController : Controller
    {

        private readonly IDatabaseAsync _redisDb;

        public ReceiversController(IDatabaseAsync redisDb)
        {
            _redisDb = redisDb;
        }

        [HttpGet]
        [Route("api/v1/{receiverAddress}")]
        public IActionResult GetReceiverInfo([FromRoute, EmailAddress, Required] string receiverAddress,
            [FromQuery, Range(0, int.MaxValue)] int offset = 0, [FromQuery, Range(1, 100)] int pageSize = 20)
        {
            if (!ModelState.IsValid) return BadRequest();
            return Ok(receiverAddress);
        }

        [HttpGet]
        [Route("api/v1/{receiverAddress}/mails/{mailGuid}")]
        public async Task<IActionResult> GetMail([FromRoute, EmailAddress, Required] string receiverAddress,
            [FromRoute, Required] Guid? mailGuid)
        {
            if (!ModelState.IsValid) return BadRequest();
            var guidString = mailGuid.HasValue ? mailGuid.Value.ToString() : "";
            if (await _redisDb.HashExistsAsync(receiverAddress, guidString))
            {
                var mail = JsonConvert.DeserializeObject<ReceivedMail>(
                    await _redisDb.HashGetAsync(receiverAddress, guidString));
                return Ok(mail);
            }
            return NotFound();
        }
    }
}