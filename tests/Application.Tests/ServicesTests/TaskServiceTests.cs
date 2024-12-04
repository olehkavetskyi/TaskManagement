using Application.DTOs;
using Application.Services;
using TaskEntity = Domain.Entities.Task;
using AutoMapper;
using Domain.Interfaces;
using FluentAssertions;
using Moq;
using Domain.Specifications;

namespace Application.Tests.ServicesTests;

public class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _taskRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly TaskService _taskService;

    public TaskServiceTests()
    {
        _taskRepositoryMock = new Mock<ITaskRepository>();
        _mapperMock = new Mock<IMapper>();
        _taskService = new TaskService(_taskRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task CreateTaskAsync_ShouldCreateTaskAndReturnDto()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var createTaskDto = new CreateTaskDto { Title = "Test Task", Description = "Test Description" };
        var taskEntity = new TaskEntity { Id = Guid.NewGuid(), Title = "Test Task", Description = "Test Description", UserId = userId };
        var taskDto = new TaskDto { Id = taskEntity.Id, Title = taskEntity.Title, Description = taskEntity.Description };

        _mapperMock.Setup(m => m.Map<TaskEntity>(createTaskDto)).Returns(taskEntity);
        _mapperMock.Setup(m => m.Map<TaskDto>(taskEntity)).Returns(taskDto);

        _taskRepositoryMock.Setup(r => r.AddAsync(taskEntity)).Returns(Task.CompletedTask);
        _taskRepositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _taskService.CreateTaskAsync(createTaskDto, userId);

        // Assert
        result.Should().BeEquivalentTo(taskDto);
        _taskRepositoryMock.Verify(r => r.AddAsync(taskEntity), Times.Once);
        _taskRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateTaskAsync_ShouldUpdateTaskAndReturnDto()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var updateTaskDto = new UpdateTaskDto { Title = "Updated Task", Description = "Updated Description" };
        var existingTask = new TaskEntity { Id = taskId, Title = "Old Task", Description = "Old Description", UserId = userId };
        var updatedTaskDto = new TaskDto { Id = taskId, Title = "Updated Task", Description = "Updated Description" };

        _taskRepositoryMock.Setup(r => r.GetByIdAsync(taskId)).ReturnsAsync(existingTask);
        _mapperMock.Setup(m => m.Map(updateTaskDto, existingTask));
        _mapperMock.Setup(m => m.Map<TaskDto>(existingTask)).Returns(updatedTaskDto);

        _taskRepositoryMock.Setup(r => r.Update(existingTask));
        _taskRepositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _taskService.UpdateTaskAsync(taskId, updateTaskDto, userId);

        // Assert
        result.Should().BeEquivalentTo(updatedTaskDto);
        _taskRepositoryMock.Verify(r => r.Update(existingTask), Times.Once);
        _taskRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteTaskAsync_ShouldDeleteTaskAndReturnTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var existingTask = new TaskEntity { Id = taskId, Title = "Task to Delete", UserId = userId };

        _taskRepositoryMock.Setup(r => r.GetByIdAsync(taskId)).ReturnsAsync(existingTask);
        _taskRepositoryMock.Setup(r => r.Delete(existingTask));
        _taskRepositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _taskService.DeleteTaskAsync(taskId, userId);

        // Assert
        result.Should().BeTrue();
        _taskRepositoryMock.Verify(r => r.Delete(existingTask), Times.Once);
        _taskRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetTaskByIdAsync_ShouldReturnTaskDto()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var taskEntity = new TaskEntity { Id = taskId, Title = "Test Task", UserId = userId };
        var taskDto = new TaskDto { Id = taskId, Title = "Test Task" };

        _taskRepositoryMock.Setup(r => r.GetByIdAsync(taskId)).ReturnsAsync(taskEntity);
        _mapperMock.Setup(m => m.Map<TaskDto>(taskEntity)).Returns(taskDto);

        // Act
        var result = await _taskService.GetTaskByIdAsync(taskId, userId);

        // Assert
        result.Should().BeEquivalentTo(taskDto);
    }

    [Fact]
    public async Task GetTasksAsync_ShouldReturnPagedResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var filter = new TaskFilterDto { PageNumber = 1, PageSize = 10 };
        var taskEntities = new List<TaskEntity> { new TaskEntity { Id = Guid.NewGuid(), Title = "Task 1", UserId = userId } };
        var taskDtos = new List<TaskDto> { new TaskDto { Id = Guid.NewGuid(), Title = "Task 1" } };

        _taskRepositoryMock.Setup(r => r.GetFilteredAsync(
            It.IsAny<TaskSpecification>(),
            filter.SortBy,
            filter.SortDescending,
            0,
            filter.PageSize)).ReturnsAsync((taskEntities, taskEntities.Count));

        _mapperMock.Setup(m => m.Map<IEnumerable<TaskDto>>(taskEntities)).Returns(taskDtos);

        // Act
        var result = await _taskService.GetTasksAsync(filter, userId);

        // Assert
        result.Items.Should().BeEquivalentTo(taskDtos);
        result.TotalCount.Should().Be(taskEntities.Count);
    }
}
