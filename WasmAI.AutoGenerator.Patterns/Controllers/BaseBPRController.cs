using AutoGenerator.Helper.Translation;
using AutoGenerator.Repositories.Base;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AutoGenerator.Controllers.Base
{
    public abstract class BaseBPRController<TRequest, TResponse, VMCreate, VMOutput, VMUpdate, VMInfo, VMDelete> : ControllerBase
       where TRequest : class
       where TResponse : class
       where VMCreate : class
       where VMOutput : class
       where VMUpdate : class
       where VMInfo : class
       where VMDelete : class
    {
        protected readonly IBPR<TRequest, TResponse> _bPR;
        protected readonly IMapper _mapper;
        protected readonly ILogger _logger;

        protected BaseBPRController(IMapper mapper, ILoggerFactory logger, IBPR<TRequest, TResponse> bPR)
        {
            _mapper = mapper;
            _logger = logger.CreateLogger<BaseBPRController<TRequest, TResponse, VMCreate, VMOutput, VMUpdate, VMInfo, VMDelete>>();
            _bPR = bPR;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public virtual async Task<ActionResult<IEnumerable<VMOutput>>> GetAllAsync()
        {
            var result = await _bPR.GetAllDataResultAsync();

            if (!result.Success)
            {
                _logger.LogWarning("Failed to retrieve all items: {Message}", result.Message);
                return BadRequest(result.Message ?? "Unknown error");
            }

            var output = _mapper.Map<IEnumerable<VMOutput>>(result.Data);
            return Ok(output);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public virtual async Task<ActionResult<VMInfo>> GetByIdAsync(string id)
        {
            var result = await _bPR.GetByIdDataResultAsync(id);

            if (!result.Success || result.Data == null)
            {
                _logger.LogWarning("Item with ID {Id} not found. Reason: {Message}", id, result.Message);
                return NotFound(new ProblemDetails { Title = "Not Found", Detail = result.Message ?? $"Item with ID {id} not found" });
            }

            var output = _mapper.Map<VMInfo>(result.Data);
            return Ok(output);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public virtual async Task<ActionResult<VMOutput>> CreateAsync([FromBody] VMCreate model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model validation failed on create.");
                return BadRequest("Invalid model");
            }

            var request = _mapper.Map<TRequest>(model);
            var result = await _bPR.CreateDataResultAsync(request);

            if (!result.Success)
            {
                _logger.LogError("Failed to create item: {Message}", result.Message);
                return BadRequest(result.Message ?? "Creation failed");
            }

            var output = _mapper.Map<VMOutput>(result.Data);
            return Ok(output);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public virtual async Task<ActionResult<VMOutput>> UpdateAsync([FromBody] VMUpdate model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model validation failed on update.");
                return BadRequest("Invalid model");
            }

            var request = _mapper.Map<TRequest>(model);
            var result = await _bPR.UpdateDataResultAsync(request);

            if (!result.Success || result.Data == null)
            {
                _logger.LogWarning("Failed to update item: {Message}", result.Message);
                return NotFound(new ProblemDetails { Title = "Update Failed", Detail = result.Message ?? "Item not found" });
            }

            var output = _mapper.Map<VMOutput>(result.Data);
            return Ok(output);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public virtual async Task<ActionResult<bool>> DeleteAsync(string id)
        {
            var result = await _bPR.DeleteDataResultAsync(id);

            if (!result.Success || !result.Data)
            {
                _logger.LogWarning("Failed to delete item with ID {Id}: {Message}", id, result.Message);
                return NotFound(new ProblemDetails { Title = "Delete Failed", Detail = result.Message ?? $"Item with ID {id} not found" });
            }

            _logger.LogInformation("Item with ID {Id} deleted successfully.", id);
            return Ok(true);
        }

        [HttpGet("count")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public virtual async Task<ActionResult<int>> CountAsync()
        {
            var result = await _bPR.CountDataResultAsync();

            if (!result.Success)
            {
                _logger.LogWarning("Failed to count items: {Message}", result.Message);
                return BadRequest(result.Message ?? "Count operation failed");
            }

            return Ok(result.Data);
        }
    }



    public abstract class BaseBPRController<TRequest, TResponse, VMCreate, VMOutput, VMUpdate, VMInfo, VMDelete, VMFelter> : BaseBPRController<TRequest, TResponse, VMCreate, VMOutput, VMUpdate, VMInfo, VMDelete>
   where TRequest : class
   where TResponse : class
   where VMCreate : class
   where VMOutput : class
   where VMUpdate : class
   where VMInfo : class
   where VMDelete : class
   where VMFelter : class




    {
        protected BaseBPRController(IMapper mapper, ILoggerFactory logger, IBPR<TRequest, TResponse> bPR) : base(mapper, logger, bPR)
        {
        }

        /// <summary>
        /// Get single item by ID and language
        /// </summary>
        [HttpPost("GetByLanguage")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public virtual async Task<ActionResult<VMOutput>> GetByLanguage([FromBody] string? id, [FromQuery] string? lg)
        {
            try
            {


                if (string.IsNullOrWhiteSpace(id))
                {
                    _logger.LogWarning("Invalid ID received.");
                    return BadRequest("Invalid ID.");
                }

                _logger.LogInformation("Fetching item with ID: {id}", id);
                var result = await _bPR.GetByIdDataResultAsync(id);

                if (!result.Success || result.Data == null)
                {
                    _logger.LogWarning("Item not found with ID: {id}", id);
                    return NotFound(new ProblemDetails
                    {
                        Title = "Item Not Found",
                        Detail = result.Message ?? $"No item found with ID: {id}"
                    });
                }

                var item = _mapper.Map<VMOutput>(result.Data, opt => opt.Items.Add(HelperTranslation.KEYLG, lg));
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching item by ID.");
                return StatusCode(500, "Internal Server Error");
            }
        }

        /// <summary>
        /// Get all items by language
        /// </summary>
        [HttpGet("GetAllByLanguage")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public virtual async Task<ActionResult<IEnumerable<VMOutput>>> GetAllByLanguage([FromQuery] string? lg)
        {
            if (string.IsNullOrWhiteSpace(lg))
            {
                _logger.LogWarning("Language is null or empty.");
                return BadRequest("Language parameter is required.");
            }

            try
            {
                _logger.LogInformation("Fetching all items with language: {lg}", lg);
                var result = await _bPR.GetAllDataResultAsync();

                if (!result.Success || result.Data == null || !result.Data.Any())
                {
                    _logger.LogWarning("No items found.");
                    return NotFound(new ProblemDetails
                    {
                        Title = "No Items Found",
                        Detail = result.Message ?? "No data found."
                    });
                }

                var items = _mapper.Map<IEnumerable<VMOutput>>(result.Data, opt => opt.Items.Add(HelperTranslation.KEYLG, lg));
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching items.");
                return StatusCode(500, "Internal Server Error");
            }
        }

    }


}
