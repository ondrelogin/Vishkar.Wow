using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using ArgentPonyWarcraftClient;

namespace Vishkar.Wow.AuctionProducer.Services
{
  public class CachedAutionHouseApi : IAuctionHouseApi
  {
    public string CacheFolderPath { get; }

    public CachedAutionHouseApi(CachedWowSettings settings)
    {
      this.CacheFolderPath = settings.CachedFolder;
    }

    public async Task<RequestResult<CommoditiesIndex>> GetCommoditiesAsync(string @namespace)
    {
      string sourceFilePath = Path.Combine(this.CacheFolderPath, "wowcomm.json");
      string json = await File.ReadAllTextAsync(sourceFilePath);

      var commIndex = JsonSerializer.Deserialize<CommoditiesIndex>(json);
      return new RequestResult<CommoditiesIndex>(commIndex);
    }

    Task<RequestResult<AuctionsIndex>> IAuctionHouseApi.GetAuctionsAsync(int connectedRealmId, string @namespace) { throw new NotImplementedException(); }
    Task<RequestResult<AuctionsIndex>> IAuctionHouseApi.GetAuctionsAsync(int connectedRealmId, string @namespace, Region region, Locale locale) { throw new NotImplementedException(); }
    Task<RequestResult<CommoditiesIndex>> IAuctionHouseApi.GetCommoditiesAsync(string @namespace, Region region, Locale locale) { throw new NotImplementedException(); }
  }
}
