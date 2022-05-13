using TechnoWorld_API.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TechnoWorld_API.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using TechnoWorld_API.Helpers;
using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Events;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Microsoft.AspNetCore.HttpOverrides;
using System.Net;
using System.Security.Claims;

namespace BNS_API
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
            services.AddDbContext<TechnoWorldContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Chenk")));
            services.AddControllers();
            services.AddMvc().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;

                //options.SerializerSettings.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Objects;
                //options.SerializerSettings.TypeNameAssemblyFormatHandling = Newtonsoft.Json.TypeNameAssemblyFormatHandling.Simple;
            });
            //services.AddDistributedMemoryCache();
            //services.AddSession(options =>
            //{
            //    options.IdleTimeout = TimeSpan.FromMinutes(1);
            //});
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = false;
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = AuthOptions.ISSUER,
                        ValidateAudience = true,
                        ValidAudience = AuthOptions.AUDIENCE,
                        ValidateLifetime = true,
                        IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                        ValidateIssuerSigningKey = true,
                        ClockSkew = TimeSpan.Zero
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accesstoken = context.Request.Query["access_token"];

                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accesstoken) &&
                                path.StartsWithSegments("/technoWorldHub"))
                            {
                                context.Token = accesstoken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddSignalR();
            services.AddControllersWithViews();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BNS_API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BNS_API v1"));
                //app.UseSerilogRequestLogging();
                app.UseSerilogRequestLogging(options =>
                {
                    options.MessageTemplate = "Запрос от {UserName} ({ClientIp}:{ClientPort}) по {RequestMethod} {RequestPath} ответил {StatusCode} за {Elapsed:0.0000} мс";
                    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                    {
                        string userName = "";
                        if (httpContext.User?.Claims.Count() == 0)
                        {
                            userName = "Неавторизированный пользователь";
                        }
                        else
                        {
                            userName = httpContext.User?.Claims.FirstOrDefault(p => p.Type == ClaimsIdentity.DefaultNameClaimType).Value;
                        }
                        diagnosticContext.Set("UserName", userName);
                        diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                        diagnosticContext.Set("ClientIp", httpContext.Connection?.RemoteIpAddress);
                        diagnosticContext.Set("ClientPort", httpContext.Connection?.RemotePort);
                    };
                });
            }
            else
            {
                app.UseSerilogRequestLogging(options =>
                {
                    options.MessageTemplate = "Запрос от {ClientIp}:{ClientPort} на {RequestMethod} {RequestPath} ответил {StatusCode} за {Elapsed:0.0000} мс | {RequestHost}";
                    options.GetLevel = (httpContext, elapsed, ex) => LogEventLevel.Debug;
                    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                    {
                        string userName = "";
                        if (httpContext.User?.Claims.Count() == 0)
                        {
                            userName = "Неавторизированный пользователь";
                        }
                        else
                        {
                            userName = httpContext.User?.Claims.FirstOrDefault(p => p.Type == ClaimsIdentity.DefaultNameClaimType).Value;
                        }
                        diagnosticContext.Set("UserName", userName);
                        diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                        diagnosticContext.Set("ClientIp", httpContext.Connection?.RemoteIpAddress);
                        diagnosticContext.Set("ClientPort", httpContext.Connection?.RemotePort);
                    };
                });
            }


            app.Use((context, next) =>
            {
                var token = context.Request.Cookies[".AspNetCore.Application.Id"];

                if (!string.IsNullOrEmpty(token))
                    context.Request.Headers.Add("Authorization", "Bearer " + token);

                return next();
            });

            app.UseRouting();
            //app.UseSession();
            //app.Use(async (context, next) =>
            //{
            //    var JWToken = context.Session.GetString("JWToken");
            //    if (!string.IsNullOrEmpty(JWToken))
            //    {
            //        context.Request.Headers.Add("Authorization", "Bearer " + JWToken);
            //    }
            //    await next();
            //});

            app.UseAuthentication();
            app.UseAuthorization();

            var supportedCultures = new[]
            {
                new CultureInfo("en-US"),
                new CultureInfo("en-GB"),
                new CultureInfo("en"),
                new CultureInfo("ru-RU"),
                new CultureInfo("ru"),
                new CultureInfo("de-DE"),
                new CultureInfo("de")
            };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("ru-RU"),
                //SupportedCultures = supportedCultures,
                //SupportedUICultures = supportedCultures
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<TechnoWorldHub>("/technoWorldHub");
            });
        }
    }
}
