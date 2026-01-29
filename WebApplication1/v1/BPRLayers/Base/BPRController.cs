
using AutoGenerator;
using AutoGenerator.Controllers.Base;
using AutoGenerator.Repositories.Base;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace v1.BPR.Layers.Base
{
    public abstract class BaseBPRControllerLayer<TRequest, TResponse, VMCreate, VMOutput, VMUpdate, VMInfo, VMDelete> 
        : BaseBPRController<TRequest, TResponse, VMCreate, VMOutput, VMUpdate, VMInfo, VMDelete>
        where TRequest : class
        where TResponse : class
        where VMCreate : class
        where VMOutput : class
        where VMUpdate : class
        where VMInfo : class
        where VMDelete : class
    {
        protected IBPRLayer<TRequest, TResponse> _bpr;
        protected BaseBPRControllerLayer(IMapper mapper, ILoggerFactory logger, IBPRLayer<TRequest, TResponse> bPR) 
            : base(mapper, logger, bPR)
        {
            _bpr = bPR;
        }

       [HttpGet( "GetAll")]
       public override  Task<ActionResult<IEnumerable<VMOutput>>> GetAllAsync()
        {
            return base.GetAllAsync();
        }

       [HttpDelete("Delete/{id}")]
       public override Task<ActionResult<bool>> DeleteAsync(string id){
          return base.DeleteAsync(id);
        }
       [HttpPut("Update/{id}")]
       public override  Task<ActionResult<VMOutput>> UpdateAsync([FromRoute] string id, [FromBody] VMUpdate model)
        {
            return base.UpdateAsync(id, model);
        } 

       [HttpPost("Create")]
       public override Task<ActionResult<VMOutput>> CreateAsync([FromBody] VMCreate model)
        {
            return base.CreateAsync(model);
        }

        [HttpGet("Get/{id}")]
        public override Task<ActionResult<VMInfo>> GetByIdAsync(string id)
        {
            return base.GetByIdAsync(id);
        }
    }


     /////////// ControllerLayerWithFilter LG/////////////
    public abstract class BaseBPRControllerLayer<TRequest, TResponse, VMCreate, VMOutput, VMUpdate, VMInfo, VMDelete, VMFilter> 
        : BaseBPRController<TRequest, TResponse, VMCreate, VMOutput, VMUpdate, VMInfo, VMDelete, VMFilter>
        where TRequest : class
        where TResponse : class
        where VMCreate : class
        where VMOutput : class
        where VMUpdate : class
        where VMInfo : class
        where VMDelete : class
        where VMFilter : class
    {
        protected IBPRLayer<TRequest, TResponse> _bpr;
        protected BaseBPRControllerLayer(IMapper mapper, ILoggerFactory logger, IBPRLayer<TRequest, TResponse> bPR) 
            : base(mapper, logger, bPR)
        {
            _bpr = bPR;
        }

              [HttpGet( "GetAll")]
       public override  Task<ActionResult<IEnumerable<VMOutput>>> GetAllAsync()
        {
            return base.GetAllAsync();
        }

       [HttpDelete("Delete/{id}")]
       public override Task<ActionResult<bool>> DeleteAsync(string id){
          return base.DeleteAsync(id);
        }
       [HttpPut("Update/{id}")]
       public override  Task<ActionResult<VMOutput>> UpdateAsync([FromRoute] string id, [FromBody] VMUpdate model)
        {
            return base.UpdateAsync(id, model);
        } 

       [HttpPost("Create")]
       public override Task<ActionResult<VMOutput>> CreateAsync([FromBody] VMCreate model)
        {
            return base.CreateAsync(model);
        }

        [HttpGet("Get/{id}")]
        public override Task<ActionResult<VMInfo>> GetByIdAsync(string id)
        {
            return base.GetByIdAsync(id);
        }
    }
}