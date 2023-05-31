using System;

namespace Vishkar.Wow.Core
{
  public interface IEcology
  {
    DateTimeOffset Now { get; }
  }

  /// <summary>
  /// Like Environment, but Mockable
  /// </summary>
  public class Ecology : IEcology
  {
    public DateTimeOffset Now => DateTimeOffset.Now;
  }
}
