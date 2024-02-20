using UniversityManagement.Domain.Entities;

namespace UniversityManagement.Application.Interfaces;

public interface IBaseService<TBaseEntity> where TBaseEntity : BaseEntity
{
    Task<TBaseEntity> GetById(int id);
    Task<IEnumerable<TBaseEntity>> GetAll();
    Task Add(TBaseEntity entity);
    Task Delete(TBaseEntity entity);
    Task Update(TBaseEntity entity);
    Task SaveChangesAsync();
}