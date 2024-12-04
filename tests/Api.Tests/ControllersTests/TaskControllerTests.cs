
using System.Security.Claims;
using Api.Controllers;
using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Api.Tests.ControllersTests;

public class TaskControllerTests
{
    private readonly Mock<ITaskService> _taskServiceMock;
    private readonly TaskController _controller;
    private readonly Guid _userId;

    public TaskControllerTests()
    {
        _taskServiceMock = new Mock<ITaskService>();
        _controller = new TaskController(_taskServiceMock.Object);

        // Mock user claims
        _userId = Guid.NewGuid();
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, _userId.ToString())
        }));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Fact]
    public async Task CreateTask_ShouldReturnCreatedResult()
    {
        // Arrange
        var createTaskDto = new CreateTaskDto { Title = "Test Task" };
        var createdTask = new TaskDto { Id = Guid.NewGuid(), Title = "Test Task" };

        _taskServiceMock
            .Setup(s => s.CreateTaskAsync(createTaskDto, _userId))
            .ReturnsAsync(createdTask);

        // Act
        var result = await _controller.CreateTask(createTaskDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(_controller.GetTaskById), createdResult.ActionName);
        Assert.Equal(createdTask.Id, ((TaskDto)createdResult.Value).Id);
    }

    [Fact]
    public async Task GetTaskById_ShouldReturnOkResult()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var taskDto = new TaskDto { Id = taskId, Title = "Test Task" };

        _taskServiceMock
            .Setup(s => s.GetTaskByIdAsync(taskId, _userId))
            .ReturnsAsync(taskDto);

        // Act
        var result = await _controller.GetTaskById(taskId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedTask = Assert.IsType<TaskDto>(okResult.Value);
        Assert.Equal(taskId, returnedTask.Id);
    }

    [Fact]
    public async Task UpdateTask_ShouldReturnOkResult()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var updateTaskDto = new UpdateTaskDto { Title = "Updated Task" };
        var updatedTask = new TaskDto { Id = taskId, Title = "Updated Task" };

        _taskServiceMock
            .Setup(s => s.UpdateTaskAsync(taskId, updateTaskDto, _userId))
            .ReturnsAsync(updatedTask);

        // Act
        var result = await _controller.UpdateTask(taskId, updateTaskDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedTask = Assert.IsType<TaskDto>(okResult.Value);
        Assert.Equal(taskId, returnedTask.Id);
        Assert.Equal("Updated Task", returnedTask.Title);
    }

    [Fact]
    public async Task DeleteTask_ShouldReturnNoContentResult_WhenTaskIsDeleted()
    {
        // Arrange
        var taskId = Guid.NewGuid();

        _taskServiceMock
            .Setup(s => s.DeleteTaskAsync(taskId, _userId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteTask(taskId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteTask_ShouldReturnNotFoundResult_WhenTaskIsNotDeleted()
    {
        // Arrange
        var taskId = Guid.NewGuid();

        _taskServiceMock
            .Setup(s => s.DeleteTaskAsync(taskId, _userId))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteTask(taskId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetTasks_ShouldReturnOkResult()
    {
        // Arrange
        var filter = new TaskFilterDto { Title = "Test" };
        var tasks = new List<TaskDto>
        {
            new TaskDto { Id = Guid.NewGuid(), Title = "Task 1" },
            new TaskDto { Id = Guid.NewGuid(), Title = "Task 2" }
        };

        var pagedResult = new PagedResultDto<TaskDto>
        {
            Items = tasks,
            TotalCount = tasks.Count
        };

        _taskServiceMock
            .Setup(s => s.GetTasksAsync(filter, _userId))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _controller.GetTasks(filter);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedResult = Assert.IsType<PagedResultDto<TaskDto>>(okResult.Value);
        Assert.Equal(tasks.Count, returnedResult.TotalCount);
    }
}
