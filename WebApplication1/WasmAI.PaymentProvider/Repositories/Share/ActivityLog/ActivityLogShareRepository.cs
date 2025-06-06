using AutoMapper;
using WasmAI.PaymentProvider.Data;
using WasmAI.PaymentProvider.Models;
using WasmAI.PaymentProvider.Repositories.Base;
using AutoGenerator.Repositories.Builder;
using WasmAI.PaymentProvider.DyModels.Dto.Build.Requests;
using WasmAI.PaymentProvider.DyModels.Dto.Build.Responses;
using AutoGenerator;
using AutoGenerator.Repositories.Base;
using AutoGenerator;
using WasmAI.PaymentProvider.Repositories.Builder;
using AutoGenerator.Repositories.Share;
using System.Linq.Expressions;
using AutoGenerator.Repositories.Base;
using AutoGenerator.Helper;
using WasmAI.PaymentProvider.DyModels.Dto.Share.Requests;
using WasmAI.PaymentProvider.DyModels.Dto.Share.Responses;
using System;
using WasmAI.PaymentProvider.BPR.Layers.Base;

namespace WasmAI.PaymentProvider.Repositories.Share
{
    /// <summary>
    /// ActivityLog class for ShareRepository.
    /// </summary>
    public class ActivityLogShareRepository : BaseBPRShareLayer<ActivityLogRequestShareDto, ActivityLogResponseShareDto, ActivityLogRequestBuildDto, ActivityLogResponseBuildDto>, IActivityLogShareRepository
    {
        // Declare the builder repository.
        private readonly ActivityLogBuilderRepository _builder;
        public ActivityLogShareRepository(IMapper mapper, ILoggerFactory logger, ActivityLogBuilderRepository bpr) : base(mapper, logger, bpr)
        {
            _builder = bpr;
        }
    }
}