using System;

namespace Vishkar.Wow.Core.Models
{
  public class AuctionItem
  {
    public DateTimeOffset AuctionStartDateTime { get; set; }
    public int Id { get; set; }
    public int ItemId { get; set; }
    public int Quantity { get; set; }
    public string TimeLeft { get; set; } = "";
    /// <summary>Unit Price in Copper Pieces</summary>
    public long UnitPriceCp { get; set; }
    public long BuyoutCp { get; set; }
    public long BidCp { get; set; }

    public string ItemName { get; set; }
    public string ItemQuality { get; set; }
    public int ItemLevel { get; set; }
    public int ItemClassId { get; set; }
    public int ItemSubclassId { get; set; }
    public string ItemInventoryType { get; set; }
    public long ItemPurchasePriceCp { get; set; }
    public int ItemPurchaseQuantity { get; set; }
    public long itemVendorSellPriceCp { get; set; }
  }
}
