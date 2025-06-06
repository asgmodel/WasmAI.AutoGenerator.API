using AutoGenerator.Helper;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using System.Reflection; // Add this using directive

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


    public static class DataResultErrorCodes
    {
        public const int InvalidInput = 3000;
        public const int NotFound = 3001;
        public const int DuplicateEntry = 3002;
        public const int Unauthorized = 3003;
        public const int OperationFailed = 3004;
        public const int ValidationError = 3005;
        public const int DependencyFailure = 3006;
        public const int Timeout = 3007;
        public const int Conflict = 3008;
        public const int DataCorruption = 3009;
        public const int ResourceUnavailable = 3010;
        public const int InsufficientPermissions = 3011;
        public const int FormatError = 3012;
        public const int ConnectionError = 3013;
        public const int AuthenticationFailed = 3014;
        public const int ServiceUnavailable = 3015;
        public const int RateLimitExceeded = 3016;
        public const int OperationCancelled = 3017;
        public const int DependencyTimeout = 3018;

        public const int Create = 3100;
        public const int Update = 3101;
        public const int Delete = 3102;
        public const int Read = 3103;

        public const int MappingError = 3201;
        public const int MappingRequest = 3202;
        public const int MappingResponse = 3203;

        public const int UnknownError = 3999;
    }



    public class DataResultException : Exception
    {
        public int Code { get; set; }

        public string NameLayer { get; set; } = "Base";

        public string? NameFun { get; set; }

        public DataResultException(string? message = "", int code = 0, string namelayer = "", string namefun = "")
            : base(message)
        {
            Code = code;
            NameLayer = namelayer;
            NameFun = namefun;
        }



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
            _logger = logger.CreateLogger(this.GetType());
            _bpr = bpr ?? throw new ArgumentNullException(nameof(bpr));
        }
        protected DataResultException HandelResultException(Exception ex, string msg, int code = 0, string functionName = "")
        {
            // Use the exception's message if the provided message is null or empty, unless it's a specific mapping error message
            string finalMsg = string.IsNullOrWhiteSpace(msg) ? ex.Message : msg;

            _logger.LogError(ex, $"[Code:{code}] {finalMsg} Layer: {this.GetType().Name}, Function: {functionName}");
            return new DataResultException($"[Code:{code}] {finalMsg} Layer: {this.GetType().Name}, Function: {functionName}", code, this.GetType().Name, functionName);
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
                throw HandelResultException(ex, $"Mapping failed from {typeof(TSource).Name} to {typeof(TResult).Name}.", DataResultErrorCodes.MappingError, MethodBase.GetCurrentMethod()?.Name ?? "Map<TSource, TResult>");
            }
        }


        protected TResult Map<TResult>(object obj)

    where TResult : class
        {
            try
            {
                return _mapper.Map<TResult>(obj);
            }
            catch (Exception ex)
            {

                throw HandelResultException(ex, $"Mapping failed from {obj.GetType().Name} to {typeof(TResult).Name}.", DataResultErrorCodes.MappingError, MethodBase.GetCurrentMethod()?.Name ?? "Map<TResult>");

            }

        }

        protected IEnumerable<TResult> Map<TSource, TResult>(IEnumerable<TSource>? source)
            where TSource : class
            where TResult : class
        {
            try
            {
                if (source == null) return Enumerable.Empty<TResult>(); // Handle null source gracefully for collections

                return _mapper.Map<IEnumerable<TResult>>(source);
            }
            catch (Exception ex)
            {
                throw HandelResultException(ex, $"Mapping failed from IEnumerable {typeof(TSource).Name} to {typeof(TResult).Name}.", DataResultErrorCodes.MappingError, MethodBase.GetCurrentMethod()?.Name ?? "Map<IEnumerable<TSource>, IEnumerable<TResult>>");
            }
        }

        public virtual async Task<IEnumerable<TResponse>> GetAllAsync()
        {
            try
            {
                var data = await _bpr.GetAllAsync();
                return Map<EResponse, TResponse>(data);
            }
            catch (DataResultException)
            {
                throw; // Re-throw DataResultExceptions as they are already logged and structured
            }
            catch (Exception ex)
            {
                throw HandelResultException(ex, "Failed to retrieve all records.", DataResultErrorCodes.Read, nameof(GetAllAsync));
            }
        }


        public virtual async Task<TResponse?> GetByIdAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    throw HandelResultException(new ArgumentNullException(nameof(id)), $"Input '{nameof(id)}' cannot be null or whitespace.", DataResultErrorCodes.InvalidInput, nameof(GetByIdAsync));


                var entity = await _bpr.GetByIdAsync(id);
                return Map<TResponse>(entity);
            }
            catch (DataResultException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw HandelResultException(ex, "Failed to retrieve record by id.", DataResultErrorCodes.Read, nameof(GetByIdAsync));
            }
        }

        public virtual async Task<TResponse?> FindAsync(Expression<Func<TResponse, bool>> predicate)
        {
            try
            {
                if (predicate == null)
                    throw HandelResultException(new ArgumentNullException(nameof(predicate)), $"Input '{nameof(predicate)}' cannot be null.", DataResultErrorCodes.InvalidInput, nameof(FindAsync));


                // Note: Client-side filtering after fetching all might be inefficient for large datasets.
                // Consider implementing predicate passing in the underlying _bpr or using ProjectTo.
                var all = await GetAllAsync();
                return all.AsQueryable().FirstOrDefault(predicate);
            }
            catch (DataResultException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw HandelResultException(ex, "Failed to find record by predicate.", DataResultErrorCodes.Read, nameof(FindAsync));
            }
        }

        public virtual IQueryable<TResponse> GetQueryable()
        {
            try
            {
                var data = _bpr.GetQueryable();
                return _mapper.ProjectTo<TResponse>(data);
            }
            catch (DataResultException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw HandelResultException(ex, "Failed to get queryable.", DataResultErrorCodes.Read, nameof(GetQueryable));
            }
        }

        public virtual async Task<TResponse> CreateAsync(TRequest entity)
        {
            try
            {
                if (entity == null)
                    throw HandelResultException(new ArgumentNullException(nameof(entity)), $"Input '{nameof(entity)}' cannot be null.", DataResultErrorCodes.InvalidInput, nameof(CreateAsync));

                var mapped = Map<TRequest, ERequest>(entity);
                var result = await _bpr.CreateAsync(mapped);

                if (result == null)
                    throw new DataResultException("Creation returned null result.", DataResultErrorCodes.OperationFailed, this.GetType().Name, nameof(CreateAsync)); // Specific operational failure


                return Map<EResponse, TResponse>(result);
            }
            catch (DataResultException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw HandelResultException(ex, "Failed to create record.", DataResultErrorCodes.Create, nameof(CreateAsync));
            }
        }

        public virtual async Task<IEnumerable<TResponse>> CreateRangeAsync(IEnumerable<TRequest> entities)
        {
            try
            {
                if (entities == null)
                    throw HandelResultException(new ArgumentNullException(nameof(entities)), $"Input '{nameof(entities)}' cannot be null.", DataResultErrorCodes.InvalidInput, nameof(CreateRangeAsync));

                if (!entities.Any()) return Enumerable.Empty<TResponse>(); // Handle empty list gracefully


                var mapped = Map<TRequest, ERequest>(entities);
                var result = await _bpr.CreateRangeAsync(mapped.ToList());

                if (result == null)
                    throw new DataResultException("Create range returned null result.", DataResultErrorCodes.OperationFailed, this.GetType().Name, nameof(CreateRangeAsync)); // Specific operational failure


                return Map<EResponse, TResponse>(result);
            }
            catch (DataResultException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw HandelResultException(ex, "Failed to create records in range.", DataResultErrorCodes.Create, nameof(CreateRangeAsync));
            }
        }

        public virtual async Task<TResponse> UpdateAsync(TRequest entity)
        {
            try
            {
                if (entity == null)
                    throw HandelResultException(new ArgumentNullException(nameof(entity)), $"Input '{nameof(entity)}' cannot be null.", DataResultErrorCodes.InvalidInput, nameof(UpdateAsync));


                var mapped = Map<TRequest, ERequest>(entity);
                var result = await _bpr.UpdateAsync(mapped);

                if (result == null)
                    throw new DataResultException("Update returned null result.", DataResultErrorCodes.OperationFailed, this.GetType().Name, nameof(UpdateAsync)); // Specific operational failure


                return Map<EResponse, TResponse>(result);
            }
            catch (DataResultException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw HandelResultException(ex, "Failed to update record.", DataResultErrorCodes.Update, nameof(UpdateAsync));
            }
        }

        public virtual async Task DeleteAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    throw HandelResultException(new ArgumentNullException(nameof(id)), $"Input '{nameof(id)}' cannot be null or whitespace.", DataResultErrorCodes.InvalidInput, nameof(DeleteAsync));


                await _bpr.DeleteAsync(id);
            }
            catch (DataResultException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw HandelResultException(ex, "Failed to delete record by id.", DataResultErrorCodes.Delete, nameof(DeleteAsync));
            }
        }

        public virtual async Task DeleteRangeAsync(Expression<Func<TResponse, bool>> predicate)
        {
            try
            {
                if (predicate == null)
                    throw HandelResultException(new ArgumentNullException(nameof(predicate)), $"Input '{nameof(predicate)}' cannot be null.", DataResultErrorCodes.InvalidInput, nameof(DeleteRangeAsync));


                // Note: Client-side filtering after fetching all might be inefficient for large datasets.
                var all = await GetAllAsync();
                var toDelete = all.AsQueryable().Where(predicate).ToList();

                if (!toDelete.Any()) return; // Nothing to delete

                var mapped = Map<TResponse, ERequest>(toDelete);
                await _bpr.DeleteRange(mapped.ToList());
            }
            catch (DataResultException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw HandelResultException(ex, "Failed to delete records by predicate.", DataResultErrorCodes.Delete, nameof(DeleteRangeAsync));
            }
        }

        public virtual async Task<bool> ExistsAsync(Expression<Func<TResponse, bool>> predicate)
        {
            try
            {
                if (predicate == null)
                    throw HandelResultException(new ArgumentNullException(nameof(predicate)), $"Input '{nameof(predicate)}' cannot be null.", DataResultErrorCodes.InvalidInput, nameof(ExistsAsync));

                // Note: Client-side filtering after fetching all might be inefficient for large datasets.
                var all = await GetAllAsync();
                return all.AsQueryable().Any(predicate);
            }
            catch (DataResultException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw HandelResultException(ex, "Failed to check existence by predicate.", DataResultErrorCodes.Read, nameof(ExistsAsync));
            }
        }

        public virtual async Task<int> CountAsync()
        {
            try
            {
                // Note: Client-side counting after fetching all might be inefficient for large datasets.
                var all = await GetAllAsync();
                return all.Count();
            }
            catch (DataResultException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw HandelResultException(ex, "Failed to count records.", DataResultErrorCodes.Read, nameof(CountAsync));
            }
        }

        public virtual async Task<TResponse?> FindAsync(params object[] id)
        {
            try
            {
                if (id == null || id.Length == 0 || id.Any(x => x == null))
                    throw HandelResultException(new ArgumentNullException(nameof(id)), $"Input '{nameof(id)}' cannot be null or empty, and must not contain null values.", DataResultErrorCodes.InvalidInput, nameof(FindAsync));


                var result = await _bpr.FindAsync(id);
                return Map<EResponse, TResponse>(result);
            }
            catch (DataResultException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw HandelResultException(ex, "Failed to find record by id.", DataResultErrorCodes.Read, nameof(FindAsync));
            }
        }

        public virtual async Task<bool> ExistsAsync(object value, string name = "Id")
        {
            try
            {
                if (value == null)
                    throw HandelResultException(new ArgumentNullException(nameof(value)), $"Input '{nameof(value)}' cannot be null.", DataResultErrorCodes.InvalidInput, nameof(ExistsAsync));
                if (string.IsNullOrWhiteSpace(name))
                    throw HandelResultException(new ArgumentNullException(nameof(name)), $"Input '{nameof(name)}' cannot be null or whitespace.", DataResultErrorCodes.InvalidInput, nameof(ExistsAsync));


                return await _bpr.ExistsAsync(value, name);
            }
            catch (DataResultException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw HandelResultException(ex, $"Failed to check existence by {name}.", DataResultErrorCodes.Read, nameof(ExistsAsync));
            }
        }

        public virtual async Task<PagedResponse<TResponse>> GetAllAsync(string[]? includes = null, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                if (pageNumber < 1)
                    throw HandelResultException(new ArgumentOutOfRangeException(nameof(pageNumber)), $"Input '{nameof(pageNumber)}' must be 1 or greater.", DataResultErrorCodes.InvalidInput, nameof(GetAllAsync));
                if (pageSize < 1)
                    throw HandelResultException(new ArgumentOutOfRangeException(nameof(pageSize)), $"Input '{nameof(pageSize)}' must be 1 or greater.", DataResultErrorCodes.InvalidInput, nameof(GetAllAsync));


                var paged = await _bpr.GetAllAsync(includes, pageNumber, pageSize);

                if (paged == null || paged.Data == null)
                    throw new DataResultException("Paged retrieval returned null result.", DataResultErrorCodes.OperationFailed, this.GetType().Name, nameof(GetAllAsync)); // Specific operational failure


                return new PagedResponse<TResponse>
                    (
                        Map<EResponse, TResponse>(paged.Data),
                        paged.PageNumber,
                        paged.PageSize,
                        paged.TotalPages

                    );
            }
            catch (DataResultException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw HandelResultException(ex, "Failed to retrieve paged records.", DataResultErrorCodes.Read, nameof(GetAllAsync));
            }
        }

        public virtual async Task<TResponse?> GetByIdAsync(object id)
        {
            try
            {
                if (id == null)
                    throw HandelResultException(new ArgumentNullException(nameof(id)), $"Input '{nameof(id)}' cannot be null.", DataResultErrorCodes.InvalidInput, nameof(GetByIdAsync));


                var result = await _bpr.GetByIdAsync(id);
                return Map<EResponse, TResponse>(result);
            }
            catch (DataResultException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw HandelResultException(ex, "Failed to retrieve record by id.", DataResultErrorCodes.Read, nameof(GetByIdAsync));
            }
        }

        public virtual async Task DeleteAllAsync()
        {
            try
            {
                await _bpr.DeleteAllAsync();
            }
            catch (DataResultException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw HandelResultException(ex, "Failed to delete all records.", DataResultErrorCodes.Delete, nameof(DeleteAllAsync));
            }
        }

        public virtual async Task DeleteAsync(TRequest entity)
        {
            try
            {
                if (entity == null)
                    throw HandelResultException(new ArgumentNullException(nameof(entity)), $"Input '{nameof(entity)}' cannot be null.", DataResultErrorCodes.InvalidInput, nameof(DeleteAsync));

                var mapped = Map<TRequest, ERequest>(entity);
                await _bpr.DeleteAsync(mapped);
            }
            catch (DataResultException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw HandelResultException(ex, "Failed to delete record.", DataResultErrorCodes.Delete, nameof(DeleteAsync));
            }
        }

        public virtual async Task DeleteAsync(object value, string key = "Id")
        {
            try
            {
                if (value == null)
                    throw HandelResultException(new ArgumentNullException(nameof(value)), $"Input '{nameof(value)}' cannot be null.", DataResultErrorCodes.InvalidInput, nameof(DeleteAsync));
                if (string.IsNullOrWhiteSpace(key))
                    throw HandelResultException(new ArgumentNullException(nameof(key)), $"Input '{nameof(key)}' cannot be null or whitespace.", DataResultErrorCodes.InvalidInput, nameof(DeleteAsync));


                await _bpr.DeleteAsync(value, key);
            }
            catch (DataResultException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw HandelResultException(ex, $"Failed to delete record by {key}.", DataResultErrorCodes.Delete, nameof(DeleteAsync));
            }
        }

        public virtual async Task DeleteRange(List<TRequest> entities)
        {
            try
            {
                if (entities == null)
                    throw HandelResultException(new ArgumentNullException(nameof(entities)), $"Input '{nameof(entities)}' cannot be null.", DataResultErrorCodes.InvalidInput, nameof(DeleteRange));
                if (!entities.Any()) return; // Nothing to delete

                var mapped = Map<TRequest, ERequest>(entities);
                await _bpr.DeleteRange(mapped.ToList());
            }
            catch (DataResultException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw HandelResultException(ex, "Failed to delete records in range.", DataResultErrorCodes.Delete, nameof(DeleteRange));
            }
        }

        public virtual async Task<PagedResponse<TResponse>> GetAllByAsync(List<FilterCondition> conditions, ParamOptions? options = null)
        {
            try
            {
                if (conditions == null)
                    throw HandelResultException(new ArgumentNullException(nameof(conditions)), $"Input '{nameof(conditions)}' cannot be null.", DataResultErrorCodes.InvalidInput, nameof(GetAllByAsync));


                var result = await _bpr.GetAllByAsync(conditions, options);

                if (result == null || result.Data == null)
                    throw new DataResultException("Filtered paged retrieval returned null result.", DataResultErrorCodes.OperationFailed, this.GetType().Name, nameof(GetAllByAsync)); // Specific operational failure


                return new PagedResponse<TResponse>
                (
                    Map<EResponse, TResponse>(result.Data),
                    result.PageNumber,
                    result.PageSize,
                    result.TotalPages

                );
            }
            catch (DataResultException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw HandelResultException(ex, "Failed to retrieve records by conditions.", DataResultErrorCodes.Read, nameof(GetAllByAsync));
            }
        }

        public virtual async Task<TResponse?> GetOneByAsync(List<FilterCondition> conditions, ParamOptions? options = null)
        {
            try
            {
                if (conditions == null)
                    throw HandelResultException(new ArgumentNullException(nameof(conditions)), $"Input '{nameof(conditions)}' cannot be null.", DataResultErrorCodes.InvalidInput, nameof(GetOneByAsync));


                var result = await _bpr.GetOneByAsync(conditions, options);
                return Map<EResponse, TResponse>(result);
            }
            catch (DataResultException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw HandelResultException(ex, "Failed to retrieve one record by conditions.", DataResultErrorCodes.Read, nameof(GetOneByAsync));
            }
        }
    }



    public class DataResult<T>
    {
        public bool Success => success;


        private  DataResultException? _exception;

        public string? Message => _exception?.Message;

        private bool success;


      


        public int? Code => _exception?.Code ;


        public DataResultException? Exception => _exception;
        public T? Data { get; set; }

        public static DataResult<T> Ok(T data, int code = 200)
            => new DataResult<T> { success = true, Data = data };

        public static DataResult<T> Fail(string message, int code = DataResultErrorCodes.OperationFailed,string  namelayer="")
            => new DataResult<T> { success = false,  Data = default, _exception = new DataResultException(message,code, namelayer) };

        public static DataResult<T> Fail(DataResultException exception)
           => new DataResult<T> { success = false, _exception = exception};





    }

    public static class DataResultExtensions
    {
  

  

        public static DataResult<T> ChangeException<T>(this DataResult<T> result, DataResultException exception)
        {
            
        
            return result;

        }
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
                var entities = await GetAllAsync(); // This can throw DataResultException or other Exceptions from BaseBPR
                return DataResult<IEnumerable<TResponse>>.Ok(entities);
            }
            catch (DataResultException dex)
            {
                // If BaseBPR threw a DataResultException (already logged), just wrap it
                return DataResult<IEnumerable<TResponse>>.Fail(dex);
            }
            catch (Exception ex)
            {
                // For unexpected exceptions, log and create a new DataResultException
                var handledEx = HandelResultException(ex, "Failed to retrieve all records.", DataResultErrorCodes.Read, nameof(GetAllDataResultAsync));
                return DataResult<IEnumerable<TResponse>>.Fail(handledEx);
            }
        }


        public override Task<int> CountAsync()
        {
            return base.CountAsync();
        }


        public async Task<DataResult<TResponse?>> GetByIdDataResultAsync(string id)
        {
            try
            {
                var entity = await GetByIdAsync(id);
                return DataResult<TResponse?>.Ok(entity);
            }
            catch (DataResultException dex)
            {
                return DataResult<TResponse?>.Fail(dex);
            }
            catch (Exception ex)
            {
                var handledEx = HandelResultException(ex, "Failed to fetch record by ID.", DataResultErrorCodes.Read, nameof(GetByIdDataResultAsync));
                return DataResult<TResponse?>.Fail(handledEx);
            }
        }

        public async Task<DataResult<TResponse?>> FindDataResultAsync(Expression<Func<TResponse, bool>> predicate)
        {
            try
            {
                // Base implementation fetches all and filters client-side, which might be inefficient.
                // A better implementation would pass the predicate down to the underlying _bpr if possible.
                var entity = await FindAsync(predicate);
                return DataResult<TResponse?>.Ok(entity);
            }
            catch (DataResultException dex)
            {
                return DataResult<TResponse?>.Fail(dex);
            }
            catch (Exception ex)
            {
                var handledEx = HandelResultException(ex, "Failed to find record by predicate.", DataResultErrorCodes.Read, nameof(FindAsync));
                return DataResult<TResponse?>.Fail(handledEx);
            }
        }

        public DataResult<IQueryable<TResponse>> GetQueryableDataResult()
        {
            try
            {
                var queryable = GetQueryable();
                return DataResult<IQueryable<TResponse>>.Ok(queryable);
            }
            catch (DataResultException dex)
            {
                return DataResult<IQueryable<TResponse>>.Fail(dex);
            }
            catch (Exception ex)
            {
                var handledEx = HandelResultException(ex, "Failed to get queryable.", DataResultErrorCodes.Read, nameof(GetQueryableDataResult));
                return DataResult<IQueryable<TResponse>>.Fail(handledEx);
            }
        }

        public async Task<DataResult<TResponse>> CreateDataResultAsync(TRequest entity)
        {
            try
            {
                var result = await CreateAsync(entity);
                return DataResult<TResponse>.Ok(result);
            }
            catch (DataResultException dex)
            {
                return DataResult<TResponse>.Fail(dex);
            }
            catch (Exception ex)
            {
                var handledEx = HandelResultException(ex, "Failed to create entity.", DataResultErrorCodes.Create, nameof(CreateDataResultAsync));
                return DataResult<TResponse>.Fail(handledEx);
            }
        }

        public async Task<DataResult<IEnumerable<TResponse>>> CreateRangeDataResultAsync(IEnumerable<TRequest> entities)
        {
            try
            {
                var created = await CreateRangeAsync(entities);
                return DataResult<IEnumerable<TResponse>>.Ok(created);
            }
            catch (DataResultException dex)
            {
                return DataResult<IEnumerable<TResponse>>.Fail(dex);
            }
            catch (Exception ex)
            {
                var handledEx = HandelResultException(ex, "Failed to create entities in range.", DataResultErrorCodes.Create, nameof(CreateRangeDataResultAsync));
                return DataResult<IEnumerable<TResponse>>.Fail(handledEx);
            }
        }

        public async Task<DataResult<TResponse>> UpdateDataResultAsync(TRequest entity)
        {
            try
            {
                var updated = await UpdateAsync(entity);
                return DataResult<TResponse>.Ok(updated);
            }
            catch (DataResultException dex)
            {
                return DataResult<TResponse>.Fail(dex);
            }
            catch (Exception ex)
            {
                var handledEx = HandelResultException(ex, "Failed to update entity.", DataResultErrorCodes.Update, nameof(UpdateDataResultAsync));
                return DataResult<TResponse>.Fail(handledEx);
            }
        }

        public async Task<DataResult<bool>> DeleteDataResultAsync(string id)
        {
            try
            {
                await DeleteAsync(id);
                return DataResult<bool>.Ok(true);
            }
            catch (DataResultException dex)
            {
                return DataResult<bool>.Fail(dex);
            }
            catch (Exception ex)
            {
                var handledEx = HandelResultException(ex, $"Failed to delete entity with ID: {id}.", DataResultErrorCodes.Delete, nameof(DeleteDataResultAsync));
                return DataResult<bool>.Fail(handledEx);
            }
        }

        public async Task<DataResult<bool>> DeleteRangeDataResultAsync(Expression<Func<TResponse, bool>> predicate)
        {
            try
            {
                // Note: Base implementation fetches all and filters client-side, which might be inefficient.
                // A better implementation would pass the predicate down to the underlying _bpr if possible.
                await DeleteRangeAsync(predicate);
                return DataResult<bool>.Ok(true);
            }
            catch (DataResultException dex)
            {
                return DataResult<bool>.Fail(dex);
            }
            catch (Exception ex)
            {
                var handledEx = HandelResultException(ex, "Failed to delete records by predicate.", DataResultErrorCodes.Delete, nameof(DeleteRangeDataResultAsync));
                return DataResult<bool>.Fail(handledEx);
            }
        }

        public async Task<DataResult<bool>> ExistsDataResultAsync(Expression<Func<TResponse, bool>> predicate)
        {
            try
            {
                // Note: Base implementation fetches all and filters client-side, which might be inefficient.
                var exists = await ExistsAsync(predicate);
                return DataResult<bool>.Ok(exists);
            }
            catch (DataResultException dex)
            {
                return DataResult<bool>.Fail(dex);
            }
            catch (Exception ex)
            {
                var handledEx = HandelResultException(ex, "Failed to check existence by predicate.", DataResultErrorCodes.Read, nameof(ExistsDataResultAsync));
                return DataResult<bool>.Fail(handledEx);
            }
        }

        public async Task<DataResult<int>> CountDataResultAsync()
        {
            try
            {
                // Note: Base implementation fetches all and counts client-side, which might be inefficient.
                var count = await CountAsync();
                return DataResult<int>.Ok(count);
            }
            catch (DataResultException dex)
            {
                return DataResult<int>.Fail(dex);
            }
            catch (Exception ex)
            {
                var handledEx = HandelResultException(ex, "Failed to count entities.", DataResultErrorCodes.Read, nameof(CountDataResultAsync));
                return DataResult<int>.Fail(handledEx);
            }
        }

        public async Task<DataResult<TResponse?>> FindDataResultAsync(params object[] id)
        {
            try
            {
                var found = await FindAsync(id);
                return DataResult<TResponse?>.Ok(found);
            }
            catch (DataResultException dex)
            {
                return DataResult<TResponse?>.Fail(dex);
            }
            catch (Exception ex)
            {
                var handledEx = HandelResultException(ex, "Find operation failed.", DataResultErrorCodes.Read, nameof(FindAsync));
                return DataResult<TResponse?>.Fail(handledEx);
            }
        }

        public async Task<DataResult<bool>> ExistsDataResultAsync(object value, string name = "Id")
        {
            try
            {
                var exists = await ExistsAsync(value, name);
                return DataResult<bool>.Ok(exists);
            }
            catch (DataResultException dex)
            {
                return DataResult<bool>.Fail(dex);
            }
            catch (Exception ex)
            {
                var handledEx = HandelResultException(ex, $"Failed to check existence by {name}.", DataResultErrorCodes.Read, nameof(ExistsDataResultAsync));
                return DataResult<bool>.Fail(handledEx);
            }
        }

        public async Task<DataResult<PagedResponse<TResponse>>> GetAllDataResultAsync(string[]? includes = null, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var result = await GetAllAsync(includes, pageNumber, pageSize);
                return DataResult<PagedResponse<TResponse>>.Ok(result);
            }
            catch (DataResultException dex)
            {
                return DataResult<PagedResponse<TResponse>>.Fail(dex);
            }
            catch (Exception ex)
            {
                var handledEx = HandelResultException(ex, "Paged retrieval failed.", DataResultErrorCodes.Read, nameof(GetAllDataResultAsync));
                return DataResult<PagedResponse<TResponse>>.Fail(handledEx);
            }
        }

        public async Task<DataResult<TResponse?>> GetByIdDataResultAsync(object id)
        {
            try
            {
                var entity = await GetByIdAsync(id);
                return DataResult<TResponse?>.Ok(entity);
            }
            catch (DataResultException dex)
            {
                return DataResult<TResponse?>.Fail(dex);
            }
            catch (Exception ex)
            {
                var handledEx = HandelResultException(ex, "GetById failed.", DataResultErrorCodes.Read, nameof(GetByIdDataResultAsync));
                return DataResult<TResponse?>.Fail(handledEx);
            }
        }

        public async Task<DataResult<bool>> DeleteAllDataResultAsync()
        {
            try
            {
                await DeleteAllAsync();
                return DataResult<bool>.Ok(true);
            }
            catch (DataResultException dex)
            {
                return DataResult<bool>.Fail(dex);
            }
            catch (Exception ex)
            {
                var handledEx = HandelResultException(ex, "DeleteAll failed.", DataResultErrorCodes.Delete, nameof(DeleteAllDataResultAsync));
                return DataResult<bool>.Fail(handledEx);
            }
        }

        public async Task<DataResult<bool>> DeleteDataResultAsync(TRequest entity)
        {
            try
            {
                await DeleteAsync(entity);
                return DataResult<bool>.Ok(true);
            }
            catch (DataResultException dex)
            {
                return DataResult<bool>.Fail(dex);
            }
            catch (Exception ex)
            {
                var handledEx = HandelResultException(ex, "Delete failed.", DataResultErrorCodes.Delete, nameof(DeleteDataResultAsync));
                return DataResult<bool>.Fail(handledEx);
            }
        }

        public async Task<DataResult<bool>> DeleteDataResultAsync(object value, string key = "Id")
        {
            try
            {
                await DeleteAsync(value, key);
                return DataResult<bool>.Ok(true);
            }
            catch (DataResultException dex)
            {
                return DataResult<bool>.Fail(dex);
            }
            catch (Exception ex)
            {
                var handledEx = HandelResultException(ex, $"Delete failed by {key}.", DataResultErrorCodes.Delete, nameof(DeleteDataResultAsync));
                return DataResult<bool>.Fail(handledEx);
            }
        }

        public async Task<DataResult<bool>> DeleteDataResultRange(List<TRequest> entities)
        {
            try
            {
                await DeleteRange(entities);
                return DataResult<bool>.Ok(true);
            }
            catch (DataResultException dex)
            {
                return DataResult<bool>.Fail(dex);
            }
            catch (Exception ex)
            {
                var handledEx = HandelResultException(ex, "DeleteRange failed.", DataResultErrorCodes.Delete, nameof(DeleteDataResultRange));
                return DataResult<bool>.Fail(handledEx);
            }
        }

        public async Task<DataResult<PagedResponse<TResponse>>> GetAllByDataResultAsync(List<FilterCondition> conditions, ParamOptions? options = null)
        {
            try
            {
                var result = await GetAllByAsync(conditions, options);

                return DataResult<PagedResponse<TResponse>>.Ok(result);
            }
            catch (DataResultException dex)
            {
                return DataResult<PagedResponse<TResponse>>.Fail(dex);
            }
            catch (Exception ex)
            {
                var handledEx = HandelResultException(ex, "Filtered query failed.", DataResultErrorCodes.Read, nameof(GetAllByDataResultAsync));
                return DataResult<PagedResponse<TResponse>>.Fail(handledEx);
            }
        }

        public async Task<DataResult<TResponse?>> GetOneByDataResultAsync(List<FilterCondition> conditions, ParamOptions? options = null)
        {
            try
            {
                var result = await GetOneByAsync(conditions, options);
                return DataResult<TResponse?>.Ok(result);
            }
            catch (DataResultException dex)
            {
                return DataResult<TResponse?>.Fail(dex);
            }
            catch (Exception ex)
            {
                var handledEx = HandelResultException(ex, "GetOneBy failed.", DataResultErrorCodes.Read, nameof(GetOneByDataResultAsync));
                return DataResult<TResponse?>.Fail(handledEx);
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
                // Log this configuration error specifically
                var ex = new InvalidOperationException($"Creation failed for {this.GetType().Name}: Specified types (TRequest, TResponse, ERequest, EResponse) do not meet the required constraints based on IT ({typeof(IT).Name}) and IE ({typeof(IE).Name}).");
                _logger.LogError(ex, ex.Message);
                throw ex; // Throw standard exception for config errors during startup

            }


        }

    }

}