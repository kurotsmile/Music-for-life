using UnityEngine;
using Crosstales.Radio.Util;
using Crosstales.Radio.Model.Enum;

namespace Crosstales.Radio.Model
{
   /// <summary>Model for a radio station.</summary>
   [System.Serializable]
   public class RadioStation
   {
      #region Variables

      /// <summary>Name of the radio station.</summary>
      [Header("Station Settings")] [Tooltip("Name of the radio station.")] public string Name;

      /// <summary>URL of the station.</summary>
      [Tooltip("URL of the station.")] public string Url;

      /// <summary>HTTP-request headers for the connection to the current radio station (e.g. authentication).</summary>
      public System.Collections.Generic.List<RequestHeaderTuple> RequestHeaders = new System.Collections.Generic.List<RequestHeaderTuple>();

      /// <summary>Name of the station.</summary>
      [Header("Meta Data")] [Tooltip("Name of the station.")] public string Station;

      /// <summary>Genres of the radio.</summary>
      [Tooltip("Genres of the radio.")] public string Genres;

      /// <summary>Your rating of the radio.</summary>
      [Tooltip("Your rating of the radio.")] [Range(0, 5f)] public float Rating;

      /// <summary>Description of the radio station.</summary>
      [Tooltip("Description of the radio station.")] [TextArea] public string Description;

      /// <summary>Icon representing the radio station.</summary>
      [Tooltip("Icon representing the radio station.")] [System.Xml.Serialization.XmlIgnore] public Sprite Icon;

      /// <summary>Icon url for the radio station.</summary>
      [Tooltip("Icon url for the radio station.")] public string IconUrl;

      /// <summary>City of the radio.</summary>
      [Tooltip("City of the radio.")] public string City;

      /// <summary>Country of the radio (ISO 3166-1, e.g. 'ch').</summary>
      [Tooltip("Country of the radio (ISO 3166-1, e.g. 'ch').")] public string Country;

      /// <summary>Language of the radio (like 'german').</summary>
      [Tooltip("Language of the radio (like 'german').")] public string Language;


      /// <summary>Audio format of the station (default: AudioFormat.MP3).</summary>
      [Header("Stream Settings")] [Tooltip("Audio format of the station.")] public AudioFormat Format = AudioFormat.MP3;

      /// <summary>Bitrate in kbit/s (default: 128).</summary>
      [Tooltip("Bitrate in kbit/s (default: 128).")] public int Bitrate = Config.DEFAULT_BITRATE;

      /// <summary>Size of the streaming-chunk in KB (default: 32).</summary>
      [Tooltip("Size of the streaming-chunk in KB (default: 32).")] public int ChunkSize = Config.DEFAULT_CHUNKSIZE;

      /// <summary>Size of the local buffer in KB (default: 48).</summary>
      [Tooltip("Size of the local buffer in KB (default: 48).")] public int BufferSize = Config.DEFAULT_BUFFERSIZE;

      /// <summary>Exclude this station if the current RadioPlayer codec is equals this one (default: AudioCodec.None).</summary>
      [Header("Codec Settings")] [Tooltip("Exclude this station if the current RadioPlayer codec is equals this one (default: AudioCodec.None).")]
      public AudioCodec ExcludedCodec = AudioCodec.None;


      /// <summary>Updates the data of the station when played (default: true).</summary>
      [Header("Data Settings")] [Tooltip("Updates the data of the station when played (default: true).")] public bool UpdateDataAtPlay = true;
      //[Tooltip("Updates the data of the station when played (default: true).")] public bool UpdateDataAtPlay = true;

      /// <summary>Channels of the station.</summary>
      [HideInInspector] public int Channels = 2;

      /// <summary>Sample rate of the station.</summary>
      [HideInInspector] public int SampleRate = 44100;

      /// <summary>Total downloaded data size in bytes.</summary>
      [HideInInspector] public long TotalDataSize;

      /// <summary>Total number of data requests.</summary>
      [HideInInspector] public int TotalDataRequests;

      /// <summary>Total playtime in seconds.</summary>
      [HideInInspector] public float TotalPlayTime;

      /// <summary>List of all played records.</summary>
      [HideInInspector] public System.Collections.Generic.List<RecordInfo> PlayedRecords = new System.Collections.Generic.List<RecordInfo>();

      /// <summary>Information about the streaming server (if available).</summary>
      [HideInInspector] public string ServerInfo = string.Empty;

/*
      /// <summary>Station-ID.</summary>
      [HideInInspector] public string ID; //TODO implement ID in text-files etc.
*/
      public const string UNKNOWN_STATION = "Unknown radio station";

      private const char splitCharText = ';';

      #endregion


      #region Constructors

      /// <summary>Default-constructor for a RadioStation.</summary>
      public RadioStation()
      {
         Name = UNKNOWN_STATION;
      }

      /// <summary>Constructor for a RadioStation.</summary>
      /// <param name="name">Name of the radio station.</param>
      /// <param name="url">Stream-URL of the station.</param>
      /// <param name="format">AudioFormat of the station.</param>
      public RadioStation(string name, string url, AudioFormat format)
      {
         //Name = name.CTToTitleCase();
         Name = string.IsNullOrEmpty(name) ? UNKNOWN_STATION : name;
         Url = url;
         Format = format;
      }

      /// <summary>Constructor for a RadioStation.</summary>
      /// <param name="name">Name of the radio station.</param>
      /// <param name="url">Stream-URL of the station.</param>
      /// <param name="format">AudioFormat of the station.</param>
      /// <param name="station">Name of the station.</param>
      /// <param name="genres">Genres of the radio.</param>
      /// <param name="bitrate">Bitrate in kbit/s.</param>
      /// <param name="rating">Your rating of the radio.</param>
      /// <param name="description">Description of the radio station.</param>
      /// <param name="icon">Icon of the radio station.</param>
      /// <param name="iconUrl">Icon url of the radio station.</param>
      /// <param name="city">City of the radio station.</param>
      /// <param name="country">Country of the radio station (ISO 3166-1, e.g. 'ch').</param>
      /// <param name="language">Language of the radio station (like 'german').</param>
      /// <param name="chunkSize">Size of the streaming-chunk in KB (default: 64, optional).</param>
      /// <param name="bufferSize">Size of the local buffer in KB (default: 64, optional).</param>
      /// <param name="excludeCodec">Excluded codec (default: AudioCodec.NONE, optional).</param>
      public RadioStation(string name, string url, AudioFormat format, string station, string genres, int bitrate, float rating, string description, Sprite icon, string iconUrl, string city, string country, string language, int chunkSize = 64, int bufferSize = 64, AudioCodec excludeCodec = AudioCodec.None) : this(name, url, format)
      {
         Station = station;
         Genres = genres.Replace(',', ' ');
         Bitrate = bitrate;
         Rating = rating;
         Description = description;
         Icon = icon;
         IconUrl = iconUrl;
         City = city;
         Country = country;
         Language = language;
         ChunkSize = chunkSize;
         BufferSize = bufferSize;
         ExcludedCodec = excludeCodec;
      }

      /// <summary>Copy constructor for a given RadioStation.</summary>
      /// <param name="station">Station to copy.</param>
      public RadioStation(RadioStation station)
      {
         Name = station.Name;
         Url = station.Url;
         RequestHeaders = station.RequestHeaders;
         Station = station.Station;
         Genres = station.Genres.Replace(',', ' ');
         Rating = station.Rating;
         Description = station.Description;
         Icon = station.Icon;
         IconUrl = station.IconUrl;
         City = station.City;
         Country = station.Country;
         Language = station.Language;
         Format = station.Format;
         Bitrate = station.Bitrate;
         ChunkSize = station.ChunkSize;
         BufferSize = station.BufferSize;
         ExcludedCodec = station.ExcludedCodec;
         UpdateDataAtPlay = station.UpdateDataAtPlay;
         Channels = station.Channels;
         SampleRate = station.SampleRate;
      }

      #endregion


      #region Public methods

      /// <summary>ToString()-variant for exporting the object.</summary>
      /// <param name="detailed">Detailed export with Chunk- and Buffer-size.</param>
      /// <returns>Text-line of the object.</returns>
      public string ToTextLine(bool detailed = true)
      {
         System.Text.StringBuilder result = new System.Text.StringBuilder();

         result.Append(Name.Replace(splitCharText, '-'));
         result.Append(splitCharText);

         result.Append(Url.Replace(splitCharText, ' '));
         result.Append(splitCharText);

         result.Append("Stream");
         result.Append(splitCharText);

         result.Append(Format);
         result.Append(splitCharText);

         result.Append(Station.Replace(splitCharText, '-'));
         result.Append(splitCharText);

         result.Append(Genres.Replace(splitCharText, ','));
         result.Append(splitCharText);

         result.Append(Bitrate);
         result.Append(splitCharText);

         result.Append(Rating.ToString("N1"));
         result.Append(splitCharText);

         result.Append(Description.Replace(splitCharText, '-'));
         result.Append(splitCharText);

         result.Append(ExcludedCodec);

         if (detailed)
         {
            result.Append(splitCharText);

            result.Append(ChunkSize);
            result.Append(splitCharText);

            result.Append(BufferSize);
            result.Append(splitCharText);

            result.Append(IconUrl.Replace(splitCharText, ' '));
            result.Append(splitCharText);

            result.Append(City.Replace(splitCharText, '-'));
            result.Append(splitCharText);

            result.Append(Country.Replace(splitCharText, '-'));
            result.Append(splitCharText);

            result.Append(Language.Replace(splitCharText, '-'));
         }

         return result.ToString();
      }

      /// <summary>ToString()-variant for displaying the object in the Editor.</summary>
      /// <returns>Text description of the object.</returns>
      public string ToShortString()
      {
         System.Text.StringBuilder result = new System.Text.StringBuilder();

         result.Append("Name='");
         result.Append(Name);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("Url='");
         result.Append(Url);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("Station='");
         result.Append(Station);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("Genres='");
         result.Append(Genres);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("Rating='");
         result.Append(Rating.ToString("N1"));
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
         result.Append("'");

         return result.ToString();
      }

      /// <summary>Shows the complete station information.</summary>
      /// <param name="withLabel">Add the label for every information (default: true, optional)</param>
      /// <param name="maxLength">Maximal length of a row (default: 0 (= unlimited), optional)</param>
      /// <param name="shortInfo">Reduced information (default: false, optional)</param>
      /// <returns>The complete station information.</returns>
      public string StationInfo(bool withLabel = true, int maxLength = 0, bool shortInfo = false)
      {
         System.Text.StringBuilder sb = new System.Text.StringBuilder();

         if (!shortInfo)
         {
            if (withLabel)
               sb.Append("Name:\t");

            sb.AppendLine(limitLength(Name, maxLength));

            if (withLabel)
               sb.Append("Url:\t");

            sb.AppendLine(limitLength(Url, maxLength));

            if (withLabel)
               sb.Append("Station:\t");

            sb.AppendLine(limitLength(Station, maxLength));

            if (withLabel)
               sb.Append("Rating:\t");

            sb.AppendLine(limitLength(Rating.ToString("N1"), maxLength));
         }

         if (!string.IsNullOrEmpty(Genres))
         {
            if (withLabel)
               sb.Append("Genres:\t");

            sb.AppendLine(limitLength(Genres.CTToTitleCase(), maxLength));
         }

         if (!shortInfo && !string.IsNullOrEmpty(Description))
         {
            if (withLabel)
               sb.Append("Description:\t");

            sb.AppendLine(limitLength(Description, maxLength));
         }

         if (!string.IsNullOrEmpty(City))
         {
            if (withLabel)
               sb.Append("City:\t");

            sb.AppendLine(limitLength(City, maxLength));
         }

         if (!string.IsNullOrEmpty(Country))
         {
            if (withLabel)
               sb.Append("Country:\t");

            sb.AppendLine(limitLength(Country, maxLength));
         }

         if (!string.IsNullOrEmpty(Language))
         {
            if (withLabel)
               sb.Append("Language:\t");

            sb.AppendLine(limitLength(Language, maxLength));
         }

         if (withLabel)
            sb.Append("Format:\t");

         sb.AppendLine(limitLength($"{Format} ({Bitrate} kbps)", maxLength));

         return sb.ToString();
      }

      /// <summary>Shows the labels for the complete station information.</summary>
      /// <param name="shortInfo">Reduced information (default: false, optional)</param>
      /// <returns>The complete station information.</returns>
      public string StationInfoLabels(bool shortInfo = false)
      {
         System.Text.StringBuilder sb = new System.Text.StringBuilder();

         if (!shortInfo)
         {
            sb.AppendLine("Name:");

            sb.AppendLine("Url:");

            sb.AppendLine("Station:");

            sb.AppendLine("Rating:");
         }

         if (!string.IsNullOrEmpty(Genres))
         {
            sb.AppendLine("Genres:");
         }

         if (!shortInfo && !string.IsNullOrEmpty(Description))
         {
            sb.AppendLine("Description:");
         }

         if (!string.IsNullOrEmpty(City))
         {
            sb.AppendLine("City:");
         }

         if (!string.IsNullOrEmpty(Country))
         {
            sb.AppendLine("Country:");
         }

         if (!string.IsNullOrEmpty(Language))
         {
            sb.AppendLine("Language:");
         }

         sb.AppendLine("Format:");

         return sb.ToString();
      }

      #endregion


      #region Private methods

      private static string limitLength(string text, int maxLength)
      {
         if (!string.IsNullOrEmpty(text) && maxLength > 0 && text.Length > maxLength)
         {
            return text.Substring(0, maxLength - 1) + "...";
         }

         return text;
      }

      #endregion


      #region Overridden methods

      public override bool Equals(object obj)
      {
         if (obj == null || GetType() != obj.GetType())
            return false;

         RadioStation rs = (RadioStation)obj;

         return Url == rs.Url;
      }

      public override int GetHashCode()
      {
         return Url == null ? base.GetHashCode() : Url.GetHashCode();
      }

      public override string ToString()
      {
         System.Text.StringBuilder result = new System.Text.StringBuilder();

         result.Append(GetType().Name);
         result.Append(Constants.TEXT_TOSTRING_START);

         result.Append("Name='");
         result.Append(Name);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("Url='");
         result.Append(Url);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("RequestHeaders='");
         result.Append(RequestHeaders?.Count);
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
/*
         result.Append("Icon='");
         result.Append(Icon);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);
*/
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

         result.Append("UpdateDataAtPlay='");
         result.Append(UpdateDataAtPlay);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("Channels='");
         result.Append(Channels);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("SampleRate='");
         result.Append(SampleRate);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("TotalDataSize='");
         result.Append(TotalDataSize);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("TotalDataRequests='");
         result.Append(TotalDataRequests);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("TotalPlayTime='");
         result.Append(TotalPlayTime);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("PlayedRecords='");
         result.Append(PlayedRecords?.Count);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("ServerInfo='");
         result.Append(ServerInfo);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER_END);

         result.Append(Constants.TEXT_TOSTRING_END);

         return result.ToString();
      }

      #endregion
   }

   [System.Serializable]
   public class RequestHeaderTuple
   {
      public string Key;
      public string Value;
   }
}
// © 2015-2024 crosstales LLC (https://www.crosstales.com)