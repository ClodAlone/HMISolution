using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using DeployServer.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authorization;
using Mindscape.Raygun4Net.AspNetCore;
using DeployServer.Hubs;
using DeployServer.Processes;
using Microsoft.AspNetCore.DataProtection;
using System.IO;

namespace DeployServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment enviroment)
        {
            Configuration = configuration;
            Enviroment = enviroment;
        }

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment Enviroment { get; }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRaygun(Configuration);
            services.AddSignalR(hubOptions =>
            {
                hubOptions.EnableDetailedErrors = true;
                hubOptions.MaximumReceiveMessageSize = long.Parse(Configuration["DeployServerSettings:MaximumMessageSize"]);
            }).AddMessagePackProtocol();

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                if (Configuration["ConnectionStrings:ConnectionType"] == "SQLite")
                    options.UseSqlite(
                        Configuration.GetConnectionString("DefaultConnection"));
                else
                    options.UseSqlServer(
                        Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            if (Configuration.GetSection("PasswordComplexityRules").Exists())
            {
                services.Configure<IdentityOptions>(options =>
                {
                    if (bool.TryParse(Configuration["PasswordComplexityRules:RequireDigit"], out bool requireDigit))
                        options.Password.RequireDigit = requireDigit;

                    if (bool.TryParse(Configuration["PasswordComplexityRules:RequireLowercase"], out bool requireLowercase))
                        options.Password.RequireLowercase = requireLowercase;

                    if (bool.TryParse(Configuration["PasswordComplexityRules:RequireNonAlphanumeric"], out bool requireNonAlphanumeric))
                        options.Password.RequireNonAlphanumeric = requireNonAlphanumeric;

                    if (bool.TryParse(Configuration["PasswordComplexityRules:RequireUppercase"], out bool requireUppercase))
                        options.Password.RequireUppercase = requireUppercase;

                    if (int.TryParse(Configuration["PasswordComplexityRules:RequiredLength"], out int requiredLength))
                        options.Password.RequiredLength = requiredLength;

                    if (int.TryParse(Configuration["PasswordComplexityRules:RequiredUniqueChars"], out int requiredUniqueChars))
                        options.Password.RequiredUniqueChars = requiredUniqueChars;
                });
            }
                 
            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddControllers(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                                 .RequireAuthenticatedUser()
                                 .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            });

            bool isHosted;
            bool.TryParse(Configuration["DeployServerSettings:Hosted"], out isHosted);
            if (isHosted)
            {
                var keysFolder = System.IO.Path.Combine(Enviroment.ContentRootPath, "Keys");
                services.AddDataProtection().
                    SetApplicationName("DeployServer")
                    .PersistKeysToFileSystem(new System.IO.DirectoryInfo(keysFolder));
            }

            // Add a DbContext to store your Database Keys
            //services.AddDbContext<ApplicationKeysContext>(options =>
            //{
            //    if (Configuration["ConnectionStrings:ConnectionType"] == "SQLite")
            //        options.UseSqlite(
            //            Configuration.GetConnectionString("DefaultConnection"));
            //    else
            //        options.UseSqlServer(
            //            Configuration.GetConnectionString("DefaultConnection"));
            //});
            //// using Microsoft.AspNetCore.DataProtection;
            //services.AddDataProtection()
            //    .PersistKeysToDbContext<ApplicationKeysContext>();

            services.AddSingleton<Services.IApplicationConfiguration, Services.ApplicationConfiguration>(e => 
                Configuration.GetSection("DeployServerSettings")
                .Get<Services.ApplicationConfiguration>());
            services.AddMvc();
        }

        static T GetSettingValue<T>(IConfiguration settings, String key, T defaultValue)
        {
            var value = settings[key];
            if (value != null)
            {
                try
                {
                    defaultValue = (T)Convert.ChangeType(value, typeof(T));
                }
                catch (Exception ex)
                {

                }
            }

            return defaultValue;
        }

        void OnStarting()
        {
            ProcessInfo.LoadProcesses();
        }

        void OnShutdown()
        {
            ProcessInfo.Terminate();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, 
                                IHostApplicationLifetime applicationLifetime)
        {
            applicationLifetime.ApplicationStarted.Register(OnStarting);
            applicationLifetime.ApplicationStopping.Register(OnShutdown);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseRaygun();
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            var proxyPath = Configuration["DeployServerSettings:ProxyPath"];
            if (!String.IsNullOrEmpty(proxyPath))
            {
                app.UsePathBase(proxyPath);
                app.Use((context, next) =>
                {
                    context.Request.PathBase = new Microsoft.AspNetCore.Http.PathString(proxyPath);
                    if (context.Request.Path.StartsWithSegments(proxyPath, out var remainder))
                        context.Request.Path = remainder;
                    return next();
                });
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
                endpoints.MapHub<DeployServerHub>("/deployserverhub", options =>
                {
                    options.ApplicationMaxBufferSize = GetSettingValue(Configuration, "DeployServerSettings:ApplicationMaxBufferSize", 1024 * 1024); //1MiB
                });
            });

            var user = Configuration["Login:User"];
            var password = Configuration["Login:Password"];

            string sepChar = Path.DirectorySeparatorChar.ToString();
            string altChar = Path.AltDirectorySeparatorChar.ToString();
            DeployServerHub.pathSourceDeploy = Configuration["DeployServerSettings:Path"].Trim();
            if (!DeployServerHub.pathSourceDeploy.EndsWith(sepChar) && !DeployServerHub.pathSourceDeploy.EndsWith(altChar))
                DeployServerHub.pathSourceDeploy += sepChar;
            DeployServerHub.WebHMIProxyPath = Configuration["DeployServerSettings:WebHMIProxyPath"];
            DeployServerHub.BrowserProcessName = Configuration["DeployServerSettings:BrowserProcessName"];
            DeployServerHub.BrowserArguments = Configuration["DeployServerSettings:BrowserArguments"];
            try
            {
                DeployServerHub.WaitForWebHMIInitialization = bool.Parse(Configuration["DeployServerSettings:WaitForWebHMIInitialization"]);
            }
            catch { }
            int.TryParse(Configuration["DeployServerSettings:ProcessOutputMaxLength"], out int processOutputMaxLength);
            ProcessInfo.SettingsMaxLength = Math.Max(500, processOutputMaxLength);

            using (var scope = app.ApplicationServices.CreateScope())
            {
                var services = scope.ServiceProvider;

                //try
                //{
                    services.GetRequiredService<ApplicationDbContext>().Database.Migrate();

                    using (var context = new ApplicationDbContext(
                        services.GetService<DbContextOptions<ApplicationDbContext>>()))
                    {
                        // For sample purposes seed both with the same password.
                        // Password is set with the following:
                        // dotnet user-secrets set SeedUserPW <pw>
                        // The admin user can do anything

                        EnsureUser(services, password, user).Wait();
                    }
                //}
                //catch { }
            }
        }

        private static async Task<string> EnsureUser(IServiceProvider serviceProvider,
                                                string testUserPw, string UserName)
        {
            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();

            var user = await userManager.FindByNameAsync(UserName);
            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = UserName,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(user, testUserPw);
                if (!result.Succeeded)
                {
                    foreach(var error in result.Errors)
                        throw new Exception(error.ToString());
                }
            }
            else
            {
                if (!await userManager.CheckPasswordAsync(user, testUserPw)) //if differing from the old one, the new password must be validated and updated
                {
                    await userManager.RemovePasswordAsync(user);
                    var result = await userManager.AddPasswordAsync(user, testUserPw);
                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                            throw new Exception(error.ToString());
                    }
                }
                else //the password must be validated anyway since complexity requirements might have changed
                {
                    string validationFailedResult = string.Empty;
                    foreach (var v in userManager.PasswordValidators)
                    {
                        var result = await v.ValidateAsync(userManager, user, testUserPw);
                        if (!result.Succeeded)
                            validationFailedResult = string.Format("{0}{1}; ", validationFailedResult, result.ToString());
                    }
                    if (!string.IsNullOrEmpty(validationFailedResult))
                        throw new Exception(validationFailedResult);
                }
            }

            //if (user == null)
            //{
            //    throw new Exception("The password is probably not strong enough!");
            //}

            return user.Id;
        }
    }
}
