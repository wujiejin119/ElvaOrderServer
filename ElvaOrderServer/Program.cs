using ElvaOrderServer.API.Middleware;
using ElvaOrderServer.Application.Services;
using ElvaOrderServer.Infrastructure.Persistence;
using ElvaOrderServer.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddDbContext<OrdersDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
      
        builder.Services.AddScoped<IOrderRepository, OrderRepository>();
        builder.Services.AddScoped<IOrderService, OrderService>();
        builder.Services.AddAutoMapper(typeof(Program));

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();

        app.UseMiddleware<ExceptionHandlingMiddleware>();

        app.MapControllers();

        app.Run();

    }
}
