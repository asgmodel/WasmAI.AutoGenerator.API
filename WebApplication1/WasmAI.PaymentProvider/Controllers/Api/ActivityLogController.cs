using AutoGenerator.Helper.Translation;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using WasmAI.PaymentProvider.BPR.Layers.Base;
using WasmAI.PaymentProvider.DyModels.Dso.Requests;
using WasmAI.PaymentProvider.DyModels.Dso.Responses;
using WasmAI.PaymentProvider.DyModels.VMs;
using WasmAI.PaymentProvider.Models;
using WasmAI.PaymentProvider.Repositories.Base;
using WasmAI.PaymentProvider.Services.Services;
using WebApplication1;
using WebApplication1.Controllers;

namespace WasmAI.PaymentProvider.Controllers.Api
{
    //[ApiExplorerSettings(GroupName = "WasmAI.PaymentProvider")]
    [Route("api/WasmAI.PaymentProvider/Api/[controller]")]
    [ApiController]
    public class ActivityLogController : BaseBPRControllerLayer<ActivityLog, ActivityLog, ActivityLogCreateVM, ActivityLogOutputVM, ActivityLogUpdateVM, ActivityLogInfoVM, ActivityLogDeleteVM, ActivityLogFilterVM>
    {
        private readonly IUseActivityLogService _service;
        private readonly BaseBPRRepository<ActivityLog> repository;
        public ActivityLogController(IMapper mapper, ILoggerFactory logger, BaseBPRRepository<ActivityLog> bPR) : base(mapper, logger, bPR)
        {
           
        }

      

        private  Task read()
        {
            // Implement the logic to read activity logs here
            // This is a placeholder for the actual implementation
            throw new NotImplementedException("Read method not implemented yet.");
        }
        public override async Task<ActionResult<IEnumerable<ActivityLogOutputVM>>> GetAllAsync()
        {
            try
            {
             await   read();
            }
            catch (Exception ex)
            {
              
                return StatusCode(500, "Internal server error");
            }
            return Ok() ;
        }
    }
}