using System;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Vishkar.Wow.Core;
using Vishkar.Wow.Core.Extensions.DependencyInjection;

namespace Vishkar.Wow.CommodityWatcher
{
  public class Program
  {
    public async static Task Main(string[] args)
    {
      await Host.CreateDefaultBuilder(args)
        .ConfigureServices(DiConfig)
        .RunConsoleAsync();
    }

    private static void DiConfig(HostBuilderContext ctx, IServiceCollection services)
    {
      services.ConfigureWowSettings(ctx.Configuration);

      services
        .AddKafka(ctx.Configuration)
        .AddWowCore()
        .AddHostedService<CommodityHostedService>();
    }
  }
}