using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

namespace HeroServer
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.GetApplicationDefault(),
            });
        }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConfiguration>(Configuration);

            KeyVaultHelper keyVaultHelper = WebEnvConfig.Initialize(Configuration).Result;

            MailHelper.Initialize();

            CertificateFunctions.Initialize();
            CybersourceFunctions.Initialize();

            AesHelper.Initialize();  // GenerateKey(connString);
            WebSysUserFunctions.Initialize();
            FirebaseFunctions.Initialize();
            StorageFunctions.Initialize();

            CardFunctions.Initialize();

            services.AddControllers()
                    .AddJsonOptions(options =>
                     {
                         options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                         options.JsonSerializerOptions.PropertyNamingPolicy = new WebJsonNamingPolicy();
                     });

            //services.AddHostedService<SatWorker>();

            // Firebase Auth Config
            services.AddMvc(options =>
            {
                options.Filters.Add(new ResponseCacheAttribute() { NoStore = true, Location = ResponseCacheLocation.None });
                options.EnableEndpointRouting = false;
            });

            services.AddAuthorizationBuilder().AddPolicy("FirebaseAccess", policy => policy.Requirements.Add(new Security.AccessRequirement()));
                                              //.AddPolicy("ApiKeyAccess", policy => policy.Requirements.Add(new Security.ApiKeyRequirement()))
                                              //.AddPolicy("FirebaseOrApiKeyAccess", policy => policy.Requirements.Add(new Security.FirebaseApiKeyRequirement()));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                //options.AutomaticAuthenticate = true;
                options.Authority = "https://securetoken.google.com/ionicfireauth1";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = "https://securetoken.google.com/" + WebEnvConfig.Firebase,
                    ValidateAudience = true,
                    ValidAudience = WebEnvConfig.Firebase,
                    ValidateLifetime = true
                };
            });

            services.AddSingleton<IAuthorizationHandler, Security.AccessHandler>();
            //services.AddTransient<IAuthorizationHandler, Security.AccessHandler>();
            //services.AddTransient<IAuthorizationHandler, Security.ApiKeyHandler>();
            //services.AddTransient<IAuthorizationHandler, Security.AccessFirebaseHandler>();
            //services.AddTransient<IAuthorizationHandler, Security.AccessApiKeyHandler>();

            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            logger.LogWarning("INFO : Environment = {Flag}", WebEnvConfig.Flag);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                logger.LogWarning("INFO : Azure Development");
            }
            else
            {
                app.UseHsts();
                logger.LogWarning("INFO : Azure Production");
            }

            // MIDDLEWARE
            //app.UseMiddleware<EncryptionMiddleware>();

            /*app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });*/

            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin();
                builder.AllowAnyHeader();
                builder.AllowAnyMethod();
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                ServeUnknownFileTypes = true,
            });

            app.UseAuthorization();     // Optional
            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
