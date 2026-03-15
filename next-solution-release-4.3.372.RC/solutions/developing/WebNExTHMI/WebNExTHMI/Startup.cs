using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mindscape.Raygun4Net.AspNetCore;
using WebNExTHMI.Hubs;
using DevExpress.AspNetCore;
using DevExpress.AspNetCore.Reporting;
using WebNExTHMI.PlatformComponents.ReportHelpers;
using Microsoft.Extensions.Hosting;
using DevExpress.XtraReports.Security;
using ReportManager.ReportService;
using DevExpress.XtraReports.Native;
using Microsoft.AspNetCore.Mvc;

namespace WebNExTHMI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            SerializationService.RegisterSerializer(CustomUntypedDataSetSerializer.Name, new CustomUntypedDataSetSerializer());
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Register reporting services in an application's dependency injection container. 
            services.AddDevExpressControls();
            services.ConfigureReportingServices(configurator => {
                configurator.ConfigureReportDesigner(designerConfigurator => {
                    designerConfigurator.RegisterDataSourceWizardConfigFileConnectionStringsProvider();
                });
                configurator.ConfigureWebDocumentViewer(viewerConfigurator => {
                    viewerConfigurator.UseCachedReportSourceBuilder();
                });
            });

            var proxyPath = Configuration["WebNExTHMISettings:ProxyPath"];
            if (proxyPath != null && !proxyPath.StartsWith("/"))
                Configuration["WebNExTHMISettings:ProxyPath"] = String.Empty;

            // Add framework services.
            services.AddSingleton<Services.IApplicationConfiguration, Services.ApplicationConfiguration>(e =>
                Configuration.GetSection("WebNExTHMISettings")
                .Get<Services.ApplicationConfiguration>());
            services
                .AddMvc()
                .AddMvcOptions(options =>
                    options.Filters.Add(
                        new ResponseCacheAttribute
                        {
                            NoStore = true,
                            Location = ResponseCacheLocation.None
                        })
                )
                .AddJsonOptions(options =>
                {
                    // options.JsonSerializerOptions. .SerializerSettings.ContractResolver = new DefaultContractResolver()
                });

            services.AddRaygun(Configuration);

            services.AddSignalR(hubOptions =>
            {
                hubOptions.EnableDetailedErrors = true;
                hubOptions.MaximumReceiveMessageSize = 41943040;
                long msgSize;
                if (!String.IsNullOrEmpty(Configuration["WebNExTHMISettings:MaximumReceiveMessageSize"]) && long.TryParse(Configuration["WebNExTHMISettings:MaximumReceiveMessageSize"], out msgSize))
                    hubOptions.MaximumReceiveMessageSize = msgSize == 0 ? null : (long?)msgSize;

                int maxpercalls;
                if (!String.IsNullOrEmpty(Configuration["WebNExTHMISettings:MaximumParallelInvocationsPerClient"]) && int.TryParse(Configuration["WebNExTHMISettings:MaximumParallelInvocationsPerClient"], out maxpercalls))
                    hubOptions.MaximumParallelInvocationsPerClient = maxpercalls;
                
            }).AddMessagePackProtocol();

            PlatformComponents.PlatformComponents.Init();
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

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var project = Configuration.GetValue<string>("project") ?? Configuration["WebNExTHMISettings:Project"];
            Console.WriteLine("Loading project: " + project);
            var ARMappingProject = Configuration["WebNExTHMISettings:ARMappingProject"];
            PlatformComponents.PlatformComponents.LogingRequired = GetSettingValue(Configuration, "WebNExTHMISettings:LoginRequired", false);
            PlatformComponents.PlatformComponents.ShowDebugWindow = GetSettingValue(Configuration, "WebNExTHMISettings:ShowDebugWindow", false);
            PlatformComponents.PlatformComponents.CacheDelay = GetSettingValue(Configuration, "WebNExTHMISettings:CacheDelay", 200);
            PlatformComponents.PlatformComponents.MapProvider = GetSettingValue(Configuration, "WebNExTHMISettings:MapProvider", "");
            PlatformComponents.PlatformComponents.MapProviderKey = GetSettingValue(Configuration, "WebNExTHMISettings:MapProviderKey", "");
            PlatformComponents.PlatformComponents.MapLayers = GetSettingValue(Configuration, "WebNExTHMISettings:MapLayers", "");
            PlatformComponents.PlatformComponents.MapZoomFactor = GetSettingValue(Configuration, "WebNExTHMISettings:MapZoomFactor", 1);
            PlatformComponents.PlatformComponents.MapMaxZoomFactor = GetSettingValue(Configuration, "WebNExTHMISettings:MapMaxZoomFactor", 256);
            PlatformComponents.PlatformComponents.MapBounds = GetSettingValue(Configuration, "WebNExTHMISettings:MapBounds", "");
            PlatformComponents.PlatformComponents.LegacyMapView = GetSettingValue(Configuration, "WebNExTHMISettings:LegacyMapView", false);
            PlatformComponents.PlatformComponents.MapViewTilesURL = GetSettingValue(Configuration, "WebNExTHMISettings:MapViewTilesURL", "");
            PlatformComponents.PlatformComponents.MapViewTilesOptions = GetSettingValue(Configuration, "WebNExTHMISettings:MapViewTilesOptions", "");
            PlatformComponents.PlatformComponents.ForceMainPageAR = GetSettingValue(Configuration, "WebNExTHMISettings:ForceMainPageAR", false);
            PlatformComponents.PlatformComponents.ActiveSessionCountVariableName = GetSettingValue(Configuration, "WebNExTHMISettings:ActiveSessionCountVariableName", "");
            PlatformComponents.PlatformComponents.ARScanQRCodeOnly = GetSettingValue(Configuration, "WebNExTHMISettings:ARScanQRCodeOnly", false);
            PlatformComponents.PlatformComponents.ARScanQRCode = GetSettingValue(Configuration, "WebNExTHMISettings:ARScanQRCode", false);
            PlatformComponents.PlatformComponents.ARScanQRToleranceMS = GetSettingValue(Configuration, "WebNExTHMISettings:ARScanQRToleranceMS", 2000);
            PlatformComponents.PlatformComponents.ARQRCodeScaleFactor = Math.Max(1, Math.Min(Configuration.GetValue<float>("WebNExTHMISettings:ARQRCodeScaleFactor", 1), 3));
            PlatformComponents.PlatformComponents.IPCameraKeepAliveTimeout = GetSettingValue(Configuration, "WebNExTHMISettings:IPCameraKeepAliveTimeout", 30000);
            PlatformComponents.PlatformComponents.ServerInvokeMinFreqOnDrag = GetSettingValue(Configuration, "WebNExTHMISettings:ServerInvokeMinFreqOnDrag", 100);
            PlatformComponents.PlatformComponents.StaticToolboxLoading = GetSettingValue(Configuration, "Toolbox:StaticLoading", false);
            
            if (PlatformComponents.PlatformComponents.StaticToolboxLoading)
            {
                var toolboxPath = GetSettingValue(Configuration, "Toolbox:SourcePath", "");
                toolboxPath = System.IO.Path.Combine(env.ContentRootPath, toolboxPath.Replace('/', System.IO.Path.DirectorySeparatorChar));
                if (System.IO.Directory.Exists(toolboxPath))
                {
                    foreach (var file in System.IO.Directory.GetFiles(toolboxPath, "*.html", System.IO.SearchOption.TopDirectoryOnly))
                        PlatformComponents.PlatformComponents.PreloadedToolboxComponents.Add(System.IO.Path.GetFileName(file));
                }
            }

            PlatformComponents.PlatformComponents.TensorFlowConnectTo = GetSettingValue(Configuration, "WebNExTHMISettings:TensorFlowConnectTo", "tcp://127.0.0.1:8080");
            PlatformComponents.PlatformComponents.TensorFlowBindTo = GetSettingValue(Configuration, "WebNExTHMISettings:TensorFlowBindTo", "tcp://*:8181");
            PlatformComponents.PlatformComponents.TensorFlowTimeoutSec = GetSettingValue(Configuration, "WebNExTHMISettings:TensorFlowTimeoutSec", 10);
            PlatformComponents.PlatformComponents.ScreenDelayUnloadMSecs = GetSettingValue(Configuration, "WebNExTHMISettings:ScreenDelayUnloadMSecs", 1000);
            PlatformComponents.PlatformComponents.ScreenAliveTimeoutSecs = GetSettingValue(Configuration, "WebNExTHMISettings:ScreenAliveTimeoutSecs", 30);

            var allowReportScriptExecution = GetSettingValue(Configuration, "WebNExTHMISettings:AllowReportScriptExecution", true);
            if (allowReportScriptExecution)
                ScriptPermissionManager.GlobalInstance = new ScriptPermissionManager(DevExpress.XtraReports.Security.ExecutionMode.Unrestricted);

            PlatformComponents.PlatformComponents.OpenProject(project);
            PlatformComponents.PlatformComponents.OpenARMappingProject(ARMappingProject);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                // app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                app.UseRaygun();
            }

            if (Configuration["WebNExTHMISettings:DisableHttpsRedirection"] != true.ToString() && Configuration["WebNExTHMISettings:HttpsListeningPort"] != "0")
                app.UseHttpsRedirection();
            app.UseStaticFiles();
            //app.UseStaticFiles(new StaticFileOptions()
            //{
            //    OnPrepareResponse = context =>
            //    {
            //        context.Context.Response.Headers.Add("Cache-Control", "no-cache, no-store");
            //        context.Context.Response.Headers.Add("Expires", "-1");
            //    }
            //});
            app.UseRouting();

            var proxyPath = Configuration["WebNExTHMISettings:ProxyPath"];
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

            // Initialize reporting services. 
            app.UseDevExpressControls();
            DevExpress.XtraReports.Web.Extensions.ReportStorageWebExtension.RegisterExtensionGlobal(new CustomReportStorageWebExtension());

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
                endpoints.MapHub<CoreHub>("/corehub", options =>
                {
                    options.ApplicationMaxBufferSize = GetSettingValue(Configuration, "WebNExTHMISettings:ApplicationMaxBufferSize", 1024 * 1024); //1MiB
                });
            });
        }
    }
}
