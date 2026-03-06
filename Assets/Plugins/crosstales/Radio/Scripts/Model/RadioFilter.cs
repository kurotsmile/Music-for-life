using UnityEngine;
using Crosstales.Radio.Util;

namespace Crosstales.Radio.Model
{
   /// <summary>Filter for radio stations.</summary>
   [System.Serializable]
   public class RadioFilter
   {
      #region Variables

      /// <summary>Part of the radio names (callsigns).</summary>
      [Tooltip("Part of the radio names (callsigns).")] public string Names = string.Empty;

      /// <summary>Part of the radio URLs.</summary>
      [Tooltip("Part of the radio URLs.")] public string Urls = string.Empty;

      /// <summary>Part of the radio stations.</summary>
      [Tooltip("Part of the radio stations.")] public string Stations = string.Empty;

      /// <summary>Part of the radio genres.</summary>
      [Tooltip("Part of the radio genres.")] public string Genres = string.Empty;

      /// <summary>Part of the radio cities.</summary>
      [Tooltip("Part of the radio cities.")] public string Cities = string.Empty;

      /// <summary>Part of the radio countries (ISO 3166-1, e.g. 'ch').</summary>
      [Tooltip("Part of the radio countries (ISO 3166-1, e.g. 'ch').")] public string Countries = string.Empty;

      /// <summary>Part of the radio languages (like 'german').</summary>
      [Tooltip("Part of the radio languages (like 'german').")] public string Languages = string.Empty;

      [UnityEngine.Serialization.FormerlySerializedAsAttribute("RatingMin")] [Tooltip("Minimal rating (default: 0)."), Range(0f, 4.9f), SerializeField]
      private float ratingMin;

      [UnityEngine.Serialization.FormerlySerializedAsAttribute("RatingMax")] [Tooltip("Maximal rating (default: 5).")] [Range(0.1f, 5f), SerializeField]
      private float ratingMax = 5f;

      /// <summary>Part of the radio formats.</summary>
      [Tooltip("Part of the radio formats.")] public string Format = string.Empty;

      [UnityEngine.Serialization.FormerlySerializedAsAttribute("BitrateMin")] [Tooltip("Minimal bitrate in kbit/s (default: 32).")] [Range(32, 499), SerializeField]
      private int bitrateMin = 32;

      [UnityEngine.Serialization.FormerlySerializedAsAttribute("BitrateMax")] [Tooltip("Maximal bitrate in kbit/s (default: 500).")] [Range(33, 500), SerializeField]
      private int bitrateMax = 500;

      /// <summary>Exclude radio stations with unsupported codecs (default: true).</summary>
      [Tooltip("Exclude radio stations with unsupported codecs (default: true).")] public bool ExcludeUnsupportedCodecs = true;

      /// <summary>Limit number of results (default: 0 = unlimited).</summary>
      [Range(0, 500)] [Tooltip("Limit number of results (default: 0 = unlimited).")] public int Limit;

      #endregion


      #region Properties

      /// <summary>Minimal rating (range: 0-4.9).</summary>
      public float RatingMin
      {
         get => ratingMin;
         set => ratingMin = Mathf.Clamp(value, 0f, 4.9f);
      }

      /// <summary>Maximal rating (range: 0.1-5).</summary>
      public float RatingMax
      {
         get => ratingMax;
         set => ratingMax = Mathf.Clamp(value, 0.1f, 5f);
      }

      /// <summary>Minimal bitrate in kbit/s (range: 32-499).</summary>
      public int BitrateMin
      {
         get => bitrateMin;
         set => bitrateMin = Mathf.Clamp(value, 32, 499);
      }

      /// <summary>Maximal bitrate in kbit/s (range: 33-500).</summary>
      public int BitrateMax
      {
         get => bitrateMax;
         set => bitrateMax = Mathf.Clamp(value, 33, 500);
      }

      /// <summary>Are filter parameters set and active?</summary>
      /// <returns>True if filter parameters are set and active.</returns>
      public bool isFiltering =>
         !string.IsNullOrEmpty(Names) ||
         !string.IsNullOrEmpty(Urls) ||
         !string.IsNullOrEmpty(Stations) ||
         !string.IsNullOrEmpty(Genres) ||
         !string.IsNullOrEmpty(Cities) ||
         !string.IsNullOrEmpty(Countries) ||
         !string.IsNullOrEmpty(Languages) ||
         ratingMin > 0f ||
         ratingMax < 5f ||
         !string.IsNullOrEmpty(Format) ||
         bitrateMin > 32 ||
         bitrateMax < 500 ||
         Limit != 0;
         //|| ExcludeUnsupportedCodecs;

      #endregion


      #region Constructors

      /// <summary>Default-constructor for a RadioFilter.</summary>
      public RadioFilter()
      {
         //empty
      }

      /// <summary>Clone-constructor for a RadioFilter.</summary>
      public RadioFilter(RadioFilter filter)
      {
         Names = filter.Names;
         Urls = filter.Urls;
         Stations = filter.Stations;
         Genres = filter.Genres;
         Cities = filter.Cities;
         Countries = filter.Countries;
         Languages = filter.Languages;
         RatingMin = filter.ratingMin;
         RatingMax = filter.ratingMax;
         Format = filter.Format;
         BitrateMin = filter.bitrateMin;
         BitrateMax = filter.bitrateMax;
         ExcludeUnsupportedCodecs = filter.ExcludeUnsupportedCodecs;
         Limit = filter.Limit;
      }

      #endregion


      #region Overridden methods

      public override bool Equals(object obj)
      {
         // Check for null values and compare run-time types.
         if (obj == null || GetType() != obj.GetType())
            return false;

         RadioFilter rf = (RadioFilter)obj;

         return Names == rf.Names &&
                Urls == rf.Urls &&
                Stations == rf.Stations &&
                Genres == rf.Genres &&
                Cities == rf.Cities &&
                Countries == rf.Countries &&
                Languages == rf.Languages &&
                System.Math.Abs(ratingMin - rf.ratingMin) < Constants.FLOAT_TOLERANCE &&
                System.Math.Abs(ratingMax - rf.ratingMax) < Constants.FLOAT_TOLERANCE &&
                Format == rf.Format &&
                bitrateMin == rf.bitrateMin &&
                bitrateMax == rf.bitrateMax &&
                Limit == rf.Limit &&
                ExcludeUnsupportedCodecs == rf.ExcludeUnsupportedCodecs;
      }

      public override int GetHashCode()
      {
         return base.GetHashCode();
      }
/* 
      public override int GetHashCode()
      {
         int hash = 0;

         if (Names != null)
            hash += Names.GetHashCode();
         if (Urls != null)
            hash += Urls.GetHashCode();
         if (Stations != null)
            hash += Stations.GetHashCode();
         if (Genres != null)
            hash += Genres.GetHashCode();
         if (Cities != null)
            hash += Cities.GetHashCode();
         if (Countries != null)
            hash += Countries.GetHashCode();
         if (Languages != null)
            hash += Languages.GetHashCode();
         hash += (int)(ratingMin * 10);
         hash += (int)(ratingMax * 10);
         if (Format != null)
            hash += Format.GetHashCode();
         hash += bitrateMin;
         hash += bitrateMax;
         hash += Limit;

         return hash;
      }
*/
      public override string ToString()
      {
         System.Text.StringBuilder result = new System.Text.StringBuilder();

         result.Append(GetType().Name);
         result.Append(Constants.TEXT_TOSTRING_START);

         result.Append("Name='");
         result.Append(Names);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("Url='");
         result.Append(Urls);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("Station='");
         result.Append(Stations);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("Genres='");
         result.Append(Genres);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("Cities='");
         result.Append(Cities);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("Countries='");
         result.Append(Countries);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("Languages='");
         result.Append(Languages);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("RatingMin='");
         result.Append(ratingMin);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("RatingMax='");
         result.Append(ratingMax);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("Format='");
         result.Append(Format);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("BitrateMin='");
         result.Append(bitrateMin);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("BitrateMax='");
         result.Append(bitrateMax);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("ExcludeUnsupportedCodecs='");
         result.Append(ExcludeUnsupportedCodecs);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("Limit='");
         result.Append(Limit);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("isFiltering='");
         result.Append(isFiltering);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER_END);

         result.Append(Constants.TEXT_TOSTRING_END);

         return result.ToString();
      }

      #endregion
   }
}
// © 2016-2024 crosstales LLC (https://www.crosstales.com)