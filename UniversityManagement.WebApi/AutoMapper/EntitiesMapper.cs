using AutoMapper;
using UniversityManagement.Domain.Entities;
using UniversityManagement.DTO.EntitiesDto;

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