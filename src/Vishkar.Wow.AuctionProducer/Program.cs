using System;
using ArgentPonyWarcraftClient;
using ArgentPonyWarcraftClient.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Vishkar.Wow.AuctionProducer.Services;
using Vishkar.Wow.Core;

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
        string clientId = ctx.Configuration.GetSection("VishWow_ClientId").Value ?? "";
        string clientSecret = ctx.Configuration.GetSection("VishWow_ClientSecret").Value ?? "";

        services.AddWarcraftClients(clientId, clientSecret);
      }

      // setup kafka
      string kafkaIniFilePath = ctx.Configuration.GetSection("VishkarKafkaIniFilePath").Value ?? "";
      if (string.IsNullOrWhiteSpace(kafkaIniFilePath))
      {
        kafkaIniFilePath = System.IO.Path.Combine(Environment.CurrentDirectory, "vishkar-kafka.props");
      }
      if (!File.Exists(kafkaIniFilePath)) throw new FileNotFoundException($"Unable to find Kafak configuration file '{kafkaIniFilePath}'!", kafkaIniFilePath);
      var kafkaBuilder = new KafkaConfigBuilder
      {
        ConfigIniFilePath = kafkaIniFilePath
      };

      services
        .AddTransient<IEcology, Ecology>()
        .AddSingleton<IKafkaConfigBuilder>(kafkaBuilder)
        .AddTransient<IQueueProducerService<string>, KafkaQueueProducerService<string>>()
        .AddTransient<WowRunner>();
    }
  }
}