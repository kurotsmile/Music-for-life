using UnityEngine;
using Crosstales.Radio.Util;
using Crosstales.Radio.Model.Enum;

namespace Crosstales.Radio.Model.Entry
{
   /// <summary>Base class for radio entries.</summary>
   [System.Serializable]
   public abstract class BaseRadioEntry
   {
      #region Variables

      /// <summary>Name of the file or radio station.</summary>
      [Header("General Settings")] [Tooltip("Name of the radio station.")] public string Name;

      /// <summary>Force the name of the station to this name (default: false).</summary>
      [Tooltip("Force the name of the station to this name (default: false).")] public bool ForceName;

      /// <summary>Enable the source in this provider (default: true).</summary>
      [Tooltip("Enable the source in this provider (default: true).")] public bool EnableSource = true;


      /// <summary>Provider of the radio stations (optional).</summary>
      [Header("Meta Data")] [Tooltip("Provider of the radio stations (optional).")] public string Station;

      /// <summary>Genres of the radios (optional).</summary>
      [Tooltip("Genres of the radios (optional).")] public string Genres;

      /// <summary>Your rating of the radios.</summary>
      [Tooltip("Your rating of the radios.")] [Range(0, 5f)] public float Rating;

      /// <summary>Description of the radio stations (optional).</summary>
      [Tooltip("Description of the radio stations (optional).")] [TextArea] public string Description;

      /// <summary>Icon to represent the radio stations (optional).</summary>
      [Tooltip("Icon to represent the radio stations (optional).")] [System.Xml.Serialization.XmlIgnore] public Sprite Icon;

      /// <summary>Icon url for the radio station.</summary>
      [Tooltip("Icon url for the radio station.")] public string IconUrl;

      /// <summary>City of the radio.</summary>
      [Tooltip("City of the radio.")] public string City;

      /// <summary>Country of the radio (ISO 3166-1, e.g. 'ch').</summary>
      [Tooltip("Country of the radio (ISO 3166-1, e.g. 'ch').")] public string Country;

      /// <summary>Language of the radio (like 'german').</summary>
      [Tooltip("Language of the radio (like 'german').")] public string Language;


      /// <summary>Default audio format of the stations (default: AudioFormat.MP3).</summary>
      [Header("Stream Settings")] [Tooltip("Default audio format of the stations (default: AudioFormat.MP3).")]
      public AudioFormat Format = AudioFormat.MP3;

      /// <summary>Default bitrate in kbit/s (default: 128).</summary>
      [Tooltip("Default bitrate in kbit/s (default: 128).")] public int Bitrate = Config.DEFAULT_BITRATE;

      /// <summary>Default size of the streaming-chunk in KB (default: 32).</summary>
      [Tooltip("Default size of the streaming-chunk in KB (default: 32).")] public int ChunkSize = Config.DEFAULT_CHUNKSIZE;

      /// <summary>Default size of the local buffer in KB (default: 48).</summary>
      [Tooltip("Default size of the local buffer in KB (default: 48).")] public int BufferSize = Config.DEFAULT_BUFFERSIZE;

      /// <summary>Exclude this station if the current RadioPlayer codec is equals this one (default: AudioCodec.None).</summary>
      [Header("Codec Settings")] [Tooltip("Exclude this station if the current RadioPlayer codec is equals this one (default: AudioCodec.None).")]
      public AudioCodec ExcludedCodec = AudioCodec.None;

      /// <summary>Is this entry initialized?.</summary>
      [HideInInspector] public bool isInitialized;

      #endregion


      #region Constructors

      /// <summary>Default-constructor for a BaseRadioEntry.</summary>
      protected BaseRadioEntry()
      {
         //default
      }

      /// <summary>Constructor for a BaseRadioEntry.</summary>
      /// <param name="name">Name of the radio station.</param>
      /// <param name="forceName">Force the name of the station to this name.</param>
      /// <param name="enableSource">Enable the source in this provider.</param>
      /// <param name="station">Name of the station.</param>
      /// <param name="genres">Genres of the radio.</param>
      /// <param name="rating">Your rating of the radio.</param>
      /// <param name="desc">Description of the radio station.</param>
      /// <param name="icon">Icon of the radio station.</param>
      /// <param name="iconUrl">Icon url of the radio station.</param>
      /// <param name="city">City of the radio station.</param>
      /// <param name="country">Country of the radio station (ISO 3166-1, e.g. 'ch').</param>
      /// <param name="language">Language of the radio station (like 'german').</param>
      /// <param name="format">AudioFormat of the station.</param>
      /// <param name="bitrate">Bitrate in kbit/s.</param>
      /// <param name="chunkSize">Size of the streaming-chunk in KB.</param>
      /// <param name="bufferSize">Size of the local buffer in KB.</param>
      /// <param name="excludeCodec">Excluded codec.</param>
      protected BaseRadioEntry(string name, bool forceName, bool enableSource, string station, string genres, float rating, string desc, Sprite icon, string iconUrl, string city, string country, string language, AudioFormat format, int bitrate, int chunkSize, int bufferSize, AudioCodec excludeCodec) : this()
      {
         Name = name;
         ForceName = forceName;
         EnableSource = enableSource;
         Station = station;
         Genres = genres;
         Rating = rating;
         Description = desc;
         Icon = icon;
         IconUrl = iconUrl;
         City = city;
         Country = country;
         Language = language;
         Format = format;
         Bitrate = bitrate;
         ChunkSize = chunkSize;
         BufferSize = bufferSize;
         ExcludedCodec = excludeCodec;
      }

      #endregion


      #region Overridden methods

      public override string ToString()
      {
         System.Text.StringBuilder result = new System.Text.StringBuilder();

         result.Append(GetType().Name);
         result.Append(Constants.TEXT_TOSTRING_START);

         result.Append("Name='");
         result.Append(Name);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("ForceName='");
         result.Append(ForceName);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("EnableSource='");
         result.Append(EnableSource);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("Station='");
         result.Append(Station);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("Genres='");
         result.Append(Genres);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("Rating='");
         result.Append(Rating);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("Description='");
         result.Append(Description);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("Icon='");
         result.Append(Icon);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("IconUrl='");
         result.Append(IconUrl);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("City='");
         result.Append(City);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("Country='");
         result.Append(Country);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("Language='");
         result.Append(Language);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("AudioFormat='");
         result.Append(Format);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("Bitrate='");
         result.Append(Bitrate);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("ChunkSize='");
         result.Append(ChunkSize);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("BufferSize='");
         result.Append(BufferSize);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("ExcludedCodec='");
         result.Append(ExcludedCodec);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("isInitialized='");
         result.Append(isInitialized);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER_END);

         result.Append(Constants.TEXT_TOSTRING_END);

         return result.ToString();
      }

      #endregion
   }
}
// © 2016-2024 crosstales LLC (https://www.crosstales.com)