using AutoMapper;
using WasmAI.PaymentProvider.Data;
using WasmAI.PaymentProvider.Models;
using v1.Repositories.Base;
using AutoGenerator.Repositories.Builder;
using v1.DyModels.Dto.Build.Requests;
using v1.DyModels.Dto.Build.Responses;
using AutoGenerator;
using AutoGenerator.Repositories.Base;
using System;
using v1.BPR.Layers.Base;

namespace v1.Repositories.Builder
{
    /// <summary>
    /// PricePlan class property for BuilderRepository.
    /// </summary>
     //
    public class PricePlanBuilderRepository : BaseBuilderRepository<PricePlan, PricePlanRequestBuildDto, PricePlanResponseBuildDto>, IPricePlanBuilderRepository<PricePlanRequestBuildDto, PricePlanResponseBuildDto>, ITBuilder
    {
        /// <summary>
        /// Constructor for PricePlanBuilderRepository.
        /// </summary>
        public PricePlanBuilderRepository(PaymentDbContext dbContext, IMapper mapper, ILoggerFactory logger) : base(dbContext, mapper, logger) // Initialize  constructor.
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