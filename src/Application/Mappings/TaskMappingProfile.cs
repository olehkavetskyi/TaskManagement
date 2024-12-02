using Application.DTOs;
using AutoMapper;
using Task = Domain.Entities.Task;

namespace Application.Mappings;

public class TaskMappingProfile : Profile
{
    public TaskMappingProfile()
    {
        // Map from DTO to Entity
        CreateMap<CreateTaskDto, Task>();
        CreateMap<UpdateTaskDto, Task>();

        // Map from Entity to DTO
        CreateMap<Task, TaskDto>();
    }
}