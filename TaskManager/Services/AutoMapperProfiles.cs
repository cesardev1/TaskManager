using AutoMapper;
using TaskManager.Entities;
using TaskManager.Models;

namespace TaskManager.Services;

public class AutoMapperProfiles:Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<TodoItem,TodoDTO>();
    }
    
}