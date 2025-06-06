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
    public class ProviderPaymentController : BaseBPRControllerLayer<ProviderPaymentRequestDso, ProviderPaymentResponseDso, ProviderPaymentCreateVM, ProviderPaymentOutputVM, ProviderPaymentUpdateVM, ProviderPaymentInfoVM, ProviderPaymentDeleteVM, ProviderPaymentFilterVM>
    {
        private readonly IUseProviderPaymentService _service;
        public ProviderPaymentController(IMapper mapper, ILoggerFactory logger, IUseProviderPaymentService bPR) : base(mapper, logger, bPR)
        {
            _service = bPR;
        }
    }
}