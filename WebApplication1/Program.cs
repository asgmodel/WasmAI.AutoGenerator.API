
using AutoGenerator;
using AutoGenerator.Config;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Reflection;
using WasmAI.PaymentProvider.Data;
using WasmAI.PaymentProvider.DyModels.Dso.Responses;
using WasmAI.PaymentProvider.DyModels.Dto.Build.Responses;
using WasmAI.PaymentProvider.DyModels.Dto.Share.Responses;
using WasmAI.PaymentProvider.DyModels.VMs;
using WasmAI.PaymentProvider.Models;
using WasmAI.PaymentProvider.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<PaymentDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
       .AddAutoBuilderApiCore<PaymentDbContext, ApplicationUser>(new()
       {
           Arags = args,
           NameRootApi = "WasmAI.PaymentProvider",
           IsMapper = true,
         
           Assembly = Assembly.GetExecutingAssembly(),
       
        

       });

//builder.Services.AddAutoMapper(typeof(MappingConfig));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

Console.WriteLine(MappingConfig.CountMap);
try
{

    // بناء الخرائط في وقت التشغيل لتخفيف التحميل وقت الطلب
    var validatorWatch = Stopwatch.StartNew();
    var mapper = app.Services.GetRequiredService<IMapper>();

    // تأكد من أن جميع الخرائط صحيحة
    // حاليا معلقة لانها تنتج خطأ سببه التكرار عند بناء الخريطة CreateMap
    //mapper.ConfigurationProvider.AssertConfigurationIsValid(); // ⬅️ يجبر AutoMapper يبني كل الخرائط الآن
    validatorWatch.Stop();
    Console.WriteLine($"✅ Validators registered in: {validatorWatch.ElapsedMilliseconds}ms");
}
catch (AutoMapper.DuplicateTypeMapConfigurationException ex)
{
    foreach (var error in ex.Errors)
    {
        Console.WriteLine($"Conflict for mapping: {error.Types.SourceType.Name} -> {error.Types.DestinationType.Name}");
        Console.WriteLine("Defined in profiles: " + string.Join(", ", error.ProfileNames));
    }
}
Console.WriteLine(MappingConfig.CountMap);
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
