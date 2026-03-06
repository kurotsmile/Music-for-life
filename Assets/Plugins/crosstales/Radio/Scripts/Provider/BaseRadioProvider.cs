using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Linq;
using Crosstales.Radio.Util;
using Crosstales.Radio.Model;
using Crosstales.Radio.Model.Entry;
using Crosstales.Radio.Model.Enum;

namespace Crosstales.Radio.Provider
{
   /// <summary>Base class for radio providers.</summary>
   [ExecuteInEditMode]
   public abstract class BaseRadioProvider : MonoBehaviour, IRadioProvider
   {
      #region Variables

      [UnityEngine.Serialization.FormerlySerializedAsAttribute("ClearStationsOnLoad")] [Header("Load Behaviour")] [Tooltip("Clears all existing stations on 'Load' (default: true)."), SerializeField]
      private bool clearStationsOnLoad = true;

      [UnityEngine.Serialization.FormerlySerializedAsAttribute("LoadOnStart")] [Tooltip("Calls 'Load' on Start (default: true)."), SerializeField]
      private bool loadOnStart = true;

      [UnityEngine.Serialization.FormerlySerializedAsAttribute("LoadOnStartInEditor")] [Tooltip("Calls 'Load' on Start in Editor (default: true)."), SerializeField]
      private bool loadOnStartInEditor = true;

      /// <summary>Allow only HTTPS streams (default: false).</summary>
      [Tooltip("Allow only HTTPS streams (default: false)."), SerializeField] private bool allowOnlyHTTPS;

      protected readonly System.Collections.Generic.List<string> coRoutines = new System.Collections.Generic.List<string>();

      private System.Collections.Generic.List<RadioStation> stations = new System.Collections.Generic.List<RadioStation>(Constants.INITIAL_LIST_SIZE);

      private bool loadedInEditor = true;

      private bool isReadySent;

      // split chars
      private static readonly char[] splitCharEquals = { '=' };
      private static readonly char[] splitCharText = { ';' };
      private static readonly char[] splitCharColon = { ':' };
      private static readonly char[] splitCharComma = { ',' };

      #endregion


      #region Properties

      /// <summary>Clears all existing stations on 'Load'.</summary>
      public bool ClearStationsOnLoad
      {
         get => clearStationsOnLoad;
         set => clearStationsOnLoad = value;
      }

      /// <summary>Calls 'Load' on Start.</summary>
      public bool LoadOnStart
      {
         get => loadOnStart;
         set => loadOnStart = value;
      }

      /// <summary>Calls 'Load' on Start in Editor.</summary>
      public bool LoadOnStartInEditor
      {
         get => loadOnStartInEditor;
         set => loadOnStartInEditor = value;
      }

      /// <summary>Allow only HTTPS streams.</summary>
      public bool AllowOnlyHTTPS
      {
         get => allowOnlyHTTPS;
         set => allowOnlyHTTPS = value;
      }

      protected abstract StationsChangeEvent onStationsChanged { get; }

      protected abstract ProviderReadyEvent onProviderReadyEvent { get; }

      #endregion


      #region Events

      /// <summary>An event triggered whenever the stations change.</summary>
      public event StationsChange OnStationsChange;

      /// <summary>An event triggered whenever the provider is ready.</summary>
      public event ProviderReady OnProviderReady;

      #endregion


      #region MonoBehaviour methods

      protected virtual void Start()
      {
         if (LoadOnStart && !Helper.isEditorMode || LoadOnStartInEditor && Helper.isEditorMode)
            Load();

         OnValidate();
      }

      private void Update()
      {
         if (!isReadySent && isReady)
         {
            isReadySent = true;
            onProviderReady();
         }
      }

      protected virtual void OnValidate()
      {
         foreach (BaseRadioEntry entry in RadioEntries.Where(entry => entry != null))
         {
            if (!entry.isInitialized)
            {
               entry.Format = AudioFormat.MP3;
               entry.EnableSource = true;

               entry.isInitialized = true;
            }

            entry.Bitrate = entry.Bitrate <= 0 ? Config.DEFAULT_BITRATE : Helper.NearestBitrate(entry.Bitrate, entry.Format);

            if (entry.ChunkSize <= 0)
            {
               entry.ChunkSize = Config.DEFAULT_CHUNKSIZE;
            }
            else if (entry.ChunkSize > Config.MAX_CACHESTREAMSIZE)
            {
               entry.ChunkSize = Config.MAX_CACHESTREAMSIZE;
            }

            if (entry.BufferSize <= 0)
            {
               entry.BufferSize = Config.DEFAULT_BUFFERSIZE;
            }
            else
            {
               switch (entry.Format)
               {
                  case AudioFormat.MP3:
                  {
                     if (entry.BufferSize < Config.DEFAULT_BUFFERSIZE / 4)
                     {
                        entry.BufferSize = Config.DEFAULT_BUFFERSIZE / 4;
                     }

                     break;
                  }
                  case AudioFormat.OGG:
                  {
                     if (entry.BufferSize < Constants.MIN_OGG_BUFFERSIZE)
                     {
                        entry.BufferSize = Constants.MIN_OGG_BUFFERSIZE;
                     }

                     break;
                  }
               }

               if (entry.BufferSize < entry.ChunkSize)
               {
                  entry.BufferSize = entry.ChunkSize;
               }
               else if (entry.BufferSize > Config.MAX_CACHESTREAMSIZE)
               {
                  entry.BufferSize = Config.MAX_CACHESTREAMSIZE;
               }
            }
         }
      }

      #endregion


      #region Implemented methods

      public abstract System.Collections.Generic.List<BaseRadioEntry> RadioEntries { get; }

      public System.Collections.Generic.List<RadioStation> Stations
      {
         get => stations;
         protected set => stations = value;
      }

      public virtual bool isReady
      {
         get
         {
            if (Helper.isEditorMode)
            {
               return loadedInEditor;
            }

            return coRoutines.Count == 0;
         }
      }

      public virtual void Load()
      {
         isReadySent = false;

         if (Helper.isEditorMode)
         {
#if UNITY_EDITOR
            initInEditor();
#endif
         }
         else
         {
            init();
         }
      }

      public void Save(string path)
      {
         if (!string.IsNullOrEmpty(path))
         {
#if (!UNITY_WSA && !UNITY_WEBGL && !UNITY_XBOXONE) || UNITY_EDITOR
            //Debug.Log($"Save: {path}");
            try
            {
               path = path.Replace(Constants.PREFIX_FILE, string.Empty); //remove file://-prefix

               using (System.IO.StreamWriter file = new System.IO.StreamWriter(path))
               {
                  file.WriteLine("# " + Constants.ASSET_NAME + " " + Constants.ASSET_VERSION);
                  file.WriteLine("# © 2015-2024 by " + Constants.ASSET_AUTHOR + " (" + Constants.ASSET_AUTHOR_URL + ")");
                  file.WriteLine("#");
                  file.WriteLine("# List of all radio stations from '" + GetType().Name + "'");
                  file.WriteLine("# Created: " + System.DateTime.Now.ToString("dd.MM.yyyy"));
                  file.WriteLine("# Name;Url;DataFormat;AudioFormat;Station (optional);Genres (optional);Bitrate (in kbit/s, optional);Rating (0-5, optional);Description (optional);ExcludeCodec (optional);ChunkSize (in KB, optional);BufferSize (in KB, optional);IconUrl (optional);City (optional);Country (optional);Language (optional)");

                  foreach (RadioStation rs in Stations)
                  {
                     file.WriteLine(rs.ToTextLine());
                  }
               }
            }
            catch (System.IO.IOException ex)
            {
               Debug.LogError("Could not write file: " + ex, this);
            }
#else
            Debug.LogWarning("'Save' is not supported on the currrent platform!", this);
#endif
         }
         else
         {
            Debug.LogWarning("'path' was null or empty! Could not save the data!", this);
         }
      }

      #endregion


      #region Private methods

      protected virtual void init()
      {
         if (ClearStationsOnLoad)
            Stations.Clear();
      }

      protected IEnumerator loadWeb(string uid, RadioEntryURL entry, bool suppressDoubleStations = false)
      {
         if (!string.IsNullOrEmpty(entry.FinalURL))
         {
            using (UnityWebRequest www = UnityWebRequest.Get(entry.FinalURL))
            {
               www.timeout = Constants.MAX_WEB_LOAD_WAIT_TIME;

               www.downloadHandler = new DownloadHandlerBuffer();
               yield return www.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
               if (www.result != UnityWebRequest.Result.ProtocolError && www.result != UnityWebRequest.Result.ConnectionError)
#else
               if (!www.isHttpError && !www.isNetworkError)
#endif
               {
                  System.Collections.Generic.List<string> list = Helper.SplitStringToLines(www.downloadHandler.text);

                  yield return null;

                  if (list.Count > 0)
                  {
                     switch (entry.DataFormat)
                     {
                        case DataFormatURL.M3U:
                           list = Helper.SplitStringToLines(www.downloadHandler.text, false); //dirty workaround
                           fillStationsFromM3U(list, entry, entry.ReadNumberOfStations, suppressDoubleStations);
                           break;
                        case DataFormatURL.PLS:
                           fillStationsFromPLS(list, entry, entry.ReadNumberOfStations, suppressDoubleStations);
                           break;
                        case DataFormatURL.Text:
                           fillStationsFromText(list, entry, entry.ReadNumberOfStations, suppressDoubleStations);
                           break;
                        default:
                           Debug.LogWarning("Not implemented!", this);
                           break;
                     }

                     onStationsChange();
                  }
                  else
                  {
                     if (Config.DEBUG)
                        Debug.Log(entry + " - URL: '" + entry.FinalURL + "' does not contain any active radio stations!", this);
                  }
               }
               else
               {
                  Debug.LogWarning(entry + " - Could not load source: '" + entry.FinalURL + "'" + System.Environment.NewLine + www.error + System.Environment.NewLine + "Did you set the correct 'URL'?", this);
               }
            }
         }
         else
         {
            Debug.LogWarning(entry + ": 'URL' is null or empty!" + System.Environment.NewLine + "Please add a valid URL.", this);
         }


         coRoutines.Remove(uid);
      }

      protected IEnumerator loadResource(string uid, RadioEntryResource entry, bool suppressDoubleStations = false)
      {
         if (entry.Resource != null)
         {
            System.Collections.Generic.List<string> list = Helper.SplitStringToLines(entry.Resource.text);

            yield return null;

            if (list.Count > 0)
            {
               switch (entry.DataFormat)
               {
                  case DataFormatResource.M3U:
                     list = Helper.SplitStringToLines(entry.Resource.text, false); //dirty workaround
                     fillStationsFromM3U(list, entry, entry.ReadNumberOfStations, suppressDoubleStations);
                     break;
                  case DataFormatResource.PLS:
                     fillStationsFromPLS(list, entry, entry.ReadNumberOfStations, suppressDoubleStations);
                     break;
                  case DataFormatResource.Text:
                     fillStationsFromText(list, entry, entry.ReadNumberOfStations, suppressDoubleStations);
                     break;
                  default:
                     Debug.LogWarning("Not implemented!", this);
                     break;
               }

               onStationsChange();
            }
            else
            {
               if (Config.DEBUG)
                  Debug.Log(entry + " - Resource: '" + entry.Resource + "' does not contain any active radio stations!", this);
            }
         }
         else
         {
            Debug.LogWarning(entry + ": resource field 'Resource' is null or empty!" + System.Environment.NewLine + "Please add a valid resource.", this);
         }

         coRoutines.Remove(uid);
      }

      protected IEnumerator loadShoutcast(string uid, RadioEntryShoutcast entry, bool suppressDoubleStations = false)
      {
         using (UnityWebRequest www = UnityWebRequest.Get(Constants.SHOUTCAST + entry.ShoutcastID.Trim()))
         {
            www.timeout = Constants.MAX_SHOUTCAST_LOAD_WAIT_TIME;

            www.downloadHandler = new DownloadHandlerBuffer();
            yield return www.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            if (www.result != UnityWebRequest.Result.ProtocolError && www.result != UnityWebRequest.Result.ConnectionError)
#else
            if (!www.isHttpError && !www.isNetworkError)
#endif
            {
               System.Collections.Generic.List<string> list = Helper.SplitStringToLines(www.downloadHandler.text);

               yield return null;

               if (list.Count > 0)
               {
                  fillStationsFromPLS(list, entry, 1, suppressDoubleStations);

                  onStationsChange();
               }
               else
               {
                  if (Config.DEBUG)
                     Debug.Log(entry + " - Shoutcast-ID: '" + entry.ShoutcastID + "' does not contain any active radio stations!");
               }
            }
            else
            {
               Debug.LogWarning(entry + " - Could not load Shoutcast-ID: '" + entry.ShoutcastID + "'" + System.Environment.NewLine + www.error + System.Environment.NewLine + "Did you set the correct 'Shoutcast-ID'?", this);
            }
         }

         coRoutines.Remove(uid);
      }

      protected void fillStationsFromM3U(System.Collections.Generic.List<string> list, BaseRadioEntry entry, int readNumberOfStations = 0, bool suppressDoubleStations = false)
      {
         int stationCount = 0;

         for (int ii = 0; ii < list.Count;)
         {
            string stationUrl = string.Empty;
            string title = string.Empty;
            string line = list[ii].Trim();

            if (ii == 0 && !line.CTEquals(Constants.M3U_EXT_ID))
               Debug.LogWarning("Data is not in the M3U-format - trying to extract stations!", this);

            if (!line.CTContains(Constants.M3U_EXT_ID)) //EXTM3U?
            {
               if (line.CTContains(Constants.M3U_EXT_INF_ID)) //EXTINF?
               {
                  string[] extsplit = line.Split(splitCharColon, System.StringSplitOptions.RemoveEmptyEntries);

                  if (extsplit.Length > 1)
                  {
                     string[] ext2split = extsplit[1].Split(splitCharComma, System.StringSplitOptions.RemoveEmptyEntries);

                     if (ext2split.Length > 1)
                     {
                        title = ext2split[1];
                     }
                  }

                  if (ii + 1 < list.Count)
                  {
                     ii++;
                     line = list[ii];

                     stationUrl = line;
                  }
               }
               else if (!string.IsNullOrEmpty(line))
               {
                  stationUrl = line;
               }

               if (!string.IsNullOrEmpty(stationUrl) && (!AllowOnlyHTTPS || (AllowOnlyHTTPS && isHTTPS(stationUrl))))
               {
                  RadioStation station = new RadioStation(entry.ForceName ? entry.Name : string.IsNullOrEmpty(title) ? entry.Name : title.Trim(), stationUrl.Trim(), entry.Format == AudioFormat.UNKNOWN ? Helper.AudioFormatFromString(stationUrl) : entry.Format, entry.Station, entry.Genres.ToLower(), entry.Bitrate, entry.Rating, entry.Description, entry.Icon, entry.IconUrl, entry.City, entry.Country, entry.Language, entry.ChunkSize, entry.BufferSize, entry.ExcludedCodec);

                  if (!Stations.Contains(station))
                  {
                     Stations.Add(station);

                     stationCount++;

                     if (Constants.DEV_DEBUG)
                        Debug.Log("Station added: " + station, this);

                     if (readNumberOfStations == stationCount)
                     {
                        break;
                     }
                  }
                  else
                  {
                     if (!suppressDoubleStations)
                     {
                        Debug.LogWarning("Station already added: '" + entry + "'", this);
                     }
                  }
               }
            }

            ii++;
         }
      }

      protected void fillStationsFromPLS(System.Collections.Generic.List<string> list, BaseRadioEntry entry, int readNumberOfStations = 0, bool suppressDoubleStations = false)
      {
         int stationCount = 0;

         for (int ii = 0; ii < list.Count;)
         {
            string line = list[ii].Trim();

            if (ii == 0 && !line.CTEquals("[playlist]"))
            {
               Debug.LogWarning("Data not in the PLS-format!", this);

               break;
            }

            string title = string.Empty;

            if (line.CTContains(Constants.PLS_FILE_ID)) //File?
            {
               string[] split = line.Split(splitCharEquals, System.StringSplitOptions.RemoveEmptyEntries);

               if (split.Length > 1)
               {
                  string stationUrl = split[1];

                  if (ii + 1 < list.Count)
                  {
                     ii++;
                     line = list[ii];

                     if (line.CTContains(Constants.PLS_TITLE_ID)) // Title?
                     {
                        string[] titlesplit = line.Split(splitCharEquals, System.StringSplitOptions.RemoveEmptyEntries);

                        if (titlesplit.Length > 1)
                        {
                           title = titlesplit[1];

                           ii++;
                        }
                     }
                  }

                  if (!string.IsNullOrEmpty(stationUrl) && (!AllowOnlyHTTPS || (AllowOnlyHTTPS && isHTTPS(stationUrl))))
                  {
                     RadioStation station = new RadioStation(entry.ForceName ? entry.Name : string.IsNullOrEmpty(title) ? entry.Name : title.Trim(), stationUrl.Trim(), entry.Format == AudioFormat.UNKNOWN ? Helper.AudioFormatFromString(stationUrl) : entry.Format, entry.Station, entry.Genres.ToLower(), entry.Bitrate, entry.Rating, entry.Description, entry.Icon, entry.IconUrl, entry.City, entry.Country, entry.Language, entry.ChunkSize, entry.BufferSize, entry.ExcludedCodec);

                     if (!Stations.Contains(station))
                     {
                        Stations.Add(station);

                        stationCount++;

                        if (Constants.DEV_DEBUG)
                           Debug.Log("Station added: " + station, this);

                        if (readNumberOfStations == stationCount)
                        {
                           break;
                        }
                     }
                     else
                     {
                        if (!suppressDoubleStations)
                           Debug.LogWarning("Station already added: '" + station + "'", this);
                     }
                  }
               }
               else
               {
                  Debug.LogWarning(entry + ": No URL found for '" + Constants.PLS_FILE_ID + "': " + line, this);
               }
            }
            else
            {
               ii++;
            }
         }
      }

      protected void fillStationsFromText(System.Collections.Generic.List<string> list, BaseRadioEntry entry, int readNumberOfStations = 0, bool suppressDoubleStations = false)
      {
         int stationCount = 0;
         int lineCount = 0;
         foreach (string line in list)
         {
            lineCount++;
            string[] content = line.Split(splitCharText, System.StringSplitOptions.None);

            if (content.Length >= 4 && content.Length <= 16)
            {
               if (!AllowOnlyHTTPS || AllowOnlyHTTPS && isHTTPS(content[1]))
               {
                  RadioStation station = new RadioStation(entry.ForceName ? entry.Name : string.IsNullOrEmpty(content[0]) ? entry.Name : content[0].Trim(), content[1].Trim(), Helper.AudioFormatFromString(content[3].Trim()));

                  if (content.Length >= 5)
                     station.Station = content[4].Trim();

                  if (content.Length >= 6)
                     station.Genres = content[5].Trim().ToLower().Replace(',', ' ');

                  if (content.Length >= 7)
                     station.Bitrate = int.TryParse(content[6].Trim(), out int bitrate) ? bitrate : entry.Bitrate;

                  if (content.Length >= 8)
                     station.Rating = float.TryParse(content[7].Trim(), out float rating) ? rating : entry.Rating;

                  if (content.Length >= 9)
                     station.Description = content[8].Trim();

                  if (content.Length >= 10)
                     station.ExcludedCodec = Helper.AudioCodecFromString(content[9].Trim());

                  if (content.Length >= 11)
                     station.ChunkSize = int.TryParse(content[10].Trim(), out int chunkSize) ? chunkSize : entry.ChunkSize;

                  if (content.Length >= 12)
                     station.BufferSize = int.TryParse(content[11].Trim(), out int bufferSize) ? bufferSize : entry.BufferSize;

                  if (content.Length >= 13)
                     station.IconUrl = content[12].Trim();

                  if (content.Length >= 14)
                     station.City = content[13].Trim();

                  if (content.Length >= 15)
                     station.Country = content[14].Trim();

                  if (content.Length == 16) //all parameters
                     station.Language = content[15].Trim();

                  if (station.Format == AudioFormat.OGG && station.BufferSize < Constants.MIN_OGG_BUFFERSIZE)
                  {
                     if (Config.DEBUG)
                        Debug.Log("Adjusted buffer size: " + station, this);

                     station.BufferSize = Constants.MIN_OGG_BUFFERSIZE;
                  }

                  stationCount++;

                  if (content[2].CTEquals("stream"))
                  {
                     if (!Stations.Contains(station))
                     {
                        Stations.Add(station);

                        if (Config.DEBUG)
                           Debug.Log("Station added: " + station, this);
                     }
                     else
                     {
                        if (!suppressDoubleStations)
                           Debug.LogWarning("Station already added: '" + station + "'", this);
                     }
                  }
                  else if (content[2].CTContains("pls"))
                  {
                     if (Helper.isEditorMode)
                     {
#if UNITY_EDITOR
                        loadWebInEditor(new RadioEntryURL(station, content[1].Trim(), DataFormatURL.PLS, 1 /*readNumberOfStations*/));
#endif
                     }
                     else
                     {
                        StartCoroutine(loadWeb(addCoRoutine(), new RadioEntryURL(station, content[1].Trim(), DataFormatURL.PLS, 1 /*readNumberOfStations*/)));
                     }
                  }
                  else if (content[2].CTContains("m3u"))
                  {
                     if (Helper.isEditorMode)
                     {
#if UNITY_EDITOR
                        loadWebInEditor(new RadioEntryURL(station, content[1].Trim(), DataFormatURL.M3U, 1 /*readNumberOfStations*/));
#endif
                     }
                     else
                     {
                        StartCoroutine(loadWeb(addCoRoutine(), new RadioEntryURL(station, content[1].Trim(), DataFormatURL.M3U, 1 /*readNumberOfStations*/)));
                     }
                  }
                  else if (content[2].CTContains("shoutcast"))
                  {
                     if (Helper.isEditorMode)
                     {
#if UNITY_EDITOR
                        loadShoutcastInEditor(new RadioEntryShoutcast(station, content[1].Trim()));
#endif
                     }
                     else
                     {
                        StartCoroutine(loadShoutcast(addCoRoutine(), new RadioEntryShoutcast(station, content[1].Trim())));
                     }
                  }
                  else
                  {
                     Debug.LogWarning("Could not determine URL for station: '" + station + "'" + System.Environment.NewLine + line, this);
                     stationCount--;
                  }

                  if (readNumberOfStations == stationCount)
                     break;
               }
            }
            else
            {
               Debug.LogWarning($"Invalid station description: '{entry}' in line {lineCount} ({list.Count}):{System.Environment.NewLine}{line}", this);
            }
         }
      }

      protected string addCoRoutine()
      {
         string uid = System.Guid.NewGuid().ToString();
         coRoutines.Add(uid);

         return uid;
      }

      private static bool isHTTPS(string url)
      {
         return url.CTContains(Constants.PREFIX_HTTPS);
      }

      #endregion


      #region Event-trigger methods

      protected void onStationsChange()
      {
         if (Config.DEBUG)
            Debug.Log("onStationsChange", this);

         if (!Helper.isEditorMode)
            onStationsChanged?.Invoke();

         OnStationsChange?.Invoke();
      }

      private void onProviderReady()
      {
         if (Config.DEBUG)
            Debug.Log("onProviderReady", this);

         if (!Helper.isEditorMode)
            onProviderReadyEvent?.Invoke();

         OnProviderReady?.Invoke();
      }

      #endregion


      #region Editor-only methods

#if UNITY_EDITOR
      public bool isReadyInEditor => loadedInEditor;

      protected virtual void initInEditor()
      {
         if (Helper.isEditorMode)
            Stations.Clear();
      }

      protected void loadWebInEditor(RadioEntryURL entry, bool suppressDoubleStations = false)
      {
         if (Helper.isEditorMode)
         {
            loadedInEditor = false;

            if (!string.IsNullOrEmpty(entry.FinalURL))
            {
               try
               {
                  System.Net.ServicePointManager.ServerCertificateValidationCallback = Crosstales.Common.Util.NetworkHelper.RemoteCertificateValidationCallback;

                  using (System.Net.WebClient client = new Crosstales.Common.Util.CTWebClient())
                  {
                     client.Encoding = System.Text.Encoding.UTF8;
                     string url = entry.FinalURL;

                     using (System.IO.Stream stream = client.OpenRead(url))
                     {
                        if (stream != null)
                           using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
                           {
                              string content = reader.ReadToEnd();

                              System.Collections.Generic.List<string> list = Helper.SplitStringToLines(content);

                              if (list.Count > 0)
                              {
                                 switch (entry.DataFormat)
                                 {
                                    case DataFormatURL.M3U:
                                       list = Helper.SplitStringToLines(content, false); //dirty workaround
                                       fillStationsFromM3U(list, entry, entry.ReadNumberOfStations, suppressDoubleStations);
                                       break;
                                    case DataFormatURL.PLS:
                                       fillStationsFromPLS(list, entry, entry.ReadNumberOfStations, suppressDoubleStations);
                                       break;
                                    case DataFormatURL.Text:
                                       fillStationsFromText(list, entry, entry.ReadNumberOfStations, suppressDoubleStations);
                                       break;
                                    default:
                                       Debug.LogWarning("Not implemented!", this);
                                       break;
                                 }
                              }
                              else
                              {
                                 if (Config.DEBUG)
                                    Debug.Log(entry + " - URL: '" + entry.FinalURL + "' does not contain any active radio stations!", this);
                              }
                           }
                     }
                  }
               }
               catch (System.Exception ex)
               {
                  Debug.LogError(ex, this);
               }
            }
            else
            {
               Debug.LogWarning(entry + ": 'URL' is null or empty!" + System.Environment.NewLine + "Please add a valid URL.", this);
            }

            loadedInEditor = true;
         }
      }

      protected void loadResourceInEditor(RadioEntryResource entry, bool suppressDoubleStations = false)
      {
         if (Helper.isEditorMode)
         {
            loadedInEditor = false;

            if (entry.Resource != null)
            {
               System.Collections.Generic.List<string> list = Helper.SplitStringToLines(entry.Resource.text);

               if (list.Count > 0)
               {
                  switch (entry.DataFormat)
                  {
                     case DataFormatResource.M3U:
                        list = Helper.SplitStringToLines(entry.Resource.text, false); //dirty workaround
                        fillStationsFromM3U(list, entry, entry.ReadNumberOfStations, suppressDoubleStations);
                        break;
                     case DataFormatResource.PLS:
                        fillStationsFromPLS(list, entry, entry.ReadNumberOfStations, suppressDoubleStations);
                        break;
                     case DataFormatResource.Text:
                        fillStationsFromText(list, entry, entry.ReadNumberOfStations, suppressDoubleStations);
                        break;
                     default:
                        Debug.LogWarning("Not implemented!", this);
                        break;
                  }
               }
               else
               {
                  if (Config.DEBUG)
                     Debug.Log(entry + " - Resource: '" + entry.Resource + "' does not contain any active radio stations!", this);
               }
            }
            else
            {
               Debug.LogWarning(entry + ": resource field 'Resource' is null or empty!" + System.Environment.NewLine + "Please add a valid resource.", this);
            }

            loadedInEditor = true;
         }
      }

      protected void loadShoutcastInEditor(RadioEntryShoutcast entry, bool suppressDoubleStations = false)
      {
         if (Helper.isEditorMode)
         {
            loadedInEditor = false;

            try
            {
               System.Net.ServicePointManager.ServerCertificateValidationCallback = Crosstales.Common.Util.NetworkHelper.RemoteCertificateValidationCallback;

               using (System.Net.WebClient client = new Crosstales.Common.Util.CTWebClient())
               {
                  client.Encoding = System.Text.Encoding.UTF8;
                  string url = Constants.SHOUTCAST + entry.ShoutcastID.Trim();

                  using (System.IO.Stream stream = client.OpenRead(url))
                  {
                     if (stream != null)
                        using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
                        {
                           string content = reader.ReadToEnd();

                           System.Collections.Generic.List<string> list = Helper.SplitStringToLines(content);

                           if (list.Count > 0)
                           {
                              fillStationsFromPLS(list, entry, 1, suppressDoubleStations);
                           }
                           else
                           {
                              if (Config.DEBUG)
                                 Debug.Log(entry + " - Shoutcast-ID: '" + entry.ShoutcastID + "' does not contain any active radio stations!", this);
                           }
                        }
                  }
               }
            }
            catch (System.Exception ex)
            {
               Debug.LogError(ex, this);
            }

            loadedInEditor = true;
         }
      }

#endif

      #endregion
   }
}
// © 2015-2024 crosstales LLC (https://www.crosstales.com)