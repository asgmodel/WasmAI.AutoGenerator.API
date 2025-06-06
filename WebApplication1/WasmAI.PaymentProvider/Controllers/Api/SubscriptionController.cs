using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using WasmAI.PaymentProvider.Services.Services;
using Microsoft.AspNetCore.Mvc;
using WasmAI.PaymentProvider.DyModels.VMs;
using System.Linq.Expressions;
using WasmAI.PaymentProvider.DyModels.Dso.Requests;
using WasmAI.PaymentProvider.DyModels.Dso.Responses;
using AutoGenerator.Helper.Translation;
using System;
using WasmAI.PaymentProvider.BPR.Layers.Base;

namespace WasmAI.PaymentProvider.Controllers.Api
{
    //[ApiExplorerSettings(GroupName = "WasmAI.PaymentProvider")]
    [Route("api/WasmAI.PaymentProvider/Api/[controller]")]
    [ApiController]
    public class SubscriptionController : BaseBPRControllerLayer<SubscriptionRequestDso, SubscriptionResponseDso, SubscriptionCreateVM, SubscriptionOutputVM, SubscriptionUpdateVM, SubscriptionInfoVM, SubscriptionDeleteVM, SubscriptionFilterVM>
    {
        private readonly IUseSubscriptionService _service;
        public SubscriptionController(IMapper mapper, ILoggerFactory logger, IUseSubscriptionService bPR) : base(mapper, logger, bPR)
        {
            _service = bPR;
        }
    }
}