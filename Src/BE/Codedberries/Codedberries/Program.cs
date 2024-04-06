
using Codedberries.Environment;
using Codedberries.Services;
using Microsoft.Extensions.Configuration;

using Codedberries.Helpers;
using Codedberries.Services;


namespace Codedberries
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddDbContext<AppDatabaseContext>();
            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<TokenService>();
            builder.Services.Configure<Config>(builder.Configuration.GetSection(nameof(Config)));
            builder.Services.AddSingleton<Config>();
            builder.Services.AddScoped<AuthorizationService>();
            builder.Services.AddScoped<ProjectService>();
            builder.Services.AddScoped<RoleService>();
            builder.Services.AddScoped<TaskService>();
            builder.Services.AddScoped<StatusService>();
            builder.Services.AddScoped<CategoryService>();
            builder.Services.AddScoped<PriorityService>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAnyOrigin", builder2 =>
                {
                    builder2.WithOrigins(builder.Configuration.GetValue<string>("Config:FrontendURL"))
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .AllowCredentials();
                });
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

            app.UseCors("AllowAnyOrigin");

            //app.UseMvc();

            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDatabaseContext>();
                dbContext.ApplyMigrations();
                 //dbData.addData1(dbContext);
                dbData.addData2(dbContext);
            }

            app.Run();
        }
    }
}
