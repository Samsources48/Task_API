using System.Linq.Expressions;
using Domain.Entities.Base;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Domain.Interfaces.Base
{
    public interface IRepository<T> where T : BaseEntity
    {
        DbContext DbContext { get; }
        DbSet<T> DbSet { get; }

        IQueryable<T> Queryable { get; }
        IQueryable<T> Where(Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includes);

        Task<List<T>> GetAllAsync();
        
        Task<List<T>> GetAllAsync(
           int? page, int? pageSize, string? sortColumn, string? sortOrder, string? searchTerm, List<string>? searchColumns, List<string>? filter, List<string>? filterOperator, List<string>? value
        );
        
        Task<List<T>> GetAllAsync(
           int? page, int? pageSize, string? sortColumn, string? sortOrder, string? searchTerm, List<string>? searchColumns, List<string>? filter, List<string>? filterOperator, List<string>? value, string[]? includes
        );
        
        Task<List<T>> GetAllAsync<TKey>(
           int? page, int? pageSize, Expression<Func<T, TKey>>? sortProperty, string? sortOrder, string? searchTerm, 
           List<string>? searchColumns, List<string>? filter, List<string>? filterOperator, List<string>? value, 
           params Expression<Func<T, object>>[] includeProperties
        );
        
        Task<List<T>> GetAllAsync(DinamicFilters parameters);
        
        Task<List<T>> GetAllAsync(DinamicFilters parameters, string[]? includes);
        
        Task<List<T>> GetAllAsync(DinamicFilters parameters, params Expression<Func<T, object>>[] includeProperties);

        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includeProperties);
        
        Task<List<T>> GetAllAsync(DinamicFilters parameters, Func<IQueryable<T>, IQueryable<T>> includeFunc);
        
        Task<int> CountAsync();
        
        Task<int> CountAsync(
            string? searchTerm, List<string>? searchColumns, List<string>? filter, List<string>? filterOperator, List<string>? value
        );
        
        Task<int> CountAsync(
            string? searchTerm, List<string>? searchColumns, List<string>? filter, List<string>? filterOperator, List<string>? value, string[]? includes
        );
        
        Task<int> CountAsync(
            string? searchTerm, List<string>? searchColumns, List<string>? filter, List<string>? filterOperator, List<string>? value, 
            params Expression<Func<T, object>>[] includeProperties
        );
        
        Task<int> CountAsync(DinamicFilters parameters);
        
        Task<int> CountAsync(DinamicFilters parameters, string[]? includes);
        
        Task<int> CountAsync(DinamicFilters parameters, params Expression<Func<T, object>>[] includeProperties);
        
        Task<int> CountAsync(Expression<Func<T, bool>> expression);
        
        Task<int> CountAsync(DinamicFilters parameters, Func<IQueryable<T>, IQueryable<T>> includeFunc);
        
        Task<T?> GetByIdAsync(long id);
        
        Task<T?> GetByIdAsync(long id, string[]? includes);
        
        Task<T?> GetByIdAsync(long id, params Expression<Func<T, object>>[] includeProperties);
        
        Task<T?> GetByIdAsync(long id, Func<IQueryable<T>, IQueryable<T>> includeFunc);
        
        Task<T> CreateAsync(T entity);
        
        Task<T> UpdateAsync(long? id, T entity);
        Task<T> UpdateAsync(T entity);
        //bool BulkUpdate(IEnumerable<T> entities);

        Task<T> UpdatePartialAsync(long id, object partialEntity);
        
        Task<T> DeleteAsync(long id);
        
        Task<bool> ExistsAsync(long id);
        
        Task<bool> ExistsAsync(Expression<Func<T, bool>> expression);
    }
}