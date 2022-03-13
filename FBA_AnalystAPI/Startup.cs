using FBA_AnalystAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FBA_AnalystAPI
{
    public class Startup
    {
        public Startup(IConfigurationRoot configuration)
        {
            Configuration = configuration;
        }
        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            string con = "Host=localhost;Port=5432;Database=FBA_analyst_db;Username=postgres;Password=password";


            services.AddDbContext<TransactionContext>(options => options.UseNpgsql(con));

            services.AddControllers();

            services.AddCors();

            services.AddSwaggerGen();
        }

        public void Configure(IApplicationBuilder app)
        {

            app.UseDeveloperExceptionPage();

            app.UseRouting();

            app.UseSwagger();

            app.UseSwaggerUI();

            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
