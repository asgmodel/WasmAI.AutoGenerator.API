
using AutoGenerator;
using AutoGenerator.Controllers.Base;
using AutoGenerator.Repositories.Base;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace WasmAI.PaymentProvider.BPR.Layers.Base
{
    public interface IBaseBPRServiceLayer<TRequest, TResponse> : IBaseBPRShareLayer<TRequest, TResponse>
        where TRequest : class
        where TResponse : class
    {
    }

    public abstract class BaseBPRServiceLayer<TRequest, TResponse, ERequest, EResponse> 
        : BaseBPRShareLayer<TRequest, TResponse, ERequest, EResponse>, IBaseBPRServiceLayer<TRequest, TResponse>
        where TRequest : class
        where TResponse : class
        where ERequest : class
        where EResponse : class
    {
        protected new readonly IBPRLayer<ERequest, EResponse> _bpr;
        protected BaseBPRServiceLayer(IMapper mapper, ILoggerFactory logger, IBPRLayer<ERequest, EResponse> bpr)
            : base(mapper, logger, bpr)
        {
            _bpr = bpr;
        }
    }
}