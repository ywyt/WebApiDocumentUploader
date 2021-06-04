using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiDocumentUploader.Auth;

namespace WebApiDocumentUploader.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : BaseController
    {
        private readonly IAuthenticateService _authService;
        public AuthenticationController(IAuthenticateService authService)
        {
            this._authService = authService;
        }

        [AllowAnonymous]
        [HttpPost, Route("requestToken")]
        public ActionResult RequestToken([FromBody] LoginRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Request");
            }

            string token;
            if (_authService.IsAuthenticated(request, out token) || !string.IsNullOrEmpty(token))
            {
                return Ok(token);
            }

            return Ok();

        }
    }
}
