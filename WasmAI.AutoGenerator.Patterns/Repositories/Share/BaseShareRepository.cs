


using AutoGenerator.Helper;
using AutoGenerator.Repositories.Base;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace AutoGenerator.Repositories.Share
{
    public interface IBaseShareRepository<TShareRequestDto, TShareResponseDto> : IBasePublicRepository<TShareRequestDto, TShareResponseDto>, ITBaseShareRepository
        where TShareRequestDto : class
        where TShareResponseDto : class

    {

    }

    public abstract class BaseShareRepository<TShareRequestDto, TShareResponseDto, TBuildRequestDto, TBuildResponseDto>
        : IBaseShareRepository<TShareRequestDto, TShareResponseDto>
        where TShareRequestDto : class
        where TShareResponseDto : class
        where TBuildRequestDto : class
        where TBuildResponseDto : class
    {
        private readonly IMapper _mapper;
        protected readonly ILogger _logger;

        public BaseShareRepository(IMapper mapper, ILoggerFactory logger)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper), "Mapper instance cannot be null.");
            _logger = logger.CreateLogger(typeof(BaseShareRepository<TShareRequestDto, TShareResponseDto, TBuildRequestDto, TBuildResponseDto>).FullName) ?? throw new ArgumentNullException(nameof(logger), "Logger instance cannot be null.");

            if (!IsAllowCreate())
            {
                _logger.LogError("Creation failed: Specified types do not meet the required conditions.");
                throw new InvalidOperationException("Creation of this repository is not allowed for the specified types.");
            }

            _logger.LogInformation("BaseShareRepository initialized successfully.");
        }

        private static bool IsAllowCreate()
        {
            return typeof(ITShareDto).IsAssignableFrom(typeof(TShareRequestDto)) &&
                   typeof(ITShareDto).IsAssignableFrom(typeof(TShareResponseDto)) &&
                   typeof(ITBuildDto).IsAssignableFrom(typeof(TBuildResponseDto)) &&
                   typeof(ITBuildDto).IsAssignableFrom(typeof(TBuildRequestDto));
        }

        protected TBuildRequestDto MapToBuildRequestDto(TShareRequestDto shareRequestDto)
        {
            if (shareRequestDto == null)
            {
                _logger.LogError("Mapping failed: TShareRequestDto is null.");
                throw new ArgumentNullException(nameof(shareRequestDto), "The share request DTO cannot be null.");
            }

            return _mapper.Map<TBuildRequestDto>(shareRequestDto);
        }

        protected TShareResponseDto MapToShareResponseDto(TBuildResponseDto buildResponseDto)
        {
            if (buildResponseDto == null)
            {
                _logger.LogError("Mapping failed: TBuildResponseDto is null.");
                throw new ArgumentNullException(nameof(buildResponseDto), "The build response DTO cannot be null.");
            }

            return _mapper.Map<TShareResponseDto>(buildResponseDto);
        }

        protected IEnumerable<TShareResponseDto> MapToShareResponseDto(IEnumerable<TBuildResponseDto> buildResponseDto)
        {
            if (buildResponseDto == null)
            {
                _logger.LogError("Mapping failed: TBuildResponseDto is null.");
                throw new ArgumentNullException(nameof(buildResponseDto), "The build response DTO cannot be null.");
            }

            return _mapper.Map<IEnumerable<TShareResponseDto>>(buildResponseDto);
        }

        protected TShareResponseDto MapToShareResponseDto(TShareRequestDto shareRequestDto)
        {
            if (shareRequestDto == null)
            {
                _logger.LogError("Mapping failed: TShareRequestDto is null.");
                throw new ArgumentNullException(nameof(shareRequestDto), "The share request DTO cannot be null.");
            }

            return _mapper.Map<TShareResponseDto>(shareRequestDto);
        }

        protected TShareRequestDto MapToShareRequestDto(TBuildRequestDto buildRequestDto)
        {
            if (buildRequestDto == null)
            {
                _logger.LogError("Mapping failed: TBuildRequestDto is null.");
                throw new ArgumentNullException(nameof(buildRequestDto), "The build request DTO cannot be null.");
            }

            return _mapper.Map<TShareRequestDto>(buildRequestDto);
        }


        protected IEnumerable<TShareRequestDto> MapToIEnumerableShareRequestDto(IEnumerable<TBuildRequestDto> buildRequestDto)
        {
            if (buildRequestDto == null)
            {
                _logger.LogError("Mapping failed: TBuildRequestDto is null.");
                throw new ArgumentNullException(nameof(buildRequestDto), "The build request DTO cannot be null.");
            }

            return _mapper.Map<IEnumerable<TShareRequestDto>>(buildRequestDto);
        }


        protected IEnumerable<TShareResponseDto> MapToIEnumerableShareResponseDto(IEnumerable<TBuildResponseDto> buildResponseDto)
        {
            if (buildResponseDto == null)
            {
                _logger.LogError("Mapping failed: TBuildRequestDto is null.");
                throw new ArgumentNullException(nameof(buildResponseDto), "The build request DTO cannot be null.");
            }

            return _mapper.Map<IEnumerable<TShareResponseDto>>(buildResponseDto);
        }


        protected TBuildResponseDto MapToBuildResponseDto(TShareResponseDto shareResponseDto)
        {
            if (shareResponseDto == null)
            {
                _logger.LogError("Mapping failed: TShareResponseDto is null.");
                throw new ArgumentNullException(nameof(shareResponseDto), "The share response DTO cannot be null.");
            }

            return _mapper.Map<TBuildResponseDto>(shareResponseDto);
        }

        public virtual Task<IEnumerable<TShareResponseDto>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public virtual Task<TShareResponseDto?> GetByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public virtual Task<TShareResponseDto?> FindAsync(Expression<Func<TShareResponseDto, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public virtual IQueryable<TShareResponseDto> GetQueryable()
        {
            throw new NotImplementedException();
        }

        public virtual Task<TShareResponseDto> CreateAsync(TShareRequestDto entity)
        {
            throw new NotImplementedException();
        }

        public virtual Task<IEnumerable<TShareResponseDto>> CreateRangeAsync(IEnumerable<TShareRequestDto> entities)
        {
            throw new NotImplementedException();
        }

        public virtual Task<TShareResponseDto> UpdateAsync(TShareRequestDto entity)
        {
            throw new NotImplementedException();
        }

        public virtual Task DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        public virtual Task DeleteRangeAsync(Expression<Func<TShareResponseDto, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public virtual Task<bool> ExistsAsync(Expression<Func<TShareResponseDto, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public virtual Task<int> CountAsync()
        {
            throw new NotImplementedException();
        }

        public virtual Task<TShareResponseDto?> FindAsync(params object[] id)
        {
            throw new NotImplementedException();
        }

        public virtual  Task<bool> ExistsAsync(object value, string name = "Id")
        {
            throw new NotImplementedException();
        }

        public virtual  Task<PagedResponse<TShareResponseDto>> GetAllAsync(string[]? includes = null, int pageNumber = 1, int pageSize = 10)
        {
            throw new NotImplementedException();
        }

        public virtual Task<TShareResponseDto?> GetByIdAsync(object id)
        {
            throw new NotImplementedException();
        }

        public virtual Task DeleteAllAsync()
        {
            throw new NotImplementedException();
        }

        public virtual Task DeleteAsync(TShareRequestDto entity)
        {
            throw new NotImplementedException();
        }

        public virtual Task DeleteAsync(object value, string key = "Id")
        {
            throw new NotImplementedException();
        }

        public  virtual Task DeleteRange(List<TShareRequestDto> entities)
        {
            throw new NotImplementedException();
        }

        public virtual Task<PagedResponse<TShareResponseDto>> GetAllByAsync(List<FilterCondition> conditions, ParamOptions? options = null)
        {
            throw new NotImplementedException();
        }
        protected PagedResponse<TShareResponseDto> MapToPagedResponse(PagedResponse<TBuildResponseDto> response)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response), "The pagination cannot be null.");
            }

            return response.ToResponse(_mapper.Map<IEnumerable<TShareResponseDto>>(response.Data));
        }
        public virtual Task<TShareResponseDto?> GetOneByAsync(List<FilterCondition> conditions, ParamOptions? options = null)
        {
            throw new NotImplementedException();
        }
    }



 


}
