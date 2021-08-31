using Liberis.OrchestrationHub.Application.Services;
using Liberis.OrchestrationHub.Application.Validation;
using Liberis.OrchestrationHub.Core.Interfaces;
using Liberis.OrchestrationHub.Core.Options;
using FluentValidation.AspNetCore;
using Gelf.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scrutor;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using Liberis.OrchestrationHub.Application.Consumer;
using MongoDB.Driver;
using MassTransit;
using RabbitMQ.Client;
using Liberis.OrchestrationHub.Messages.V1;
using Liberis.OrchestrationHub.Messages.V1.Advert;
using Liberis.OrchestrationHub.Application.Providers;
using Liberis.OrchestrationAdapter.Messages.V1;
using Liberis.OrchestrationHub.Application.Repository;
using MongoDB.Bson.Serialization.Conventions;
using Liberis.OrchestrationAdapter.Messages.V1.Advert;

namespace Liberis.OrchestrationHub.Application
{
    public class Startup
    {
        private const string NginxServiceNameRequiredError = "Please configure NGINXServiceName in appsettings.secrets.json";

        // TODO: Modify this to provide a general description of your API
        private static readonly OpenApiInfo SwaggerInfo = new OpenApiInfo
        {
            Version = "v1",
            Title = "Liberis Base API",
            Description = "Template API to use as a base for building APIs with business logic",
            Contact = new OpenApiContact
            {
                Name = "Liberis Tech Team",
                Email = "devteam@liberis.co.uk",
                Url = new Uri("https://www.liberis.co.uk")
            },
            License = new OpenApiLicense
            {
                Name = "TODO: Decide on a license",
                Url = new Uri("https://www.liberis.co.uk")
            }
        };

        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddJsonFile("secrets/appsettings.secrets.json", optional: false, reloadOnChange: true);

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets(Assembly.Load(new AssemblyName(env.ApplicationName)));
            }

            builder.AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Logging:
            void ConfigureGelf(ILoggingBuilder loggingBuilder) =>
                loggingBuilder.AddGelf(options =>
                {
                    var gelfOptions = new GelfOptions();
                    Configuration.GetSection(GelfOptions.Options).Bind(gelfOptions);

                    var assembly = Assembly.GetEntryAssembly();

                    options.Host = gelfOptions.Host;
                    options.Port = gelfOptions.Port;
                    options.Protocol = gelfOptions.Protocol;
                    options.LogSource = gelfOptions.Source ?? assembly?.GetName().Name;
                    options.AdditionalFields["machine_name"] = Environment.MachineName;
                    options.AdditionalFields["app_version"] = assembly
                        ?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                        .InformationalVersion;
                });

            services.AddLogging(ConfigureGelf);

            // Options:
            services.Configure<ApiOptions>(Configuration.GetSection(ApiOptions.Example));
            services.Configure<MessageBrokerOptions>(Configuration.GetSection(MessageBrokerOptions.Options));

            // Security:
            var key = Encoding.ASCII.GetBytes(Configuration["SecurityOptions:IssuerSigningKey"]);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(x =>
                {
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = true,
                        ValidIssuer = Configuration["SecurityOptions:ValidIssuer"],
                        ValidateAudience = true,
                        ValidAudience = Configuration["SecurityOptions:ValidAudience"]
                    };
                });

            //Mongodb configurations
            services.AddSingleton<IMongoDatabase>(c =>
            {
                var mongoDBOptions = new MongoDBOptions();
                Configuration.GetSection(MongoDBOptions.Options).Bind(mongoDBOptions);
                var mongoClient = new MongoClient(mongoDBOptions.Url);
                //Mongodb Conventions
                var pack = new ConventionPack();
                pack.Add(new IgnoreExtraElementsConvention(true));
                ConventionRegistry.Register("Liberis.OrchestrationHub.Conventions", pack, t => true);

                return mongoClient.GetDatabase(mongoDBOptions.DatabaseName);
            });

            //Services:
            services.Scan(scan => scan
                .FromCallingAssembly()
                .FromAssemblies(
                    typeof(ExampleService).Assembly,
                    typeof(IService).Assembly
                )
                .AddClasses(c => c.InNamespaceOf<ExampleService>())
                .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            );

            services.AddScoped<IExampleService, ExampleService>();
            services.AddScoped<IHubService<GetAdvertRequest>, HubService<GetAdvertRequest>>();
            services.AddScoped<IAdapterNameProvider<GetAdvertRequest>, AdvertAdapterNameProvider>();
            services.AddScoped<IBaseRepository<AdapterResponse<object>>, BaseRepository<object>>();

            services.AddControllers();
            services.AddHttpClient();
            services.AddHealthChecks();

            // Add the OpenAPI/Swagger doc generator
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(SwaggerInfo.Version, SwaggerInfo);
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
            services.AddSwaggerGenNewtonsoftSupport();

            services.AddMvc().AddFluentValidation(
                fv =>
                {
                    fv.ImplicitlyValidateChildProperties = true;
                    fv.RegisterValidatorsFromAssemblyContaining<ExampleRequestValidator>();
                }).AddNewtonsoftJson();

            services.AddDistributedMemoryCache();

            // RabbitMQ
            var messageBrokerOptions = new MessageBrokerOptions();
            Configuration.GetSection(MessageBrokerOptions.Options).Bind(messageBrokerOptions);

            services.AddMassTransit(transitCfg =>
            {
                transitCfg.AddConsumer<AdapterResponseConsumer<object>>();
                transitCfg.UsingRabbitMq((context, rabbitMqCfg) =>
                {
                    rabbitMqCfg.Host(messageBrokerOptions.Host, h =>
                    {
                        h.Username(messageBrokerOptions.Username);
                        h.Password(messageBrokerOptions.Password);
                    });

                    rabbitMqCfg.ReceiveEndpoint("Liberis.OrchestrationHub.Example", ep =>
                    {
                        ep.ConfigureConsumeTopology = false;
                        ep.Bind<AdapterResponse<object>>(c =>
                        {
                            c.ExchangeType = ExchangeType.Direct;
                        });

                        ep.ConfigureConsumer<AdapterResponseConsumer<object>>(context);
                    });

                    rabbitMqCfg.Publish<HubRequest<GetAdvertRequest>>(x => x.ExchangeType = ExchangeType.Topic);
                });
            });

            services.AddMassTransitHostedService();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/api/error");
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks(Configuration["HealthChecks:Liveness"], new HealthCheckOptions
                {
                    Predicate = _ => false,
                    ResponseWriter = HealthCheckFormatter.LivenessResponseAsync
                });

                endpoints.MapHealthChecks(Configuration["HealthChecks:Readiness"], new HealthCheckOptions
                {
                    ResponseWriter = HealthCheckFormatter.ReadinessResponseAsync
                });
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.DocumentTitle = SwaggerInfo.Title;
                c.SwaggerEndpoint("./swagger/v1/swagger.json", $"{SwaggerInfo.Title} {SwaggerInfo.Version}");
                c.RoutePrefix = string.Empty;
            });
        }
    }
}
