using AddressManager.Infra.Data.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AddressManager.Infra.Data.Extensions;

public static class DatabaseExtensions
{
    public static void AddDatabase(this IApplicationBuilder app)
    {
        using (var scope = app.ApplicationServices.CreateScope())
        {
            var serviceDb = scope.ServiceProvider.GetService<AppDbContext>();

            serviceDb?.Database.Migrate();
        }
    }
}
