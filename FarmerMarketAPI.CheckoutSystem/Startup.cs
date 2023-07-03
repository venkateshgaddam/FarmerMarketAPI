using FarmerMarketAPI.CheckoutSystem.Services;
using FarmerMarketAPI.CheckoutSystem.Services.Interface;
using FarmerMarketAPI.Common.Interfaces;
using FarmerMarketAPI.Common.Services;
using FarmerMarketAPI.Data.DbContext;
using FarmerMarketAPI.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace FarmerMarketAPI.CheckoutSystem;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<BasketDbContext>(options => options.UseInMemoryDatabase(databaseName: "BasketDB"));
        services.AddControllers();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IDiscountRuleEngine, DiscountRuleEngine>();
        services.AddScoped<ICheckoutService, CheckoutService>();

        services.AddScoped(a =>
        {
            var rules = new List<IDiscountRule>()
            {
                new BuyOneGetOneFreeRule(),
                new ApplesDiscountRule(),
                new ChaiMilkDiscountRule(),
                new OatmealApplesDiscountRule()
            };
            return rules;
        });
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        services.AddControllers();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "FarmerMarketApi", Version = "v1" });
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();

        app.UseRouting();
        app.UseCors();
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            //c.SwaggerEndpoint("/swagger/v1/swagger.json", "FarmerMarketApi v1");
            //c.RoutePrefix = "";

            c.SwaggerEndpoint("/Prod/swagger/v1/swagger.json", "FarmerMarketApi v1");
            c.RoutePrefix = "swagger/ui"; // Set Swagger UI at the root URL
        });

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("Welcome to running ASP.NET Core on AWS Lambda");
            });
        });
    }
}