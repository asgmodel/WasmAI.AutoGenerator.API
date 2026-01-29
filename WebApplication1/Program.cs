
using System.Diagnostics;
using System.Reflection;
using AutoGenerator;
using AutoGenerator.Config;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WasmAI.AutoGenerator.Auth;
using WasmAI.PaymentProvider.Data;
using WasmAI.PaymentProvider.Models;


var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


builder.Services
       .AddWasmAutoGeneratorApi<PaymentDbContext, ApplicationUser>(new()
       {
           Arags = args,
           NameRootApi = "v1",
           Assembly = Assembly.GetExecutingAssembly(),
           DbConnectionString = builder.Configuration.GetConnectionString("DefaultConnection"),
       });


var app = builder.Build();


app.UseWasmAutoGeneratorApi();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
