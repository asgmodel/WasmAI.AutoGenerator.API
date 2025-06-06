using AutoMapper;
using WasmAI.PaymentProvider.Data;
using WasmAI.PaymentProvider.Models;
using WasmAI.PaymentProvider.Repositories.Base;
using AutoGenerator.Repositories.Builder;
using WasmAI.PaymentProvider.DyModels.Dto.Build.Requests;
using WasmAI.PaymentProvider.DyModels.Dto.Build.Responses;
using AutoGenerator;
using AutoGenerator.Repositories.Base;
using System;
using WasmAI.PaymentProvider.BPR.Layers.Base;

namespace WasmAI.PaymentProvider.Repositories.Builder
{
    /// <summary>
    /// Invoice class property for BuilderRepository.
    /// </summary>
     //
    public class InvoiceBuilderRepository : BaseBuilderRepository<Invoice, InvoiceRequestBuildDto, InvoiceResponseBuildDto>, IInvoiceBuilderRepository<InvoiceRequestBuildDto, InvoiceResponseBuildDto>, ITBuilder
    {
        /// <summary>
        /// Constructor for InvoiceBuilderRepository.
        /// </summary>
        public InvoiceBuilderRepository(PaymentDbContext dbContext, IMapper mapper, ILoggerFactory logger) : base(dbContext, mapper, logger) // Initialize  constructor.
        {
        // Initialize necessary fields or call base constructor.
        ///
        /// 
         
        /// 
        }
    //
    // Add additional methods or properties as needed.
    }
}