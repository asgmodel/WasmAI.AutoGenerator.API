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
    public class WebhookEventController : BaseBPRControllerLayer<WebhookEventRequestBuildDto, WebhookEventResponseBuildDto, WebhookEventCreateVM, WebhookEventOutputVM, WebhookEventUpdateVM, WebhookEventInfoVM, WebhookEventDeleteVM, WebhookEventFilterVM>
    {
        private readonly WebhookEventBuilderRepository _service;
        public WebhookEventController(IMapper mapper, ILoggerFactory logger, WebhookEventBuilderRepository bPR) : base(mapper, logger, bPR)
        {
            _service = bPR;
        }
    }
}