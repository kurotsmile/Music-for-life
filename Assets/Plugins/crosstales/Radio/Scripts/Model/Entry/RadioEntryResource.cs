using UnityEngine;
using Crosstales.Radio.Util;
using Crosstales.Radio.Model.Enum;

namespace Crosstales.Radio.Model.Entry
{
   /// <summary>Model for a Resource entry.</summary>
   [System.Serializable]
   public class RadioEntryResource : BaseRadioEntry
   {
      #region Variables

      /// <summary>Text-, M3U-, PLS- or ShoutcastID-file with the radios.</summary>
      [Header("Source Settings")] [Tooltip("Text-, M3U-, PLS- or ShoutcastID-file with the radios.")] public TextAsset Resource;

      /// <summary>Data format of the data with the radios (default: DataFormatResource.Text).</summary>
      [Tooltip("Data format of the resource with the radios (default: DataFormatResource.Text).")] public DataFormatResource DataFormat = DataFormatResource.Text;

      /// <summary>Reads only the given number of radio stations (default: : 0 (= all))</summary>
      [Tooltip("Reads only the given number of radio stations (default: : 0 (= all))")] public int ReadNumberOfStations;

      #endregion


      #region Constructor

      /// <summary>Constructor for a RadioEntryResource.</summary>
      /// <param name="entry">BaseRadioEntry as base.</param>
      /// <param name="resource">Text-, M3U-, PLS- or ShoutcastID-file with the radios.</param>
      /// <param name="dataFormat">Data format of the data with the radios (default: DataFormatResource.Text, optional).</param>
      /// <param name="readNumberOfStations">Reads only the given number of radio stations (default: : 0 (= all), optional).</param>
      public RadioEntryResource(BaseRadioEntry entry, TextAsset resource, DataFormatResource dataFormat = DataFormatResource.Text, int readNumberOfStations = 0) : base(entry.Name, entry.ForceName, entry.EnableSource, entry.Station, entry.Genres, entry.Rating, entry.Description, entry.Icon, entry.IconUrl, entry.City, entry.Country, entry.Language, entry.Format, entry.Bitrate, entry.ChunkSize, entry.BufferSize, entry.ExcludedCodec)
      {
         Resource = resource;
         DataFormat = dataFormat;
         ReadNumberOfStations = readNumberOfStations;
      }

      #endregion


      #region Overridden methods

      public override string ToString()
      {
         System.Text.StringBuilder result = new System.Text.StringBuilder(base.ToString());

         result.Append(GetType().Name);
         result.Append(Constants.TEXT_TOSTRING_START);

         result.Append("Resource='");
         result.Append(Resource);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("DataFormat='");
         result.Append(DataFormat);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("ReadNumberOfStations='");
         result.Append(ReadNumberOfStations);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER_END);

         result.Append(Constants.TEXT_TOSTRING_END);

         return result.ToString();
      }

      #endregion
   }
}
// © 2016-2024 crosstales LLC (https://www.crosstales.com)