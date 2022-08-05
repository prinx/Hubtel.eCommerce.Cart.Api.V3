
using System.Text.Json.Serialization;
using Hubtel.eCommerce.Cart.Api.Extensions;
using Hubtel.eCommerce.Cart.Api.Models;
using Hubtel.eCommerce.Cart.Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Hubtel.eCommerce.Cart.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ICartItemsService, CartItemsService>();
            services.AddScoped<IProductsService, ProductsService>();
            services.AddScoped<IUsersService, UsersService>();

            //services.AddSingleton<IPagination<T>, typeof(PaginationService<>)>();
            services.AddSingleton<IExceptionHandlerService, ExceptionHandlerService>();

            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                    //options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                });;
            
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            //services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<CartContext>(options => options.UseNpgsql(connectionString));
            //services.AddDatabaseDeveloperPageExceptionFilter();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseDeveloperExceptionPage();
            }

            // Handle exception with default handler middleware
            //var exceptionService = app.Services.GetRequiredService<IExceptionHandlerService>();
            //app.ConfigureExceptionHandler(exceptionService);

            // Handle global exception with custom middleware
            app.ConfigureCustomExceptionHandler();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}