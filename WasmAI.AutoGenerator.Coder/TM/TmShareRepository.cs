
using AutoGenerator.ApiFolder;

namespace AutoGenerator.TM
{

    public class TmShareRepository
    {

        public static string GetTmShareRepository(string nameShareTM, bool isBPR = false, TmOptions options = null)
        {





            if (ApiFolderInfo.IsBPR)
                return $@"

             /// <summary>
             /// {nameShareTM} class for ShareRepository.
             /// </summary>


                      public class {nameShareTM}ShareRepository : BaseBPRShareLayer<{nameShareTM}RequestShareDto, {nameShareTM}ResponseShareDto, {nameShareTM}RequestBuildDto, {nameShareTM}ResponseBuildDto>, I{nameShareTM}ShareRepository
    {{
        // Declare the builder repository.
        private readonly {nameShareTM}BuilderRepository _builder;

        public {nameShareTM}ShareRepository(IMapper mapper, ILoggerFactory logger, {nameShareTM}BuilderRepository bpr) : base(mapper, logger, bpr)
        {{
            _builder = bpr;
        }}

    

    }}


";






                return @$"
 /// <summary>
 /// {nameShareTM} class for ShareRepository.
 /// </summary>
 public class {nameShareTM}ShareRepository : BaseShareRepository<{nameShareTM}RequestShareDto, {nameShareTM}ResponseShareDto, {nameShareTM}RequestBuildDto, {nameShareTM}ResponseBuildDto>, I{nameShareTM}ShareRepository
 {{
     // Declare the builder repository.
     private readonly {nameShareTM}BuilderRepository _builder;

     /// <summary>
     /// Constructor for {nameShareTM}ShareRepository.
     /// </summary>
     public {nameShareTM}ShareRepository({ApiFolderInfo.TypeContext.Name} dbContext, IMapper mapper, ILoggerFactory logger,{nameShareTM}BuilderRepository b) : base(mapper, logger)
     {{
         // Initialize the builder repository.
         _builder = b;
         // Initialize the logger.
     
     }}

     /// <summary>
     /// Method to count the number of entities.
     /// </summary>
     public override Task<int> CountAsync()
     {{
         try
         {{
             _logger.LogInformation(""Counting {nameShareTM} entities..."");
            return _builder.CountAsync();
         }}
         catch (Exception ex)
         {{
             _logger.LogError(ex, ""Error in CountAsync for {nameShareTM} entities."");
             return Task.FromResult(0);
         }}
     }}

     /// <summary>
     /// Method to create a new entity asynchronously.
     /// </summary>
     public override async Task<{nameShareTM}ResponseShareDto> CreateAsync({nameShareTM}RequestShareDto entity)
     {{
         try
         {{
             _logger.LogInformation(""Creating new {nameShareTM} entity..."");
             // Call the create method in the builder repository.
             var result = await _builder.CreateAsync(entity);
             // Convert the result to ResponseShareDto type.
             var output = MapToShareResponseDto(result);
             _logger.LogInformation(""Created {nameShareTM} entity successfully."");
             // Return the final result.
             return output;
         }}
         catch (Exception ex)
         {{
             _logger.LogError(ex, ""Error while creating {nameShareTM} entity."");
             return null;
         }}
     }}

  
   
     /// <summary>
     /// Method to retrieve all entities.
     /// </summary>
     public override async Task<IEnumerable<{nameShareTM}ResponseShareDto>> GetAllAsync()
     {{
         try
         {{
             _logger.LogInformation(""Retrieving all {nameShareTM} entities..."");
             return MapToIEnumerableShareResponseDto(await _builder.GetAllAsync());
         }}
         catch (Exception ex)
         {{
             _logger.LogError(ex, ""Error in GetAllAsync for {nameShareTM} entities."");
             return null;
         }}
     }}

     /// <summary>
     /// Method to get an entity by its unique ID.
     /// </summary>
     public override async Task<{nameShareTM}ResponseShareDto?> GetByIdAsync(string id)
     {{
         try
         {{
             _logger.LogInformation($""Retrieving {nameShareTM} entity with ID: {{id}}..."");
             return MapToShareResponseDto(await _builder.GetByIdAsync(id));
         }}
         catch (Exception ex)
         {{
             _logger.LogError(ex, $""Error in GetByIdAsync for {nameShareTM} entity with ID: {{id}}."");
             return null;
         }}
     }}

    

     /// <summary>
     /// Method to retrieve data as an IQueryable object.
     /// </summary>
     public override IQueryable<{nameShareTM}ResponseShareDto> GetQueryable()
     {{
         try
         {{
             _logger.LogInformation(""Retrieving IQueryable<{nameShareTM}ResponseShareDto> for {nameShareTM} entities..."");
             return  MapToIEnumerableShareResponseDto(_builder.GetQueryable().ToList()).AsQueryable();
         }}
         catch (Exception ex)
         {{
             _logger.LogError(ex, ""Error in GetQueryable for {nameShareTM} entities."");
             return null;
         }}
     }}

     /// <summary>
     /// Method to save changes to the database.
     /// </summary>
     public Task SaveChangesAsync()
     {{
         try
         {{
             _logger.LogInformation(""Saving changes to the database for {nameShareTM} entities..."");
             throw new NotImplementedException();
         }}
         catch (Exception ex)
         {{
             _logger.LogError(ex, ""Error in SaveChangesAsync for {nameShareTM} entities."");
             return Task.CompletedTask;
         }}
     }}

     /// <summary>
     /// Method to update a specific entity.
     /// </summary>
     public override async Task<{nameShareTM}ResponseShareDto> UpdateAsync({nameShareTM}RequestShareDto entity)
     {{
         try
         {{
             _logger.LogInformation(""Updating {nameShareTM} entity..."");
             return MapToShareResponseDto(await _builder.UpdateAsync(entity));
         }}
         catch (Exception ex)
         {{
             _logger.LogError(ex, ""Error in UpdateAsync for {nameShareTM} entity."");
             return null;
         }}
     }}

     public override async Task<bool> ExistsAsync(object value, string name = ""Id"")
     {{
         try
         {{
             _logger.LogInformation(""Checking if {nameShareTM} exists with {{Key}}: {{Value}}"", name, value);
             var exists = await _builder.ExistsAsync(value, name);
             if (!exists)
             {{
                 _logger.LogWarning(""{nameShareTM} not found with {{Key}}: {{Value}}"", name, value);
             }}

             return exists;
         }}
         catch (Exception ex)
         {{
             _logger.LogError(ex, ""Error while checking existence of {nameShareTM} with {{Key}}: {{Value}}"", name, value);
             return false;
         }}
     }}

     public override async Task<PagedResponse<{nameShareTM}ResponseShareDto>> GetAllAsync(string[]? includes = null, int pageNumber = 1, int pageSize = 10)
     {{
         try
         {{
             _logger.LogInformation(""Fetching all {nameShareTM}s with pagination: Page {{PageNumber}}, Size {{PageSize}}"", pageNumber, pageSize);
             var results = (await _builder.GetAllAsync(includes, pageNumber, pageSize));
             var items =MapToIEnumerableShareResponseDto(results.Data);
             return new PagedResponse<{nameShareTM}ResponseShareDto>(items, results.PageNumber, results.PageSize, results.TotalPages);
         }}
         catch (Exception ex)
         {{
             _logger.LogError(ex, ""Error while fetching all {nameShareTM}s."");
             return new PagedResponse<{nameShareTM}ResponseShareDto>(new List<{nameShareTM}ResponseShareDto>(), pageNumber, pageSize, 0);
         }}
     }}

     public override async Task<{nameShareTM}ResponseShareDto?> GetByIdAsync(object id)
     {{
         try
         {{
             _logger.LogInformation(""Fetching {nameShareTM} by ID: {{Id}}"", id);
             var result = await _builder.GetByIdAsync(id);
             if (result == null)
             {{
                 _logger.LogWarning(""{nameShareTM} not found with ID: {{Id}}"", id);
                 return null;
             }}

             _logger.LogInformation(""Retrieved {nameShareTM} successfully."");
             return MapToShareResponseDto(result);
         }}
         catch (Exception ex)
         {{
             _logger.LogError(ex, ""Error while retrieving {nameShareTM} by ID: {{Id}}"", id);
             return null;
         }}
     }}

      public override Task DeleteAsync(string id)
        {{
            return _builder.DeleteAsync(id);
        }}
     
     public override async Task DeleteAsync(object value, string key = ""Id"")
     {{
         try
         {{
             _logger.LogInformation(""Deleting {nameShareTM} with {{Key}}: {{Value}}"", key, value);
             await _builder.DeleteAsync(value, key);
             _logger.LogInformation(""{nameShareTM} with {{Key}}: {{Value}} deleted successfully."", key, value);
         }}
         catch (Exception ex)
         {{
             _logger.LogError(ex, ""Error while deleting {nameShareTM} with {{Key}}: {{Value}}"", key, value);
         }}
     }}

     public override async Task DeleteRange(List<{nameShareTM}RequestShareDto> entities)
     {{
         try
         {{
             var builddtos = entities.OfType<{nameShareTM}RequestBuildDto>().ToList();
             _logger.LogInformation(""Deleting {{Count}} {nameShareTM}s..."", 201);
             await _builder.DeleteRange(builddtos);
             _logger.LogInformation(""{{Count}} {nameShareTM}s deleted successfully."", 202);
         }}
         catch (Exception ex)
         {{
             _logger.LogError(ex, ""Error while deleting multiple {nameShareTM}s."");
         }}
     }}

 

    public override async Task<PagedResponse<{nameShareTM}ResponseShareDto>> GetAllByAsync(List<FilterCondition> conditions, ParamOptions? options = null)
        {{
            try
            {{
                _logger.LogInformation(""[Share]Retrieving  {nameShareTM} entities as pagination..."");
                return MapToPagedResponse(await _builder.GetAllByAsync(conditions, options));
            }}
            catch (Exception ex)
            {{
                _logger.LogError(ex, ""[Share]Error in GetAllByAsync for {nameShareTM} entities as pagination."");
                return null;
            }}
        }}

        public override async Task<{nameShareTM}ResponseShareDto?> GetOneByAsync(List<FilterCondition> conditions, ParamOptions? options = null)
        {{
            try
            {{
                _logger.LogInformation(""[Share]Retrieving {nameShareTM} entity..."");
                return MapToShareResponseDto(await _builder.GetOneByAsync(conditions, options));
            }}
            catch (Exception ex)
            {{
                _logger.LogError(ex, ""[Share]Error in GetOneByAsync  for {nameShareTM} entity."");
                return null;
            }}
        }}



}}

";
        }


    }
}