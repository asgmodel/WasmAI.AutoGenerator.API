using AutoGenerator.Data;
using AutoGenerator.Helper;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections;
using System.Linq.Expressions;
namespace AutoGenerator.Repositories.Base
{
    public class RepositoryException : Exception
    {
        public RepositoryException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    class RepositoryLogger { }
    public interface IBaseRepository<T> where T : class
    {
        long CounItems { get; }
        Task<T> GetByAsync(Expression<Func<T, bool>> filter, Func<IQueryable<T>, IQueryable<T>>? setInclude = null, bool tracked = false);
        Task<T?> CreateAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<T> AttachAsync(T entity);
        Task RemoveAsync(T entity);
        Task RemoveAsync(Expression<Func<T, bool>> predicate);
        Task RemoveRange(List<T> entities);
        Task<int> SaveAsync();
        Task<bool> Exists(Expression<Func<T, bool>> filter);
        IBaseRepository<T> Include(Func<IQueryable<T>, IQueryable<T>> include);
        IBaseRepository<T> Includes(params string[] includes);
        IQueryable<T> GetAll(Expression<Func<T, bool>>? filter = null, string[]? includes = null, int skip = 0, int take = 0, bool isOrdered = false, Expression<Func<T, long>>? order = null);
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IQueryable<T>>? include = null, int skip = 0, int take = 0, Expression<Func<T, object>>? order = null);
        Task RemoveAllAsync();
        IQueryable<T> Get(Expression<Func<T, bool>>? expression = null);

        Task<PagedResponse<T>> GetAllAsPaginateAsync(int pageNumber = 1, int pageSize = 10);
        Task<T> GetBy2Async(Expression<Func<T, bool>> filter, string[]? includes = null, bool noTracking = true);
        Task<T?> FindModelAsync(params object[] id);
        Task<int> RemoveRange(IEnumerable<T> entities);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> filter);
        IQueryable<T> GetQueryable(string[]? includes = null, bool noTracking = true);

        Task<PagedResponse<T>> GetAllByAsync(List<FilterCondition> conditions, ParamOptions? options = null);
        Task<T?> GetOneByAsync(List<FilterCondition> conditions, ParamOptions? options = null);
        Task<bool> ExecuteTransactionAsync(Func<Task<bool>> operation);


        DbSet<T>? DbSet { get; }
    }



    public abstract class TBaseRepository<TUser, TRole, TValue, T> : IBaseRepository<T>
    where TUser : IdentityUser<TValue>
    where TRole : IdentityRole<TValue>
    where TValue : IEquatable<TValue>
    where T : class

    {
        private readonly AutoIdentityDataContext<TUser, TRole, TValue> _db;
        private readonly DbSet<T>? _dbSet;
        protected readonly ILogger _logger;
        public long CounItems { get => _count; }
        private long _count = 0;
        public IQueryable<T>? query;

        public DbSet<T>? DbSet => _dbSet;

        protected TBaseRepository(AutoIdentityDataContext<TUser, TRole, TValue> db, ILoggerFactory logger)
        {

            if (!IsAllowCreate())
            {
                throw new InvalidOperationException("Creation of this repository is not allowed for the specified types.");
            }
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _dbSet = _db.TSet<T>();
            query = _dbSet.AsQueryable();
            _logger = logger.CreateLogger<TBaseRepository<TUser, TRole, TValue, T>>();
        }

        private static bool IsAllowCreate()
        {
            return typeof(ITModel).IsAssignableFrom(typeof(T));
        }

        public IQueryable<T> Get(Expression<Func<T, bool>>? expression = null)
        {
            if (expression != null) query = query.Where(expression);
            return query;
        }

        public IBaseRepository<T> Include(Func<IQueryable<T>, IQueryable<T>> include)
        {
            query = include(Get());
            return this;

        }

        public IBaseRepository<T> Includes(params string[] includes)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return this;

        }

        public async Task<T> GetByAsync(Expression<Func<T, bool>> filter, Func<IQueryable<T>, IQueryable<T>>? setInclude = null, bool tracked = false)
        {
            try
            {
                if (!tracked) query = query.AsNoTracking();
                if (filter != null) query = query.Where(filter);
                if (setInclude != null) query = setInclude(query);
                return await query.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving entity");
                throw new RepositoryException("Error retrieving entity", ex);
            }
        }

        public virtual async Task<T?> CreateAsync(T entity)
        {
            try
            {
                var item = (await _dbSet.AddAsync(entity)).Entity;
                await SaveAsync();
                return item;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating entity");
                return null;
            }
        }

        public async Task<T> UpdateAsync(T entity)
        {
            try
            {
                _db.ChangeTracker.Clear();
                var item = _dbSet.Update(entity).Entity;
                await SaveAsync();
                return item;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency error during update");
                throw new RepositoryException("Concurrency error during update", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating entity");
                throw new RepositoryException("Error updating entity", ex);
            }
        }

        public async Task<T> AttachAsync(T entity)
        {
            try
            {
                _dbSet.Attach(entity);
                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error attaching entity");
                throw new RepositoryException("Error attaching entity", ex);
            }
        }

        public async Task RemoveAsync(T entity)
        {
            try
            {
                if (_db.Entry(entity).State == EntityState.Detached)
                    _dbSet.Attach(entity);
                _dbSet.Remove(entity);
                await SaveAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing entity");
                throw new RepositoryException("Error removing entity", ex);
            }
        }

        public async Task RemoveAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {
                // Explicitly specify the namespace to resolve ambiguity for ExecuteDeleteAsync
                await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ExecuteDeleteAsync(_dbSet.Where(predicate));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing entities");
                throw new RepositoryException("Error removing entities", ex);
            }
        }

        public async Task RemoveAllAsync()
        {
            try
            {
                // Explicitly specify the namespace to resolve ambiguity for ExecuteDeleteAsync
                await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ExecuteDeleteAsync(_dbSet);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing all entities");
                throw new RepositoryException("Error removing all entities", ex);
            }
        }

        public async Task<int> SaveAsync()
        {
            try
            {
                return await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving changes");
                throw new RepositoryException("Error saving changes", ex);
            }
        }

        public async Task<bool> Exists(Expression<Func<T, bool>> filter)
        {
            try
            {
                return await _dbSet.AnyAsync(filter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking existence");
                throw new RepositoryException("Error checking existence", ex);
            }
        }

        public static IQueryable<T> SetSkipAndTake(IQueryable<T> query, int skip, int take)
        {
            if (skip >= 0) query = query.Skip(skip);
            if (take > 0) query = query.Take(take);
            return query;
        }

        // Handle transactions: to ensure multiple operations are executed as a single unit
        public async Task<bool> ExecuteTransactionAsync(Func<Task<bool>> operation)
        {
            var strategy = _db.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = await _db.Database.BeginTransactionAsync())
                {

                    try
                    {
                        bool result = await operation();
                        if (result)
                        {
                            await transaction.CommitAsync();
                        }
                        else
                        {
                            await transaction.RollbackAsync();
                        }
                        //return result;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError(ex, "Error during transaction");
                        throw new RepositoryException("Error during transaction", ex);
                    }
                }

            });
            return true;
        }

        public async Task RemoveRange(List<T> entities)
        {
            try
            {
                _dbSet.RemoveRange(entities);
                await SaveAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing entity");
                throw new RepositoryException("Error removing entity", ex);
            }
        }

        public IQueryable<T> GetAll(Expression<Func<T, bool>>? filter = null, string[]? includes = null, int skip = 0, int take = 0, bool isOrdered = false, Expression<Func<T, long>>? order = null)
        {
            try
            {
                query = query.AsNoTracking();
                if (includes != null) Includes(includes);
                return query;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving entity");
                throw new RepositoryException("Error retrieving entity", ex);
            }
        }

        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IQueryable<T>>? include = null, int skip = 0, int take = 0, Expression<Func<T, object>>? order = null)
        {
            try
            {
                query = query.AsNoTracking();
                if (filter != null) query = query.Where(filter);
                if (include != null) Include(include);
                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving entity");
                throw new RepositoryException("Error retrieving entity", ex);
            }
        }

        public async Task<PagedResponse<T>> GetAllAsPaginateAsync(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                return await query.AsNoTracking()
                    .ToPagedResponseAsync<T>(pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving entity");
                throw new RepositoryException("Error retrieving entity", ex);
            }
        }

        public async Task<T> GetBy2Async(Expression<Func<T, bool>> filter, string[]? includes = null, bool noTracking = true)
        {
            try
            {
                if (noTracking) query = query.AsNoTracking();
                if (filter != null) query = query.Where(filter);
                if (includes != null) Includes(includes);
                return await query.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving entity");
                throw new RepositoryException("Error retrieving entity", ex);
            }
        }

        public async Task<T?> FindModelAsync(params object[] id)
        {
            var entity = await _dbSet.FindAsync(id);
            return entity;
        }


        public async Task<int> RemoveRange(IEnumerable<T> entities)
        {
            try
            {
                _dbSet.RemoveRange(entities);
                return await SaveAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing entity");
                throw new RepositoryException("Error removing entity", ex);
            }
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> filter)
        {
            try
            {
                return await _dbSet.AnyAsync(filter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking existence");
                throw new RepositoryException("Error checking existence", ex);
            }
        }

        public IQueryable<T> GetQueryable(string[]? includes = null, bool noTracking = true)
        {
            try
            {
                if (noTracking) query = query.AsNoTracking();
                if (includes != null) Includes(includes);
                return query;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving entity");
                throw new RepositoryException("Error retrieving entity", ex);
            }
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Get all entities by filtering
        /// </summary>
        /// <param name="conditions"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        /// <exception cref="RepositoryException"></exception>
        public async Task<PagedResponse<T>> GetAllByAsync(List<FilterCondition> conditions, ParamOptions? options = null)
        {
            try
            {
                var parameter = Expression.Parameter(typeof(T), "e");

                var fullExpression = BuildExpression(conditions, parameter);
                var lambda = Expression.Lambda<Func<T, bool>>(fullExpression, parameter);
                query = query.AsNoTracking().Where(lambda);
                options ??= new ParamOptions();
                if (options.Includes.Count > 0) Includes([.. options.Includes]);

                var result = await query.ToPagedResponseAsync(options.PageNumber, options.PageSize);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving entity");
                throw new RepositoryException("Error retrieving entities", ex);
            }
        }

        /// <summary>
        /// Get one entity by filtering
        /// </summary>
        /// <param name="conditions"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        /// <exception cref="RepositoryException"></exception>
        public async Task<T?> GetOneByAsync(List<FilterCondition> conditions, ParamOptions? options = null)
        {
            try
            {
                var parameter = Expression.Parameter(typeof(T), "e");

                var fullExpression = BuildExpression(conditions, parameter);
                var lambda = Expression.Lambda<Func<T, bool>>(fullExpression, parameter);
                query = query.AsNoTracking().Where(lambda);
                options ??= new ParamOptions();
                if (options.Includes.Count > 0) Includes([.. options.Includes]);

                var result = await query.FirstOrDefaultAsync();
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving entity");
                throw new RepositoryException("Error retrieving entity", ex);
            }
        }

        private Expression BuildExpression(List<FilterCondition> group, ParameterExpression? parameter)
        {
            Expression? finalExpression = null;
            foreach (var condition in group)
            {
                Expression? expression = null;
                if (condition.Group?.Any() == true)
                {
                    finalExpression = BuildExpression(condition.Group, parameter);
                }

                if (!string.IsNullOrEmpty(condition.PropertyName))
                {
                    var member = Expression.PropertyOrField(parameter, condition.PropertyName);
                    var constant = Expression.Constant(condition.Value);
                    switch (condition.Operator)
                    {
                        case FilterOperator.Equals: expression = Expression.Equal(member, constant); break;
                        case FilterOperator.NotEqual: expression = Expression.NotEqual(member, constant); break;
                        case FilterOperator.GreaterThan: expression = Expression.GreaterThan(member, constant); break;
                        case FilterOperator.GreaterThanOrEqual: expression = Expression.GreaterThanOrEqual(member, constant); break;
                        case FilterOperator.LessThan: expression = Expression.LessThan(member, constant); break;
                        case FilterOperator.LessThanOrEqual: expression = Expression.LessThanOrEqual(member, constant); break;
                        case FilterOperator.Contains: expression = Expression.Call(member, "Contains", null, constant); break;
                        case FilterOperator.NotContains: expression = Expression.Not(Expression.Call(member, "Contains", null, constant)); break;
                        case FilterOperator.StartsWith: expression = Expression.Call(member, "StartsWith", null, constant); break;
                        case FilterOperator.EndsWith: expression = Expression.Call(member, "EndsWith", null, constant); break;
                        case FilterOperator.In: expression = In(condition, member); break;
                        case FilterOperator.NotIn: expression = NotIn(condition, member); break;
                    }
                }

                if (expression == null) continue;

                finalExpression = finalExpression == null
                    ? expression
                    : condition.Logic == FilterLogic.And
                        ? Expression.AndAlso(finalExpression, expression)
                        : Expression.OrElse(finalExpression, expression);
            }

            return finalExpression!;
        }

        private Expression? In(FilterCondition condition, MemberExpression member)
        {
            var values = (condition.Value as IEnumerable)?.Cast<object>()?.ToList();
            if (values is not null)
            {
                var containsMethod = typeof(List<object>).GetMethod("Contains", new[] { typeof(object) })!;
                return Expression.Call(Expression.Constant(values), containsMethod, Expression.Convert(member, typeof(object)));
            }

            return null;
        }

        private Expression? NotIn(FilterCondition condition, MemberExpression member)
        {
            var expression = In(condition, member);
            if (expression is not null) expression = Expression.Not(expression);

            return expression;
        }
    }

    public abstract class TBaseBPRRepository<TUser, TRole, TValue, T> : TBaseRepository<TUser, TRole, TValue, T>, IBPR<T, T>
            where TUser : IdentityUser<TValue>
            where TRole : IdentityRole<TValue>
            where TValue : IEquatable<TValue>
            where T : class
    {

        protected TBaseBPRRepository(AutoIdentityDataContext<TUser, TRole, TValue> db, ILoggerFactory logger) : base(db, logger)
        {
        }

        public Task<int> CountAsync()
        {
            return Task.FromResult((int)base.CounItems);
        }

        public override async Task<T?> CreateAsync(T entity)
        {
            try
            {
                T item = (await DbSet.AddAsync(entity)).Entity;
                await SaveAsync();
                return item;
            }
            catch (RepositoryException exception)
            {
                _logger.LogError(exception, "Error creating entity");
                throw exception;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error creating entity");
                throw new Exception("Error creating entity");
            }
        }
        public async Task<bool> ExecuteTransactionAsync(Func<Task<bool>> operation)
        {
            return await base.ExecuteTransactionAsync(operation);
        }

        public new async Task<bool> ExistsAsync(object value, string name = "Id")
        {
            return await base.ExistsAsync(e => EF.Property<object>(e, name) == value);
        }

        public async Task<PagedResponse<T>> GetAllAsync(string[]? includes = null, int pageNumber = 1, int pageSize = 10)
        {
            var query = GetQueryable(includes, false);
            return await query.ToPagedResponseAsync(pageNumber, pageSize);
        }

        public async Task<IEnumerable<T>> GetAllAsync(string propertyName, object value, string[]? includes = null)
        {
            var query = GetQueryable(includes, false);
            query = query.Where(e => EF.Property<object>(e, propertyName) == value);
            return await query.ToListAsync();
            //return await base.GetAllAsync(e=>EF.Property<object>(e,propertyName)==value,s=>s.Include());
        }
        public async Task<DataResult<IEnumerable<T>>> GetAllDataResultAsync()
        {
            try
            {
                var entities = await GetAllAsync();

                return DataResult<IEnumerable<T>>.Ok(entities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all records.");
                return DataResult<IEnumerable<T>>.Fail("An error occurred while fetching records.");
            }
        }

        public async Task<DataResult<T?>> GetByIdDataResultAsync(string id)
        {
            try
            {
                var entity = await GetByIdAsync(id);

                return DataResult<T?>.Ok(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching record by ID.");
                return DataResult<T?>.Fail("An error occurred while fetching the record.");
            }
        }

        public async Task<DataResult<T?>> FindDataResultAsync(Expression<Func<T, bool>> predicate)
        {
            _logger.LogWarning("Client-side predicate search is not supported on server-side repository.");
            return DataResult<T?>.Fail("Cannot use predicate on mapped entity. Use a service-level implementation.");
        }

        public DataResult<IQueryable<T>> GetQueryableDataResult()
        {
            _logger.LogWarning("Client-side IQueryable is not supported for mapped DTOs.");
            return DataResult<IQueryable<T>>.Fail("Queryable not supported for DTO types.");
        }

        public async Task<DataResult<T>> CreateDataResultAsync(T entity)
        {
            try
            {
                var result = await CreateAsync(entity);
                return DataResult<T>.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during create operation.");
                return DataResult<T>.Fail("Failed to create entity.");
            }
        }

        public async Task<DataResult<IEnumerable<T>>> CreateRangeDataResultAsync(IEnumerable<T> entities)
        {
            try
            {
                var created = await CreateRangeAsync(entities);
                return DataResult<IEnumerable<T>>.Ok(created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during range create operation.");
                return DataResult<IEnumerable<T>>.Fail("Failed to create entities.");
            }
        }

        public async Task<DataResult<T>> UpdateDataResultAsync(T entity)
        {
            try
            {
                var updated = await UpdateAsync(entity);
                return DataResult<T>.Ok(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during update operation.");
                return DataResult<T>.Fail("Failed to update entity.");
            }
        }


        public async Task<DataResult<bool>> DeleteDataResultAsync(string id)
        {
            try
            {
                await DeleteAsync(id);
                return DataResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to delete entity with ID: {id}");
                return DataResult<bool>.Fail("Deletion failed.");
            }
        }

        public async Task<DataResult<bool>> DeleteRangeDataResultAsync(Expression<Func<T, bool>> predicate)
        {
            _logger.LogWarning("DeleteRange with predicate not supported directly on mapped DTO.");
            return DataResult<bool>.Fail("DeleteRange with predicate not supported for DTOs.");
        }

        public async Task<DataResult<bool>> ExistsDataResultAsync(Expression<Func<T, bool>> predicate)
        {
            _logger.LogWarning("Exists with predicate not supported on mapped DTOs.");
            return DataResult<bool>.Fail("Exists check with predicate is unsupported.");
        }

        public async Task<DataResult<int>> CountDataResultAsync()
        {
            try
            {
                var count = await CountAsync();
                return DataResult<int>.Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during count operation.");
                return DataResult<int>.Fail("Failed to count entities.");
            }
        }

        public async Task<DataResult<T?>> FindDataResultAsync(params object[] id)
        {
            try
            {
                var found = await FindAsync(id);
                return DataResult<T?>.Ok(found);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during Find by keys.");
                return DataResult<T?>.Fail("Find operation failed.");
            }
        }

        public async Task<DataResult<bool>> ExistsDataResultAsync(object value, string name = "Id")
        {
            try
            {
                var exists = await ExistsAsync(value, name);
                return DataResult<bool>.Ok(exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking existence by {name}.");
                return DataResult<bool>.Fail("Existence check failed.");
            }
        }

        public async Task<DataResult<PagedResponse<T>>> GetAllDataResultAsync(string[]? includes = null, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var result = await GetAllAsync(includes, pageNumber, pageSize);
                return DataResult<PagedResponse<T>>.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during paged retrieval.");
                return DataResult<PagedResponse<T>>.Fail("Paged retrieval failed.");
            }
        }

        public async Task<DataResult<T?>> GetByIdDataResultAsync(object id)
        {
            try
            {
                var entity = await GetByIdAsync(id);
                return DataResult<T?>.Ok(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during GetById.");
                return DataResult<T?>.Fail("GetById failed.");
            }
        }

        public async Task<DataResult<bool>> DeleteAllDataResultAsync()
        {
            try
            {
                await DeleteAllAsync();
                return DataResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting all data.");
                return DataResult<bool>.Fail("DeleteAll failed.");
            }
        }

        public async Task<DataResult<bool>> DeleteDataResultAsync(T entity)
        {
            try
            {
                await DeleteAsync(entity);
                return DataResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during Delete.");
                return DataResult<bool>.Fail("Delete failed.");
            }
        }

        public async Task<DataResult<bool>> DeleteDataResultAsync(object value, string key = "Id")
        {
            try
            {
                await DeleteAsync(value, key);
                return DataResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting by {key}.");
                return DataResult<bool>.Fail("Delete failed.");
            }
        }

        public async Task<DataResult<bool>> DeleteDataResultRange(List<T> entities)
        {
            try
            {
                await DeleteRange(entities);
                return DataResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting range.");
                return DataResult<bool>.Fail("DeleteRange failed.");
            }
        }

        public async Task<DataResult<PagedResponse<T>>> GetAllByDataResultAsync(List<FilterCondition> conditions, ParamOptions? options = null)
        {
            try
            {
                var result = await GetAllByAsync(conditions, options);

                return DataResult<PagedResponse<T>>.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during filtered paged retrieval.");
                return DataResult<PagedResponse<T>>.Fail("Filtered query failed.");
            }
        }

        public async Task<DataResult<T?>> GetOneByDataResultAsync(List<FilterCondition> conditions, ParamOptions? options = null)
        {
            try
            {
                var result = await GetOneByAsync(conditions, options);
                return DataResult<T?>.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during GetOneBy.");
                return DataResult<T?>.Fail("GetOneBy failed.");
            }
        }
        public Task<T?> FindAsync(Expression<Func<T, bool>> predicate)
        {

            throw new NotImplementedException();
        }

        public Task<T?> FindAsync(params object[] id)
        {
            return base.FindModelAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return (await base.GetAllAsync()).AsEnumerable();
        }

        public Task<T?> GetByIdAsync(string id)
        {
            return base.FindModelAsync(id);
        }

        public IQueryable<T> GetQueryable()
        {
            return base.GetQueryable();
        }

        public async Task<IEnumerable<T>> CreateRangeAsync(IEnumerable<T> entities)
        {
            var newen = new List<T>();
            foreach (var entity in entities) {

                var nitem =await CreateAsync(entity);
                if (nitem != null)
                {
                    newen.Add(nitem);
                }
            }
            return newen.AsEnumerable();
        }

        public async Task DeleteAsync(string id)
        {

            var item =await GetByIdAsync(id);
             await base.RemoveAsync(item);

        }

        public Task DeleteRangeAsync(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

  

        public Task<T?> GetByIdAsync(object id)
        {
            return base.FindModelAsync(id);
        }

        public Task DeleteAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(object value, string key = "Id")
        {
            throw new NotImplementedException();
        }

        public Task DeleteRange(List<T> entities)
        {
            throw new NotImplementedException();
        }
    }
    public static class PagedResponseExtensions
    {
        public static async Task<PagedResponse<T>> ToPagedResponseAsync<T>(
            this IQueryable<T> query,
            int pageNumber,
            int pageSize)
        {
            var totalRecords = await query.CountAsync();
            var data = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResponse<T>(data, pageNumber, pageSize, totalRecords);
        }
    }
}
