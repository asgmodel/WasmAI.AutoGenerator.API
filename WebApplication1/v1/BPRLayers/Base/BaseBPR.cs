
using AutoGenerator;
using AutoGenerator.Controllers.Base;
using AutoGenerator.Repositories.Base;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace v1.BPR.Layers.Base
{
    public interface IBPRLayer<TRequest, TResponse> : IBPR<TRequest, TResponse>
        where TRequest : class
        where TResponse : class
    {
    }

    public abstract class BaseBPRLayer<TRequest, TResponse, ERequest, EResponse, IT, IE> 
        : TBPR<TRequest, TResponse, ERequest, EResponse, IT, IE>,IBPRLayer<TRequest, TResponse>
        where TRequest : class
        where TResponse : class
        where ERequest : class
        where EResponse : class
        where IT : ITBase
        where IE : ITBase
    {
        protected new readonly IBPRLayer<ERequest, EResponse> _bpr;
        protected BaseBPRLayer(IMapper mapper, ILoggerFactory logger, IBPRLayer<ERequest, EResponse> bpr) : base(mapper, logger, bpr)
        {
            _bpr = bpr;
        }
    }
}