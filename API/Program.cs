using System;
using API.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var host = CreateHostBuilder(args).Build();

      // Instead of an immediate .Run() after code above, we first make sure the scope has the required StoreContext
      using var scope = host.Services.CreateScope();
      var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
      // setting up a logger on the scope too. Logs exceptions to the terminal.
      var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
      try
      {
        context.Database.Migrate();
        // adding context so it can add our products to the application 
        DbInitializer.Initialize(context);
      }
      catch (Exception ex)
      {
        logger.LogError(ex, "Problem migrating data");
      }
      //   finally
      //   {
      //     // This triggers the garbage collecting, but instead we use using above to make it drop from memory as soon as done with
      //     scope.Dispose();
      //   }

      host.Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
              webBuilder.UseStartup<Startup>();
            });
  }
}
