using UnityEngine;
using Crosstales.Radio.Model.Enum;

namespace Crosstales.Radio.Util
{
   /// <summary>Collected constants of very general utility for the asset.</summary>
   public abstract class Constants : Crosstales.Common.Util.BaseConstants
   {
      #region Constant variables

      /// <summary>Name of the asset.</summary>
      public const string ASSET_NAME = "Radio PRO";

      /// <summary>Version of the asset.</summary>
      public const string ASSET_VERSION = "2024.1.2";

      /// <summary>Build number of the asset.</summary>
      public const int ASSET_BUILD = 20240315;

      /// <summary>Create date of the asset (YYYY, MM, DD).</summary>
      public static readonly System.DateTime ASSET_CREATED = new System.DateTime(2015, 2, 25);

      /// <summary>Change date of the asset (YYYY, MM, DD).</summary>
      public static readonly System.DateTime ASSET_CHANGED = new System.DateTime(2024, 3, 15);

      /// <summary>URL of the PRO asset in UAS.</summary>
      public const string ASSET_PRO_URL = "https://assetstore.unity.com/packages/slug/32034?aid=1011lNGT";

      /// <summary>URL of the 3rd party assets in UAS.</summary>
      public const string ASSET_3P_URL = "https://assetstore.unity.com/lists/radio-friends-42211?aid=1011lNGT"; // Radio&Friends list

      /// <summary>URL for update-checks of the asset</summary>
      public const string ASSET_UPDATE_CHECK_URL = "https://www.crosstales.com/media/assets/radio_versions.txt";
      //public const string ASSET_UPDATE_CHECK_URL = "https://www.crosstales.com/media/assets/test/radio_versions_test.txt";

      /// <summary>Contact to the owner of the asset.</summary>
      public const string ASSET_CONTACT = "radio@crosstales.com";

      /// <summary>URL of the asset manual.</summary>
      public const string ASSET_MANUAL_URL = "https://www.crosstales.com/media/data/assets/radio/Radio-doc.pdf";

      /// <summary>URL of the asset API.</summary>
      public const string ASSET_API_URL = "https://www.crosstales.com/en/assets/radio/api";

      /// <summary>URL of the asset forum.</summary>
      public const string ASSET_FORUM_URL = "https://forum.unity.com/threads/radio-pro-mp3-and-ogg-streaming-solution.334604/";

      /// <summary>URL of the asset in crosstales.</summary>
      public const string ASSET_WEB_URL = "https://www.crosstales.com/en/portfolio/radio/";

      /// <summary>URL of the promotion video of the asset (Youtube).</summary>
      public const string ASSET_VIDEO_PROMO = "https://youtu.be/1ZsxY788w-w?list=PLgtonIOr6Tb41XTMeeZ836tjHlKgOO84S";

      /// <summary>URL of the tutorial video of the asset (Youtube).</summary>
      public const string ASSET_VIDEO_TUTORIAL = "https://youtu.be/E0s0NVRX-ec?list=PLgtonIOr6Tb41XTMeeZ836tjHlKgOO84S";

      /// <summary>URL of the 3rd party asset "Audio Visualizer".</summary>
      public const string ASSET_3P_AUDIO_VISUALIZER = "https://assetstore.unity.com/packages/slug/47866?aid=1011lNGT";

      /// <summary>URL of the 3rd party asset "Complete Sound Suite".</summary>
      public const string ASSET_3P_SOUND_SUITE = "https://assetstore.unity.com/packages/slug/19994?aid=1011lNGT";

      /// <summary>URL of the 3rd party asset "Visualizer Studio".</summary>
      public const string ASSET_3P_VISUALIZER_STUDIO = "https://assetstore.unity.com/packages/slug/1761?aid=1011lNGT";

      /// <summary>URL of the 3rd party asset "Apollo Visualizer Kit".</summary>
      public const string ASSET_3P_APOLLO_VISUALIZER = "https://assetstore.unity.com/packages/slug/59035?aid=1011lNGT";

      /// <summary>URL of the 3rd party asset "Rhythm Visualizator Pro".</summary>
      public const string ASSET_3P_RHYTHM_VISUALIZATOR = "https://assetstore.unity.com/packages/slug/88041?aid=1011lNGT";

      public const string M3U_EXT_ID = "#EXTM3U";
      public const string M3U_EXT_INF_ID = "#EXTINF";
      public const string PLS_FILE_ID = "file";
      public const string PLS_TITLE_ID = "title";

      // Keys for the configuration of the asset
      public const string KEY_PREFIX = "RADIO_CFG_";

      //public const string KEY_ASSET_PATH = KEY_PREFIX + "ASSET_PATH";
      public const string KEY_DEBUG = KEY_PREFIX + "DEBUG";

      public const string KEY_DEFAULT_BITRATE = KEY_PREFIX + "DEFAULT_BITRATE";
      public const string KEY_DEFAULT_CHUNKSIZE = KEY_PREFIX + "DEFAULT_CHUNKSIZE";
      public const string KEY_DEFAULT_BUFFERSIZE = KEY_PREFIX + "DEFAULT_BUFFERSIZE";
      public const string KEY_DEFAULT_CACHESTREAMSIZE = KEY_PREFIX + "DEFAULT_CACHESTREAMSIZE";
      public const string KEY_MAX_CACHESTREAMSIZE = KEY_PREFIX + "MAX_CACHESTREAMSIZE";

      // Default values
      public const int DEFAULT_DEFAULT_BITRATE = 128; //128kbps (16KB/s)
      public const int DEFAULT_DEFAULT_CHUNKSIZE = 32; //in KB (32KB)
      public const int DEFAULT_DEFAULT_CACHESTREAMSIZE = 1 * FACTOR_KB; //in KB (1MB)
      public const int DEFAULT_MAX_CACHESTREAMSIZE = 16 * FACTOR_KB; //in KB (16MB)
      public const int DEFAULT_DEFAULT_BUFFERSIZE = 48; //in KB (48KB)

      /// <summary>Minimal buffer-size for OGG-streams.</summary>
      public const int MIN_OGG_BUFFERSIZE = 64; //in KB (64KB)

#if UNITY_2019_1_OR_NEWER
      public const string TAB = "\t\t";
#else
      public const string TAB = "\t";
#endif

      #endregion


      #region Changable variables

      // Technical settings
      /// <summary>Default MP3-codec.</summary>
      public static AudioCodec DEFAULT_CODEC_MP3 = AudioCodec.MP3_NLayer;

      /// <summary>Default MP3-codec under Windows.</summary>
#if ENABLE_IL2CPP
      public static AudioCodec DEFAULT_CODEC_MP3_WINDOWS = AudioCodec.MP3_NLayer;
#else
      public static AudioCodec DEFAULT_CODEC_MP3_WINDOWS = AudioCodec.MP3_NAudio;
      //public static AudioCodec DEFAULT_CODEC_MP3_WINDOWS = AudioCodec.MP3_NLayer;
#endif

      /// <summary>URL for the Shoutcast-Query.</summary>
      public static string SHOUTCAST = "https://yp.shoutcast.com/sbin/tunein-station.pls?id=";

      /// <summary>Delay for Invoke-calls (typically between a "Stop"- and "Play"-call).</summary>
      public const float INVOKE_DELAY = 0.3f;

      /// <summary>Delay for Buffer-calls (used to give the codecs some data if SkipBuffering is enabled).</summary>
      public static readonly WaitForSeconds WFS_BUFFER = new WaitForSeconds(0.5f);

      /// <summary>Maximal load wait time in in seconds.</summary>
      public static int MAX_LOAD_WAIT_TIME = 5;

      /// <summary>Maximal load time for web resources in seconds.</summary>
      public static int MAX_WEB_LOAD_WAIT_TIME = 8;

      /// <summary>Maximal load time for Shoutcast resources in seconds.</summary>
      public static int MAX_SHOUTCAST_LOAD_WAIT_TIME = 5;

      /// <summary>Defines the speed of 'Play'-calls in seconds.</summary>
      public static float PLAY_CALL_SPEED = 0.5f;

      /// <summary>Minimal interval for the OGG clean in frames.</summary>
      public static int OGG_CLEAN_INTERVAL_MIN = 1000;

      /// <summary>Maximal interval for the OGG clean in frames.</summary>
      public static int OGG_CLEAN_INTERVAL_MAX = 6000;

      /// <summary>Initial list size for players and stations.</summary>
      public static int INITIAL_LIST_SIZE = 250;

      // Text fragments for the asset
      public static string TEXT_BUFFER = "Buffer: ";
      public static string TEXT_STOPPED = "stopped";
      public static string TEXT_QUESTIONMARKS = "???";

      // Prefixes for paths
      public static string PREFIX_TEMP_PATH => Crosstales.Common.Util.FileHelper.TempPath;

      #endregion
   }
}
// © 2015-2024 crosstales LLC (https://www.crosstales.com)