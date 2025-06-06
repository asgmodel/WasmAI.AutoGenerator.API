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
    public class WebhookEventController : BaseBPRControllerLayer<WebhookEventRequestDso, WebhookEventResponseDso, WebhookEventCreateVM, WebhookEventOutputVM, WebhookEventUpdateVM, WebhookEventInfoVM, WebhookEventDeleteVM, WebhookEventFilterVM>
    {
        private readonly IUseWebhookEventService _service;
        public WebhookEventController(IMapper mapper, ILoggerFactory logger, IUseWebhookEventService bPR) : base(mapper, logger, bPR)
        {
            _service = bPR;
        }
    }
}