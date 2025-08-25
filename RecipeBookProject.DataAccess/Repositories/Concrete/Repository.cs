using Microsoft.EntityFrameworkCore;
using RecipeBookProject.Data.Context;
using RecipeBookProject.DataAccess.Repositories.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using RecipeBookProject.Data.Entities;
namespace RecipeBookProject.DataAccess.Repositories.Concrete;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly RecipeBookProjectDbContext _db;
    protected readonly DbSet<T> _set;
    public Repository(RecipeBookProjectDbContext db)
    {
        _db = db;
        _set = _db.Set<T>();
    }

    public IQueryable<T> Query() => _set.AsQueryable();

    public Task<T?> GetAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        => _set.FirstOrDefaultAsync(predicate, ct);

    public async Task AddAsync(T entity, CancellationToken ct = default)
        => await _set.AddAsync(entity, ct);

    public Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        _set.Update(entity);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(T entity, CancellationToken ct = default)
    {
        _set.Remove(entity);
        return Task.CompletedTask;
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}
