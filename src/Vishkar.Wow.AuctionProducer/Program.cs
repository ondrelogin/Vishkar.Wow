using System;
using ArgentPonyWarcraftClient;
using ArgentPonyWarcraftClient.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Vishkar.Wow.AuctionProducer.Services;
using Vishkar.Wow.Core;
using Vishkar.Wow.Core.Extensions.DependencyInjection;

namespace Vishkar.Wow.AuctionProducer
{
  internal class Program
  {
    static int Main(string[] args)
    {
      var host = Host.CreateDefaultBuilder(args)
        .ConfigureServices(DiConfig)
        .Build();

      return host.Services
        .GetService<WowRunner>()
        .Execute()
        .Result;
    }

    private static void DiConfig(HostBuilderContext ctx, IServiceCollection services)
    {
      // register wow client..
      string useWowCache = ctx.Configuration.GetSection("UseWowCache").Value ?? "";
      if (useWowCache.EqualsAnyCase("true"))
      {
        string cacheFolderPath = ctx.Configuration.GetSection("WowCacheFolderPath").Value ?? "";
        if (string.IsNullOrWhiteSpace(cacheFolderPath)) { cacheFolderPath = Environment.CurrentDirectory; }

        var wowSettings = new CachedWowSettings
        {
          CachedFolder = cacheFolderPath
        };
        services
          .AddSingleton<CachedWowSettings>(wowSettings)
          .AddTransient<IAuctionHouseApi, CachedAutionHouseApi>();
      }
      else
      {
        var secret = services.ConfigureWarcraftClientSecrets(ctx.Configuration);
        services.AddWarcraftClients(secret.ClientId, secret.ClientSecret);
      }

      services
        .AddKafka(ctx.Configuration)
        .AddWowCore()
        .AddTransient<IQueueProducerService<string>, KafkaQueueProducerService<string>>()
        .AddTransient<WowRunner>();
    }
  }
}