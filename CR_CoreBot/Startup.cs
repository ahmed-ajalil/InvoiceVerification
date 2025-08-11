// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.18.1
using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using CR_CoreBot.Dialogs;
using CR_CoreBot.Helpers;
using CR_CoreBot_DataAccess.CR_OpenAI;
using CR_CoreBot_DTO;
using CR_HRPortalAI_DataAcess.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace CR_CoreBot
{
	public class Startup

    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            AppSettingHelper.Initialize(Configuration);
            services.AddDbContext<CropenAiContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("RestoreDefaultConnection"));
            });


            services.AddHttpClient().AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.MaxDepth = HttpHelper.BotMessageSerializerSettings.MaxDepth;
            });
            services.Configure<AppSettingsDTO>(Configuration.GetSection("Credential"));
            services.Configure<ConnectionModel>(Configuration.GetSection("ConnectionStrings"));
            //reshav
            services.AddDbContext<CropenAiContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("RestoreDefaultConnection"));
            });
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            // Create the Bot Framework Authentication to be used with the Bot Adapter.
            services.AddSingleton<BotFrameworkAuthentication, ConfigurationBotFrameworkAuthentication>();

            //NToastNotify
            services.AddNotyf(config =>
            {
                config.DurationInSeconds = 5;
                config.IsDismissable = true;
                config.Position = NotyfPosition.BottomRight;
            });
            // Create the Bot Adapter with error handling enabled.
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();
            services.AddSingleton<ITextAnalyticsHelper, TextAnalyticsHelper>();

            // Create the storage we'll be using for User and Conversation state. (Memory is great for testing purposes.)
            services.AddSingleton<IStorage, MemoryStorage>();

            // Create the User state. (Used in this bot's Dialog implementation.)
            services.AddSingleton<UserState>();

            // Create the Conversation state. (Used by the Dialog system itself.)
            services.AddSingleton<ConversationState>();



            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            //Comment By rupesh for Channel
            //services.AddTransient<IBot, DialogAndWelcomeBot<MainDialog>>();
            services.AddTransient<IBot, Bots.CR_CoreBot>();
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddRazorPages().AddRazorRuntimeCompilation();
            services.AddDistributedMemoryCache();
            services.AddMvc().AddSessionStateTempDataProvider();

            string conStr = this.Configuration.GetConnectionString("MyConn");

            services.AddAuthentication(AzureADDefaults.AuthenticationScheme)
         .AddAzureAD(options => Configuration.Bind("AzureAd", options));
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(600);
            });
            //services.AddHttpContextAccessor();
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseNotyf();
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            

            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllerRoute(
            //        name: "default",
            //        pattern: "{controller=Login}/{action=Index}/{id?}");
            //});
            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapAreaControllerRoute(
                //   name: "Banking",
                //   areaName: "Banking",
                //   pattern: "Banking/{controller=Sales}/{action=Index}");

                endpoints.MapControllerRoute(
                    name: "areaRoute",
                    pattern: "{area:exists}/{controller}/{action}/{id?}"
                );
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Login}/{action=Index}/{id?}");

            });
        }
    }
}
