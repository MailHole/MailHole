using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MailHole.Api.Models;
using MailHole.Common.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace MailHole.Api.Controllers
{
    public class MailsController : Controller
    {
        
        private readonly IDatabaseAsync _redisDb;

        public MailsController(IDatabaseAsync redisDb)
        {
            _redisDb = redisDb;
        }

        [HttpGet]
        [Route("api/v1/{receiverAddress}/mails")]
        [ProducesResponseType(typeof(List<string>), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 404)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> GetMails([FromRoute, EmailAddress, Required] string receiverAddress,[FromQuery, Range(0, int.MaxValue)] int offset = 0, [FromQuery, Range(1, 100)] int pageSize = 20)
        {
            return BadRequest();
        }
        
        [HttpGet]
        [Route("api/v1/{receiverAddress}/mails/count")]
        [ProducesResponseType(typeof(CountResult), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 404)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> GetMailCount([FromRoute, EmailAddress, Required] string receiverAddress)
        {
            return BadRequest();
        }
        
        [HttpGet]
        [Route("api/v1/{receiverAddress}/mails/{mailGuid}")]
        [ProducesResponseType(typeof(ReceivedMail), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 404)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> GetMail([FromRoute, EmailAddress, Required] string receiverAddress, [FromRoute] Guid? mailGuid)
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

        [HttpDelete]
        [Route("api/v1/{receiverAddress}/mails/{mailGuid}")]
        [ProducesResponseType(typeof(ReceivedMail), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 404)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> DeleteMail([FromRoute, EmailAddress, Required] string receiverAddress, [FromRoute] Guid? mailGuid)
        {
            return BadRequest();
        }
    }
}