using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using AsyncAuthFlowCore;
using AsyncAuthFlowCore.Abstractions;
using LabelLocker;
using LabelLocker.EFCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scim.ExternalServices;
using ScimServe;
using Serilog;
using Serilog.Events;
using ScimServe.Endpoints;
using ScimServe.EFCoreRepositories;
using ScimServe.Services;
using ScimServe.WebAPI.DocumentFilter;
using ScimServe.WebAPI.Middleware;
using ScimServe.WebAPI.Properties;
using ScimServe.WebAPI.SchemaFilters;


var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

// Configure UseCases
builder.Services.AddScimServe();
builder.Services.AddScoped<IPasswordHashingService, PasswordHashingService>();
builder.Services.AddLabelManagementServices();

builder.Services.AddLabelManagementEntityFrameworkRepositories(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("ScimDatabase") 
                           ?? throw new InvalidOperationException("The SQL Server connection string 'ScimDatabase' is not configured.");

    options.UseSqlServer(connectionString,
        b => b.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName));
});

Array.Empty<JsonContent>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],

            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],

            ValidateLifetime = true,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),

            RequireExpirationTime = true,
            ClockSkew = TimeSpan.Zero  // Optional: Reduce or eliminate clock skew tolerance
        };
    });
builder.Services.AddAuthorization();

// Add Endpoints
builder.Services.ConfigureEndpoints();

//Configure EF Core
builder.Services.ConfigureEfCore(builder.Configuration,  Assembly.GetExecutingAssembly());

// Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.DocumentFilter<ExcludeTypesDocumentFilter>();
    options.SchemaFilter<UserCredentialsSchemaFilter>();
    options.SchemaFilter<SeedUserSchemaFilter>();
    options.SchemaFilter<PresentableUserSchemaFilter>();
    options.SchemaFilter<TokenInfoSchemaFilter>();

    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo 
    { 
        Title = "SCIM 2.0 API", 
        Version = "v1",
        Description = "WORK IN PROGRESS - SCIM 2.0 API",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Richard Doucet",
            Email = "rick@richard-doucet.com",
            Url = new Uri("https://richard-doucet.com"),
        },
        License = new Microsoft.OpenApi.Models.OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://localhost:7140/scim/LICENSE"),
        }
        
    });
    
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\""
    });
    
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
    
    // Set the comments path for the Swagger JSON and UI.
    var xmlFile = $"{typeof(EndpointsInstaller).Assembly.GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
    options.EnableAnnotations();
});

builder.Services.AddScoped<IUserPermissionsRepository, UserRepository>();
builder.Services.AddScoped<IUserContextConfigurator>(serviceProvider =>
{
    // Use serviceProvider to resolve any dependencies required for creating IUserContextConfigurator
    var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
    var userPermissionsRepository = serviceProvider.GetRequiredService<IUserPermissionsRepository>();

    // Assuming you have logic to determine the userId from the current HTTP context
    var userId = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;


    // Ensure there's a user ID available from the current context
    if (string.IsNullOrEmpty(userId))
    {
        throw new InvalidOperationException("Cannot create IUserContextConfigurator without a user context.");
    }

    // Use your concrete implementation of IUserContextConfigurator, configured for the current user
    var userContextConfigurator = UserContext.Create(userPermissionsRepository, userId);

    return userContextConfigurator;
});



var app = builder.Build();
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.Use(async (context, next) =>
{
    // Check if the ContentType is "application/json+scim"
    if (context.Request.ContentType == "application/json+scim")
    {
        // Override the ContentType to "application/json"
        context.Request.ContentType = "application/json";
    }

    await next();
});


app.MapCreateUserEndpoints();
app.MapLoginEndpoints();
app.MapGetUserEndpoint();
app.MapSeedSystemUserEndpoints();
app.MapDeleteUserEndpoints();

app.UseSwagger(options =>
{
    options.RouteTemplate = "scim/api-docs/{documentName}/swagger.json";
});

app.UseSwaggerUI(options => 
{
    options.SwaggerEndpoint("/scim/api-docs/v1/swagger.json", "SCIM 2.0 V1");
    options.RoutePrefix = "scim/api-docs";
    options.DocumentTitle = "SCIM 2.0 API";
    // Inject custom CSS to change the banner color
    options.HeadContent = @"
        <style>
            .swagger-ui .topbar { background-color: #708090 !important; }
            .swagger-ui .topbar .logo img {
                content:url('scimserve-logo.jpg');
            }
            .swagger-ui { background-color: #cfd8dc !important; }
            body { background-color: #708090 !important; }
        </style>
    ";
    
    options.InjectJavascript("favicon-swapper.js");
});

app.MapGet("/scim/LICENSE", async context =>
{
    var stream = new MemoryStream(Resources.LICENSE);
    context.Response.ContentType = "text/plain"; // Adjust as needed
    await stream.CopyToAsync(context.Response.Body);
});

app.MapGet("scim/favicon-32x32.png", async context =>
{
    var stream = new MemoryStream(Resources.favicon_32x32);
    context.Response.ContentType = "image/png"; // Adjust as needed
    await stream.CopyToAsync(context.Response.Body);
});

app.MapGet("scim/api-docs/favicon-swapper.js", async context =>
{
    var stream = new MemoryStream(Encoding.UTF8.GetBytes(Resources.favicon_swapper));
    context.Response.ContentType = "text/javascript"; // Adjust as needed
    await stream.CopyToAsync(context.Response.Body);
});


app.Run();

