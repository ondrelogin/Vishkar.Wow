using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vishkar.Wow.Core.Settings;

namespace Vishkar.Wow.Core.Extensions.DependencyInjection
{
  public static class ServiceCollectionExtensions
  {
    public static ArgentPonySettings ConfigureWarcraftClientSecrets(this IServiceCollection services, IConfiguration config)
    {
      string clientId = config.GetSection("VishWow_ClientId").Value ?? "";
      string clientSecret = config.GetSection("VishWow_ClientSecret").Value ?? "";

      var info = new ArgentPonySettings
      {
        ClientId = clientId,
        ClientSecret = clientSecret
      };
      services.AddSingleton(info);
      return info;
    }

    public static IServiceCollection AddKafka(this IServiceCollection services, IConfiguration config)
    {
      // setup kafka
      string kafkaIniFilePath = config.GetSection("VishkarKafkaIniFilePath").Value ?? "";
      if (string.IsNullOrWhiteSpace(kafkaIniFilePath))
      {
        kafkaIniFilePath = System.IO.Path.Combine(Environment.CurrentDirectory, "vishkar-kafka.props");
      }
      if (!File.Exists(kafkaIniFilePath)) throw new FileNotFoundException($"Unable to find Kafak configuration file '{kafkaIniFilePath}'!", kafkaIniFilePath);
      var kafkaBuilder = new KafkaConfigBuilder
      {
        ConfigIniFilePath = kafkaIniFilePath
      };

      services.AddSingleton<IKafkaConfigBuilder>(kafkaBuilder);
      return services;
    }

    public static IServiceCollection AddWowCore(this IServiceCollection services)
    {
      return services.AddTransient<IEcology, Ecology>();
    }
  }
}
