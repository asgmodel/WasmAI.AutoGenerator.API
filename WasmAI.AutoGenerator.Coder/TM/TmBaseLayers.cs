using System;
using System.IO;
using AutoGenerator.ApiFolder;
using AutoGenerator.Code.Config;

namespace AutoGenerator.TM;
public  class TmBaseLayers
{
    public static void GenerateGroupedBaseLayerFiles(string baseDirectory, string rootNamespace)
    {
        var files = new (string FileName, string Content)[]
        {
            ("BaseBPR.cs", GetBaseBPRFile(rootNamespace)),
            ("BPRShare.cs", GetShareBPRFile(rootNamespace)),
            ("BPRService.cs", GetServiceBPRFile(rootNamespace)),
            ("BPRController.cs", GetControllerBPRFile(rootNamespace)),
        };

        foreach (var (fileName, content) in files)
        {
            string fullPath = Path.Combine(baseDirectory, fileName);
            if (!File.Exists(fullPath))
            {
                File.WriteAllText(fullPath, content);
                Console.WriteLine($"Created: {fullPath}");
            }
        }
    }

    private static string GetBaseBPRFile(string root)
    {
        return $@"
using AutoGenerator;
using AutoGenerator.Controllers.Base;
using AutoGenerator.Repositories.Base;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace {root}.BPR.Layers.Base
{{
    public interface IBPRLayer<TRequest, TResponse> : IBPR<TRequest, TResponse>
        where TRequest : class
        where TResponse : class
    {{
    }}

    public abstract class BaseBPRLayer<TRequest, TResponse, ERequest, EResponse, IT, IE> 
        : TBPR<TRequest, TResponse, ERequest, EResponse, IT, IE>,IBPRLayer<TRequest, TResponse>
        where TRequest : class
        where TResponse : class
        where ERequest : class
        where EResponse : class
        where IT : ITBase
        where IE : ITBase
    {{
        protected new readonly IBPRLayer<ERequest, EResponse> _bpr;
        protected BaseBPRLayer(IMapper mapper, ILoggerFactory logger, IBPRLayer<ERequest, EResponse> bpr) : base(mapper, logger, bpr)
        {{
            _bpr = bpr;
        }}
    }}
}}";
    }

    private static string GetShareBPRFile(string root)
    {
        return $@"
using AutoGenerator;
using AutoGenerator.Controllers.Base;
using AutoGenerator.Repositories.Base;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace {root}.BPR.Layers.Base
{{
    public interface IBaseBPRShareLayer<TRequest, TResponse> : IBPRLayer<TRequest, TResponse>
        where TRequest : class
        where TResponse : class
    {{
    }}

    public abstract class BaseBPRShareLayer<TRequest, TResponse, ERequest, EResponse> 
        : BaseBPRLayer<TRequest, TResponse, ERequest, EResponse, ITBase, ITBase>, IBaseBPRShareLayer<TRequest, TResponse>
        where TRequest : class
        where TResponse : class
        where ERequest : class
        where EResponse : class
    {{
        protected new readonly IBPRLayer<ERequest, EResponse> _bpr;
        protected BaseBPRShareLayer(IMapper mapper, ILoggerFactory logger, IBPRLayer<ERequest, EResponse> bpr) : base(mapper, logger, bpr)
        {{
            _bpr = bpr;
        }}
    }}
}}";
    }

    private static string GetServiceBPRFile(string root)
    {
        return $@"
using AutoGenerator;
using AutoGenerator.Controllers.Base;
using AutoGenerator.Repositories.Base;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace {root}.BPR.Layers.Base
{{
    public interface IBaseBPRServiceLayer<TRequest, TResponse> : IBaseBPRShareLayer<TRequest, TResponse>
        where TRequest : class
        where TResponse : class
    {{
    }}

    public abstract class BaseBPRServiceLayer<TRequest, TResponse, ERequest, EResponse> 
        : BaseBPRShareLayer<TRequest, TResponse, ERequest, EResponse>, IBaseBPRServiceLayer<TRequest, TResponse>
        where TRequest : class
        where TResponse : class
        where ERequest : class
        where EResponse : class
    {{
        protected new readonly IBaseBPRShareLayer<ERequest, EResponse> _bpr;
        protected BaseBPRServiceLayer(IMapper mapper, ILoggerFactory logger, IBaseBPRShareLayer<ERequest, EResponse> bpr)
            : base(mapper, logger, bpr)
        {{
            _bpr = bpr;
        }}
    }}
}}";
    }

     static string getnameLayer()
    {
        if(BPRLAYERGENERATOR.GenType==BPRLayerGenType.Dso)
               return "IBaseBPRServiceLayer";
        if(BPRLAYERGENERATOR.GenType==BPRLayerGenType.ShareDto)
                return "IBaseBPRShareLayer";
       
        
        
        
        return "IBPRLayer";


    }
    private static string GetControllerBPRFile(string root)
    {
        var nameLayer = getnameLayer();

        return $@"
using AutoGenerator;
using AutoGenerator.Controllers.Base;
using AutoGenerator.Repositories.Base;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace {root}.BPR.Layers.Base
{{
    public abstract class BaseBPRControllerLayer<TRequest, TResponse, VMCreate, VMOutput, VMUpdate, VMInfo, VMDelete> 
        : BaseBPRController<TRequest, TResponse, VMCreate, VMOutput, VMUpdate, VMInfo, VMDelete>
        where TRequest : class
        where TResponse : class
        where VMCreate : class
        where VMOutput : class
        where VMUpdate : class
        where VMInfo : class
        where VMDelete : class
    {{
        protected {nameLayer}<TRequest, TResponse> _bpr;
        protected BaseBPRControllerLayer(IMapper mapper, ILoggerFactory logger, {nameLayer}<TRequest, TResponse> bPR) 
            : base(mapper, logger, bPR)
        {{
            _bpr = bPR;
        }}

       [HttpGet( ""GetAll"")]
       public override  Task<ActionResult<IEnumerable<VMOutput>>> GetAllAsync()
        {{
            return base.GetAllAsync();
        }}

       [HttpDelete(""Delete/{{id}}"")]
       public override Task<ActionResult<bool>> DeleteAsync(string id){{
          return base.DeleteAsync(id);
        }}
       [HttpPut(""Update/{{id}}"")]
       public override  Task<ActionResult<VMOutput>> UpdateAsync([FromRoute] string id, [FromBody] VMUpdate model)
        {{
            return base.UpdateAsync(id, model);
        }} 

       [HttpPost(""Create"")]
       public override Task<ActionResult<VMOutput>> CreateAsync([FromBody] VMCreate model)
        {{
            return base.CreateAsync(model);
        }}

        [HttpGet(""Get/{{id}}"")]
        public override Task<ActionResult<VMInfo>> GetByIdAsync(string id)
        {{
            return base.GetByIdAsync(id);
        }}
    }}


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
    {{
        protected {nameLayer}<TRequest, TResponse> _bpr;
        protected BaseBPRControllerLayer(IMapper mapper, ILoggerFactory logger, {nameLayer}<TRequest, TResponse> bPR) 
            : base(mapper, logger, bPR)
        {{
            _bpr = bPR;
        }}

              [HttpGet( ""GetAll"")]
       public override  Task<ActionResult<IEnumerable<VMOutput>>> GetAllAsync()
        {{
            return base.GetAllAsync();
        }}

       [HttpDelete(""Delete/{{id}}"")]
       public override Task<ActionResult<bool>> DeleteAsync(string id){{
          return base.DeleteAsync(id);
        }}
       [HttpPut(""Update/{{id}}"")]
       public override  Task<ActionResult<VMOutput>> UpdateAsync([FromRoute] string id, [FromBody] VMUpdate model)
        {{
            return base.UpdateAsync(id, model);
        }} 

       [HttpPost(""Create"")]
       public override Task<ActionResult<VMOutput>> CreateAsync([FromBody] VMCreate model)
        {{
            return base.CreateAsync(model);
        }}

        [HttpGet(""Get/{{id}}"")]
        public override Task<ActionResult<VMInfo>> GetByIdAsync(string id)
        {{
            return base.GetByIdAsync(id);
        }}
    }}
}}";
    }
}
