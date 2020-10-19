using HGV.Buckler.Identity.Data;
using HGV.Buckler.Identity.Services;
using HGV.Daedalus;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Security.Cryptography.X509Certificates;

namespace HGV.Buckler.Identity
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AuthMessageSenderOptions>(Configuration.GetSection(AuthMessageSenderOptions.Prefix));

            services.AddHttpClient();

            services.AddSingleton<ISteamKeyProvider, SteamKeyProvider>();
            services.AddSingleton<IDotaApiClient, DotaApiClient>();

            services.AddDbContext<ApplicationDbContext>(o => o.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<IdentityUser>(o => { 
                o.SignIn.RequireConfirmedAccount = true;
                o.Tokens.ProviderMap.Add("CustomEmailConfirmation",new TokenProviderDescriptor(typeof(CustomEmailConfirmationTokenProvider<IdentityUser>)));
                o.Tokens.EmailConfirmationTokenProvider = "CustomEmailConfirmation";
            }).AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            services.Configure<DataProtectionTokenProviderOptions>(o => o.TokenLifespan = TimeSpan.FromHours(3));
            
            services.AddTransient<CustomEmailConfirmationTokenProvider<IdentityUser>>();
            services.AddTransient<IEmailSender, EmailSender>();
            services.Configure<AuthMessageSenderOptions>(Configuration);

            services.AddTransient<IProfileService, IdentityWithAdditionalClaimsProfileService>();

            var builder = services.AddIdentityServer(options =>
            {
                options.IssuerUri = "https://buckler.highgroundvision.com/";
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
                options.EmitStaticAudienceClaim = true; // see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
            })
            .AddInMemoryApiScopes(IdentityServerConfig.ApiScopes(Configuration))
            .AddInMemoryIdentityResources(IdentityServerConfig.IdentityResources(Configuration))
            .AddInMemoryClients(IdentityServerConfig.Clients(Configuration))
            .AddAspNetIdentity<IdentityUser>()
            .AddProfileService<IdentityWithAdditionalClaimsProfileService>();

            if (Environment.IsDevelopment())
            {
                builder.AddDeveloperSigningCredential();
            }
            else
            {
                var key = Configuration["buckler"];
                var pfxBytes = Convert.FromBase64String(key);
                var cert = new X509Certificate2(pfxBytes, (string)null, X509KeyStorageFlags.MachineKeySet);
                builder.AddSigningCredential(cert);
            }
            
            services.AddAuthentication()
                .AddSteam(o => {
                    o.ApplicationKey = Configuration["Authentication:Steam:ApplicationKey"];
                })
                .AddDiscord(o => { 
                    o.Scope.Add("identify");
                    o.ClientId = Configuration["Authentication:Discord:ClientId"];
                    o.ClientSecret = Configuration["Authentication:Discord:ClientSecret"];
                });

             services.AddCors();
            
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts(); // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors(o => o.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
