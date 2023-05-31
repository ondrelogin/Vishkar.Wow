using System;
using System.Collections.Generic;

namespace Vishkar.Wow.Core
{
  public static class GeneralExtensions
  {
    public static bool EqualsAnyCase(this string source, string value)
    {
      if (source == null)
      {
        if (value == null) return true;
        return false;
      }

      return source.Equals(value, StringComparison.InvariantCultureIgnoreCase);
    }
  }
}
