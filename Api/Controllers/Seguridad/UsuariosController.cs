using Application.DTOs.Seguridad;
using Application.Features.Seguridad.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Plant_HexArquitecture_API.Controllers.Seguridad
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsuariosController(IUsuariosOperation usuariosOperation) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<UserDto>>> GetAll()
        {
            return Ok(await usuariosOperation.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetById(long id)
        {
            return Ok(await usuariosOperation.GetById(id));
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserDto dto)
        {
            return Ok(await usuariosOperation.Create(dto));
        }

        [HttpPut]
        public async Task<ActionResult<UserDto>> Update([FromBody] UpdateUserDto dto)
        {
            return Ok(await usuariosOperation.Update(dto));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(long id)
        {
            await usuariosOperation.Delete(id);
            return NoContent();
        }
    }
}
