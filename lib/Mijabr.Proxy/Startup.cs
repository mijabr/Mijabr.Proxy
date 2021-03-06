using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProxyKit;

namespace Mijabr.Proxy
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddProxy();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.Use(async (context, next) =>
            {
                Console.WriteLine($"Proxy Request: {context.Request.Path}");
                await next();
            });

            app.MapWhen(context => context.Request.Path.Equals("/"), home =>
            {
                home.RunProxy(context => context
                    .ForwardTo("http://home/home/")
                    .AddXForwardedHeaders()
                    .Send());
            });

            app.Map("/home", home =>
            {
                home.RunProxy(context => context
                    .ForwardTo("http://home/home/")
                    .AddXForwardedHeaders()
                    .Send());
            });

            app.Map("/scrabble", home =>
            {
                home.RunProxy(context => context
                    .ForwardTo("http://scrabble/scrabble/")
                    .AddXForwardedHeaders()
                    .Send());
            });

            app.RunProxy(context => context
                .ForwardTo("http://identity/")
                .Send());
        }
    }
}
