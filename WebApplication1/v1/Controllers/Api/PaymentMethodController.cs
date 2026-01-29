using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using v1.Repositories.Builder;
using Microsoft.AspNetCore.Mvc;
using v1.DyModels.VMs;
using System.Linq.Expressions;
using v1.DyModels.Dto.Build.Requests;
using v1.DyModels.Dto.Build.Responses;
using AutoGenerator.Helper.Translation;
using System;
using v1.BPR.Layers.Base;

namespace v1.Controllers.Api
{
    //[ApiExplorerSettings(GroupName = "v1")]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PaymentMethodController : BaseBPRControllerLayer<PaymentMethodRequestBuildDto, PaymentMethodResponseBuildDto, PaymentMethodCreateVM, PaymentMethodOutputVM, PaymentMethodUpdateVM, PaymentMethodInfoVM, PaymentMethodDeleteVM, PaymentMethodFilterVM>
    {
        private readonly PaymentMethodBuilderRepository _service;
        public PaymentMethodController(IMapper mapper, ILoggerFactory logger, PaymentMethodBuilderRepository bPR) : base(mapper, logger, bPR)
        {
            _service = bPR;
        }
    }
}