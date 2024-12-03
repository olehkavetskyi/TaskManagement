using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers;

[Authorize]
public class TaskController : BaseController
{
    private readonly ITaskService _taskService;

    public TaskController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto dto)
    {
        var userId = GetUserId();

        var createdTask = await _taskService.CreateTaskAsync(dto, userId);
        return CreatedAtAction(nameof(GetTaskById), new { id = createdTask.Id }, createdTask);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTaskById(Guid id)
    {
        var userId = GetUserId();
        var task = await _taskService.GetTaskByIdAsync(id, userId);
        return Ok(task);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateTask(Guid id, [FromBody] UpdateTaskDto dto)
    {
        var userId = GetUserId();
        var updatedTask = await _taskService.UpdateTaskAsync(id, dto, userId);
        return Ok(updatedTask);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTask(Guid id)
    {
        var userId = GetUserId();
        var isDeleted = await _taskService.DeleteTaskAsync(id, userId);

        if (isDeleted)
        {
            return NoContent();
        }

        return NotFound();
    }

    [HttpGet]
    public async Task<IActionResult> GetTasks([FromQuery] TaskFilterDto filter)
    {
        var userId = GetUserId();
        var tasks = await _taskService.GetTasksAsync(filter, userId);
        return Ok(tasks);
    }

    private Guid GetUserId()
    {
        return new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
    }
}
