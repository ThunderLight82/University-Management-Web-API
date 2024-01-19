using Microsoft.EntityFrameworkCore;
using UniversityManagement.Application.Services.Interfaces;
using UniversityManagement.Domain.Entities;
using UniversityManagement.DataAccess;

namespace UniversityManagement.Application.Services;

public abstract class BaseService<TBaseEntity> : IBaseService<TBaseEntity> where TBaseEntity : BaseEntity
{
    protected readonly UniversityDbContext _dbContext;

    protected BaseService(UniversityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public virtual async Task<TBaseEntity> GetById(int id) =>
        (await _dbContext.Set<TBaseEntity>().FindAsync(id))!;

    public virtual async Task<IEnumerable<TBaseEntity>> GetAll() =>
        await _dbContext.Set<TBaseEntity>().ToListAsync();

    public virtual async Task Add(TBaseEntity entity)
    {
        await _dbContext.Set<TBaseEntity>().AddAsync(entity);
        await _dbContext.SaveChangesAsync();
    }

    public virtual async Task Delete(TBaseEntity entity)
    {
        _dbContext.Set<TBaseEntity>().Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    public virtual async Task Update(TBaseEntity entity)
    {
        _dbContext.Entry(entity).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();
    }
}