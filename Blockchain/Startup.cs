using System;
using Blockchain.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Blockchain
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.Configure<BlockchainOptions>(_config.GetSection("Blockchain"));
            services.AddSingleton<IBlockchainService, Services.Blockchain>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc(ConfigureRoutes);
        }

        private void ConfigureRoutes(IRouteBuilder obj)
        {
            obj.MapRoute("mine", "mine", new { controller = "Home", action = "Mine" });
            obj.MapRoute("chain", "chain", new { controller = "Home", action = "Chain" });
            obj.MapRoute("transactionnew", "transaction/new", new { controller = "Home", action = "New" });
            obj.MapRoute("register", "nodes/register", new { controller = "Home", action = "Register" });
            obj.MapRoute("resolve", "nodes/resolve", new { controller = "Home", action = "ConsensusAsync" });
        }
    }
}
