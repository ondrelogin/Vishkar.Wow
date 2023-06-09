using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Vishkar.Wow.Core;

namespace Vishkar.Wow.CommodityWatcher
{
  public class CommodityHostedService : BackgroundService
  {
    private const string _sourceTopic = "commodities";

    private readonly IKafkaConfigBuilder _builder;
    private readonly ILogger<CommodityHostedService> _logger;

    public CommodityHostedService(IKafkaConfigBuilder builder, ILogger<CommodityHostedService> logger)
    {
      _builder = builder;
      _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      _logger.LogInformation("Execute start....");
      await Task.Delay(1); // TODO temporary until we have an actual async call so we don't get any warnings

      _logger.LogInformation($"Connecting to {_sourceTopic}....");
      using (var consumer = _builder.CreateConsumer<string, string>())
      {
        consumer.Subscribe(_sourceTopic);

        int consumeTimeoutMilliseconds = 1500;
        while (!stoppingToken.IsCancellationRequested)
        {
          var cr = consumer.Consume(consumeTimeoutMilliseconds);
          // TODO store to persistent store
          // TODO see if someone has "subscribed" to a particular recipe that requires this
          //      if so, then we can maybe run it through a rules engine to see if we should
          //      send a notification letting them know that the item is on sale based on
          //      maybe quantity and price
          return;
        }
      }
  
      _logger.LogInformation("Waiting for stuff....");
    }
  }
}
