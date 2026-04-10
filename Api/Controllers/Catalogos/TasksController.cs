using Application.DTOs;
using Application.Features.Products.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;


namespace Api.Controllers.Catalogos
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class TasksController(ILogger<TasksController> _logger, ITasksOperation _tasksOperation) : ControllerBase
    {

        [HttpGet("Dashboard")]
        [ProducesResponseType(typeof(TaskDashboard), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<TaskDashboard>> GetTaskDasboard([FromQuery] string idUser)
        {
            var data = await _tasksOperation.GetTaskDasboard(idUser);
            return Ok(data);
        }

        [HttpGet()]
        [ProducesResponseType(typeof(List<TasksDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<List<TasksDto>>> GetAll([FromQuery]  string idUser)
        {
            var data = await _tasksOperation.GetAll(idUser);
            return Ok(data);
        }


        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductsDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<TasksDto>> GetById(int id)
        {
            var data = await _tasksOperation.GetById(id);
            return Ok(data);
        }


        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(List<ProductsDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<TasksDto>> Create([FromBody] SaveTasksDto values)
        {
            var data = await _tasksOperation.Create(values);
            return Ok(data);
        }


        [HttpPut]
        [ProducesResponseType(typeof(ProductsDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<TasksDto>> Update([FromBody] SaveTasksDto values)
        {
            var data = await _tasksOperation.Update(values);
            return Ok(data);
        }


        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            var data = await _tasksOperation.Delete(id);
            return Ok(data);
        }
    }
}
