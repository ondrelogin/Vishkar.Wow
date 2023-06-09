using System;

namespace Vishkar.Wow.Core
{
  public interface IQueueProducerService<T>
  {
    Task SendMessageAsync(string key, T message, string topic);
    Task SendMessageWithPartitionAsync(string key, T message, string topic, int partition);
    void Flush();
  }
}
