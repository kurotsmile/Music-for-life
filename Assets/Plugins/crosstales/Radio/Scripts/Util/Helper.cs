using UnityEngine;
using System.Linq;
using Crosstales.Radio.Model;
using Crosstales.Radio.Model.Enum;

namespace Crosstales.Radio.Util
{
   /// <summary>Various helper functions.</summary>
   public abstract class Helper : Crosstales.Common.Util.BaseHelper
   {
      #region Static variables

      private static readonly int[] mp3Bitrates =
      {
         32,
         40,
         48,
         56,
         64,
         80,
         96,
         112,
         128,
         160,
         192,
         224,
         256,
         320
      };

      private static readonly int[] oggBitrates =
      {
         32,
         45,
         48,
         64,
         80,
         96,
         112,
         128,
         160,
         192,
         224,
         256,
         320,
         500
      };

      #endregion


      #region Static properties

      /// <summary>Checks if the current platform is supported.</summary>
      /// <returns>True if the current platform is supported.</returns>
      public static bool isSupportedPlatform => !isWSAPlatform && !isWebPlatform;

      #endregion


      #region Public methods

      /// <summary>Checks if the given RadioStation is sane.</summary>
      /// <returns>True if the given RadioStation is sane.</returns>
      public static bool isSane(ref RadioStation station)
      {
         if (station == null)
         {
            Debug.LogError("'Station' is null!" + System.Environment.NewLine + "Add a valid station to the player.");

            return false;
         }

         if (string.IsNullOrEmpty(station.Url))
         {
            Debug.LogError(station + System.Environment.NewLine + "'Url' is null or empty!" + System.Environment.NewLine + "Cannot start playback -> please add a valid url of a radio station (see 'Radios.txt' for some examples).");

            return false;
         }

         if (!Crosstales.Common.Util.NetworkHelper.isURL(station.Url))
         {
            Debug.LogError(station + System.Environment.NewLine + "'Url' is not valid!" + System.Environment.NewLine + "Cannot start playback -> please add a valid url of a radio station (see 'Radios.txt' for some examples).");

            return false;
         }

         if (!isValidFormat(station.Format))
         {
            Debug.LogError(station + System.Environment.NewLine + "'Format' is invalid: '" + station.Format + "'" + System.Environment.NewLine + "Cannot start playback -> please add a valid audio format for a radio station (see 'Radios.txt' for some examples).");

            return false;
         }

         if (!isValidBitrate(station.Bitrate, station.Format))
         {
            Debug.LogWarning(station + System.Environment.NewLine + "'Bitrate' is invalid: " + station.Bitrate + System.Environment.NewLine + "Automatically using " + Config.DEFAULT_BITRATE + " kbit/s for playback.");
            station.Bitrate = Config.DEFAULT_BITRATE;
         }

         if (station.ChunkSize < 1)
         {
            Debug.LogWarning(station + System.Environment.NewLine + "'ChunkSize' is smaller than 1KB!" + System.Environment.NewLine + "Automatically using " + Config.DEFAULT_CHUNKSIZE + "KB for playback.");
            station.ChunkSize = Config.DEFAULT_CHUNKSIZE;
         }

         if (station.Format == AudioFormat.MP3)
         {
            if (station.BufferSize < Config.DEFAULT_BUFFERSIZE / 4)
            {
               Debug.LogWarning(station + System.Environment.NewLine + "'BufferSize' is smaller than DEFAULT_BUFFERSIZE/4!" + System.Environment.NewLine + "Automatically using " + Config.DEFAULT_BUFFERSIZE / 4 + "KB for playback.");
               station.BufferSize = Config.DEFAULT_BUFFERSIZE / 4;
            }
         }
         else
         {
            if (station.BufferSize < Constants.MIN_OGG_BUFFERSIZE)
            {
               station.BufferSize = Constants.MIN_OGG_BUFFERSIZE;
            }
         }

         if (station.BufferSize < station.ChunkSize)
         {
            Debug.LogWarning(station + System.Environment.NewLine + "'BufferSize' is smaller than 'ChunkSize'!" + System.Environment.NewLine + "Automatically using " + station.ChunkSize + "KB for playback.");
            station.BufferSize = station.ChunkSize;
         }

         return true;
      }

      /// <summary>Save all stations as M3U file.</summary>
      /// <param name="filePath">Path for the file</param>
      /// <param name="stations">Stations to save</param>
      public static void SaveAsM3U(string filePath, System.Collections.Generic.List<RadioStation> stations)
      {
         System.Text.StringBuilder sb = new System.Text.StringBuilder();

         sb.AppendLine(Constants.M3U_EXT_ID);

         foreach (RadioStation station in stations)
         {
            sb.Append(Constants.M3U_EXT_INF_ID);
            sb.Append(":0,");
            sb.Append(station.Name);
            sb.AppendLine();
            sb.AppendLine(station.Url);
         }

         try
         {
            Crosstales.Common.Util.FileHelper.WriteAllText(filePath, sb.ToString());
         }
         catch (System.Exception ex)
         {
            Debug.LogError("Could not save file: " + filePath + System.Environment.NewLine + ex);
         }
      }

      /// <summary>Save all stations as PLS file.</summary>
      /// <param name="filePath">Path for the file</param>
      /// <param name="stations">Stations to save</param>
      public static void SaveAsPLS(string filePath, System.Collections.Generic.List<RadioStation> stations)
      {
         System.Text.StringBuilder sb = new System.Text.StringBuilder();

         sb.AppendLine("[playlist]");

         int ii = 1;

         foreach (RadioStation station in stations)
         {
            sb.Append("File");
            sb.Append(ii);
            sb.Append("=");
            sb.Append(station.Url);
            sb.AppendLine();
            sb.Append("Title");
            sb.Append(ii);
            sb.Append("=");
            sb.Append(station.Name);
            sb.AppendLine();
            sb.Append("Length");
            sb.Append(ii);
            sb.Append("=-1");
            sb.AppendLine();

            ii++;
         }

         sb.Append("NumberOfEntries=");
         sb.Append(stations.Count);
         sb.AppendLine();
         sb.AppendLine("Version=2");
         sb.AppendLine();

         try
         {
            Crosstales.Common.Util.FileHelper.WriteAllText(filePath, sb.ToString());
         }
         catch (System.Exception ex)
         {
            Debug.LogError("Could not save file: " + filePath + System.Environment.NewLine + ex);
         }
      }

      /// <summary>Save all stations as PLS file.</summary>
      /// <param name="filePath">Path for the file</param>
      /// <param name="stations">Stations to save</param>
      public static void SaveAsXSPF(string filePath, System.Collections.Generic.List<RadioStation> stations)
      {
         System.Text.StringBuilder sb = new System.Text.StringBuilder();

         sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
         sb.AppendLine("<playlist version=\"1\" xmlns=\"http://xspf.org/ns/0/\" >");

         sb.Append("<creator>");
         sb.Append(Constants.ASSET_NAME);
         sb.AppendLine("</creator>");

         sb.Append("<info>");
         sb.Append(Constants.ASSET_AUTHOR_URL);
         sb.AppendLine("</info>");

         sb.AppendLine("<trackList>");

         foreach (RadioStation station in stations)
         {
            sb.AppendLine("<track>");

            sb.Append("<location>");
            sb.Append(cleanForXSPF(station.Url));
            sb.AppendLine("</location>");

            sb.Append("<creator>");
            sb.Append(cleanForXSPF(station.Name)); //normally the artist, e.g.: Led Zeppelin
            sb.AppendLine("</creator>");

            sb.Append("<album>");
            sb.Append(cleanForXSPF(station.Name)); //normally the album, e.g.: Houses of the Holy
            sb.AppendLine("</album>");

            sb.Append("<title>");
            sb.Append(cleanForXSPF(station.Name)); //normally the title, e.g.: No Quarter
            sb.AppendLine("</title>");

            if (!string.IsNullOrEmpty(station.Description))
            {
               sb.Append("<annotation>");
               sb.Append(cleanForXSPF(station.Description));
               sb.AppendLine("</annotation>");
            }

            if (station.Station.StartsWith("http", System.StringComparison.OrdinalIgnoreCase))
            {
               sb.Append("<info>");
               sb.Append(cleanForXSPF(station.Station));
               sb.AppendLine("</info>");
            }

            sb.AppendLine("</track>");
         }

         sb.AppendLine("</trackList>");
         sb.AppendLine("</playlist>");

         try
         {
            Crosstales.Common.Util.FileHelper.WriteAllText(filePath, sb.ToString());
         }
         catch (System.Exception ex)
         {
            Debug.LogError("Could not save file: " + filePath + System.Environment.NewLine + ex);
         }
      }

      /// <summary>Converts a string to an AudioFormat. If the format couldn't be determined, the method returns AudioFormat.MP3.</summary>
      /// <param name="format">Audio format as string to convert</param>
      /// <returns>Converted AudioFormat.</returns>
      public static AudioFormat AudioFormatFromString(string format)
      {
         AudioFormat result = AudioFormat.MP3; //set MP3 as default!

         if (!string.IsNullOrEmpty(format))
         {
            if (format.CTEquals("ogg"))
            {
               result = AudioFormat.OGG;
            }
         }

         return result;
      }

      /// <summary>Converts a string to an AudioCodec. If the codec couldn't be determined, the method returns AudioCodec.None.</summary>
      /// <param name="codec">Audio codec as string to convert</param>
      /// <returns>Converted AudioCodec.</returns>
      public static AudioCodec AudioCodecFromString(string codec)
      {
         AudioCodec result = AudioCodec.None; //set NONE as default!

         if (!string.IsNullOrEmpty(codec))
         {
            if (codec.CTEquals("MP3_NLayer"))
            {
               result = AudioCodec.MP3_NLayer;
            }
            else if (codec.CTEquals("MP3_NAudio"))
            {
               result = AudioCodec.MP3_NAudio;
            }
            else if (codec.CTEquals("OGG_NVorbis"))
            {
               result = AudioCodec.OGG_NVorbis;
            }
         }

         return result;
      }

      /// <summary>Converts an AudioFormat to an AudioCodec for the current platform. If the codec couldn't be determined, the method returns AudioCodec.None.</summary>
      /// <param name="format">AudioFormat to convert</param>
      /// <returns>Converted AudioCodec.</returns>
      public static AudioCodec AudioCodecForAudioFormat(AudioFormat format)
      {
         switch (format)
         {
            case AudioFormat.MP3:
               return isWindowsPlatform && !isMacOSEditor && !isLinuxEditor ? Constants.DEFAULT_CODEC_MP3_WINDOWS : Constants.DEFAULT_CODEC_MP3;
            case AudioFormat.OGG:
               return AudioCodec.OGG_NVorbis;
            default:
               return AudioCodec.None;
         }
      }

      /// <summary>Checks if an AudioFormat is valid.</summary>
      /// <param name="format">AudioFormat to check</param>
      /// <returns>True if the AudioFormat is valid.</returns>
      public static bool isValidFormat(AudioFormat format)
      {
         return format == AudioFormat.MP3 || format == AudioFormat.OGG;
      }

      /// <summary>Returns the nearest bitrate for a given value and an AudioFormat.</summary>
      /// <param name="bitrate">Bitrate value as base value for the bitrate</param>
      /// <param name="format">AudioFormat for the bitrate definition</param>
      /// <returns>The nearest bitrate for the given value and AudioFormat.</returns>
      public static int NearestBitrate(int bitrate, AudioFormat format)
      {
         return format == AudioFormat.MP3 ? NearestMP3Bitrate(bitrate) : NearestOGGBitrate(bitrate);
      }

      /// <summary>Returns the nearest bitrate for a given value and MP3.</summary>
      /// <param name="bitrate">Bitrate value as base value for the bitrate</param>
      /// <returns>The nearest bitrate for the given value and MP3.</returns>
      public static int NearestMP3Bitrate(int bitrate)
      {
         return mp3Bitrates.Aggregate((x, y) => System.Math.Abs(x - bitrate) < System.Math.Abs(y - bitrate) ? x : y);
      }

      /// <summary>Returns the nearest bitrate for a given value and OGG.</summary>
      /// <param name="bitrate">Bitrate value as base value for the bitrate</param>
      /// <returns>The nearest bitrate for the given value and OGG.</returns>
      public static int NearestOGGBitrate(int bitrate)
      {
         return oggBitrates.Aggregate((x, y) => System.Math.Abs(x - bitrate) < System.Math.Abs(y - bitrate) ? x : y);
      }


      /// <summary>Checks if a bitrate for an AudioFormat is valid.</summary>
      /// <param name="bitrate">Bitrate to check</param>
      /// <param name="format">AudioFormat to check</param>
      /// <returns>True if the bitrate for the AudioFormat is valid.</returns>
      public static bool isValidBitrate(int bitrate, AudioFormat format)
      {
         return format == AudioFormat.MP3 ? isValidMP3Bitrate(bitrate) : isValidOGGBitrate(bitrate);
      }

      /// <summary>Checks if the MP3 bitrate is valid.</summary>
      /// <param name="bitrate">Bitrate to check</param>
      /// <returns>True if the MP3 bitrate is valid.</returns>
      public static bool isValidMP3Bitrate(int bitrate)
      {
         return mp3Bitrates.Contains(bitrate);
      }

      /// <summary>Checks if the OGG bitrate is valid.</summary>
      /// <param name="bitrate">Bitrate to check</param>
      /// <returns>True if the OGG bitrate is valid.</returns>
      public static bool isValidOGGBitrate(int bitrate)
      {
         return oggBitrates.Contains(bitrate);
      }

      #endregion


      #region Private methods

      private static string cleanForXSPF(string input)
      {
         return input.Replace("&", "+").Replace("°", "");
      }

      #endregion
   }
}
// © 2015-2024 crosstales LLC (https://www.crosstales.com)