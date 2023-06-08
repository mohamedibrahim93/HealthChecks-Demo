using System.ServiceProcess;
using HealthChecks.Network.Core;
using HealthChecks.UI.Client;
using HealthChecks.UI.Core;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text;
using System.Text.Json;
using Grpc.Health.V1;
using Grpc.Net.Client;
using HealthChecks.ApplicationStatus.DependencyInjection;
using MassTransit;
using Microsoft.AspNetCore.SignalR.Client;
using JsonSerializer = System.Text.Json.JsonSerializer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//-1 Basic health probe
builder.Services.AddHealthChecks();

//-2 Register health check services
//Microsoft.Bcl.AsyncInterfaces
//builder.Services
//    .AddHealthChecks()
//    .AddCheck<SampleHealthCheck>("Sample");

//-3
//builder.Services.AddHealthChecks()
//    .AddCheck<SampleHealthCheck>(
//        "Sample",
//        failureStatus: HealthStatus.Degraded,
//        tags: new[] { "sample" });

//-4
//builder.Services.AddHealthChecks()
//    .AddCheck("Sample", () => HealthCheckResult.Healthy("A healthy result."));

//-5 call AddTypeActivatedCheck
//builder.Services.AddHealthChecks()
//    .AddTypeActivatedCheck<SampleHealthCheckWithArgs>(
//        "Sample",
//        failureStatus: HealthStatus.Degraded,
//        tags: new[] { "sample" },
//        args: new object[] { 1, "Arg" });

//-9 Filter health checks
//builder.Services.AddHealthChecks()
//    .AddCheck("Sample1", () => HealthCheckResult.Healthy("A healthy result."), new List<string>() { "sample1" })
//    .AddCheck("Sample2", () => HealthCheckResult.Unhealthy("A unhealthy result."), new List<string>() { "sample2" });

//-10 Customize the HTTP status code
//builder.Services.AddHealthChecks()
//    .AddCheck("Sample1", () => HealthCheckResult.Degraded("A Degraded result."), new List<string>() { "sample1" });

//-13 UIResponseWriter
//builder.Services.AddHealthChecks()
//    .AddCheck("Sample1", () => HealthCheckResult.Healthy("A healthy result."), new List<string>() { "sample1" })
//    .AddCheck("Sample2", () => HealthCheckResult.Degraded("A Degraded result."), new List<string>() { "sample2" })
//    .AddCheck("Sample3", () => HealthCheckResult.Unhealthy("A Unhealthy result."), new List<string>() { "sample3" });

//-14 Database probe
//builder.Services
//    .AddHealthChecks()
//    .AddSqlServer(
//        connectionString: builder.Configuration["Data:SQL"],
//        healthQuery: "SELECT 1;",
//        name: "sql",
//        failureStatus: HealthStatus.Degraded,
//        tags: new string[] { "db", "sql", "sqlserver" });

//-14-2 Database probe
//builder.Services.AddHealthChecks()
//    .AddSqlServer(builder.Configuration["Data:SQL"]!);

//-15 Entity Framework Core DbContext probe
//builder.Services.AddDbContext<SampleDbContext>(options =>
//    options.UseSqlServer(builder.Configuration["Data:SQL"]));

//builder.Services
//    .AddHealthChecks()
//    .AddDbContextCheck<SampleDbContext>()
//    .AddRedis(builder.Configuration["Data:Redis"], "Redis 2", HealthStatus.Degraded, new List<string>() { "cache db" })
//    .AddMongoDb(builder.Configuration["Data:Mongo"])
//    .AddNpgSql(builder.Configuration["Data:Postgre"])
//    .AddUrlGroup(new Uri("https://localhost:7703/swagger/index.html"), "API", HealthStatus.Degraded)
//    .AddRabbitMQ(builder.Configuration["Data:RabbitMQ"], null, "RabbitMQ")
//    .AddCheck<RandomHealthCheck>("random")
//    .AddDiskStorageHealthCheck(setup =>
//    {
//        setup.AddDrive("C:\\", 90_000);
//    })
//    .AddFolder(setup =>
//    {
//        setup.AddFolder("C:\\API");
//    })
//    .AddPrivateMemoryHealthCheck(8000)
//    .AddProcessAllocatedMemoryHealthCheck(2_000)
//    .AddVirtualMemorySizeHealthCheck(8_000)
//    .AddDnsResolveHealthCheck(setup => setup.ResolveHost("stg1.gafi.gov.eg"))
//    .AddPingHealthCheck(setup => setup.AddHost("8.8.8.88", 100))
//    .AddWindowsServiceHealthCheck("redis", controller => controller.Status == ServiceControllerStatus.Running)
//    .AddWorkingSetHealthCheck(20_000)
//    .AddApplicationStatus()
//    .AddCheck("d", c => HealthCheckResult.Degraded())
//    .AddSmtpHealthCheck(setup =>
//    {
//        setup.Host = "YourMailServer";
//        setup.Port = 587;
//        setup.ConnectionType = SmtpConnectionType.TLS;
//        setup.LoginWith("you@example.com", "Passw0rd");
//        setup.AllowInvalidRemoteCertificates = true;
//    }, tags: new[] { "smtp" }, failureStatus: HealthStatus.Degraded)
//    .AddProcessHealthCheck("", c => c.First().)
//    .AddIdentityServer(new Uri("http://localhost:6060"))
//    .AddAzureServiceBusQueue("Endpoint=sb://unaidemo.servicebus.windows.net/;SharedAccessKeyName=policy;SharedAccessKey=5RdimhjY8yfmnjr5L9u5Cf0pCFkbIM7u0HruJuhjlu8=", "que1")
//    .AddAzureServiceBusTopic("Endpoint=sb://unaidemo.servicebus.windows.net/;SharedAccessKeyName=policy;SharedAccessKey=AQhdhXwnkzDO4Os0abQV7f/kB6esTfz2eFERMYKMsKk=", "to1")
//    .AddApplicationInsightsPublisher(saveDetailedReport: true)
//.AddSendGrid(builder.Configuration["Data:ApiKey"])
//.AddApplicationInsightsPublisher()
//.AddSeqPublisher(setup =>
//{
//    setup.Endpoint = "https://webhook.site/0007536e-9554-493b-be3f-a53c72b98d15";
//})
//.AddCloudWatchPublisher()
//.AddDatadogPublisher("myservice.healthchecks")
//.AddPrometheusGatewayPublisher("test","j1")

;

//MassTransit - RabbitMQ Configuration
//builder.Services.AddMassTransit(config =>
//{
//    config.UsingRabbitMq((ctx, cfg) =>
//    {
//        cfg.Host(builder.Configuration["Data:RabbitMQ"]);
//        cfg.UseHealthCheck(ctx);
//    });
//});
//builder.Services.AddMassTransitHostedService();

//builder.Services.AddHealthChecks()
//    .AddAsyncCheck("Grpc", async () =>
//    {
//        var channel = GrpcChannel.ForAddress(builder.Configuration["Data:Grpc"]);
//        var client = new Health.HealthClient(channel);
//        var response = await client.CheckAsync(new HealthCheckRequest());
//        var status = response.Status;
//        return status == HealthCheckResponse.Types.ServingStatus.Serving ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy();
//    });

//builder.Services.AddGrpcHealthChecks();

//-16 HealthCheckUI
builder.Services
        .AddHealthChecksUI()
//UI Storage Providers
        .AddInMemoryStorage()
//.AddSqlServerStorage(builder.Configuration["Data:SQLHC"])
//.AddPostgreSqlStorage("connectionString");
//.AddMySqlStorage("connectionString");
//.AddSqliteStorage($"Data Source=sqlite.db");
;

//builder.Services
//    .AddHealthChecksUI(setup =>
//    {
        // UI Polling interval
        //setup.SetEvaluationTimeInSeconds(5);

        // UI API max active requests - Default value is 3 active requests:
        //Only one active request will be executed at a time.
        //All the excedent requests will result in 429 (Too many requests)
        //setup.SetApiMaxActiveRequests(1);

        // UI Database Migrations
        //setup.DisableDatabaseMigrations();
        //"HealthChecksUI": {
        //    "DisableMigrations": true
        //}

        // Set the maximum history entries by endpoint that will be served by the UI api middleware
        //setup.MaximumHistoryEntriesPerEndpoint(50);

        //setup.AddHealthCheckEndpoint("endpoint1", "http://localhost:8001/hc");
        //setup.AddHealthCheckEndpoint("endpoint2", "http://remoteendpoint:9000/hc");

        //setup.AddHealthCheckEndpoint("endpoint3", "/hc");

        //setup.AddWebhookNotification("W01", "https://webhook.site/0007536e-9554-493b-be3f-a53c72b98d15","[[LIVENESS]] is running normally", "[[LIVENESS]] is not running normally}");

        //setup.AddWebhookNotification("W01", "https://webhook.site/0007536e-9554-493b-be3f-a53c72b98d15","{\"Msg\":\"service is not healthy\"}", "{\"Msg\":\"service is healthy\"}");

        //setup.SetMinimumSecondsBetweenFailureNotifications(60);

        //1.- HealthChecks: The collection of health checks uris to evaluate.
        //2.- EvaluationTimeInSeconds: Number of elapsed seconds between health checks.
        //3.- Webhooks: If any health check returns a *Failure * result, this collections will be used to notify the error status.
        //      (Payload is the json payload and must be escaped.For more information see the notifications documentation section)
        //4.- MinimumSecondsBetweenFailureNotifications: The minimum seconds between failure notifications to avoid receiver flooding.

        /*
         *  Webhooks and Failure Notifications

            If the WebHooks section is configured, HealthCheck-UI automatically posts a new notification into the webhook collection. HealthCheckUI uses a simple replace method for values in the webhook's Payload and RestorePayload properties. At this moment we support two bookmarks:

            [[LIVENESS]] The name of the liveness that returns Down.

            [[FAILURE]] A detail message with the failure.

            [[DESCRIPTIONS]] Failure descriptions

            Webhooks can be configured with configuration providers and also by code. Using code allows greater customization as you can setup you own user functions to customize output messages or configuring if a payload should be sent to a given webhook endpoint.

            The web hooks section contains more information and webhooks samples for Microsoft Teams, Azure Functions, Slack and more.
         *
         */

        //setup.AddWebhookNotification("webhook1",
        //    uri: "https://webhook.site/0007536e-9554-493b-be3f-a53c72b98d15",
        //    payload: "{ message: \"Webhook report for [[LIVENESS]]: [[FAILURE]] - Description: [[DESCRIPTIONS]]\"}",
        //    restorePayload: "{ message: \"[[LIVENESS]] is back to life\"}",
        //    shouldNotifyFunc: (livenessName, report) => DateTime.UtcNow.Hour >= 8 && DateTime.UtcNow.Hour <= 23,
        //customMessageFunc: (livenessName, report) =>
        //{
        //    var failing = report.Entries.Where(e => e.Value.Status == UIHealthStatus.Unhealthy);
        //    return $"{failing.Count()} healthchecks are failing";
        //},
        //customDescriptionFunc: (livenessName, report) =>
        //{
        //    var failing = report.Entries.Where(e => e.Value.Status == UIHealthStatus.Unhealthy);
        //    return $"HealthChecks with names {string.Join("/", failing.Select(f => f.Key))} are failing";
        //});

        // Since version 2.2.34, UI supports custom styles and branding by using a custom style sheet and css variables.To add your custom styles sheet, use the UI setup method:
        //setup.AddCustomStylesheet();

        //UI Configure HttpClient and HttpMessageHandler for Api and Webhooks endpoints
        //setup.ConfigureApiEndpointHttpclient((sp, client) =>
        //{
        //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "supertoken");
        //});
        //setup.ConfigureWebhooksEndpointHttpclient((sp, client) =>
        //{
        //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "sampletoken");
        //});

    //    setup.SetHeaderText("Health Check Demo");
    //})
    //.AddInMemoryStorage()
    //;

//services.AddHealthChecksUI(setupSettings: setup =>
//    {
//        setup.ConfigureApiEndpointHttpclient((sp, client) =>
//            {
//                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "supertoken");
//            })
//            .UseApiEndpointHttpMessageHandler(sp =>
//            {
//                return new HttpClientHandler
//                {
//                    Proxy = new WebProxy("http://proxy:8080")
//                };
//            })
//            .UseApiEndpointDelegatingHandler<CustomDelegatingHandler>()
//            .ConfigureWebhooksEndpointHttpclient((sp, client) =>
//            {
//                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "sampletoken");
//            })
//            .UseWebhookEndpointHttpMessageHandler(sp =>
//            {
//                return new HttpClientHandler()
//                {
//                    Properties =
//                    {
//                        ["prop"] = "value"
//                    }
//                };
//            })
//            .UseWebHooksEndpointDelegatingHandler<CustomDelegatingHandler2>();
//    })
//    .AddInMemoryStorage();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//1-
// app.MapHealthChecks("/hc");

//-6 Use Health Checks Routing
//app.MapHealthChecks("/healthz");

//-7 Require host
//app.MapHealthChecks("/hc")
//    .RequireHost("www.contoso.com:7151");

//app.MapHealthChecks("/hc")
//    .RequireHost("*:7151");

//-8 Require authorization
//app.MapHealthChecks("/hc")
//    .RequireAuthorization();

//Health check options
//-9 Filter health checks
//app.MapHealthChecks("/hc", new HealthCheckOptions
//{
//    Predicate = healthCheck => healthCheck.Tags.Contains("sample2")
//});

//-10 Customize the HTTP status code
//app.MapHealthChecks("/hc", new HealthCheckOptions
//{
//    ResultStatusCodes =
//    {
//        [HealthStatus.Healthy] = StatusCodes.Status200OK,
//        [HealthStatus.Degraded] = StatusCodes.Status400BadRequest,
//        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
//    }
//});

//-11 Suppress cache headers
//app.MapHealthChecks("/hc", new HealthCheckOptions
//{
//    AllowCachingResponses = true
//});

//-12 Customize output
//app.MapHealthChecks("/hc", new HealthCheckOptions
//{
//    ResponseWriter = Helper.WriteResponse2
//});

//-13 UIResponseWriter
app.MapHealthChecks("/hc", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

//app.UseHealthChecksPrometheusExporter("/hc2");

//-16 HealthCheckUI
//app.UseEndpoints(config => config.MapHealthChecksUI());
app.MapHealthChecksUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

//-2 Create health checks
public class SampleHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var isHealthy = true;

        // ...

        if (isHealthy)
        {
            return Task.FromResult(HealthCheckResult.Healthy("A healthy result."));
        }

        return Task.FromResult(new HealthCheckResult(context.Registration.FailureStatus, "An unhealthy result."));
    }
}

public class RandomHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (DateTime.UtcNow.Minute % 2 == 0)
        {
            return Task.FromResult(HealthCheckResult.Healthy());
        }

        return Task.FromResult(HealthCheckResult.Unhealthy(description: "failed", exception: new InvalidCastException("Invalid cast from to to to")));
    }
}

//-5 call AddTypeActivatedCheck
public class SampleHealthCheckWithArgs : IHealthCheck
{
    private readonly int _arg1;
    private readonly string _arg2;

    public SampleHealthCheckWithArgs(int arg1, string arg2) => (_arg1, _arg2) = (arg1, arg2);

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        // ...

        return Task.FromResult(HealthCheckResult.Unhealthy("A healthy result."));
    }
}

//-12 Customize output
public class Helper
{
    public static Task WriteResponse1(HttpContext context, HealthReport healthReport)
    {
        context.Response.ContentType = "application/json; charset=utf-8";

        return context.Response.WriteAsync(healthReport.Entries.First().Key);
    }

    public static Task WriteResponse2(HttpContext context, HealthReport healthReport)
    {
        context.Response.ContentType = "application/json; charset=utf-8";

        var options = new JsonWriterOptions { Indented = true };

        using var memoryStream = new MemoryStream();
        using (var jsonWriter = new Utf8JsonWriter(memoryStream, options))
        {
            jsonWriter.WriteStartObject();
            jsonWriter.WriteString("status", healthReport.Status.ToString());
            jsonWriter.WriteStartObject("results");

            foreach (var healthReportEntry in healthReport.Entries)
            {
                jsonWriter.WriteStartObject(healthReportEntry.Key);
                jsonWriter.WriteString("status", healthReportEntry.Value.Status.ToString());
                jsonWriter.WriteString("description", healthReportEntry.Value.Description);
                jsonWriter.WriteStartObject("data");

                foreach (var item in healthReportEntry.Value.Data)
                {
                    jsonWriter.WritePropertyName(item.Key);

                    JsonSerializer.Serialize(jsonWriter, item.Value, item.Value?.GetType() ?? typeof(object));
                }

                jsonWriter.WriteEndObject();
                jsonWriter.WriteEndObject();
            }

            jsonWriter.WriteEndObject();
            jsonWriter.WriteEndObject();
        }

        return context.Response.WriteAsync(Encoding.UTF8.GetString(memoryStream.ToArray()));
    }
}

//-15 Entity Framework Core DbContext probe
public class SampleDbContext : DbContext
{
    public SampleDbContext(DbContextOptions<SampleDbContext> options) : base(options)
    {
    }
}