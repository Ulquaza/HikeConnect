
namespace HikeConnect.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddRouting(options => options.LowercaseUrls = true);
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowWebApp", policy =>
                {
                    policy
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .WithOrigins(origins!);
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
            app.UseCors("AllowWebApp");
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
