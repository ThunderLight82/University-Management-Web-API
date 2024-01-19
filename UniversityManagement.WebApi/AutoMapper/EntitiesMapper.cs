using AutoMapper;
using UniversityManagement.Application.EntitiesDto;
using UniversityManagement.Domain.Entities;

namespace UniversityManagement.WebApi.AutoMapper;

public class EntitiesMapper : Profile
{
    public EntitiesMapper()
    {
        CreateMap<Course, CourseDto>();
        CreateMap<Group, GroupDto>();
        CreateMap<Student, StudentDto>();
        
        CreateMap<CourseDto, Course>();
        CreateMap<GroupDto, Group>();
        CreateMap<StudentDto, Student>();
    }
}