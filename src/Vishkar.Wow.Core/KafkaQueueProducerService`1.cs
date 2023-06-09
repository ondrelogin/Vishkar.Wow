using System;
using Confluent.Kafka;

namespace Vishkar.Wow.Core
{
  public class KafkaQueueProducerService<T> : IQueueProducerService<T>, IDisposable
  {
    private readonly Lazy<IProducer<string, T>> _producerSvc;

    public KafkaQueueProducerService(IKafkaConfigBuilder builder)
    {
      //
      _producerSvc = new Lazy<IProducer<string, T>>(builder.CreateProducer<string, T>);
    }

    public async Task SendMessageAsync(string key, T message, string topic)
    {
      var msg = new Message<string, T>();
      msg.Key = key;
      msg.Value = message;
      await _producerSvc.Value.ProduceAsync(topic, msg);
      // TODO handle error
    }

    public async Task SendMessageWithPartitionAsync(string key, T message, string topic, int partition)
    {
      var tp = new TopicPartition(topic, new Partition(partition));

      var msg = new Message<string, T>();
      msg.Key = key;
      msg.Value = message;
      await _producerSvc.Value.ProduceAsync(tp, msg);
      // TODO handle error
    }

    public void Flush()
    {
      // use to ensure that outgoing messages are sent before disposing
      //   does dispose call this automatically?
      _producerSvc.Value.Flush(TimeSpan.FromSeconds(10));
    }

    public void Dispose()
    {
      if (_producerSvc.Value != null)
      {
        _producerSvc.Value.Dispose();
      }
    }
  }
}
