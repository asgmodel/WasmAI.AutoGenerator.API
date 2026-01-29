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
    public class DiscountController : BaseBPRControllerLayer<DiscountRequestBuildDto, DiscountResponseBuildDto, DiscountCreateVM, DiscountOutputVM, DiscountUpdateVM, DiscountInfoVM, DiscountDeleteVM, DiscountFilterVM>
    {
        private readonly DiscountBuilderRepository _service;
        public DiscountController(IMapper mapper, ILoggerFactory logger, DiscountBuilderRepository bPR) : base(mapper, logger, bPR)
        {
            _service = bPR;
        }
    }
}