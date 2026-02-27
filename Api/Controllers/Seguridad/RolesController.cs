using Application.DTOs.Seguridad;
using Application.Features.Seguridad.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Plant_HexArquitecture_API.Controllers.Seguridad
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RolesController(IRolesOperation rolesOperation) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<RoleDto>>> GetAll()
        {
            return Ok(await rolesOperation.GetAll());
        }

        [HttpPost]
        public async Task<ActionResult<RoleDto>> Create([FromBody] CreateRoleDto dto)
        {
            return Ok(await rolesOperation.Create(dto));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(long id)
        {
            await rolesOperation.Delete(id);
            return NoContent();
        }
    }
}
