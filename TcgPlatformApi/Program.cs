using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using TcgPlatformApi.Data;
using TcgPlatformApi.Middleware;
using TcgPlatformApi.Services;
using TcgPlatformApi.Settings;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using TcgPlatformApi.Swagger;

namespace TcgPlatformApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var builder = WebApplication.CreateBuilder(args);

                //for appsettings.json
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

                //smtp
                builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

                //db string
                builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

                //jwt
                builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

                var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

                builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtSettings.Issuer,

                        ValidateAudience = true,
                        ValidAudience = jwtSettings.Audience,

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),

                        ValidateLifetime = true, 
                        ClockSkew = TimeSpan.Zero 
                    };
                });

                builder.Services.AddControllers();
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();
                builder.Services.AddScoped<IPlayerProfileService, PlayerProfileService>();
                builder.Services.AddScoped<IRegVerLogService, RegVerLogService>();
                builder.Services.AddScoped<IAvatarService, AvatarService>();
                builder.Services.AddScoped<IPlayerBoosterService, PlayerBoosterService>();
                builder.Services.AddScoped<IPlayerCardService, PlayerCardService>();
                builder.Services.AddScoped<IPlayerDeckService, PlayerDeckService>();
                builder.Services.AddScoped<IEmailService, EmailService>();
                builder.Services.AddScoped<ITokenService, TokenService>();

                builder.Services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TcgPlatformAPI", Version = "v1" });

                    c.AddSecurityDefinition("JWT", new OpenApiSecurityScheme
                    {
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        Description = "Enter your token",
                    });

                    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Id = "JWT",
                                    Type = ReferenceType.SecurityScheme
                                }
                            },
                            new string[] {}
                        }
                    });

                    c.OperationFilter<FileUploadOperationFilter>();
                    c.SchemaFilter<FormFileSchemaFilter>();
                });

                var app = builder.Build();

                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }
             
                app.UseStaticFiles();
                app.UseHttpsRedirection();
                app.UseAuthentication(); //jwt
                app.UseAuthorization();
                app.UseMiddleware<ExceptionMiddleware>();
                app.MapControllers();
                app.Run();
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Application failed to start: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                throw;
            }   
        }
    }
}

