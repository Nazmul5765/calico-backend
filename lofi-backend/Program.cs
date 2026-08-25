using lofi_backend.Database;
using lofi_backend.HealthChecks;
using lofi_backend.Repository;
using lofi_backend.Service;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Supabase;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Memory;

namespace lofi_backend
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.SetBasePath(AppContext.BaseDirectory);
            builder.Configuration.AddUserSecrets<Program>();

            var supabaseUrl = builder.Configuration["Supabase:Url"]!;
            var supabaseKey = builder.Configuration["Supabase:Key"]!;
            var options = new SupabaseOptions
            {
                AutoRefreshToken = false,
                AutoConnectRealtime = false
            };

            using var httpClient = new HttpClient();
            var jwksJson = await httpClient.GetStringAsync($"{supabaseUrl}/auth/v1/.well-known/jwks.json");
            var jwks = new JsonWebKeySet(jwksJson);
            var validIssuers = supabaseUrl + "/auth/v1";
            List<string> validAudiences = ["authenticated"];

            builder.Services.AddAuthorization();

            builder.Services.AddAuthentication().AddJwtBearer(o =>
            {
                o.MapInboundClaims = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKeys = jwks.GetSigningKeys(),
                    ValidateIssuer = true,
                    ValidIssuer = validIssuers,
                    ValidateAudience = true,
                    ValidAudiences = ["authenticated"],
                    ValidateLifetime = true
                };
                o.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context => 
                    { 
                        if (context.Request.Cookies.TryGetValue("jwt", out var token))
                        {
                            context.Token = token;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            builder.Services.AddScoped(provider =>
                new Client(supabaseUrl, supabaseKey, options));

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddHttpClient();
            builder.Services.AddScoped<IYoutubeRepository, YoutubeRepository>();
            builder.Services.AddScoped<IYoutubeService, YoutubeService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserService, UserService>();

            builder.Services.AddScoped<ITaskTimerRepository, TaskTimerRepository>();
            builder.Services.AddScoped<ITaskTimerService, TaskTimerService>();

            builder.Services.AddScoped<IPlaylistRepository, PlaylistRepository>();
            builder.Services.AddScoped<IPlaylistService, PlaylistService>();
            builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
            builder.Services.AddScoped<IProjectService, ProjectService>();
            builder.Services.AddScoped<IMusicRepository, MusicRepository>();
            builder.Services.AddScoped<IMusicService, MusicService>();
            builder.Services.AddMemoryCache();

            builder.Services.AddHealthChecks().AddCheck<ApiHealthCheck>("api_health_check",
                failureStatus: HealthStatus.Unhealthy, tags: new[] { "api", "users" }).AddCheck<DatabaseHealthCheck>("database_health_check",
                failureStatus: HealthStatus.Unhealthy, tags: new[] {"database", "users" });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<LoFiDbContext>(options =>
            {
                var _connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") is "Development")
                {
                    var connection = new SqliteConnection(_connectionString);
                    connection.Open();
                    options.UseSqlite(connection);
                }
                else
                {
                    Console.WriteLine($"Connection: ${_connectionString}");
                    options.UseSqlServer(_connectionString);
                }
            });
           
            var app = builder.Build();

            using (IServiceScope scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<LoFiDbContext>();

                if (app.Environment.IsDevelopment()) db.Database.EnsureCreated();
                else db.Database.Migrate();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapHealthChecks("/health");


            app.MapControllers();

            app.Run();
        }
    }
}
