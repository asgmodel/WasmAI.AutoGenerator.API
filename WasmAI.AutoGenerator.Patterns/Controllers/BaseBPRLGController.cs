using AutoGenerator.Controllers.Base;
using AutoGenerator.Helper.Translation;
using AutoGenerator.Repositories.Base;
using AutoMapper;

using Microsoft.Extensions.Logging;

namespace AutoGenerator.Layers.Base
{

    public interface IBPRLayer<TRequest, TResponse>: IBPR<TRequest, TResponse>
        where TRequest : class
       where TResponse : class
    {

    }


    public abstract class BaseBPRLayer<TRequest, TResponse, ERequest, EResponse, IT, IE> : TBPR<TRequest, TResponse, ERequest, EResponse, IT, IE>
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



    public abstract class BaseBPRShareLayer<TRequest, TResponse, ERequest, EResponse> : BaseBPRLayer<TRequest, TResponse, ERequest, EResponse, ITBase,ITBase>, IBaseBPRShareLayer<TRequest, TResponse>
         where TRequest : class
         where TResponse : class
         where ERequest : class
         where EResponse : class
        


    {
            protected new readonly IBPRLayer<ERequest, EResponse> _bpr;
            protected BaseBPRShareLayer(IMapper mapper, ILoggerFactory logger, IBPRLayer<ERequest, EResponse> bpr) : base(mapper, logger, bpr)
            {
                _bpr = bpr;
            }


        }


    public interface IBaseBPRShareLayer<TRequest, TResponse> : IBPRLayer<TRequest, TResponse>
        where TRequest : class
        where TResponse : class
    {

    }


    public interface IBaseBPRServiceLayer<TRequest, TResponse> : IBaseBPRShareLayer<TRequest, TResponse>
    where TRequest : class
    where TResponse : class
    {

    }
    public abstract class BaseBPRServiceLayer<TRequest, TResponse, ERequest, EResponse> :BaseBPRShareLayer<TRequest, TResponse, ERequest, EResponse>, IBaseBPRServiceLayer<TRequest, TResponse>
     where TRequest : class
     where TResponse : class
     where ERequest : class
     where EResponse : class
 

    {
        protected new readonly IBaseBPRShareLayer<ERequest, EResponse> _bpr;
        protected BaseBPRServiceLayer(IMapper mapper, ILoggerFactory logger, IBaseBPRShareLayer<ERequest, EResponse> bpr) : base(mapper, logger, bpr)
        {
            _bpr = bpr;
        }
         

    }

    public abstract class BaseBPRControllerLayer<TRequest, TResponse, VMCreate, VMOutput, VMUpdate, VMInfo, VMDelete> : BaseBPRController<TRequest, TResponse, VMCreate, VMOutput, VMUpdate, VMInfo, VMDelete>
       where TRequest : class
       where TResponse : class
       where VMCreate : class
       where VMOutput : class
       where VMUpdate : class
       where VMInfo : class
       where VMDelete : class





    {
        protected IBaseBPRServiceLayer<TRequest, TResponse> _bpr;
        protected BaseBPRControllerLayer(IMapper mapper, ILoggerFactory logger, IBaseBPRServiceLayer<TRequest, TResponse> bPR) : base(mapper, logger, bPR)
        {
            _bpr = bPR;
        }


      }



   public abstract class BaseBPRControllerLayer<TRequest, TResponse, VMCreate, VMOutput, VMUpdate, VMInfo, VMDelete, VMFelter> : BaseBPRController<TRequest, TResponse, VMCreate, VMOutput, VMUpdate, VMInfo, VMDelete, VMFelter>
   where TRequest : class
   where TResponse : class
   where VMCreate : class
   where VMOutput : class
   where VMUpdate : class
   where VMInfo : class
   where VMDelete : class
   where VMFelter : class




    {
        protected IBaseBPRServiceLayer<TRequest, TResponse> _bpr;
        protected BaseBPRControllerLayer(IMapper mapper, ILoggerFactory logger, IBaseBPRServiceLayer<TRequest, TResponse> bPR) : base(mapper, logger, bPR)
        {
            _bpr = bPR;
        }


    }

}
