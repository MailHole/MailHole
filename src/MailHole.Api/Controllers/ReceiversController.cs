using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using MailHole.Api.Auth;
using MailHole.Common.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Minio;
using Newtonsoft.Json;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MailHole.Api.Controllers
{
    public class ReceiversController : Controller
    {
        private readonly ILogger<ReceiversController> _logger;
        private readonly MinioClient _minio;

        public ReceiversController(ILogger<ReceiversController> logger, MinioClient minio)
        {
            _logger = logger;
            _minio = minio;
        }

        [HttpGet]
        [AuthorizeUsers]
        [Route("api/v1/{receiverAddress}")]
        [ProducesResponseType(typeof(Receiver), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 500)]
        public IActionResult GetReceiverInfo([FromRoute, EmailAddress, Required] string receiverAddress)
        {
            _logger.LogDebug($"Called {nameof(GetReceiverInfo)} with receiverAddress {receiverAddress}");
            if (!ModelState.IsValid) return BadRequest();
            return Ok(receiverAddress);
        }
        
        [HttpGet]
        [Route("api/v1/{receiverAddress}/files/{fileGuid}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 404)]
        public async Task<IActionResult> GetAttachementFile(
            [FromRoute, EmailAddress, Required] string receiverAddress,
            [FromRoute, Required] Guid? fileGuid)
        {
            var memoryStream = new MemoryStream();
            await _minio.GetObjectAsync("test", "Exercise-2-Container.zip", stream =>
            {
                stream.CopyTo(memoryStream);
            });
            memoryStream.Position = 0;
            return File(memoryStream, "application/octet-stream", "test.zip");
        }
    }
}