using System;
using ArgentPonyWarcraftClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Vishkar.Wow.AuctionEnhancer.Services;
using Vishkar.Wow.Core;
using Vishkar.Wow.Core.Extensions.DependencyInjection;

namespace Vishkar.Wow.AuctionEnhancer
{
  internal class Program
  {
    static int Main(string[] args)
    {
      var host = Host.CreateDefaultBuilder(args)
        .ConfigureServices(DiConfig)
        .Build();

      return host.Services
        .GetService<RawItemWatcherService>()
        .ExecuteAsync()
        .Result;
    }

    private static void DiConfig(HostBuilderContext ctx, IServiceCollection services)
    {
      services.ConfigureWarcraftClientSecrets(ctx.Configuration);
      services.ConfigureWowSettings(ctx.Configuration);

      services
        .AddKafka(ctx.Configuration)
        .AddWowCore()
        .AddSingleton<IItemApi, CachedWarcraftItemApi>()
        .AddTransient<IQueueProducerService<string>, KafkaQueueProducerService<string>>()
        .AddTransient<RawItemWatcherService>();
    }
  }
}