using Application.DTOs;
using Application.Features.Tasks.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Api.Controllers.Catalogos
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class TaskCategoriesController(ILogger<TaskCategoriesController> logger, ITaskCategoryOperation taskCategoryOperation) : ControllerBase
    {
        private readonly ILogger<TaskCategoriesController> _logger = logger;
        private readonly ITaskCategoryOperation _taskCategoryOperation = taskCategoryOperation;

        [HttpGet()]
        [ProducesResponseType(typeof(List<TaskCategoryDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<List<TaskCategoryDto>>> GetAll()
        {
            var data = await _taskCategoryOperation.GetAll();
            return Ok(data);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TaskCategoryDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<TaskCategoryDto>> GetById(int id)
        {
            var data = await _taskCategoryOperation.GetById(id);
            return Ok(data);
        }

        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(TaskCategoryDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<TaskCategoryDto>> Create([FromBody] SaveTaskCategoryDto values)
        {
            var data = await _taskCategoryOperation.Create(values);
            return Ok(data);
        }

        [HttpPut]
        [ProducesResponseType(typeof(TaskCategoryDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<TaskCategoryDto>> Update([FromBody] SaveTaskCategoryDto values)
        {
            var data = await _taskCategoryOperation.Update(values);
            return Ok(data);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            var data = await _taskCategoryOperation.Delete(id);
            return Ok(data);
        }
    }
}