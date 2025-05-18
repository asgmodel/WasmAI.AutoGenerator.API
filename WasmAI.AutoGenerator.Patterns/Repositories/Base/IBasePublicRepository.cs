using AutoGenerator.Helper;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace AutoGenerator.Repositories.Base
{
    public interface IBasePublicRepository<TRequest, TResponse>
          where TRequest : class
          where TResponse : class
    {
        Task<IEnumerable<TResponse>> GetAllAsync();
        Task<TResponse?> GetByIdAsync(string id);
        Task<TResponse?> FindAsync(Expression<Func<TResponse, bool>> predicate);
        IQueryable<TResponse> GetQueryable();

        Task<TResponse> CreateAsync(TRequest entity);
        Task<IEnumerable<TResponse>> CreateRangeAsync(IEnumerable<TRequest> entities);

        Task<TResponse> UpdateAsync(TRequest entity);

        Task DeleteAsync(string id);
        Task DeleteRangeAsync(Expression<Func<TResponse, bool>> predicate);

        Task<bool> ExistsAsync(Expression<Func<TResponse, bool>> predicate);
        Task<int> CountAsync();

        Task<TResponse?> FindAsync(params object[] id);
        Task<bool> ExistsAsync(object value, string name = "Id");

        Task<PagedResponse<TResponse>> GetAllAsync(string[]? includes = null, int pageNumber = 1, int pageSize = 10);
        Task<TResponse?> GetByIdAsync(object id);
        Task DeleteAllAsync();
        Task DeleteAsync(TRequest entity);
        Task DeleteAsync(object value, string key = "Id");
        Task DeleteRange(List<TRequest> entities);

        Task<PagedResponse<TResponse>> GetAllByAsync(List<FilterCondition> conditions, ParamOptions? options = null);
        Task<TResponse?> GetOneByAsync(List<FilterCondition> conditions, ParamOptions? options = null);
    }



    
    public abstract class BaseBPR<TRequest, TResponse, ERequest, EResponse> : IBasePublicRepository<TRequest, TResponse>
    where TRequest : class
    where TResponse : class
    where ERequest : class
    where EResponse : class
    {
        protected readonly IMapper _mapper;
        protected readonly ILogger _logger;
        protected readonly IBasePublicRepository<ERequest, EResponse> _bpr;

        protected BaseBPR(IMapper mapper, ILoggerFactory logger, IBasePublicRepository<ERequest, EResponse> bpr)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger =logger.CreateLogger(this.GetType());
            _bpr = bpr ?? throw new ArgumentNullException(nameof(bpr));
        }

        protected TResult Map<TSource, TResult>(TSource obj)
            where TSource : class
            where TResult : class
        {
            try
            {
                return _mapper.Map<TResult>(obj);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Mapping failed from {typeof(TSource)} to {typeof(TResult)}.");
                throw new ApplicationException($"Mapping failed from {typeof(TSource)} to {typeof(TResult)}.");
            }
        }

        protected IEnumerable<TResult> Map<TSource, TResult>(IEnumerable<TSource>? source)
            where TSource : class
            where TResult : class
        {
            try
            {
                return source == null ? Enumerable.Empty<TResult>() : _mapper.Map<IEnumerable<TResult>>(source);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Mapping list failed from {typeof(TSource)} to {typeof(TResult)}.");
                throw new ApplicationException($"Mapping list failed from {typeof(TSource)} to {typeof(TResult)}.");
            }
        }

        public virtual async Task<IEnumerable<TResponse>> GetAllAsync()
        {
            try
            {
                var data = await _bpr.GetAllAsync();
                return Map<EResponse, TResponse>(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllAsync.");
                throw;
            }
        }

        public virtual async Task<TResponse?> GetByIdAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    throw new ArgumentNullException(nameof(id));

                var entity = await _bpr.GetByIdAsync(id);
                return Map<EResponse, TResponse>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetByIdAsync.");
                throw;
            }
        }

        public virtual async Task<TResponse?> FindAsync(Expression<Func<TResponse, bool>> predicate)
        {
            try
            {
                var all = await GetAllAsync();
                return all.AsQueryable().FirstOrDefault(predicate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in FindAsync.");
                throw;
            }
        }

        public virtual IQueryable<TResponse> GetQueryable()
        {
            try
            {
                var data = _bpr.GetQueryable();
                return _mapper.ProjectTo<TResponse>(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetQueryable.");
                throw;
            }
        }

        public virtual async Task<TResponse> CreateAsync(TRequest entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                var mapped = Map<TRequest, ERequest>(entity);
                var result = await _bpr.CreateAsync(mapped);

                if(result == null)
                    throw new ArgumentNullException(nameof(result));
                return Map<EResponse, TResponse>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateAsync.");
                throw;
            }
        }

        public virtual async Task<IEnumerable<TResponse>> CreateRangeAsync(IEnumerable<TRequest> entities)
        {
            try
            {
                var mapped = Map<TRequest, ERequest>(entities);
                var result = await _bpr.CreateRangeAsync(mapped.ToList());
                return Map<EResponse, TResponse>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateRangeAsync.");
                throw;
            }
        }

        public virtual async Task<TResponse> UpdateAsync(TRequest entity)
        {
            try
            {
                var mapped = Map<TRequest, ERequest>(entity);
                var result = await _bpr.UpdateAsync(mapped);
                return Map<EResponse, TResponse>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateAsync.");
                throw;
            }
        }

        public virtual async Task DeleteAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    throw new ArgumentNullException(nameof(id));

                await _bpr.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteAsync (by id).");
                throw;
            }
        }

        public virtual async Task DeleteRangeAsync(Expression<Func<TResponse, bool>> predicate)
        {
            try
            {
                var all = await GetAllAsync();
                var toDelete = all.AsQueryable().Where(predicate).ToList();
                var mapped = Map<TResponse, ERequest>(toDelete);
                await _bpr.DeleteRange(mapped.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteRangeAsync (by predicate).");
                throw;
            }
        }

        public virtual async Task<bool> ExistsAsync(Expression<Func<TResponse, bool>> predicate)
        {
            try
            {
                var all = await GetAllAsync();
                return all.AsQueryable().Any(predicate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ExistsAsync (by predicate).");
                throw;
            }
        }

        public virtual async Task<int> CountAsync()
        {
            try
            {
                var all = await GetAllAsync();
                return all.Count();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CountAsync.");
                throw;
            }
        }

        public virtual async Task<TResponse?> FindAsync(params object[] id)
        {
            try
            {
                var result = await _bpr.FindAsync(id);
                return Map<EResponse, TResponse>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in FindAsync (params object[] id).");
                throw;
            }
        }

        public virtual async Task<bool> ExistsAsync(object value, string name = "Id")
        {
            try
            {
                return await _bpr.ExistsAsync(value, name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ExistsAsync (by key).");
                throw;
            }
        }

        public virtual async Task<PagedResponse<TResponse>> GetAllAsync(string[]? includes = null, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var paged = await _bpr.GetAllAsync(includes, pageNumber, pageSize);
                return new PagedResponse<TResponse>
                    (
                        Map<EResponse, TResponse>(paged.Data),
                        paged.PageNumber,
                        paged.PageSize,
                        paged.TotalPages

                    );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllAsync with paging.");
                throw;
            }
        }

        public virtual async Task<TResponse?> GetByIdAsync(object id)
        {
            try
            {
                var result = await _bpr.GetByIdAsync(id);
                return Map<EResponse, TResponse>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetByIdAsync (object id).");
                throw;
            }
        }

        public virtual async Task DeleteAllAsync()
        {
            try
            {
                await _bpr.DeleteAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteAllAsync.");
                throw;
            }
        }

        public virtual async Task DeleteAsync(TRequest entity)
        {
            try
            {
                var mapped = Map<TRequest, ERequest>(entity);
                await _bpr.DeleteAsync(mapped);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteAsync (by entity).");
                throw;
            }
        }

        public virtual async Task DeleteAsync(object value, string key = "Id")
        {
            try
            {
                await _bpr.DeleteAsync(value, key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteAsync (object value, string key).");
                throw;
            }
        }

        public virtual async Task DeleteRange(List<TRequest> entities)
        {
            try
            {
                var mapped = Map<TRequest, ERequest>(entities);
                await _bpr.DeleteRange(mapped.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteRange (list).");
                throw;
            }
        }

        public virtual async Task<PagedResponse<TResponse>> GetAllByAsync(List<FilterCondition> conditions, ParamOptions? options = null)
        {
            try
            {
                var result = await _bpr.GetAllByAsync(conditions, options);
                return new PagedResponse<TResponse>
                (
                    Map<EResponse, TResponse>(result.Data),
                    result.PageNumber,
                    result.PageSize,
                    result.TotalPages

                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllByAsync.");
                throw;
            }
        }

        public virtual async Task<TResponse?> GetOneByAsync(List<FilterCondition> conditions, ParamOptions? options = null)
        {
            try
            {
                var result = await _bpr.GetOneByAsync(conditions, options);
                return Map<EResponse, TResponse>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetOneByAsync.");
                throw;
            }
        }
    }



    public class DataResult<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }

        public static DataResult<T> Ok(T data, string? message = null)
            => new DataResult<T> { Success = true, Message = message, Data = data };

        public static DataResult<T> Fail(string message)
            => new DataResult<T> { Success = false, Message = message, Data = default };
    }

    public interface IBPR<TRequest, TResponse> : IBasePublicRepository<TRequest, TResponse>
    where TRequest : class
    where TResponse : class
    {
        Task<DataResult<IEnumerable<TResponse>>> GetAllDataResultAsync();
        Task<DataResult<TResponse?>> GetByIdDataResultAsync(string id);
        Task<DataResult<TResponse?>> FindDataResultAsync(Expression<Func<TResponse, bool>> predicate);
        DataResult<IQueryable<TResponse>> GetQueryableDataResult();

        Task<DataResult<TResponse>> CreateDataResultAsync(TRequest entity);
        Task<DataResult<IEnumerable<TResponse>>> CreateRangeDataResultAsync(IEnumerable<TRequest> entities);

        Task<DataResult<TResponse>> UpdateDataResultAsync(TRequest entity);

        Task<DataResult<bool>> DeleteDataResultAsync(string id);
        Task<DataResult<bool>> DeleteRangeDataResultAsync(Expression<Func<TResponse, bool>> predicate);

        Task<DataResult<bool>> ExistsDataResultAsync(Expression<Func<TResponse, bool>> predicate);
        Task<DataResult<int>> CountDataResultAsync();

        Task<DataResult<TResponse?>> FindDataResultAsync(params object[] id);
        Task<DataResult<bool>> ExistsDataResultAsync(object value, string name = "Id");

        Task<DataResult<PagedResponse<TResponse>>> GetAllDataResultAsync(string[]? includes = null, int pageNumber = 1, int pageSize = 10);
        Task<DataResult<TResponse?>> GetByIdDataResultAsync(object id);
        Task<DataResult<bool>> DeleteAllDataResultAsync();
        Task<DataResult<bool>> DeleteDataResultAsync(TRequest entity);
        Task<DataResult<bool>> DeleteDataResultAsync(object value, string key = "Id");
        Task<DataResult<bool>> DeleteDataResultRange(List<TRequest> entities);

        Task<DataResult<PagedResponse<TResponse>>> GetAllByDataResultAsync(List<FilterCondition> conditions, ParamOptions? options = null);
        Task<DataResult<TResponse?>> GetOneByDataResultAsync(List<FilterCondition> conditions, ParamOptions? options = null);
    }


    public abstract class BPR<TRequest, TResponse, ERequest, EResponse> : BaseBPR<TRequest, TResponse, ERequest, EResponse>, IBPR<TRequest, TResponse>
     where TRequest : class
     where TResponse : class
     where ERequest : class
     where EResponse : class
    {

        protected new readonly IBPR<ERequest, EResponse> _bpr;

        public BPR(
            IMapper mapper,
            ILoggerFactory logger,
            IBPR<ERequest, EResponse> bpr) : base(mapper, logger, bpr)
        {

            _bpr = bpr;
        }



        public async Task<DataResult<IEnumerable<TResponse>>> GetAllDataResultAsync()
        {
            try
            {
                var entities = await GetAllAsync();

                return DataResult<IEnumerable<TResponse>>.Ok(entities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all records.");
                return DataResult<IEnumerable<TResponse>>.Fail("An error occurred while fetching records.");
            }
        }

      
        public async Task<DataResult<TResponse?>> GetByIdDataResultAsync(string id)
        {
            try
            {
                var entity = await GetByIdAsync(id);

                return DataResult<TResponse?>.Ok(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching record by ID.");
                return DataResult<TResponse?>.Fail("An error occurred while fetching the record.");
            }
        }

        public async Task<DataResult<TResponse?>> FindDataResultAsync(Expression<Func<TResponse, bool>> predicate)
        {
            _logger.LogWarning("Client-side predicate search is not supported on server-side repository.");
            return DataResult<TResponse?>.Fail("Cannot use predicate on mapped entity. Use a service-level implementation.");
        }

        public DataResult<IQueryable<TResponse>> GetQueryableDataResult()
        {
            _logger.LogWarning("Client-side IQueryable is not supported for mapped DTOs.");
            return DataResult<IQueryable<TResponse>>.Fail("Queryable not supported for DTO types.");
        }

        public async Task<DataResult<TResponse>> CreateDataResultAsync(TRequest entity)
        {
            try
            {
                var result = await CreateAsync(entity);
                return DataResult<TResponse>.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during create operation.");
                return DataResult<TResponse>.Fail("Failed to create entity.");
            }
        }

        public async Task<DataResult<IEnumerable<TResponse>>> CreateRangeDataResultAsync(IEnumerable<TRequest> entities)
        {
            try
            {
                var created = await CreateRangeAsync(entities);
                return DataResult<IEnumerable<TResponse>>.Ok(created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during range create operation.");
                return DataResult<IEnumerable<TResponse>>.Fail("Failed to create entities.");
            }
        }

        public async Task<DataResult<TResponse>> UpdateDataResultAsync(TRequest entity)
        {
            try
            {
                var updated = await UpdateAsync(entity);
                return DataResult<TResponse>.Ok(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during update operation.");
                return DataResult<TResponse>.Fail("Failed to update entity.");
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

        public async Task<DataResult<bool>> DeleteRangeDataResultAsync(Expression<Func<TResponse, bool>> predicate)
        {
            _logger.LogWarning("DeleteRange with predicate not supported directly on mapped DTO.");
            return DataResult<bool>.Fail("DeleteRange with predicate not supported for DTOs.");
        }

        public async Task<DataResult<bool>> ExistsDataResultAsync(Expression<Func<TResponse, bool>> predicate)
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

        public async Task<DataResult<TResponse?>> FindDataResultAsync(params object[] id)
        {
            try
            {
                var found = await FindAsync(id);
                return DataResult<TResponse?>.Ok(found);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during Find by keys.");
                return DataResult<TResponse?>.Fail("Find operation failed.");
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

        public async Task<DataResult<PagedResponse<TResponse>>> GetAllDataResultAsync(string[]? includes = null, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var result = await GetAllAsync(includes, pageNumber, pageSize);
                return DataResult<PagedResponse<TResponse>>.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during paged retrieval.");
                return DataResult<PagedResponse<TResponse>>.Fail("Paged retrieval failed.");
            }
        }

        public async Task<DataResult<TResponse?>> GetByIdDataResultAsync(object id)
        {
            try
            {
                var entity = await GetByIdAsync(id);
                return DataResult<TResponse?>.Ok(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during GetById.");
                return DataResult<TResponse?>.Fail("GetById failed.");
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

        public async Task<DataResult<bool>> DeleteDataResultAsync(TRequest entity)
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

        public async Task<DataResult<bool>> DeleteDataResultRange(List<TRequest> entities)
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

        public async Task<DataResult<PagedResponse<TResponse>>> GetAllByDataResultAsync(List<FilterCondition> conditions, ParamOptions? options = null)
        {
            try
            {
                var result = await GetAllByAsync(conditions, options);

                return DataResult<PagedResponse<TResponse>>.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during filtered paged retrieval.");
                return DataResult<PagedResponse<TResponse>>.Fail("Filtered query failed.");
            }
        }

        public async Task<DataResult<TResponse?>> GetOneByDataResultAsync(List<FilterCondition> conditions, ParamOptions? options = null)
        {
            try
            {
                var result = await GetOneByAsync(conditions, options);
                return DataResult<TResponse?>.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during GetOneBy.");
                return DataResult<TResponse?>.Fail("GetOneBy failed.");
            }
        }
    }



    public abstract class TBPR<TRequest, TResponse, ERequest, EResponse, IT, IE> : BPR<TRequest, TResponse, ERequest, EResponse>

     where TRequest : class
     where TResponse : class
     where ERequest : class
     where EResponse : class


    {

        private static bool IsAllowCreate()
        {
            return typeof(IT).IsAssignableFrom(typeof(TRequest)) &&
                   typeof(IT).IsAssignableFrom(typeof(TResponse)) &&
                   typeof(IE).IsAssignableFrom(typeof(ERequest)) &&
                   typeof(IE).IsAssignableFrom(typeof(EResponse));
        }
        protected TBPR(IMapper mapper, ILoggerFactory logger, IBPR<ERequest, EResponse> bpr) : base(mapper, logger, bpr)
        {

            if (!IsAllowCreate())
            {
                _logger.LogError("Creation failed: Specified types do not meet the required conditions.");
                throw new InvalidOperationException("Creation of this repository is not allowed for the specified types.");
            }

            _logger.LogInformation("BaseShareRepository initialized successfully.");

        }

    }

}
