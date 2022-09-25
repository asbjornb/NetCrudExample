using DataAccess;
using DataAccess.EmployeeRepository;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;

namespace DemoApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        //Add swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        //Register services
        var connectionString = builder.Configuration.GetConnectionString("AppDatabase");
        builder.Services.AddSingleton<IDatabaseProvider>(_ => new DatabaseProvider(connectionString));
        builder.Services.AddSingleton<IEmployeeRepository, EmployeeSqlRepository>();
        builder.Services.AddSingleton<IEmployeeValidator, EmployeeValidator>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.ConfigureApi();
        LogManager.GetCurrentClassLogger().Info("Api configured. Ready to receive requests");
        app.Run();
    }
}
