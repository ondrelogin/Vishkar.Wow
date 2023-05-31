using System;
using System.Text.Json;
using ArgentPonyWarcraftClient;
using Microsoft.Extensions.Logging;
using Vishkar.Wow.Core;
using Vishkar.Wow.Core.Models;

namespace Vishkar.Wow.AuctionProducer
{
  public class WowRunner
  {
    private readonly IAuctionHouseApi _auctionHouse;
    private readonly IEcology _ecology;
    private readonly IQueueProducerService<string> _queueSvc;
    private readonly ILogger<WowRunner> _logger;

    private const string _topic = "commodities-raw";

    public WowRunner(IAuctionHouseApi auctionHouse, IQueueProducerService<string> queueSvc, IEcology ecology, ILogger<WowRunner> logger)
    {
      _auctionHouse = auctionHouse;
      _ecology = ecology;
      _queueSvc = queueSvc;
      _logger = logger;
    }

    public async Task<int> ExecuteAsync()
    {
      // later need this more of a polling basis...
      _logger.LogInformation("retrieving commodities...");
      var resultComm = await _auctionHouse.GetCommoditiesAsync("dynamic-us");
      if (!resultComm.Success) throw new Exception(resultComm.Error.Detail);
      var comm = resultComm.Value;
      _logger.LogInformation($"{comm.Auctions.LongLength:N0} commodities received...");

      var now = _ecology.Now;
      foreach (var item in comm.Auctions)
      {
        var msg = new RawAuctionItem();
        msg.AuctionStartDateTime = now;
        msg.Id = item.Id;
        msg.ItemId = item.Item.Id;
        msg.TimeLeft = item.TimeLeft;
        msg.Quantity = item.Quantity;
        msg.UnitPriceCp = (item.UnitPrice == null) ? 0 : item.UnitPrice.Value;
        msg.BuyoutCp = 0;
        msg.BidCp = 0;
        var msgJson = JsonSerializer.Serialize(msg);

        string key = $"raw_{msg.Id}";
        await _queueSvc.SendMessageAsync(key, msgJson, _topic);
      }

      return 0;
    }
  }
}
