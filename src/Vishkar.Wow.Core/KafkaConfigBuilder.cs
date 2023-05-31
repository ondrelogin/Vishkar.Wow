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
      return new ConfigurationBuilder()
            .AddIniFile(this.ConfigIniFilePath)
            .Build();
    }
  }
}
