using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blockchain.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Blockchain
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSingleton<IBlockchainService>(new Blockchain.Services.Blockchain(new BlockchainOptions { NodeId = Guid.NewGuid().ToString() }));
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
