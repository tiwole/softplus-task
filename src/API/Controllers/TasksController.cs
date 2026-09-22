using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Tasks;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController(ITaskService taskService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] TaskQuery query, CancellationToken cancellationToken)
    {
        var tasks = await taskService.GetPagedAsync(query, cancellationToken);
        return Ok(tasks);
    }
}
