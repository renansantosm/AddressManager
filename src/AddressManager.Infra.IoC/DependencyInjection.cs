using AddressManager.Application.Interfaces;
using AddressManager.Application.Services;
using AddressManager.Application.Validators;
using AddressManager.Domain.Interfaces;
using AddressManager.Infra.Data.Context;
using AddressManager.Infra.Data.ExternalServices;
using AddressManager.Infra.Data.Repositories;
using AddressManager.Infra.Data.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AddressManager.Infra.IoC;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
            b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        // Repository
        services.AddScoped<IAddressRepository, AddressRepository>();

        // Unit Of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Fluent Validation
        //services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);
        services.AddValidatorsFromAssembly(typeof(CreateAddressDtoValidator).Assembly);
        services.AddValidatorsFromAssembly(typeof(UpdateAddressDtoValidator).Assembly);


        // HttpClient
        services.AddHttpClient<IViaCepClient, ViaCepClient>(client =>
        {
            client.BaseAddress = new Uri("https://viacep.com.br/");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        // Services
        services.AddScoped<IAddressService, AddressService>();

        return services;
    }
}
