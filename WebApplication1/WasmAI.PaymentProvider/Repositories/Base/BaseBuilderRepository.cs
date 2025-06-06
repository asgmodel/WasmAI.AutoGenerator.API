using WasmAI.PaymentProvider.Data;
using WasmAI.PaymentProvider.Models;
using AutoMapper;
using AutoGenerator;
using AutoGenerator.Repositories.Builder;
using System;
using WasmAI.PaymentProvider.BPR.Layers.Base;

namespace WasmAI.PaymentProvider.Repositories.Base
{
    /// <summary>
    /// BaseRepository class for ShareRepository.
    /// </summary>
    public abstract class BaseBuilderRepository<TModel, TBuildRequestDto, TBuildResponseDto> : BaseBPRLayer<TBuildRequestDto, TBuildResponseDto, TModel, TModel, ITBase, ITModel>, IBPRLayer<TBuildRequestDto, TBuildResponseDto>, ITBuildRepository, IBaseBuilderRepository<TBuildRequestDto, TBuildResponseDto> where TModel : class where TBuildRequestDto : class where TBuildResponseDto : class
    {
        protected BaseBuilderRepository(PaymentDbContext dbContext, IMapper mapper, ILoggerFactory logger) : this(dbContext, mapper, logger, new BaseBPRRepository<TModel>(dbContext, logger))
        {
        }

        protected BaseBuilderRepository(PaymentDbContext dbContext, IMapper mapper, ILoggerFactory logger, BaseBPRRepository<TModel> bpr) : base(mapper, logger, bpr)
        {
        }
    }
}