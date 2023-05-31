using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
        .Execute()
        .Result;
    }

    private static void DiConfig(HostBuilderContext ctx, IServiceCollection services)
    {
      // TODO add wow Client..

      services
        .AddKafka(ctx.Configuration)
        .AddWowCore()
        .AddTransient<IQueueProducerService<string>, KafkaQueueProducerService<string>>()
        .AddTransient<RawItemWatcherService>();
    }
  }
}