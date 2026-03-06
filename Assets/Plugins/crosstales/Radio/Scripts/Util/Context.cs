namespace Crosstales.Radio.Util
{
   /// <summary>Context for the asset.</summary>
   public static class Context
   {
      #region Changable variables

      /// <summary>Total downloaded data size in bytes for all RadioPlayer.</summary>
      public static long TotalDataSize = 0;

      /// <summary>Total number of data requests for all RadioPlayer.</summary>
      public static int TotalDataRequests = 0;

      /// <summary>Total playtime in seconds for all RadioPlayer.</summary>
      public static double TotalPlayTime = 0;

      /// <summary>List of all played records.</summary>
      public static readonly System.Collections.Generic.List<Crosstales.Radio.Model.RecordInfo> AllPlayedRecords = new System.Collections.Generic.List<Crosstales.Radio.Model.RecordInfo>();

      #endregion
   }
}
// © 2017-2024 crosstales LLC (https://www.crosstales.com)