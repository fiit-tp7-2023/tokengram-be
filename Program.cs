using FluentValidation.AspNetCore;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Neo4jClient;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Tokengram.Services.Interfaces;
using Tokengram.Services;
using Tokengram.Database.Tokengram;
using Microsoft.EntityFrameworkCore;
using Tokengram.Models.Config;
using System.Text;
using Tokengram.Middlewares;
using Tokengram.Hubs;
using Tokengram.Models.Hubs;
using Tokengram.Models.Mappings;
using Microsoft.AspNetCore.SignalR;
using Tokengram.Database.Indexer;
using AutoMapper;

namespace Tokengram
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.SetBasePath(builder.Environment.ContentRootPath);
            builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            builder.Configuration.AddJsonFile(
                $"appsettings.{builder.Environment.EnvironmentName}.json",
                optional: true
            );
            builder.Configuration.AddEnvironmentVariables();

            // Db context configuration
            builder.Services.AddDbContext<TokengramDbContext>(
                options => options.UseNpgsql(builder.Configuration.GetConnectionString("TokengramDatabase"))
            );
            builder.Services.AddDbContext<IndexerDbContext>(
                options => options.UseNpgsql(builder.Configuration.GetConnectionString("IndexerDatabase"))
            );

            // Services configuration
            var jwtOptionsSection = builder.Configuration.GetRequiredSection("JWT");
            builder.Services.Configure<JWTOptions>(jwtOptionsSection);

            if (builder.Environment.IsProduction() && builder.Configuration["DeploymentEnv"] != "AZURE")
            {
                builder.Services
                    .AddDataProtection()
                    .PersistKeysToFileSystem(new DirectoryInfo(builder.Configuration["DataProtection:StoragePath"]!))
                    .ProtectKeysWithCertificate(
                        new X509Certificate2(Convert.FromBase64String(builder.Configuration["DataProtection:Key"]!))
                    );
            }

            builder.Services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = true;
            });

            // Add services to the container.
            builder.Services.AddSingleton(provider =>
                new MapperConfiguration(cfg =>
                    {
                        cfg.AddProfile(new ChatInvitationProfile());
                        cfg.AddProfile(new ChatMessageProfile());
                        cfg.AddProfile(new ChatProfile());
                        cfg.AddProfile(new UserProfile(builder.Configuration));
                        cfg.AddProfile(new NFTProfile());
                        cfg.AddProfile(new CommentProfile());
                        cfg.AddProfile(new CommentLikeProfile());
                        cfg.AddProfile(new PostLikeProfile());
                    }
                ).CreateMapper());

            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IChatService, ChatService>();
            builder.Services.AddScoped<INFTService, NFTService>();
            builder.Services.AddScoped<IPostService, PostService>();
            builder.Services.AddScoped<ICommentService, CommentService>();
            builder.Services.AddScoped<IProfileService, ProfileService>();
            builder.Services.AddSingleton<List<ConnectedUser>>();
            builder.Services.AddSingleton<List<ChatGroup>>();

            builder.Services.AddControllers();
            builder.Services.AddSignalR(options =>
            {
                options.AddFilter<ValidationHubFilter>();
            });

            var neo4jUri = builder.Configuration["Neo4j:Uri"];
            var neo4jUsername = builder.Configuration["Neo4j:Username"];
            var neo4jPassword = builder.Configuration["Neo4j:Password"];
            var neo4jClient = new BoltGraphClient(new Uri(neo4jUri!), neo4jUsername, neo4jPassword);
            neo4jClient.ConnectAsync();
            builder.Services.AddSingleton<IGraphClient>(neo4jClient);

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme()
                    {
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.Http,
                        Scheme = "Bearer"
                    }
                );
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
                c.AddSecurityRequirement(
                    new OpenApiSecurityRequirement()
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference { Id = "Bearer", Type = ReferenceType.SecurityScheme }
                            },
                            new List<string>()
                        }
                    }
                );
            });

            // Fluent validation
            builder.Services.AddValidatorsFromAssemblyContaining<Program>();
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddFluentValidationClientsideAdapters();

            builder.Services
                .AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false,
                        ValidateIssuer = true,
                        ValidIssuer = jwtOptionsSection["ValidIssuer"],
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.ASCII.GetBytes(jwtOptionsSection["Secret"]!)
                        ),
                        ValidateLifetime = true,
                    };
                });

            builder.Services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.MapHub<ChatHub>(
                "/hubs/chat",
                options =>
                {
                    options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets;
                }
            );

            app.UseMiddleware<ExceptionHandlerMiddleware>();

            app.Run();
        }
    }
}
