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
    /// ProviderPayment class for ShareRepository.
    /// </summary>
    public class ProviderPaymentShareRepository : BaseBPRShareLayer<ProviderPaymentRequestShareDto, ProviderPaymentResponseShareDto, ProviderPaymentRequestBuildDto, ProviderPaymentResponseBuildDto>, IProviderPaymentShareRepository
    {
        // Declare the builder repository.
        private readonly ProviderPaymentBuilderRepository _builder;
        public ProviderPaymentShareRepository(IMapper mapper, ILoggerFactory logger, ProviderPaymentBuilderRepository bpr) : base(mapper, logger, bpr)
        {
            _builder = bpr;
        }
    }
}