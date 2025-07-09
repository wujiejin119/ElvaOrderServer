using ElvaOrderServer.API.Exceptions;
using ElvaOrderServer.Application.Services;
using ElvaOrderServer.Infrastructure.Persistence;
using ElvaOrderServer.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Snowflake.Net;

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
        var machineId = builder.Configuration.GetValue<int>("SnowflakeSettings:MachineId", 1);
        var datacenterId = builder.Configuration.GetValue<int>("SnowflakeSettings:DatacenterId", 1);

        builder.Services.AddSingleton(new IdWorker(machineId, datacenterId));

        var app = builder.Build();

        app.UseMiddleware<ExceptionHandlingMiddleware>();
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        //app.UseRouting();

        
        app.UseAuthorization();

        app.MapControllers();

        app.Run();

    }
}
