using System;
using System.Text.Json;
using ArgentPonyWarcraftClient;
using Microsoft.Extensions.Logging;
using Vishkar.Wow.AuctionEnhancer.Services;
using Vishkar.Wow.Core;
using Vishkar.Wow.Core.Models;

namespace Vishkar.Wow.AuctionEnhancer
{
  public class RawItemWatcherService
  {
    private readonly IKafkaConfigBuilder _builder;
    private readonly IItemApi _itemSvc;
    private readonly IQueueProducerService<string> _targetQueue;
    private readonly ILogger<RawItemWatcherService> _logger;

    private const string _sourceTopic = "commodities-raw";
    private const string _targetTopic = "commodities";

    public RawItemWatcherService(IKafkaConfigBuilder builder, IItemApi itemSvc, IQueueProducerService<string> targetQueue, ILogger<RawItemWatcherService> logger)
    {
      _builder = builder;
      _itemSvc = itemSvc;
      _targetQueue = targetQueue;
      _logger = logger;
    }

    public async Task<int> ExecuteAsync()
    {
      var cts = new CancellationTokenSource();
      Console.CancelKeyPress += (_, e) => {
        e.Cancel = true; // prevent the process from terminating.
        cts.Cancel();
      };

      var asCache = _itemSvc as CachedWarcraftItemApi;
      if (asCache != null)
      {
        _logger.LogInformation("preloading cache...");
        await asCache.PreloadCacheAsync();
      }
      
      _logger.LogInformation($"Connecting to {_sourceTopic}....");
      using (var consumer = _builder.CreateConsumer<string, string>())
      {
        consumer.Subscribe(_sourceTopic);
        try
        {
          while (true)
          {
            var cr = consumer.Consume(cts.Token);
            
            try
            {
              string json = cr.Message.Value;
              var rawItem = JsonSerializer.Deserialize<RawAuctionItem>(json);
              if (rawItem == null)
              {
                _logger.LogError($"Unable to deserialize raw auction item {cr.Message.Key}!");
              }
              else
              {
                await this.ProcessItemAsync(rawItem);
              }
            }
            catch (Exception messageEx)
            {
              _logger.LogError(messageEx, "Error processing message.");
            }
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

    private async Task ProcessItemAsync(RawAuctionItem rawItem)
    {
      var resultItemType = await _itemSvc.GetItemAsync(rawItem.ItemId, "static-us");
      if (!resultItemType.Success) throw new Exception(resultItemType.Error.Detail);
      var itemType = resultItemType.Value;

      var auction = new Vishkar.Wow.Core.Models.AuctionItem
      {
        AuctionStartDateTime = rawItem.AuctionStartDateTime,
        Id = rawItem.Id,
        ItemId = rawItem.ItemId,
        Quantity = rawItem.Quantity,
        TimeLeft = rawItem.TimeLeft,
        UnitPriceCp = rawItem.UnitPriceCp,
        BuyoutCp = rawItem.BuyoutCp,
        BidCp = rawItem.BidCp
      };
      auction.ItemName = itemType.Name;
      auction.ItemQuality = itemType.Quality.Type;
      auction.ItemLevel = itemType.Level;
      auction.ItemClassId = itemType.ItemClass.Id;
      auction.ItemSubclassId = itemType.ItemSubclass.Id;
      auction.ItemInventoryType = itemType.InventoryType.Type;
      auction.ItemPurchasePriceCp = itemType.PurchasePrice;
      auction.ItemPurchaseQuantity = itemType.PurchaseQuantity;
      auction.itemVendorSellPriceCp = itemType.SellPrice;

      string key = $"auction_{auction.Id}";
      string json = JsonSerializer.Serialize(auction);

      _logger.LogInformation($"Sending msg {auction.Id}...");

      // 10 partitions, maybe something more "smarter" later...
      int partition = auction.ItemClassId % 10;
      await _targetQueue.SendMessageWithPartitionAsync(key, json, _targetTopic, partition);
      _logger.LogInformation("- done.");
    }
  }
}
