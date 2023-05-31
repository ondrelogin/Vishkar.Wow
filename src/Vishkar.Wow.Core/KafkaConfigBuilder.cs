using System;
using System.Collections.Generic;
using System.IO;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

namespace Vishkar.Wow.Core
{
  public interface IKafkaConfigBuilder
  {
    IConfigurationRoot Build();
  }

  public class KafkaConfigBuilder : IKafkaConfigBuilder
  {
    public string ConfigIniFilePath { get; init; }

    public IConfigurationRoot Build()
    {
      var config = new ConfigurationBuilder()
            .AddIniFile(this.ConfigIniFilePath)
            .Build();

      string kafkaGroupId = config.GetSection("group.id").Value ?? "";
      if (string.IsNullOrWhiteSpace(kafkaGroupId))
      {
        config["group.id"] = "kafka-dotnet-getting-started";
      }

      string autoOffReset = config.GetSection("auto.offset.reset").Value ?? "";
      if (string.IsNullOrWhiteSpace(autoOffReset))
      {
        config["auto.offset.reset"] = "earliest";
      }

      return config;
    }
  }
}
