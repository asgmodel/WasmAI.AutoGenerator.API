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
    /// ActivityLog class property for BuilderRepository.
    /// </summary>
     //
    public class ActivityLogBuilderRepository : BaseBuilderRepository<ActivityLog, ActivityLogRequestBuildDto, ActivityLogResponseBuildDto>, IActivityLogBuilderRepository<ActivityLogRequestBuildDto, ActivityLogResponseBuildDto>, ITBuilder
    {
        /// <summary>
        /// Constructor for ActivityLogBuilderRepository.
        /// </summary>
        public ActivityLogBuilderRepository(PaymentDbContext dbContext, IMapper mapper, ILoggerFactory logger) : base(dbContext, mapper, logger) // Initialize  constructor.
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