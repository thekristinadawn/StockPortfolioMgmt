using StockPortfolioMgmt.Context;
using StockPortfolioMgmt.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddScoped<IPortfolioRepository, PortfolioRepository>();
builder.Services.AddScoped<ITransactionsRepository, TransactionsRepository>();
builder.Services.AddScoped<IBrokerRepository, BrokerRepository>();
builder.Services.Configure<DapperContext>(builder.Configuration.GetSection("SqlConnection"));
//Load configuration to appsettings
var configuration = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

//var connectionString = configuration.GetConnectionString("SqlConnection");
//builder.Services.AddScoped<DapperContext>(_ => new DapperContext(configuration));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();


app.MapFallbackToPage("/Portfolio");

app.Run();
