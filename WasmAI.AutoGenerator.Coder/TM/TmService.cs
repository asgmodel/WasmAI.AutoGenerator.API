using AutoGenerator.ApiFolder;

namespace AutoGenerator.TM
{
    public class TmOptions
    {

        public string Type { get; set; }
        public string SubType { get; set; }
        public string NamespaceName { get; set; }
        public string PathFile { get; set; }
        public Dictionary<string, string> Items { get; set; } = new Dictionary<string, string>();
    }

    public class TmService
    {


        public static string GetTmService(string classNameServiceTM, TmOptions options = null)
        {


            if (ApiFolderInfo.IsBPR)


                return @$"

                  public class {classNameServiceTM}Service : BaseBPRServiceLayer<{classNameServiceTM}RequestDso, {classNameServiceTM}ResponseDso, {classNameServiceTM}RequestShareDto, {classNameServiceTM}ResponseShareDto>, IUse{classNameServiceTM}Service
    {{
        private readonly I{classNameServiceTM}ShareRepository _share;

        public {classNameServiceTM}Service(IMapper mapper, ILoggerFactory logger, I{classNameServiceTM}ShareRepository bpr) : base(mapper, logger, bpr)
        {{
            _share = bpr;
        }}
    }}

";

            return @$"

public class {classNameServiceTM}Service : BaseService<{classNameServiceTM}RequestDso, {classNameServiceTM}ResponseDso>, IUse{classNameServiceTM}Service
{{
    private readonly I{classNameServiceTM}ShareRepository _share;
    public {classNameServiceTM}Service(I{classNameServiceTM}ShareRepository build{classNameServiceTM}ShareRepository, IMapper mapper, ILoggerFactory logger) : base(mapper, logger)
    {{
        _share = build{classNameServiceTM}ShareRepository;
    }}


    public override Task<int> CountAsync()
    {{
        try
        {{
            _logger.LogInformation(""Counting {classNameServiceTM} entities..."");
            return _share.CountAsync();
        }}
        catch (Exception ex)
        {{
            _logger.LogError(ex,""Error in CountAsync for {classNameServiceTM} entities."");
            return Task.FromResult(0);
        }}
    }}

    public override async Task<{classNameServiceTM}ResponseDso> CreateAsync({classNameServiceTM}RequestDso entity)
    {{
        try
        {{
            _logger.LogInformation(""Creating new {classNameServiceTM} entity..."");
            var result = await _share.CreateAsync(entity);
            var output = GetMapper().Map<{classNameServiceTM}ResponseDso>(result);
            _logger.LogInformation(""Created {classNameServiceTM} entity successfully."");
            return output;
        }}
        catch (Exception ex)
        {{
            _logger.LogError(ex,""Error while creating {classNameServiceTM} entity."");
            return null;
        }}
    }}

    public override Task DeleteAsync(string id)
    {{
        try
        {{
            _logger.LogInformation($""Deleting {classNameServiceTM} entity with ID: {{id}}..."");
            return _share.DeleteAsync(id);
        }}
        catch (Exception ex)
        {{
            _logger.LogError(ex, $""Error while deleting {classNameServiceTM} entity with ID: {{id}}."");
            return Task.CompletedTask;
        }}
    }}

  
public override async Task<IEnumerable<{classNameServiceTM}ResponseDso>> GetAllAsync()
        {{
            try
            {{
                _logger.LogInformation(""Retrieving all {classNameServiceTM} entities..."");
                var results = await _share.GetAllAsync();
                return GetMapper().Map<IEnumerable<{classNameServiceTM}ResponseDso>>(results);
            }}
            catch (Exception ex)
            {{
                _logger.LogError(ex, ""Error in GetAllAsync for {classNameServiceTM} entities."");
                return null;
            }}
        }}

        public override async Task<{classNameServiceTM}ResponseDso?> GetByIdAsync(string id)
        {{
            try
            {{
                _logger.LogInformation($""Retrieving {classNameServiceTM} entity with ID: {{id}}..."");
                var result = await _share.GetByIdAsync(id);

                var item = GetMapper().Map<{classNameServiceTM}ResponseDso>(result);
                _logger.LogInformation(""Retrieved {classNameServiceTM} entity successfully."");
                return item;
            }}
            catch (Exception ex)
            {{
                _logger.LogError(ex, $""Error in GetByIdAsync for {classNameServiceTM} entity with ID: {{id}}."");
                return null;
            }}
        }}



        public override IQueryable<{classNameServiceTM}ResponseDso> GetQueryable()
        {{
            try
            {{
                _logger.LogInformation(""Retrieving IQueryable<{classNameServiceTM}ResponseDso> for {classNameServiceTM} entities..."");
                var queryable = _share.GetQueryable();
                var result = GetMapper().ProjectTo<{classNameServiceTM}ResponseDso>(queryable);
                return result;
            }}
            catch (Exception ex)
            {{
                _logger.LogError(ex, ""Error in GetQueryable for {classNameServiceTM} entities."");
                return null;
            }}
        }}



        public override async Task<{classNameServiceTM}ResponseDso> UpdateAsync({classNameServiceTM}RequestDso entity)
        {{
            try
            {{
                _logger.LogInformation(""Updating {classNameServiceTM} entity..."");

                var result = await _share.UpdateAsync(entity);

                return GetMapper().Map<{classNameServiceTM}ResponseDso>(result);
            }}
            catch (Exception ex)
            {{
                _logger.LogError(ex, ""Error in UpdateAsync for {classNameServiceTM} entity."");
                return null;
            }}
        }}

    public override async Task<bool> ExistsAsync(object value, string name =""Id"")
    {{
        try
        {{
            _logger.LogInformation(""Checking if {classNameServiceTM} exists with {{Key}}: {{Value}}"", name, value);
            var exists = await _share.ExistsAsync(value, name);

            if (!exists)
            {{
                _logger.LogWarning(""{classNameServiceTM} not found with {{Key}}: {{Value}}"", name, value);
            }}

            return exists;
        }}
        catch (Exception ex)
        {{
            _logger.LogError(ex,""Error while checking existence of {classNameServiceTM} with {{Key}}: {{Value}}"", name, value);
            return false;
        }}
    }}

    public override async Task<PagedResponse<{classNameServiceTM}ResponseDso>> GetAllAsync(string[]? includes = null, int pageNumber = 1, int pageSize = 10)
    {{
        try
        {{
            _logger.LogInformation(""Fetching all {classNameServiceTM}s with pagination: Page {{PageNumber}}, Size {{PageSize}}"", pageNumber, pageSize);
            var results = (await _share.GetAllAsync(includes, pageNumber, pageSize));
            var items = GetMapper().Map<List<{classNameServiceTM}ResponseDso>>(results.Data);
            return new PagedResponse<{classNameServiceTM}ResponseDso>(items, results.PageNumber, results.PageSize, results.TotalPages);
        }}
        catch (Exception ex)
        {{
            _logger.LogError(ex,""Error while fetching all {classNameServiceTM}s."");
            return new PagedResponse<{classNameServiceTM}ResponseDso>(new List<{classNameServiceTM}ResponseDso>(), pageNumber, pageSize, 0);
        }}
    }}

    public override async Task<{classNameServiceTM}ResponseDso?> GetByIdAsync(object id)
    {{
        try
        {{
            _logger.LogInformation(""Fetching {classNameServiceTM} by ID: {{Id}}"", id);
            var result = await _share.GetByIdAsync(id);

            if (result == null)
            {{
                _logger.LogWarning(""{classNameServiceTM} not found with ID: {{Id}}"", id);
                return null;
            }}

            _logger.LogInformation(""Retrieved {classNameServiceTM} successfully."");
            return GetMapper().Map<{classNameServiceTM}ResponseDso>(result);
        }}
        catch (Exception ex)
        {{
            _logger.LogError(ex,""Error while retrieving {classNameServiceTM} by ID: {{Id}}"", id);
            return null;
        }}
    }}

    public override async Task DeleteAsync(object value, string key =""Id"")
    {{
        try
        {{
            _logger.LogInformation(""Deleting {classNameServiceTM} with {{Key}}: {{Value}}"", key, value);
            await _share.DeleteAsync(value, key);
            _logger.LogInformation(""{classNameServiceTM} with {{Key}}: {{Value}} deleted successfully."", key, value);
        }}
        catch (Exception ex)
        {{
            _logger.LogError(ex,""Error while deleting {classNameServiceTM} with {{Key}}: {{Value}}"", key, value);
        }}
    }}

    public override async Task DeleteRange(List<{classNameServiceTM}RequestDso> entities)
    {{
        try
        {{
            var builddtos = entities.OfType<{classNameServiceTM}RequestShareDto>().ToList();
            _logger.LogInformation(""Deleting {{Count}} {classNameServiceTM}s..."", 201);
            await _share.DeleteRange(builddtos);
            _logger.LogInformation(""{{Count}} {classNameServiceTM}s deleted successfully."", 202);
        }}
        catch (Exception ex)
        {{
            _logger.LogError(ex,""Error while deleting multiple {classNameServiceTM}s."");
        }}
    }}






    public override async Task<PagedResponse<{classNameServiceTM}ResponseDso>> GetAllByAsync(List<FilterCondition> conditions, ParamOptions? options = null)
        {{
            try
            {{
                _logger.LogInformation(""Retrieving all {classNameServiceTM} entities..."");
                var results = await _share.GetAllAsync();
                var response = await _share.GetAllByAsync(conditions, options);
                return response.ToResponse(GetMapper().Map<IEnumerable<{classNameServiceTM}ResponseDso>>(response.Data));
            }}
            catch (Exception ex)
            {{
                _logger.LogError(ex, ""Error in GetAllAsync for {classNameServiceTM} entities."");
                return null;
            }}
        }}

        public override async Task<{classNameServiceTM}ResponseDso?> GetOneByAsync(List<FilterCondition> conditions, ParamOptions? options = null)
        {{
            try
            {{
                _logger.LogInformation(""Retrieving {classNameServiceTM} entity..."");
                return GetMapper().Map<{classNameServiceTM}ResponseDso>(await _share.GetOneByAsync(conditions, options));
            }}
            catch (Exception ex)
            {{
                _logger.LogError(ex, ""Error in GetOneByAsync  for {classNameServiceTM} entity."");
                return null;
            }}
        }}

}}

";



        }


    }
}