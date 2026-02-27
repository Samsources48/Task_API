using Application.DTOs.Seguridad;
using Application.Features.Seguridad.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Plant_HexArquitecture_API.Controllers.Seguridad
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthOperation authOperation) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
        {
            var response = await authOperation.Login(dto);
            return Ok(response);
        }
    }
}
