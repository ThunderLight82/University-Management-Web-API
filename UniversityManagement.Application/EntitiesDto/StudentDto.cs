﻿using UniversityManagement.Domain.Entities;

namespace UniversityManagement.Application.EntitiesDto;

public class StudentDto : BaseEntityDto
{
    public string FirstName { get; set; }

    public string LastName { get; set; }
    
    public int GroupId { get; set; }
    
    public Group Group { get; set; }
}