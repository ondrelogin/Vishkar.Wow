using System;

namespace Vishkar.Wow.Core
{
  public interface IQueueProducerService<T>
  {
    Task SendMessageAsync(string key, T message, string topic);
    void Flush();
  }
}
