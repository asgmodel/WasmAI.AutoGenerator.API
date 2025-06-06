using AutoGenerator;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using AutoGenerator.Services.Base;
using WasmAI.PaymentProvider.DyModels.Dso.Requests;
using WasmAI.PaymentProvider.DyModels.Dso.Responses;
using WasmAI.PaymentProvider.Models;
using WasmAI.PaymentProvider.DyModels.Dto.Share.Requests;
using WasmAI.PaymentProvider.DyModels.Dto.Share.Responses;
using WasmAI.PaymentProvider.Repositories.Share;
using System.Linq.Expressions;
using WasmAI.PaymentProvider.Repositories.Builder;
using AutoGenerator.Repositories.Base;
using AutoGenerator.Helper;
using System;
using WasmAI.PaymentProvider.BPR.Layers.Base;

namespace WasmAI.PaymentProvider.Services.Services
{
    public interface ISubscriptionService<TServiceRequestDso, TServiceResponseDso>
        where TServiceRequestDso : class where TServiceResponseDso : class
    {
    }
}