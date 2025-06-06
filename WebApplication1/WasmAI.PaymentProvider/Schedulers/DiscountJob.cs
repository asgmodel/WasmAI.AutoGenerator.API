using AutoGenerator.Schedulers;
using System;
using WasmAI.PaymentProvider.BPR.Layers.Base;

namespace WasmAI.PaymentProvider.Schedulers
{
    public class DiscountJob : BaseJob
    {
        public override Task Execute(JobEventArgs context)
        {
            Console.WriteLine($"Executing job: {_options.JobName} with cron: {_options.Cron}");
            return Task.CompletedTask;
        }

        protected override void InitializeJobOptions()
        {
            // _options.
            _options.JobName = "Discount1";
        }
    }
}