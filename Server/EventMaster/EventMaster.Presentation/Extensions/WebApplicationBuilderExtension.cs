using System.Text;
using EventMaster.BLL.Infrastructure.Mapper;
using EventMaster.BLL.Infrastructure.Validators;
using EventMaster.BLL.Services.Implementation;
using EventMaster.BLL.Services.Interfaces;
using EventMaster.DAL.Infrastructure;
using EventMaster.DAL.Infrastructure.Database;
using EventMaster.DAL.Repositories.Implementations;
using EventMaster.DAL.Repositories.Interfaces;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace EventMaster.Extensions;

public static class WebApplicationBuilderExtension
{
    public static void AddSwaggerDocumentation(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    Description = @"Enter JWT Token please.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                }
            );
            options.AddSecurityRequirement(
                new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                        },
                        new List<string>()
                    }
                }
            );
        });
    }
    public static void AddMapping(this WebApplicationBuilder builder)
    {
        builder.Services.AddAutoMapper(
            typeof(EventProfile).Assembly,
            typeof(UserProfile).Assembly,
            typeof(EventCategoryProfile).Assembly,
            typeof(RoleProfile).Assembly,
            typeof(ParticipantProfile).Assembly
        );
    }

    public static void AddAuthentication(this WebApplicationBuilder builder)
    {
        var jwtSettings = builder.Configuration.GetSection("Jwt");

        var key = Encoding.UTF8.GetBytes(jwtSettings["Secret"]);
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(key),
            };
        });
        builder.Services.AddAuthorization(options => options.DefaultPolicy =
            new AuthorizationPolicyBuilder
                    (JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build());
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminArea", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole("Admin");
            });
        });
    }
    public static void AddDatabase(this WebApplicationBuilder builder)
    {
        string? connectionString = builder.Configuration.GetConnectionString("ConnectionString");
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });
    }

    public static void AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<IEmailService, EmailService>();
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<IRoleService,RoleService>();
        builder.Services.AddScoped<IEventService,EventService>();
        builder.Services.AddScoped<IUserService,UserService>();
        builder.Services.AddScoped<IEventCategoryService,EventCategoryService>();
        builder.Services.AddScoped<IParticipantService,ParticipantService>();
        
        builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();
        
        builder.Services.AddScoped<IEventCategoryRepository,EventCategoryRepository>();
        builder.Services.AddScoped<IEventRepository,EventRepository>();
        builder.Services.AddScoped<IUserRepository,UserRepository>();
        builder.Services.AddScoped<IParticipantRepository,ParticipantRepository>();
        builder.Services.AddScoped<IRoleRepository,RoleRepository>();
        
        builder.Services.AddControllers(); 
        
    }

    public static void AddValidation(this WebApplicationBuilder builder)
    {
        builder
            .Services.AddFluentValidationAutoValidation()
            .AddFluentValidationClientsideAdapters();
        builder.Services.AddValidatorsFromAssemblyContaining<EventFilterDTOValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<CreateEventDTOValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<CreateParticipantDTOValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<TokenDTOValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<UpdateEventDTOValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<UserDTOValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<UserRoleDTOValidator>();
    }

    public static void AddCache(this WebApplicationBuilder builder)
    {
        builder.Services.AddMemoryCache();
    }
}