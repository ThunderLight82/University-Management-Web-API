using UniversityManagement.Domain.Entities;

namespace UniversityManagement.Application.Services.Interfaces;

public interface IBaseService<TBaseEntity> where TBaseEntity : BaseEntity
{
    Task<TBaseEntity> GetById(int id);
    Task<IEnumerable<TBaseEntity>> GetAll();
    Task Add(TBaseEntity entity);
    Task Delete(int id);
    Task Update(TBaseEntity entity);
}