using Microsoft.EntityFrameworkCore;
using TcgPlatformApi.Data;
using TcgPlatformApi.Services;

namespace TcgPlatformApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //for appsettings.json
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            //db string
            builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddScoped<IPlayerProfileService, PlayerProfileService>();
            builder.Services.AddScoped<IRegVerLogService, RegVerLogService>();
            builder.Services.AddScoped<IAvatarService, AvatarService>();
            builder.Services.AddScoped<IPlayerBoosterService, PlayerBoosterService>();
            builder.Services.AddScoped<IPlayerCardService, PlayerCardService>();
            builder.Services.AddScoped<IPlayerDeckService, PlayerDeckService>();
            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}

