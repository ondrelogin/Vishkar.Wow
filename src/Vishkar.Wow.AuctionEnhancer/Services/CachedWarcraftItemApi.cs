using System;
using System.Text.Json;
using ArgentPonyWarcraftClient;
using Microsoft.Extensions.Logging;
using Vishkar.Wow.Core;
using Vishkar.Wow.Core.Settings;

namespace Vishkar.Wow.AuctionEnhancer.Services
{
  public class CachedWarcraftItemApi : IItemApi
  {
    private readonly ArgentPonySettings _settings;
    private readonly IVishkarFileService _fileSvc;
    private readonly ILogger<CachedWarcraftItemApi> _logger;
    private readonly IDictionary<int, string> _cachedItemsById;

    public string SourceItemCacheFolderPath { get; }

    public CachedWarcraftItemApi(ArgentPonySettings settings, IVishkarFileService fileSvc, CachedWowSettings cacheSettings, ILogger<CachedWarcraftItemApi> logger)
    {
      _settings = settings;
      _fileSvc = fileSvc;
      _logger = logger;
      _cachedItemsById = new System.Collections.Concurrent.ConcurrentDictionary<int, string>();

      this.SourceItemCacheFolderPath = Path.Combine(cacheSettings.CachedFolder, "Items");
    }

    public async Task PreloadCacheAsync()
    {
      await _fileSvc.EnsureDirectoryExistsAsync(this.SourceItemCacheFolderPath);
      var cacheFileList = await _fileSvc.GetAllFilesInFolderPathAsync(this.SourceItemCacheFolderPath, "*.json");

      foreach (var filePath in cacheFileList)
      {
        string json = await _fileSvc.ReadAllTextAsync(filePath);
        var item = JsonSerializer.Deserialize<Item>(json);
        if (item != null) { _cachedItemsById[item.Id] = json; }
      }
      _logger.LogInformation($"loaded {_cachedItemsById.Count:N0} items into the cache.");
    }

    public async Task<RequestResult<Item>> GetItemAsync(int itemId, string namespaceString)
    {
      string? json = _cachedItemsById.GetAsString(itemId);
      if (json != null)
      {
        _logger.LogInformation($"Retrieving Cached Item {itemId}...");
        var item = JsonSerializer.Deserialize<Item>(json);
        if (item != null) return new RequestResult<Item>(item);
        _logger.LogInformation($"Cached Json for {itemId} was invalid!");
      }

      _logger.LogInformation($"Looking up info on {itemId}...");
      var wsvc = new WarcraftClient(_settings.ClientId, _settings.ClientSecret, Region.US, Locale.en_US);
      var resultItem = await wsvc.GetItemAsync(itemId, namespaceString);
      if (!resultItem.Success) return resultItem;
      _logger.LogInformation($"- found {resultItem.Value.Name}");

      json = JsonSerializer.Serialize(resultItem.Value);
      _cachedItemsById[itemId] = json;

      string fullFilePath = Path.Combine(this.SourceItemCacheFolderPath, $"item{resultItem.Value.Id:F0}.json");

      await _fileSvc.WriteAllTextAsync(fullFilePath, json);
      return resultItem.Value;
    }

    Task<RequestResult<Item>> IItemApi.GetItemAsync(int itemId, string @namespace, Region region, Locale locale) { throw new NotImplementedException(); }
    Task<RequestResult<ItemClass>> IItemApi.GetItemClassAsync(int itemClassId, string @namespace) { throw new NotImplementedException(); }
    Task<RequestResult<ItemClass>> IItemApi.GetItemClassAsync(int itemClassId, string @namespace, Region region, Locale locale) { throw new NotImplementedException(); }
    Task<RequestResult<ItemClassesIndex>> IItemApi.GetItemClassesIndexAsync(string @namespace) { throw new NotImplementedException(); }
    Task<RequestResult<ItemClassesIndex>> IItemApi.GetItemClassesIndexAsync(string @namespace, Region region, Locale locale) { throw new NotImplementedException(); }
    Task<RequestResult<ItemMedia>> IItemApi.GetItemMediaAsync(int itemId, string @namespace) { throw new NotImplementedException(); }
    Task<RequestResult<ItemMedia>> IItemApi.GetItemMediaAsync(int itemId, string @namespace, Region region, Locale locale) { throw new NotImplementedException(); }
    Task<RequestResult<ItemSet>> IItemApi.GetItemSetAsync(int itemSetId, string @namespace) { throw new NotImplementedException(); }
    Task<RequestResult<ItemSet>> IItemApi.GetItemSetAsync(int itemSetId, string @namespace, Region region, Locale locale) { throw new NotImplementedException(); }
    Task<RequestResult<ItemSetsIndex>> IItemApi.GetItemSetsIndexAsync(string @namespace) { throw new NotImplementedException(); }
    Task<RequestResult<ItemSetsIndex>> IItemApi.GetItemSetsIndexAsync(string @namespace, Region region, Locale locale) { throw new NotImplementedException(); }
    Task<RequestResult<ItemSubclass>> IItemApi.GetItemSubclassAsync(int itemClassId, int itemSubclassId, string @namespace) { throw new NotImplementedException(); }
    Task<RequestResult<ItemSubclass>> IItemApi.GetItemSubclassAsync(int itemClassId, int itemSubclassId, string @namespace, Region region, Locale locale) { throw new NotImplementedException(); }
  }
}
