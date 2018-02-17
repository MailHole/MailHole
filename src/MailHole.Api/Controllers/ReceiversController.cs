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
        [ProducesResponseType(typeof(Receiver), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 500)]
        public IActionResult GetReceiverInfo([FromRoute, EmailAddress, Required] string receiverAddress)
        {
            if (!ModelState.IsValid) return BadRequest();
            return Ok(receiverAddress);
        }
    }
}