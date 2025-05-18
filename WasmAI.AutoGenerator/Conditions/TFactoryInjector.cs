
using AutoGenerator.Notifications;
using AutoMapper;


namespace AutoGenerator.Conditions
{

    public interface ITBaseFactoryInjector
    {

  

        public IMapper Mapper { get; }

        public IAutoNotifier Notifier { get; }








    }


    public class TBaseFactoryInjector : ITBaseFactoryInjector
    {

        private  readonly IMapper _mapper;
        private readonly IAutoNotifier _notifier;
        public TBaseFactoryInjector( IMapper mapper, IAutoNotifier notifier)
        {
            _mapper = mapper;
            _notifier = notifier;
        }

        public IMapper Mapper =>_mapper;

        public IAutoNotifier Notifier => _notifier;
   
    
    }




}
