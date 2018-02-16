using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MailHole.Common.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.SwaggerGen;

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
        [ProducesResponseType(400)]
        [ProducesResponseType(typeof(string), 200)]
        public IActionResult GetReceiverInfo([FromRoute, EmailAddress, Required] string receiverAddress,
            [FromQuery, Range(0, int.MaxValue)] int offset = 0, [FromQuery, Range(1, 100)] int pageSize = 20)
        {
            if (!ModelState.IsValid) return BadRequest();
            return Ok(receiverAddress);
        }

        [HttpGet]
        [Route("api/v1/{receiverAddress}/mails/{mailGuid}")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(404)]
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


        /// <summary>
        /// Checks whether attachment was transferred successfully
        /// </summary>
        [HttpPost]
        [Route("api/v1/{receiverAdress}/mails/{mailGuid}/checkAttachmentHash")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PostAttachmentHash([FromRoute, EmailAddress, Required] string receiverAddress,
            [FromRoute, Required] Guid? mailGuid, [FromBody, Required] string attachmentHash)
        {
            if (!ModelState.IsValid) return BadRequest();
            var guidString = mailGuid.HasValue ? mailGuid.Value.ToString() : "";

            if (string.IsNullOrEmpty(attachmentHash) || string.IsNullOrWhiteSpace(attachmentHash)) return BadRequest();

            if (!await _redisDb.HashExistsAsync(receiverAddress, guidString)) return NotFound();


            var mail = JsonConvert.DeserializeObject<ReceivedMail>(
                await _redisDb.HashGetAsync(receiverAddress, guidString));

            if (!mail.HasAttachements) return Ok(false);

            foreach (var mailAttachement in mail.Attachements)
            {
                //todo query mail attachment, hash and compare with given attachment hash
            }

            return Ok(true);

        }
    }
}