using AutoMapper;
using TaskManager.Entities;
using TaskManager.Models;

namespace TaskManager.Services;

public class AutoMapperProfiles:Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<TodoItem,TodoDTO>()
            .ForMember(dto => dto.StepsTotal, ent 
                => ent.MapFrom(x => x.Steps.Count()))
            .ForMember(dto => dto.StepsDone, ent 
                => ent.MapFrom(x => x.Steps.Where(p=>p.IsCompleted).Count()));
    }
    
}