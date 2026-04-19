using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using challenge1.Application.Bll.Common.Interfaces;
using challenge1.Application.DTO.Common;
using challenge1.Application.Filter.Common;
using challenge1.Common.Logging;

namespace challenge1.Application.API.Controllers.Common
{
    [ApiController]
    [Route("Common/[controller]")]
    public class BackendUserController : ControllerBase
    {
        private readonly ILogging _logging;
        private readonly IBackendUserBll _backendUserBll;

        public BackendUserController(ILogging logging, IBackendUserBll backendUserBll)
        {
            _logging = logging;
            _backendUserBll = backendUserBll;
        }

        [HttpPost("GetBackendUserByLoginId")]
        public async Task<IActionResult> GetBackendUserByLoginIdAsync([FromBody] string loginId)
        {
            try
            {
                var result = await _backendUserBll.GetBackendUserByLoginIdAsync(loginId);
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

        [HttpPost("GetBackendUsers")]
        public async Task<IActionResult> GetBackendUsersAsync([FromBody] GetBackendUsersFilter request)
        {
            try
            {
                var result = await _backendUserBll.GetBackendUsersAsync(request);
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

        [HttpPost("Create")]
        public async Task<IActionResult> CreateBackendUserAsync([FromBody] CreateBackendUserRequestDTO request)
        {
            try
            {
                var userId = await _backendUserBll.CreateBackendUserAsync(request);
                if (userId != null) {
                    return Ok(userId);
                }
                else
                {
                    return BadRequest("Failed to create backend user.");
                }
            }
            catch (Exception ex)
            {
                _logging.LogControllerError(ex.ToString());
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost("Update")]
        public async Task<IActionResult> UpdateBackendUserAsync([FromBody] UpdateBackendUserRequestDTO request)
        {
            try
            {
                var updateSuccess = await _backendUserBll.UpdateBackendUserAsync(request);
                if (updateSuccess)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest("Failed to update backend user.");
                }
            }
            catch (Exception ex)
            {
                _logging.LogControllerError(ex.ToString());
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost("SoftDelete")]
        public async Task<IActionResult> SoftDeleteBackendUserAsync([FromBody] SoftDeleteBackendUserRequestDTO request)
        {
            try
            {
                var deleteSuccess = await _backendUserBll.SoftDeleteBackendUserAsync(request);
                if (deleteSuccess)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest("Failed to delete backend user.");
                }
            }
            catch (Exception ex)
            {
                _logging.LogControllerError(ex.ToString());
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost("GetBackendUsersRolesAsync")]
        public async Task<IActionResult> GetBackendUsersRolesAsync([FromBody] Guid userId)
        {
            try
            {
                var result = await _backendUserBll.GetBackendUsersRolesAsync(userId);
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
