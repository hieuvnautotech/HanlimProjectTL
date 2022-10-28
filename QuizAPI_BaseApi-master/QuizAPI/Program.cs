using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QuizAPI.DbAccess;
using QuizAPI.Extensions;
using QuizAPI.Middlewares;
using QuizAPI.Services.Common;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;
using System.Text;

//var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

////Add services to the container.
//builder.Services.AddSingleton<ISqlDataAccess, SqlDataAccess>();
//builder.Services.AddScoped<IPersonService, PersonService>();
//builder.Services.AddTransient<IUserInfoService, UserInfoService>();

////Auto adding services to the container.
builder.Services.RegisterServices(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsApi",
        builder => builder.WithOrigins("http://localhost:3001", "http://hl.autonsi.com")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

builder.Services.AddControllers().AddJsonOptions(opt =>
{
    opt.JsonSerializerOptions.PropertyNamingPolicy = null;
})
//.AddFluentValidation(options =>
//            {
//                // Validate child properties and root collection elements
//                options.ImplicitlyValidateChildProperties = true;
//                options.ImplicitlyValidateRootCollectionElements = true;

//                // Automatic registration of validators in assembly
//                options.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
//            })
;


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

/// <summary>
/// Config Authorization for Swagger
/// </summary>
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "JWT", Version = "v1" });

    var securitySchema = new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    options.AddSecurityDefinition("Bearer", securitySchema);

    var securityRequirement = new OpenApiSecurityRequirement {
        { securitySchema, new[] { "Bearer" } }
    };

    options.AddSecurityRequirement(securityRequirement);

    //options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
    //{
    //    Description = "Standard Authorization header using the Bearer scheme (\"bearer {token}\")",
    //    Name = "Authorization",
    //    In = ParameterLocation.Header,
    //    Type = SecuritySchemeType.Http,
    //    Scheme = "bearer",
    //    Reference = new OpenApiReference
    //    {
    //        Type = ReferenceType.SecurityScheme,
    //        Id = "Bearer"
    //    }
    //});

    //options.OperationFilter<SecurityRequirementsOperationFilter>();
});

/// <summary>
/// Add Authentication
/// </summary>
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
//{
//    //string key = builder.Configuration.GetSection("Jwt:Key").Value;
//    opt.TokenValidationParameters = new TokenValidationParameters
//    {
//        //ValidateLifetime = true,

//        ValidateIssuer = true,
//        ValidIssuer = ConnectionString.ISSUER,

//        ValidateAudience = true,
//        ValidAudience = ConnectionString.AUDIENCE,

//        ValidateIssuerSigningKey = true,
//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConnectionString.SECRET)),
//        ClockSkew = TimeSpan.Zero
//    };
//});

builder.Services.AddMemoryCache();
builder.Services.AddHostedService<InitializeCacheService>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddFluentValidationAutoValidation(config =>
{
    config.DisableDataAnnotationsValidation = true;
});

var app = builder.Build();

app.UseMiddleware<JwtMiddleware>();



////config CORS
app.UseCors("CorsApi");
//app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().WithExposedHeaders("access-token", "refresh-token", "content-type"));

//var path = Path.Combine(app.Environment.ContentRootPath, "Upload");
//if (!Directory.Exists(path))
//{
//    Directory.CreateDirectory(path);
//}

//app.UseStaticFiles(new StaticFileOptions
//{
//    RequestPath = "/VersionApp",
//    FileProvider = new PhysicalFileProvider(path)
//});

// Configure the HTTP request pipeline.
app.UseSwagger();
if (app.Environment.IsProduction())
{
    app.UseSwaggerUI(c =>
    {
        c.DefaultModelsExpandDepth(-1); // Disable swagger schemas at bottom
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Jwt v1");
        c.RoutePrefix = string.Empty;
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
    });
}
else
{
    app.UseSwaggerUI(c =>
    {
        c.DefaultModelsExpandDepth(-1); // Disable swagger schemas at bottom
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Jwt v1");
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
    });
}

//app.UseAuthentication();

//app.UseAuthorization();

app.UseStaticFiles();

app.MapControllers();

app.Run();
