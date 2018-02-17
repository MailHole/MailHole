using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MailHole.Api.Models.Validation;
using MailHole.Common.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace MailHole.Api.Controllers
{
    public class AttachementsController : Controller
    {
        private readonly IDatabaseAsync _redisDb;

        public AttachementsController(IDatabaseAsync redisDb)
        {
            _redisDb = redisDb;
        }

        /// <summary>
        ///     Get details of attachement like MIME type, hashes, download link, ...
        /// </summary>
        /// <param name="receiverAddress">Mail address of the original receiver</param>
        /// <param name="mailGuid">GUID that was assigned to the mail by MailHole</param>
        /// <param name="attachementGuid">GUID that was assigned to the attachement by MailHole</param>
        /// <response code="200">Found attachement</response>
        /// <response code="400">Any error with the specified parameters</response>
        /// <response code="404">Parameters were ok but no matching attachement was found</response>
        [HttpGet]
        [Route("api/v1/{receiverAddress}/mails/{mailGuid}/attachements/{attachementGuid}")]
        [ProducesResponseType(typeof(AttachementInfo), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 404)]
        public async Task<IActionResult> GetAttachemntDetails([FromRoute, EmailAddress, Required] string receiverAddress,
            [FromRoute, Required] Guid? mailGuid, [FromRoute, Required] Guid? attachementGuid)
        {
            return BadRequest();
        }
        
        /// <summary>
        /// Checks whether attachment was transferred successfully
        /// </summary>
        [HttpPut]
        [Route("api/v1/{receiverAddress}/mails/{mailGuid}/attachements/{attachementGuid}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 404)]
        [ProducesResponseType(typeof(void), 409)]
        public async Task<IActionResult> Validate([FromRoute, EmailAddress, Required] string receiverAddress,
            [FromRoute, Required] Guid? mailGuid, [FromRoute, Required] Guid? attachementGuid, [FromBody] ValidationRequest validationRequest)
        {
            if (!ModelState.IsValid) return BadRequest();
            var guidString = mailGuid.HasValue ? mailGuid.Value.ToString() : "";

            if (string.IsNullOrEmpty(validationRequest.HashToValidate) || string.IsNullOrWhiteSpace(validationRequest.HashToValidate)) return BadRequest();

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