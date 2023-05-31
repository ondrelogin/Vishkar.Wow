using System;
using System.Collections.Generic;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

namespace Vishkar.Wow.Core
{
  public class KafkaQueueProducerService<T> : IQueueProducerService<T>, IDisposable
  {
    private readonly IKafkaConfigBuilder _builder;
    private readonly Lazy<IProducer<string, T>> _producerSvc;

    public KafkaQueueProducerService(IKafkaConfigBuilder builder)
    {
      //
      _builder = builder;
      _producerSvc = new Lazy<IProducer<string, T>>(this.CreateProducer);
    }

    public async Task SendMessageAsync(string key, T message, string topic)
    {
      var msg = new Message<string, T>();
      msg.Key = key;
      msg.Value = message;
      await _producerSvc.Value.ProduceAsync(topic, msg);

      // TODO handle error
    }

    public void Flush()
    {
      // need to read up on what this actually is
      _producerSvc.Value.Flush(TimeSpan.FromSeconds(10));
    }

    public void Dispose()
    {
      if (_producerSvc.Value != null)
      {
        _producerSvc.Value.Dispose();
      }
    }

    private IProducer<string, T> CreateProducer()
    {
      var config = _builder.Build();
      return new ProducerBuilder<string, T>(config.AsEnumerable()).Build();
    }
  }
}
