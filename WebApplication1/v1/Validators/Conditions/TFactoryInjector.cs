using AutoGenerator;
using AutoGenerator.Conditions;
using AutoGenerator.Notifications;
using AutoMapper;
using WasmAI.PaymentProvider.Data;
using System;
using v1.BPR.Layers.Base;

namespace v1.Validators.Conditions
{
    public class TFactoryInjector : TBaseFactoryInjector, ITFactoryInjector
    {
        private readonly PaymentDbContext _context;
        public TFactoryInjector(IMapper mapper, IAutoNotifier notifier, PaymentDbContext context) : base(mapper, notifier)
        {
            _context = context;
        }

        public PaymentDbContext Context => _context;
    // يمكنك حقن اي طبقة
    }
}