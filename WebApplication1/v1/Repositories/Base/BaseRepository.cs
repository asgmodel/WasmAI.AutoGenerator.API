using AutoGenerator.Repositories.Base;
using WasmAI.PaymentProvider.Data;
using WasmAI.PaymentProvider.Models;
using Microsoft.AspNetCore.Identity;
using System;
using v1.BPR.Layers.Base;

namespace v1.Repositories.Base
{
    public sealed class BaseBPRRepository<T> : TBaseBPRRepository<ApplicationUser, IdentityRole, string, T>, IBPRLayer<T, T>, IBaseRepository<T> where T : class
    {
        public BaseBPRRepository(PaymentDbContext db, ILoggerFactory logger) : base(db, logger)
        {
        }
    }
}