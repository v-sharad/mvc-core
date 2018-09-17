using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OdeToFood.Data;
using OdeToFood.Services;
namespace OdeToFood
{
    public class Startup
    {
        private IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddOpenIdConnect(options =>
            {
                _configuration.Bind("GoogleOpenId", options);
            })
            .AddCookie();
            services.AddSingleton<IGreeter, Greeter>();
            services.AddDbContext<OdeToFoodDbContext>(options => options.UseSqlServer(_configuration.GetConnectionString("OdeToFood")));
            services.AddScoped<IRestaurantData, SqlRestaurantData>();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
                               IGreeter greeter,ILogger<Startup> logger)
        {
            if (env.IsDevelopment()) 
            {
                app.UseDeveloperExceptionPage(); //gives details of exception, if any
            }

            app.UseRewriter(new RewriteOptions()
                                .AddRedirectToHttpsPermanent());

            // Function that will be invoked only once when .net core framework is setting up the pipeline
            app.Use(next =>
                {
                    #region middleware function that is invoked per Http request
                    return async context =>
                    {
                        logger.LogInformation("Request incoming");
                        if (context.Request.Path.StartsWithSegments("/mym"))
                        {
                            await context.Response.WriteAsync("My Middleware responding!!!");
                            logger.LogInformation("Request handled");
                        }
                        else
                        {
                            await next(context);
                            logger.LogInformation("Response outgoing");
                        }
                    };
                    #endregion
                });

            //app.UseDefaultFiles(); //Use index.html as default page
            app.UseStaticFiles();// Use static files like .js .html 

            app.UseAuthentication();

            app.UseMvc(ConfigureRoutes);

            app.UseWelcomePage(
                new WelcomePageOptions
                {
                    Path="/wp"
                }
                );

            app.Run(async (context) =>
            {
                //throw new Exception("Exception");
                string greeting = greeter.GetMessageOfTheDay();
                await context.Response.WriteAsync($"{greeting} : {env.EnvironmentName}");
            });
        }

        private void ConfigureRoutes(IRouteBuilder routeBuilder)
        {
            routeBuilder.MapRoute("Default",
                "{controller=Home}/{action=Index}/{id?}"
                );
        }
    }
}
