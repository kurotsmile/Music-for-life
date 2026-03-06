using UnityEngine;
using Crosstales.Radio.Util;

namespace Crosstales.Radio.Model.Entry
{
   /// <summary>Model for a Shoutcast entry.</summary>
   [System.Serializable]
   public class RadioEntryShoutcast : BaseRadioEntry
   {
      #region Variables

      /// <summary>Shoutcast-ID for the radio.</summary>
      [Header("Source Settings")] [Tooltip("Shoutcast-ID for the radio.")] public string ShoutcastID;

      #endregion


      #region Constructor

      /// <summary>Constructor for a RadioEntryShoutcast.</summary>
      /// <param name="entry">RadioStation as base.</param>
      /// <param name="shoutcastID">Shoutcast-ID from the radio station.</param>
      public RadioEntryShoutcast(RadioStation entry, string shoutcastID) : base(entry.Name, true, true, entry.Station, entry.Genres, entry.Rating, entry.Description, entry.Icon, entry.IconUrl, entry.City, entry.Country, entry.Language, entry.Format, entry.Bitrate, entry.ChunkSize, entry.BufferSize, entry.ExcludedCodec)
      {
         ShoutcastID = shoutcastID;
      }

      #endregion


      #region Overridden methods

      public override string ToString()
      {
         System.Text.StringBuilder result = new System.Text.StringBuilder(base.ToString());

         result.Append(GetType().Name);
         result.Append(Constants.TEXT_TOSTRING_START);

         result.Append("ShoutcastID='");
         result.Append(ShoutcastID);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER_END);

         result.Append(Constants.TEXT_TOSTRING_END);

         return result.ToString();
      }

      #endregion
   }
}
// © 2016-2024 crosstales LLC (https://www.crosstales.com)