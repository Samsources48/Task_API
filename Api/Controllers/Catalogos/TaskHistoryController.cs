using Application.DTOs;
using Application.Features.Tasks.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Api.Controllers.Catalogos
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class TaskHistoryController(ITaskHistoryOperation taskHistoryOperation) : ControllerBase
    {
        [HttpGet("byTask/{idTaskItem}")]
        [ProducesResponseType(typeof(List<TaskHistoryDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<List<TaskHistoryDto>>> GetByTaskItem(long idTaskItem)
        {
            var data = await taskHistoryOperation.GetByTaskItem(idTaskItem);
            return Ok(data);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TaskHistoryDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<TaskHistoryDto>> GetById(long id)
        {
            var data = await taskHistoryOperation.GetById(id);
            return Ok(data);
        }

        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(TaskHistoryDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<TaskHistoryDto>> Create([FromBody] SaveTaskHistoryDto dto)
        {
            var data = await taskHistoryOperation.Create(dto);
            return StatusCode(StatusCodes.Status201Created, data);
        }

        [HttpPut]
        [ProducesResponseType(typeof(TaskHistoryDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<TaskHistoryDto>> Update([FromBody] UpdateTaskHistoryDto dto)
        {
            var data = await taskHistoryOperation.Update(dto);
            return Ok(data);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Delete(long id)
        {
            var data = await taskHistoryOperation.Delete(id);
            return Ok(data);
        }
    }
}
