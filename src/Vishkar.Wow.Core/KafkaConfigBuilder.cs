using System;
using System.Collections.Generic;
using System.IO;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

namespace Vishkar.Wow.Core
{
  public interface IKafkaConfigBuilder
  {
    IProducer<TKey, TValue> CreateProducer<TKey, TValue>();

    IConsumer<TKey, TValue> CreateConsumer<TKey, TValue>();
  }

  public class KafkaConfigBuilder : IKafkaConfigBuilder
  {
    public string ConfigIniFilePath { get; init; }

    public IProducer<TKey, TValue> CreateProducer<TKey, TValue>()
    {
      var config = this.BuildConfigRoot();
      return new ProducerBuilder<TKey, TValue>(config.AsEnumerable()).Build();
    }

    public IConsumer<TKey, TValue> CreateConsumer<TKey, TValue>()
    {
      var config = this.BuildConfigRoot();

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

      return new ConsumerBuilder<TKey, TValue>(config.AsEnumerable()).Build();
    }

    private IConfigurationRoot BuildConfigRoot()
    {
      return new ConfigurationBuilder()
            .AddIniFile(this.ConfigIniFilePath)
            .Build();
    }
  }
}
