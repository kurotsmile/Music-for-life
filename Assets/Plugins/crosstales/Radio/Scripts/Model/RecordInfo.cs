using UnityEngine;
using Crosstales.Radio.Util;

namespace Crosstales.Radio.Model
{
   /// <summary>Contains information about the current audio record from a radio station (for Icecast-servers).</summary>
   [System.Serializable]
   public class RecordInfo
   {
      #region Variables

      /// <summary>Original, unparsed information as string.</summary>
      [Tooltip("Original, unparsed information as string.")] public string Info = string.Empty;

      /// <summary>Your rating of the record.</summary>
      [Tooltip("Your rating of the audio record.")] [Range(0, 5f)] public float Rating;

      /// <summary>Icon representing the record.</summary>
      [Tooltip("Icon representing the record.")] [System.Xml.Serialization.XmlIgnore] public Sprite Icon;

      /// <summary>Icon url for the record.</summary>
      [Tooltip("Icon url for the record.")] public string IconUrl;

      /// <summary>Duration of the record in seconds (after playback).</summary>
      [HideInInspector] [Tooltip("Duration of the record in seconds (after playback).")] public float Duration;

      private string title;
      private string artist;
      private string streamTitle;
      private string streamUrl;

      private string spotifyUrl;

      //private string itunesUrl;
      private string lyricsUrl;
      private System.DateTime created = System.DateTime.Now;

      private const string streamTitleString = "StreamTitle='";
      private const string streamUrlString = "StreamUrl='";

      private static readonly string[] splitChar = { " - " };
      private static readonly System.Text.RegularExpressions.Regex lyricsRegex = new System.Text.RegularExpressions.Regex(@"[^A-Za-z0-9]+");

      #endregion


      #region Properties

      /// <summary>Returns the title of the audio record.</summary>
      /// <returns>Title of the audio record.</returns>
      public string Title
      {
         get
         {
            if (title == null)
            {
               parseStreamTitle();
            }

            return title;
         }

         set => title = value;
      }

      /// <summary>Returns the artist of the audio record.</summary>
      /// <returns>Artist of the audio record.</returns>
      public string Artist
      {
         get
         {
            if (artist == null)
            {
               parseStreamTitle();
            }

            return artist;
         }

         set => artist = value;
      }

      /// <summary>Returns the content of the 'StreamTitle'-tag.</summary>
      /// <returns>Content of the 'StreamTitle'-tag.</returns>
      public string StreamTitle
      {
         get
         {
            if (streamTitle == null)
            {
               parseStreamTitle();
            }

            return streamTitle;
         }

         set => streamTitle = value;
      }


      /// <summary>Returns the content of the 'StreamUrl'-tag.</summary>
      /// <returns>Content of the 'StreamUrl'-tag.</returns>
      public string StreamUrl
      {
         get
         {
            if (streamUrl == null)
            {
               parseStreamUrl();
            }

            return streamUrl;
         }

         set => streamUrl = value;
      }

      /// <summary>Returns the Spotify-url for the record.</summary>
      /// <returns>Spotify-url for the record.</returns>
      public string SpotifyUrl
      {
         get
         {
            if (spotifyUrl == null)
               parseSpotifyUrl();

            return spotifyUrl;
         }
      }

      /*
      /// <summary>Returns the iTunes-url for the record.</summary>
      /// <returns>iTunes-url for the record.</returns>
      public string iTunesUrl
      {
          get
          {
              if (itunesUrl == null)
              {
                  parseITunesUrl();
              }

              return itunesUrl;
          }
      }
      */

      /// <summary>Returns the lyrics-url for the record.</summary>
      /// <returns>Lyrics-url for the record.</returns>
      public string LyricsUrl
      {
         get
         {
            if (lyricsUrl == null)
            {
               parseLyricsUrl();
            }

            return lyricsUrl;
         }
      }

      /// <summary>Returns the creation time of the RecordInfo.</summary>
      /// <returns>Creation time of the RecordInfo.</returns>
      public System.DateTime Created
      {
         get => created;

         set => created = value;
      }

      #endregion


      #region Constructors

      /// <summary>Default-constructor for a RecordInfo.</summary>
      public RecordInfo()
      {
         //empty
      }

      /// <summary>Constructor for a RecordInfo.</summary>
      /// <param name="info">Information as string.</param>
      public RecordInfo(string info) : this()
      {
         Info = info.Trim();
         parseStreamTitle();
         //Info = System.Text.RegularExpressions.Regex.Replace(info.Trim(), @"\s+", " ");
      }

      #endregion


      #region Public methods

      /// <summary>ToString()-variant for displaying the object in the Editor.</summary>
      /// <returns>Text description of the object.</returns>
      public string ToShortString()
      {
         System.Text.StringBuilder result = new System.Text.StringBuilder();

         result.Append("Title='");
         result.Append(Title);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("Artist='");
         result.Append(Artist);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("Duration='");
         result.Append(Duration);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("Created='");
         result.Append(Created);
         result.Append("'");

         return result.ToString();
      }

      #endregion


      #region Private methods

      private void parseStreamTitle()
      {
         title = string.Empty;
         artist = string.Empty;
         streamTitle = string.Empty;

         int startIndex = Info.IndexOf(streamTitleString, System.StringComparison.OrdinalIgnoreCase);

         if (startIndex != -1)
         {
            string part = Info.Substring(startIndex + streamTitleString.Length);

            int endIndex = part.CTIndexOf("';");

            if (endIndex != -1)
            {
               streamTitle = part.Substring(0, endIndex).Replace('˗', '-').Replace('@', ' ').Replace('*', ' ').Replace('+', ' ').Replace(Constants.PATH_DELIMITER_WINDOWS, "-").Replace(Constants.PATH_DELIMITER_UNIX, "-").Trim();

               string[] parts = streamTitle.Split(splitChar, System.StringSplitOptions.RemoveEmptyEntries);
               if (parts.Length >= 2)
               {
                  artist = parts[0].Trim().CTToTitleCase();

                  //handle special chars |
                  string titleTemp = parts[parts.Length - 1];
                  int charLocation = titleTemp.IndexOf('|');

                  if (charLocation > 0)
                  {
                     titleTemp = titleTemp.Substring(0, charLocation);
                  }

                  title = titleTemp.Trim().CTToTitleCase();
               }
               else if (parts.Length == 1)
               {
                  title = parts[0].Trim().CTToTitleCase();
               }
            }
         }
      }

      private void parseStreamUrl()
      {
         streamUrl = string.Empty;

         int startIndex = Info.IndexOf(streamUrlString, System.StringComparison.OrdinalIgnoreCase);

         if (startIndex != -1)
         {
            string part = Info.Substring(startIndex + streamUrlString.Length);

            int endIndex = part.CTIndexOf("'");

            if (endIndex != -1)
               streamUrl = part.Substring(0, endIndex); //.Replace('˗', '-');
         }
      }

      private void parseSpotifyUrl()
      {
         string artist = Artist?.Replace(' ', '+');
         string title = Title?.Replace(' ', '+');

         if (string.IsNullOrEmpty(artist) && string.IsNullOrEmpty(title))
         {
            spotifyUrl = null;
         }
         else if (string.IsNullOrEmpty(artist) || string.IsNullOrEmpty(title))
         {
            spotifyUrl = $"spotify:search:{artist}{title}";
         }
         else
         {
            spotifyUrl = $"spotify:search:{artist}+{title}";
         }
      }

      /*
              private void parseITunesUrl()
              {
                  itunesUrl = "itms://phobos.apple.com/WebObjects/MZSearch.woa/wa/advancedSearchResults?artistTerm=" + Artist.Replace(' ', '+') + "&songTerm=" + Title.Replace(' ', '+');
                  //itunesUrl = "https://search.itunes.apple.com/WebObjects/MZSearch.woa/wa/advancedSearch?genreIndex=1&amp;media=music&amp;restrict=true&amp;submit=seeAllLockups&amp;entity=songs&amp;allArtistNames=" + Artist.Replace(' ', '+') + "&amp;allTitle=" + Title.Replace(' ', '+');
              }
      */

      private void parseLyricsUrl()
      {
         string _artist = lyricsRegex.Replace(Artist.ToLower(), string.Empty);
         string _title = lyricsRegex.Replace(Title.ToLower(), string.Empty);

         if (_artist.CTStartsWith("the"))
            _artist = _artist.Substring(3);

         lyricsUrl = "https://azlyrics.com/lyrics/" + _artist + "/" + _title + ".html";

         //Debug.Log(lyricsUrl);
      }

      #endregion


      #region Overridden methods

      public override bool Equals(object obj)
      {
         if (obj == null || GetType() != obj.GetType())
            return false;

         RecordInfo ri = (RecordInfo)obj;

         return title == ri.title &&
                artist == ri.artist &&
                Info == ri.Info;
      }

      public override int GetHashCode()
      {
         int hash = 0;

         hash += (int)Rating;
         if (title != null)
            hash += title.GetHashCode();
         if (artist != null)
            hash += artist.GetHashCode();
         if (Info != null)
            hash += Info.GetHashCode();

         return hash;
      }

      public override string ToString()
      {
         System.Text.StringBuilder result = new System.Text.StringBuilder();

         result.Append(GetType().Name);
         result.Append(Constants.TEXT_TOSTRING_START);
/*
         result.Append("Info='");
         result.Append(Info.Trim());
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);
*/
         result.Append("Rating='");
         result.Append(Rating);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);
/*
         result.Append("Icon='");
         result.Append(Icon);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);
*/
         result.Append("IconUrl='");
         result.Append(IconUrl);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("Duration='");
         result.Append(Duration);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("Title='");
         result.Append(Title);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("Artist='");
         result.Append(Artist);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("StreamTitle='");
         result.Append(streamTitle);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("StreamUrl='");
         result.Append(StreamUrl);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("SpotifyUrl='");
         result.Append(SpotifyUrl);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("LyricsUrl='");
         result.Append(LyricsUrl);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("Created='");
         result.Append(Created);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER_END);

         result.Append(Constants.TEXT_TOSTRING_END);

         return result.ToString();
      }

      #endregion
   }
}
// © 2016-2024 crosstales LLC (https://www.crosstales.com)