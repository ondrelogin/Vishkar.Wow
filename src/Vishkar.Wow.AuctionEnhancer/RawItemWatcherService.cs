using System;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Vishkar.Wow.Core;

namespace Vishkar.Wow.AuctionEnhancer
{
  public class RawItemWatcherService
  {
    private readonly IKafkaConfigBuilder _builder;
    private readonly ILogger<RawItemWatcherService> _logger;

    private const string _topic = "commodities-raw";

    public RawItemWatcherService(IKafkaConfigBuilder builder, ILogger<RawItemWatcherService> logger)
    {
      _builder = builder;
      _logger = logger;
    }

    public async Task<int> Execute()
    {
      var cts = new CancellationTokenSource();
      Console.CancelKeyPress += (_, e) => {
        e.Cancel = true; // prevent the process from terminating.
        cts.Cancel();
      };

      _logger.LogInformation($"Connecting to {_topic}....");
      using (var consumer = new ConsumerBuilder<string, string>(_builder.Build().AsEnumerable()).Build())
      {
        consumer.Subscribe(_topic);
        try
        {
          while (true)
          {
            var cr = consumer.Consume(cts.Token);

            // TODO process the message for real...
            _logger.LogInformation($"Consumed msg from topic {_topic} with key {cr.Message.Key, -10}...");
          }
        }
        catch (OperationCanceledException)
        {
          // Ctrl-C was pressed.
        }
        finally
        {
          consumer.Close();
        }
      }
      return 0;
    }
  }
}
