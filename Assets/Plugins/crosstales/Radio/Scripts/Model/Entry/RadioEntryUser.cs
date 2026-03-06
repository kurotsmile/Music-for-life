using UnityEngine;
using Crosstales.Radio.Util;
using Crosstales.Radio.Model.Enum;

namespace Crosstales.Radio.Model.Entry
{
   /// <summary>Model for an User entry.</summary>
   [System.Serializable]
   public class RadioEntryUser : BaseRadioEntry
   {
      #region Variables

      /// <summary>Text-, M3U or PLS-file with the radios.</summary>
      [Header("Base Settings")] [Tooltip("Text-, M3U or PLS-file with the radios.")] public TextAsset Resource;

      /// <summary>Data format of the data with the radios (default: DataFormatResource.Text).</summary>
      [Tooltip("Data format of the resource with the radios (default: DataFormatResource.Text).")] public DataFormatResource DataFormat = DataFormatResource.Text;

      /// <summary>Reads only the given number of radio stations (default: : 0 (= all))</summary>
      [Tooltip("Reads only the given number of radio stations (default: : 0 (= all))")] public int ReadNumberOfStations;

      /// <summary>Loads the radio stations only once (default: true).</summary>
      [Tooltip("Loads the radio stations only once (default: true).")] public bool LoadOnlyOnce = true;


      /// <summary>Path to the text-file with the radios.</summary>
      [Header("Source Settings")] [Tooltip("Path to the text-file with the radios.")] public string Path;

      /// <summary>Prefixes for the path (default: PathPrefix.None).</summary>
      [Tooltip("Prefixes for the path (default: PathPrefix.None).")] public PathPrefix Prefix = PathPrefix.None;

      #endregion


      #region Properties

      /// <summary>Returns the final path including an optional prefix.</summary>
      /// <returns>Final path including an optional prefix.</returns>
      public string FinalPath
      {
         get
         {
            string result = Path.Trim();

            switch (Prefix)
            {
               case PathPrefix.PersistentDataPath:
                  result = Application.persistentDataPath + '/' + Path.Trim();
                  break;
               case PathPrefix.DataPath:
                  result = Application.dataPath + '/' + Path.Trim();
                  break;
               case PathPrefix.TempPath:
                  result = Constants.PREFIX_TEMP_PATH + Path.Trim();
                  break;
            }

            return Crosstales.Common.Util.FileHelper.ValidateFile(result);
         }
      }

      #endregion


      #region Constructors

      /// <summary>Constructor for a RadioEntryUser.</summary>
      /// <param name="entry">RadioStation as base.</param>
      /// <param name="url">Stream-URL of the station.</param>
      public RadioEntryUser(RadioStation entry, string url) : base(entry.Name, true, true, entry.Station, entry.Genres, entry.Rating, entry.Description, entry.Icon, entry.IconUrl, entry.City, entry.Country, entry.Language, entry.Format, entry.Bitrate, entry.ChunkSize, entry.BufferSize, entry.ExcludedCodec)
      {
         Path = url;
      }

      #endregion


      #region Overridden methods

      public override string ToString()
      {
         System.Text.StringBuilder result = new System.Text.StringBuilder(base.ToString());

         result.Append(GetType().Name);
         result.Append(Constants.TEXT_TOSTRING_START);

         result.Append("URL='");
         result.Append(Path);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("Prefix='");
         result.Append(Prefix);

         result.Append(Constants.TEXT_TOSTRING_DELIMITER_END);

         result.Append(Constants.TEXT_TOSTRING_END);

         return result.ToString();
      }

      #endregion
   }
}
// © 2016-2024 crosstales LLC (https://www.crosstales.com)