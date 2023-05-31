using System;

namespace Vishkar.Wow.Core.Models
{
  public class RawAuctionItem
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
  }
}