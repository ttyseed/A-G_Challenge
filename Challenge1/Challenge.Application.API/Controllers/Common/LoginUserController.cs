using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using challenge1.Application.Bll.Common.Interfaces;
using challenge1.Application.Filter.Common;
using challenge1.Common.Logging;

namespace challenge1.Application.API.Controllers.Common
{
    [ApiController]
    [Route("Common/[controller]")]
    public class LoginUserController : ControllerBase
    {
        private readonly ILogging _logging;
        private readonly ILoginUserBll _loginUserBll;

        public LoginUserController(ILogging logging, ILoginUserBll loginUserBll)
        {
            _logging = logging;
            _loginUserBll = loginUserBll;
        }

        [HttpPost("GetLoginUserByLoginIdAsync")]
        public async Task<IActionResult> GetLoginUserByLoginIdAsync([FromBody] string loginId)
        {
            try
            {
                var result = await _loginUserBll.GetLoginUserByLoginIdAsync(loginId);
                if (result != null)
                {
                    return Ok(result);
                }
                else
                {
                    return NotFound(result);
                }
            }
            catch (Exception ex)
            {
                _logging.LogControllerError(ex.ToString());
                return StatusCode(500, "Internal Server Error");
            }
        }


        [HttpPost("GetLoginUsersAsync")]
        public async Task<IActionResult> GetLoginUsersAsync([FromBody] GetLoginUsersFilter request)
        {
            try
            {
                var result = await _loginUserBll.GetLoginUsersAsync(request);
                if (result != null)
                {
                    return Ok(result);
                }
                else
                {
                    return NotFound(result);
                }
            }
            catch (Exception ex)
            {
                _logging.LogControllerError(ex.ToString());
                return StatusCode(500, "Internal Server Error");
            }
        }

    }
}
