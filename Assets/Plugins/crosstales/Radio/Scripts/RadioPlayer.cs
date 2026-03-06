using UnityEngine;
using System.Collections;
using System.Linq;
using Crosstales.Radio.Util;
using Crosstales.Radio.Model;
using Crosstales.Radio.Model.Enum;

namespace Crosstales.Radio
{
   /// <summary>Player for a radio station.</summary>
   [ExecuteInEditMode]
   [RequireComponent(typeof(AudioSource))]
   [HelpURL("https://www.crosstales.com/media/data/assets/radio/api/class_crosstales_1_1_radio_1_1_radio_player.html")]
   public class RadioPlayer : BasePlayer
   {
      #region Variables

      //[Header("Radio Station")]
      [UnityEngine.Serialization.FormerlySerializedAsAttribute("Station")] [Tooltip("Radio station for this RadioPlayer."), SerializeField]
      private RadioStation station;

      [UnityEngine.Serialization.FormerlySerializedAsAttribute("PlayOnStart")] [Header("Behaviour Settings"), Tooltip("Play the RadioPlayer on start on/off (default: false)."), SerializeField]
      private bool playOnStart;

      [UnityEngine.Serialization.FormerlySerializedAsAttribute("Delay")] [Tooltip("Delay in seconds until the RadioPlayer starts playing (default: 0.1)."), SerializeField]
      private float delay = 0.1f;

      [UnityEngine.Serialization.FormerlySerializedAsAttribute("HandleFocus")] [Tooltip("Starts and stops the RadioPlayer depending on the focus and running state (default: false)."), SerializeField]
      private bool handleFocus = false;


      [UnityEngine.Serialization.FormerlySerializedAsAttribute("CacheStreamSize")] [Header("General Settings"), Tooltip("Size of the cache stream in KB (default: 1024, max: 16384)."), SerializeField]
      private int cacheStreamSize = Config.DEFAULT_CACHESTREAMSIZE;

      [UnityEngine.Serialization.FormerlySerializedAsAttribute("LegacyMode")] [Tooltip("Enable or disable legacy mode. Legacy mode disables all record information, but is more stable (default: false)."), SerializeField]
      private bool legacyMode;

      [UnityEngine.Serialization.FormerlySerializedAsAttribute("CaptureDataStream")] [Tooltip("Capture the encoded PCM-stream from this RadioPlayer (default: false)."), SerializeField]
      private bool captureDataStream;

      [Tooltip("Enable or disable skipping of the pre-buffering. Skip pre-buffering allows for faster playback, but is less resilient (default: false)."), SerializeField]
      private bool skipPreBuffering = false;

      private bool error;
      private string errorMessage;

#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN) && !UNITY_EDITOR_OSX && !UNITY_EDITOR_LINUX
      protected NAudio.Wave.Mp3FileReader nAudioReader;
#endif
      private int oggCacheCleanFrameCount;
      protected bool stopped = true;
      protected bool bufferAvailable;
      protected bool playback;
      private bool restarted;
      private float maxPlayTime;
      private RecordInfo recordInfo = new RecordInfo();
      private RecordInfo nextRecordInfo = new RecordInfo();
      private RecordInfo lastNextRecordInfo;
      private float nextRecordDelay;
      private float lastNextRecordDelay = float.MinValue;
      private System.IO.Stream ms;
      private const bool ignoreExcludeCodec = false; //enable only for tests!
      private bool wasRunning;
      private float pitch;

      //public string uid;

      private string lastUrl;

#if (!UNITY_WSA && !UNITY_WEBGL && !UNITY_XBOXONE) || UNITY_EDITOR
      protected NVorbis.VorbisReader nVorbisReader;
      protected NLayer.MpegFile nLayerReader;
      private System.Threading.Thread worker;
      private System.Threading.Thread timer;
#endif

      private static RadioPlayer instance;
      private float lastPlaytime = float.MinValue;

      #endregion


      #region Properties

      /// <summary>Returns the singleton instance of this class.</summary>
      /// <returns>Singleton instance of this class.</returns>
      public static RadioPlayer Instance
      {
         get
         {
            if (Crosstales.Common.Util.SingletonHelper.isQuitting && Helper.isEditorMode)
            {
               //Debug.LogWarning($"[Singleton] Instance '{nameof(RadioPlayer)}' already destroyed. Returning null.");
               return instance;
            }

            if (instance == null)
            {
#if UNITY_2023_1_OR_NEWER
               instance = FindFirstObjectByType<RadioPlayer>();
#else
               instance = (RadioPlayer)FindObjectOfType(typeof(RadioPlayer));
#endif
               // Create new instance if one doesn't already exist.
               if (instance == null)
               {
                  const string prefabPath = "Prefabs/RadioPlayer";

                  if (!string.IsNullOrEmpty(prefabPath))
                  {
                     RadioPlayer prefab = Resources.Load<RadioPlayer>(prefabPath);

                     if (prefab == null)
                     {
                        Debug.LogWarning("Prefab missing: " + prefabPath);
                     }
                     else
                     {
                        instance = Instantiate(prefab);
                        //Debug.LogWarning($"{Time.timeSinceLevelLoad}-[Singleton] Instance '{nameof(RadioPlayer)}' CREATE Prefab: {instance.GetInstanceID()}");
                     }
                  }

                  if (instance == null)
                  {
                     if (Helper.isEditorMode)
                     {
#if UNITY_EDITOR
                        //instanceEditor = UnityEditor.EditorUtility.CreateGameObjectWithHideFlags($"{nameof(Turntable)} (Hidden Singleton)", HideFlags.DontSaveInBuild | HideFlags.HideInHierarchy).AddComponent<Turntable>();
                        instance = new GameObject($"{nameof(RadioPlayer)} (Singleton)").AddComponent<RadioPlayer>();
#endif
                        //Debug.LogWarning($"{Time.timeSinceLevelLoad}-[Singleton] Instance '{nameof(Turntable)}' CREATE Editor: {instance.GetInstanceID()}");
                     }
                     else
                     {
                        instance = new GameObject($"{nameof(RadioPlayer)} (Singleton)").AddComponent<RadioPlayer>();

                        //Debug.LogWarning($"{Time.timeSinceLevelLoad}-[Singleton] Instance '{nameof(Turntable)}' CREATE Play: {instance.GetInstanceID()}");
                     }
                  }
               }
            }

            //Debug.LogWarning($"{Time.timeSinceLevelLoad}-[Singleton] Instance '{typeof(T)}' GET: {activeInstance.GetInstanceID()}");
            return instance;
         }

         private set
         {
            //Debug.LogWarning($"{Time.timeSinceLevelLoad}-[Singleton] Instance '{nameof(Turntable)}' SET: {value?.GetInstanceID()}");
            instance = value;
         }
      }

      public override RadioStation Station
      {
         get => station;
         set => station = value;
      }

      /// <summary>Play the RadioPlayer on start on/off.</summary>
      public bool PlayOnStart
      {
         get => playOnStart;
         set => playOnStart = value;
      }

      /// <summary>Delay in seconds until the RadioPlayer starts playing.</summary>
      public float Delay
      {
         get => delay;
         set => delay = value;
      }

      public override bool HandleFocus
      {
         get => handleFocus;
         set => handleFocus = value;
      }

      public override int CacheStreamSize
      {
         get => cacheStreamSize * Constants.FACTOR_KB;
         set
         {
            if (value <= 0)
            {
               cacheStreamSize = Config.DEFAULT_CACHESTREAMSIZE;
            }
            else if (value <= station?.BufferSize * Constants.FACTOR_KB)
            {
               cacheStreamSize = station.BufferSize;
            }
            else if (value > Config.MAX_CACHESTREAMSIZE * Constants.FACTOR_KB)
            {
               cacheStreamSize = Config.MAX_CACHESTREAMSIZE;
            }
            else
            {
               cacheStreamSize = value / Constants.FACTOR_KB;
            }
         }
      }

      public override bool LegacyMode
      {
         get => legacyMode;
         set => legacyMode = value;
      }

      public override bool CaptureDataStream
      {
         get => captureDataStream;
         set => captureDataStream = value;
      }

      public override bool SkipPreBuffering
      {
         get => skipPreBuffering;
         set => skipPreBuffering = value;
      }

      public override AudioSource Source { get; protected set; }

      public override AudioCodec Codec { get; protected set; }

      public override float PlayTime { get; protected set; }

      public override float BufferProgress { get; protected set; }

      public override bool isPlayback => playback;

      public override bool isAudioPlaying => playback && !isBuffering;

      public override bool isBuffering => !bufferAvailable;

      public override float RecordPlayTime { get; protected set; }

      public override RecordInfo RecordInfo => recordInfo;

      public override RecordInfo NextRecordInfo => nextRecordInfo;

      public override float NextRecordDelay => nextRecordDelay;

      public override long CurrentBufferSize => ms != null ? ms.Length - ms.Position : 0;

      public override long CurrentDownloadSpeed => ms != null && PlayTime > 0f ? (long)(ms.Length / PlayTime) : 0;

      public override Crosstales.Common.Util.MemoryCacheStream DataStream { get; protected set; }

      public override int Channels => station?.Channels ?? 0;

      public override int SampleRate => station?.SampleRate ?? 0;

      public override float Volume
      {
         get => Source != null ? Source.volume : 1f;

         set
         {
            if (Source != null)
               Source.volume = value;
         }
      }

      public override float Pitch
      {
         get => Source != null ? Source.pitch : 1f;

         set
         {
            if (Source != null)
               Source.pitch = value;
         }
      }

      public override float StereoPan
      {
         get => Source != null ? Source.panStereo : 1f;

         set
         {
            if (Source != null)
               Source.panStereo = value;
         }
      }

      public override bool isMuted
      {
         get => Source != null && Source.mute;

         set
         {
            if (Source != null)
               Source.mute = value;
         }
      }

      protected override PlaybackStartEvent onPlaybackStarted => OnPlaybackStarted;

      protected override PlaybackEndEvent onPlaybackEnded => OnPlaybackEnded;

      protected override RecordChangeEvent onRecordChanged => OnRecordChanged;

      protected override BufferingStartEvent onBufferingStarted => OnBufferingStarted;

      protected override BufferingEndEvent onBufferingEnded => OnBufferingEnded;

      protected override AudioStartEvent onAudioStarted => OnAudioStarted;

      protected override AudioEndEvent onAudioEnded => OnAudioEnded;

      protected override ErrorEvent onError => OnError;

      #endregion


      #region Events

      [Header("Events")] public PlaybackStartEvent OnPlaybackStarted;
      public BufferingStartEvent OnBufferingStarted;
      public BufferingEndEvent OnBufferingEnded;
      public AudioStartEvent OnAudioStarted;
      public AudioEndEvent OnAudioEnded;
      public PlaybackEndEvent OnPlaybackEnded;
      public RecordChangeEvent OnRecordChanged;

      public ErrorEvent OnError;

      #endregion


      #region MonoBehaviour methods

      private void Awake()
      {
         Helper.ApplicationIsPlaying = Application.isPlaying; //needed to enforce the right mode

         Instance = this;

         oggCacheCleanFrameCount = Random.Range(Constants.OGG_CLEAN_INTERVAL_MIN, Constants.OGG_CLEAN_INTERVAL_MAX);

         Source = GetComponent<AudioSource>();

         if (Source != null)
         {
            Source.playOnAwake = false;
            Source.Stop(); //always stop the AudioSource at startup
         }
         else
         {
            Debug.LogError("No 'AudioSource' found on the gameobject! Please attach one.", this);
         }
      }

      private void Start()
      {
         if (playOnStart && !Helper.isEditorMode)
            Invoke(nameof(Play), delay);
      }

      private void Update()
      {
         if (Station != null)
         {
            if (Helper.isEditorMode && !isAudioPlaying && Station.Url != lastUrl)
               //if (!isAudioPlaying && Station.Url != lastUrl)
            {
               //Debug.Log("Created new Station!", this);

               lastUrl = Station.Url;

               Station = new RadioStation(Station.Name, Station.Url, Station.Format);
            }

            if (isAudioPlaying && !restarted)
            {
               pitch = Pitch;

               if (!LegacyMode && lastNextRecordInfo?.Equals(NextRecordInfo) != true)
               {
                  lastNextRecordInfo = NextRecordInfo;

                  onNextRecordChange(Station, NextRecordInfo, nextRecordDelay);
               }

               //float _pitchedTime = Time.deltaTime * Source.pitch;
               //Context.TotalPlayTime += _pitchedTime;

               //station.TotalPlayTime += _pitchedTime;
               //PlayTime += _pitchedTime;
               //RecordPlayTime += _pitchedTime;
               //nextRecordDelay -= _pitchedTime;

               onAudioPlayTimeUpdate(Station, PlayTime);
               onRecordPlayTimeUpdate(Station, RecordInfo, RecordPlayTime);
/*
               if (updateFirstRecord)
               {
                  updateFirstRecord = false;
                  onRecordChange(Station, NextRecordInfo);
               }
*/
               if (!LegacyMode && nextRecordDelay >= -0.1f && System.Math.Abs(lastNextRecordDelay - nextRecordDelay) > Constants.FLOAT_TOLERANCE) //lastNextRecordDelay != nextRecordDelay)
               {
                  lastNextRecordDelay = nextRecordDelay;
                  onNextRecordDelayUpdate(Station, NextRecordInfo, nextRecordDelay);
               }
            }

            if (PlayTime > maxPlayTime)
            {
               if (Config.DEBUG)
                  Debug.Log("+++ RESTART - Point reached: " + Helper.FormatSecondsToHRF(PlayTime), this);

               restarted = true;
               Restart();
            }
         }
      }

      private void OnDisable()
      {
         Stop();
      }

      private void OnValidate()
      {
         if (delay < 0f)
            delay = 0f;

         if (Station != null)
         {
            Station.Bitrate = Station.Bitrate <= 0 ? Config.DEFAULT_BITRATE : Helper.NearestBitrate(station.Bitrate, Station.Format);

            if (Station.ChunkSize <= 0)
            {
               Station.ChunkSize = Config.DEFAULT_CHUNKSIZE;
            }
            else if (Station.ChunkSize > Config.MAX_CACHESTREAMSIZE)
            {
               Station.ChunkSize = Config.MAX_CACHESTREAMSIZE;
            }

            if (Station.BufferSize <= 0)
            {
               Station.BufferSize = Config.DEFAULT_BUFFERSIZE;
            }
            else
            {
               switch (Station.Format)
               {
                  case AudioFormat.MP3:
                  {
                     if (Station.BufferSize < Config.DEFAULT_BUFFERSIZE / 4)
                        Station.BufferSize = Config.DEFAULT_BUFFERSIZE / 4;

                     break;
                  }
                  case AudioFormat.OGG:
                  {
                     if (Station.BufferSize < Constants.MIN_OGG_BUFFERSIZE)
                        Station.BufferSize = Constants.MIN_OGG_BUFFERSIZE;

                     break;
                  }
               }

               if (Station.BufferSize < Station.ChunkSize)
               {
                  Station.BufferSize = Station.ChunkSize;
               }
               else if (Station.BufferSize > Config.MAX_CACHESTREAMSIZE)
               {
                  Station.BufferSize = Config.MAX_CACHESTREAMSIZE;
               }
            }
         }

         if (cacheStreamSize <= 0)
         {
            cacheStreamSize = Config.DEFAULT_CACHESTREAMSIZE;
         }
         else if (cacheStreamSize <= Station?.BufferSize)
         {
            cacheStreamSize = Station.BufferSize;
         }
         else if (cacheStreamSize > Config.MAX_CACHESTREAMSIZE)
         {
            cacheStreamSize = Config.MAX_CACHESTREAMSIZE;
         }
      }

      private void OnApplicationFocus(bool hasFocus)
      {
         if ((Helper.isMobilePlatform || !Application.runInBackground) && handleFocus)
         {
#if UNITY_ANDROID || UNITY_IOS
            if (!TouchScreenKeyboard.isSupported || !TouchScreenKeyboard.visible)
            {
#endif
            if (hasFocus)
            {
               if (wasRunning)
                  Invoke(nameof(Play), 0.1f);
            }
            else
            {
               wasRunning = playback;
               Stop();
            }
#if UNITY_ANDROID || UNITY_IOS
            }
#endif
         }
      }

      private void OnDestroy()
      {
         killWorker();

         StopAllCoroutines();

         if (instance == this)
         {
            //Debug.LogWarning($"{Time.timeSinceLevelLoad}-[Singleton] Instance '{nameof(Turntable)}' ONDESTROY: {instance.GetInstanceID()}");

            Instance = null;
         }
      }

      #endregion


      #region Public methods

      public override void Play()
      {
         if (this != null && !isActiveAndEnabled)
            return;

         if (Helper.isSupportedPlatform)
         {
            if (stopped)
            {
               if (Crosstales.Common.Util.NetworkHelper.isInternetAvailable)
               {
                  if (Helper.isSane(ref station))
                  {
                     float currentTime = Time.realtimeSinceStartup;

                     if (lastPlaytime + Constants.PLAY_CALL_SPEED < currentTime)
                     {
                        lastPlaytime = currentTime;

                        Codec = Helper.AudioCodecForAudioFormat(station.Format);

                        if (Codec == AudioCodec.None)
                        {
                           errorMessage = station + System.Environment.NewLine + "Audio format not supported - can't play station: " + station.Format;
                           Debug.LogError(errorMessage, this);
                           onErrorInfo(station, errorMessage);

                           return;
                        }

                        if (!ignoreExcludeCodec && station.ExcludedCodec == Codec)
                        {
                           errorMessage = station + System.Environment.NewLine + "Excluded codec matched - can't play station: " + Codec;
                           Debug.LogError(errorMessage, this);
                           onErrorInfo(station, errorMessage);
                        }
                        else
                        {
#if (!UNITY_WSA && !UNITY_WEBGL && !UNITY_XBOXONE) || UNITY_EDITOR
                           StartCoroutine(playAudioFromUrl());
#endif
                        }
                     }
                     else
                     {
                        Debug.LogWarning("'Play' called too fast - please slow down!", this);
                     }
                  }
                  else
                  {
                     errorMessage = station + System.Environment.NewLine + "Could not start playback. Please verify the station settings.";
                     Debug.LogError(errorMessage, this);
                     onErrorInfo(station, errorMessage);
                  }
               }
               else
               {
                  errorMessage = "No internet connection available! Can't play (stream) any stations!";
                  Debug.LogError(errorMessage, this);
                  onErrorInfo(station, errorMessage);
               }
            }
            else
            {
               errorMessage = station + System.Environment.NewLine + "Station is already playing!";
               Debug.LogWarning(errorMessage, this);
               onErrorInfo(station, errorMessage);
            }
         }
         else
         {
            logUnsupportedPlatform();
         }
      }

      public override void Stop()
      {
         if (this != null && !isActiveAndEnabled)
            return;

         playback = false;
         lastNextRecordDelay = float.MinValue;

         if (Source != null /*&& !Helper.isEditorMode*/) // could already be destroyed
         {
            Source.Stop();
            Source.clip = null;
         }

         stopped = true;
      }

      public override void Restart(float invokeDelay = Constants.INVOKE_DELAY)
      {
         if (this != null && !isActiveAndEnabled)
            return;

         Stop();

         Invoke(nameof(Play), invokeDelay);
      }

      public virtual string ToShortString()
      {
         System.Text.StringBuilder result = new System.Text.StringBuilder();

         result.Append("Station='");
         result.Append(station);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("PlayOnStart='");
         result.Append(playOnStart);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("CacheStreamSize='");
         result.Append(cacheStreamSize);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER_END);

         return result.ToString();
      }

      /// <summary>Loads the RadioPlayer.</summary>
      public void Load()
      {
         //TODO implement load feature!
         Debug.LogWarning("Not implemented!", this);
      }

      /// <summary>Saves the RadioPlayer.</summary>
      public void Save()
      {
         //TODO implement save feature!
         Debug.LogWarning("Not implemented!", this);
      }

      public override void Mute()
      {
         if (this != null && !isActiveAndEnabled)
            return;

         isMuted = true;
      }

      public override void UnMute()
      {
         if (this != null && !isActiveAndEnabled)
            return;

         isMuted = false;
      }

      #endregion


      #region Private methods

#if (!UNITY_WSA && !UNITY_WEBGL && !UNITY_XBOXONE) || UNITY_EDITOR
      private IEnumerator playAudioFromUrl()
      {
         RadioStation _station = station; //make reference to original (it could change)
         AudioClip ac = null;

         playback = false;
         restarted = false;
         error = false;
         errorMessage = string.Empty;

         PlayTime = 0f;

         RecordPlayTime = 0f;
         BufferProgress = 0f;
         bufferAvailable = false;
         float _bufferCurrentProgress = 0f;

         recordInfo = new RecordInfo();
         nextRecordInfo = new RecordInfo();
         nextRecordDelay = 0f;
         bool _success = true;

         onPlaybackStart(_station);

         onBufferingStart(_station);

         onBufferingProgressUpdate(_station, BufferProgress);

         try
         {
            //using (ms = new MemoryCacheStream(100 * CacheStreamSize * Constants.FACTOR_KB, 5 * Config.MAX_CACHESTREAMSIZE * Constants.FACTOR_KB))
            using (ms = new Crosstales.Common.Util.MemoryCacheStream(CacheStreamSize, Config.MAX_CACHESTREAMSIZE * Constants.FACTOR_KB))
            {
               killWorker();
               worker = legacyMode ? new System.Threading.Thread(() => readStreamLegacy(ref _station, ref playback, ref ms, ref error, ref errorMessage)) : new System.Threading.Thread(() => readStream(ref _station, ref playback, ref ms, ref error, ref errorMessage, ref nextRecordInfo, ref nextRecordDelay));
               worker.Start();

               // Waiting for stream
               do
               {
                  yield return null;
               } while (!playback && !stopped && !error);

               int bufferSize = _station.BufferSize * Constants.FACTOR_KB + _station.ChunkSize * Constants.FACTOR_KB;

               // Pre-buffering some data to allow start playing
               if (SkipPreBuffering)
               {
                  yield return Constants.WFS_BUFFER;
               }
               else
               {
                  do
                  {
                     BufferProgress = (float)ms.Length / bufferSize;

                     if (Mathf.Abs(BufferProgress - _bufferCurrentProgress) > Constants.FLOAT_TOLERANCE)
                     {
                        onBufferingProgressUpdate(_station, BufferProgress);
                        _bufferCurrentProgress = BufferProgress;
                     }

                     yield return null;
                  } while (playback && !stopped && ms.Length < bufferSize);
               }

               BufferProgress = 1f;
               onBufferingProgressUpdate(_station, BufferProgress);

               bufferAvailable = true;
               onBufferingEnd(_station);

               //Debug.Log(station);

               if (playback && !stopped)
               {
                  timer = new System.Threading.Thread(() => timerTask(ref _station, ref playback, ref nextRecordDelay));
                  timer.Start();

                  try
                  {
                     if (Codec == AudioCodec.MP3_NLayer)
                     {
                        nLayerReader = new NLayer.MpegFile(ms);

                        _station.SampleRate = nLayerReader.SampleRate;

                        if (_station.SampleRate < 32000 || _station.SampleRate > 48000)
                        {
                           _success = false;
                           errorMessage = $"Only MP3 with layer 3 specs is supported (current {_station.SampleRate}Hz)! MPEG-1 (Audio Layer III) allows the following sample rates: 32000Hz, 44100Hz and 48000Hz!";
                           Debug.LogError(errorMessage, this);
                        }
                        else
                        {
                           _station.Channels = nLayerReader.Channels;
                        }
                     }
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN) && !UNITY_EDITOR_OSX && !UNITY_EDITOR_LINUX
                     else if (Codec == AudioCodec.MP3_NAudio)
                     {
                        nAudioReader = new NAudio.Wave.Mp3FileReader(ms);

                        _station.SampleRate = nAudioReader.WaveFormat.SampleRate;
                        _station.Channels = nAudioReader.WaveFormat.Channels;
                     }
#endif
                     else if (Codec == AudioCodec.OGG_NVorbis)
                     {
                        //Debug.Log("VORBIS");
                        nVorbisReader = new NVorbis.VorbisReader(ms, false);

                        _station.SampleRate = nVorbisReader.SampleRate;
                        _station.Channels = nVorbisReader.Channels;

                        //_station.SampleRate = 48000;
                     }
                     else
                     {
                        _success = false;
                        errorMessage = _station + System.Environment.NewLine + "Unsupported codec: " + Codec;
                        Debug.LogError(errorMessage, this);
                     }
                  }
                  catch (System.Exception ex)
                  {
                     Debug.LogError(_station + System.Environment.NewLine + "Could not read data from url!" + System.Environment.NewLine + ex, this);

                     _success = false;
                  }

                  if (!_success)
                  {
                     error = true;
                     errorMessage = _station + System.Environment.NewLine + "Could not play the stream -> Please try another station!";
                     Debug.LogError(errorMessage, this);

                     playback = false;
                  }
                  else
                  {
                     if (Codec == AudioCodec.OGG_NVorbis)
                        ms.Position = 0;

                     maxPlayTime = int.MaxValue / _station.SampleRate - 120; //reserve of 2 minutes
                     //maxPlayTime = 10; //for tests only

                     DataStream = new Crosstales.Common.Util.MemoryCacheStream(128 * Constants.FACTOR_KB, 512 * Constants.FACTOR_KB);

                     ac = AudioClip.Create(_station.Name, int.MaxValue, _station.Channels, _station.SampleRate, true, readPCMData);
                     Source.clip = ac;

                     Source.Play();

                     onAudioStart(_station);
                  }

                  do
                  {
                     yield return null;

                     if (Codec == AudioCodec.OGG_NVorbis && Time.frameCount % oggCacheCleanFrameCount == 0)
                     {
                        if (Constants.DEV_DEBUG)
                           Debug.Log("Clean cache: " + oggCacheCleanFrameCount + " - " + PlayTime, this);

                        NVorbis.Mdct.ClearSetupCache();
                     }
                  } while (playback && !stopped);
               }
            }
         }
         finally
         {
            Source.Stop();
            Source.clip = null;
            if (ac != null)
               Destroy(ac);

            if (_success) // && !error)
               onAudioEnd(_station);

            if (Codec == AudioCodec.MP3_NLayer)
            {
               if (nLayerReader != null)
               {
                  nLayerReader.Dispose();
                  nLayerReader = null;
               }
            }
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN) && !UNITY_EDITOR_OSX && !UNITY_EDITOR_LINUX
            else if (Codec == AudioCodec.MP3_NAudio)
            {
               if (nAudioReader != null)
               {
                  nAudioReader.Dispose();
                  nAudioReader = null;
               }
            }
#endif
            else if (Codec == AudioCodec.OGG_NVorbis)
            {
               if (nVorbisReader != null)
               {
                  nVorbisReader.Dispose();
                  nVorbisReader = null;

                  NVorbis.Mdct.ClearSetupCache();
               }
            }

            DataStream?.Dispose();

            if (error)
               onErrorInfo(_station, errorMessage);

            onPlaybackEnd(_station);
         }
      }

      private void readPCMData(float[] data)
      {
         if (data != null)
         {
            if (playback && !stopped && bufferAvailable)
            {
               if (Codec == AudioCodec.MP3_NLayer)
               {
                  if (nLayerReader != null)
                  {
                     try
                     {
                        if (nLayerReader.ReadSamples(data, 0, data.Length) > 0)
                        {
                           //do nothing
                        }
                        else
                        {
                           logNoMoreData();
                        }
                     }
                     catch (System.Exception ex)
                     {
                        logDataError(ex);
                     }
                  }
               }
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN) && !UNITY_EDITOR_OSX && !UNITY_EDITOR_LINUX
               else if (Codec == AudioCodec.MP3_NAudio)
               {
                  if (nAudioReader != null)
                  {
                     byte[] buffer = new byte[data.Length * 2];

                     try
                     {
                        int count;
                        if ((count = nAudioReader.Read(buffer, 0, buffer.Length)) > 0)
                        {
                           //System.Buffer.BlockCopy(Helper.ConvertByteArrayToFloatArray(buffer, count), 0, data, 0, count * 2);
                           float[] converted = buffer.CTToFloatArray(count);
                           System.Array.Copy(converted, 0, data, 0, converted.Length);
                        }
                        else
                        {
                           logNoMoreData();
                        }
                     }
                     catch (System.Exception ex)
                     {
                        logDataError(ex);
                     }
                  }
               }
#endif
               else if (Codec == AudioCodec.OGG_NVorbis)
               {
                  if (nVorbisReader != null)
                  {
                     try
                     {
                        if (nVorbisReader.ReadSamples(data, 0, data.Length) > 0)
                        {
                           //do nothing
                        }
                        else
                        {
                           logNoMoreData();
                        }
                     }
                     catch (System.Exception ex)
                     {
                        logDataError(ex);
                     }
                  }
               }
            }
            else
            {
               //System.Buffer.BlockCopy(new float[data.Length], 0, data, 0, data.Length * 4);
               System.Array.Copy(new float[data.Length], 0, data, 0, data.Length);
            }

            if (captureDataStream && DataStream != null)
            {
               byte[] bytes = data.CTToByteArray();
               DataStream.Write(bytes, 0, bytes.Length);
            }
         }
      }

      private void timerTask(ref RadioStation _station, ref bool _playback, ref float _nextRecordDelay)
      {
         int sleep = 100;
         System.DateTime lastDate = System.DateTime.Now;
         System.DateTime currentDate;

         float _pitchedTime = 0;
         float timeDifference = 0f;

         do
         {
            currentDate = System.DateTime.Now;

            if (!isBuffering)
            {
               timeDifference = (float)(currentDate - lastDate).TotalSeconds;
               _pitchedTime = timeDifference * pitch;

               Context.TotalPlayTime += _pitchedTime;
               _station.TotalPlayTime += _pitchedTime;
               PlayTime += _pitchedTime;
               RecordPlayTime += _pitchedTime;
               _nextRecordDelay -= _pitchedTime;
            }

            lastDate = currentDate;

            System.Threading.Thread.Sleep(sleep);
         } while (_playback);
      }

      private void readStream(ref RadioStation _station, ref bool _playback, ref System.IO.Stream _ms, ref bool _error, ref string _errorMessage, ref RecordInfo _nextRecordInfo, ref float _nextRecordDelay)
      {
         if (_station != null)
         {
            if (_station.Url.CTStartsWith(Constants.PREFIX_HTTP) || _station.Url.CTStartsWith(Constants.PREFIX_HTTPS))
            {
               try
               {
                  System.Net.ServicePointManager.ServerCertificateValidationCallback = Crosstales.Common.Util.NetworkHelper.RemoteCertificateValidationCallback;

                  using (Crosstales.Common.Util.CTWebClient client = new Crosstales.Common.Util.CTWebClient(int.MaxValue))
                  {
                     System.Net.HttpWebRequest _request = (System.Net.HttpWebRequest)client.CTGetWebRequest(_station.Url.Trim());

                     // clear old request header and build own header to receive ICY-metadata
                     _request.Headers.Clear();
                     _request.Headers.Add("GET", "/ HTTP/1.1");
                     _request.Headers.Add("Icy-MetaData", "1"); // needed to receive metadata information
                     if (_station.RequestHeaders?.Count > 0)
                     {
                        foreach (RequestHeaderTuple rht in _station.RequestHeaders)
                        {
                           if (!string.IsNullOrEmpty(rht.Key) && !string.IsNullOrEmpty(rht.Value))
                              _request.Headers.Add(rht.Key, rht.Value);
                        }
                     }

                     _request.UserAgent = "WinampMPEG/5.09";
                     //request.KeepAlive = true;

                     using (System.Net.HttpWebResponse _response = (System.Net.HttpWebResponse)_request.GetResponse())
                     {
                        // read blocksize to find metadata header
                        int _metaint = int.MaxValue;

                        if (!string.IsNullOrEmpty(_response.GetResponseHeader("icy-metaint")))
                           int.TryParse(_response.GetResponseHeader("icy-metaint"), out _metaint);

                        if (Constants.DEV_DEBUG)
                           Debug.LogWarning("icy-metaint: " + _metaint, this);

                        // server info
                        string serverInfo = string.IsNullOrEmpty(_response.GetResponseHeader("icy-notice2")) ? _station.ServerInfo : _response.GetResponseHeader("icy-notice2");
                        _station.ServerInfo = serverInfo;

                        if (Constants.DEV_DEBUG)
                           Debug.LogWarning("icy-notice2: " + serverInfo, this);

                        _station.Url = _response.ResponseUri.ToString(); //always update URL

                        if (_station.UpdateDataAtPlay)
                        {
                           // name
                           string _name = string.IsNullOrEmpty(_response.GetResponseHeader("icy-name")) || _response.GetResponseHeader("icy-name").Equals("-") ? _station.Name : _response.GetResponseHeader("icy-name");

                           if (Constants.DEV_DEBUG)
                              Debug.LogWarning("icy-name: '" + _name + "' - " + _station.Name, this);

                           _station.Name = _name;

                           // url
                           //string url = string.IsNullOrEmpty(_response.GetResponseHeader("icy-url")) ? _station.Station : _response.GetResponseHeader("icy-url");
                           //_station.Station = url;
                           //if (Constants.DEV_DEBUG)
                           //   Debug.LogWarning("icy-url: " + url, this);

                           //_station.Station = _response.ResponseUri.ToString();

                           // genres
                           string genres = string.IsNullOrEmpty(_response.GetResponseHeader("icy-genre")) ? _station.Genres : _response.GetResponseHeader("icy-genre");
                           _station.Genres = genres.Replace(',', ' ');

                           if (Constants.DEV_DEBUG)
                              Debug.LogWarning("icy-genre: " + genres, this);

                           // bitrate
                           if (!string.IsNullOrEmpty(_response.GetResponseHeader("icy-br")))
                           {
                              if (int.TryParse(_response.GetResponseHeader("icy-br"), out int bitrate))
                                 _station.Bitrate = Helper.NearestBitrate(bitrate, _station.Format);
                           }

                           if (Constants.DEV_DEBUG)
                              Debug.LogWarning("icy-br: " + _station.Bitrate, this);
                        }

                        using (System.IO.Stream _stream = _response.GetResponseStream())
                        {
                           if (_stream != null)
                           {
                              byte[] _buffer = new byte[_station.ChunkSize * Constants.FACTOR_KB];
                              _playback = true;
                              Context.TotalDataRequests++;
                              _station.TotalDataRequests++;

                              int _status = 0;
                              bool _isFirsttime = true;
                              int codepage = 0;

                              //_nextRecordDelay = 0f;

                              //RecordPlayTime = 0f;
                              //System.DateTime lastDate = System.DateTime.Now;
                              //System.DateTime currentDate;

                              //float timeDifference = 0f;
                              //float _pitchedTime = 0f;

                              do
                              {
                                 int _read;
                                 if ((_read = _stream.Read(_buffer, 0, _buffer.Length)) > 0)
                                 {
                                    //if (_read > 16384)
                                    //   Debug.LogWarning($"BIG READ: {_station.Name} - {_read}");

                                    Context.TotalDataSize += _read;
                                    _station.TotalDataSize += _read;

                                    /*
                                    currentDate = System.DateTime.Now;

                                    if (!isBuffering)
                                    {
                                       timeDifference = (float)(currentDate - lastDate).TotalSeconds;
                                       _pitchedTime = timeDifference * pitch;

                                       Context.TotalPlayTime += _pitchedTime;
                                       _station.TotalPlayTime += _pitchedTime;
                                       PlayTime += _pitchedTime;
                                       RecordPlayTime += _pitchedTime;
                                    }

                                    lastDate = currentDate;
                                    */

                                    int _offset = 0;

                                    if (_metaint > 0 && _read + _status > _metaint)
                                    {
                                       for (int ii = 0; ii < _read && _playback;)
                                       {
                                          if (_status == _metaint)
                                          {
                                             _status = 0;

                                             _ms.Write(_buffer, _offset, ii - _offset);
                                             _offset = ii;

                                             int _metadataLength = System.Convert.ToInt32(_buffer[ii]) * 16; // length of metadata header
                                             ii++;
                                             _offset++;

                                             if (_metadataLength > 0)
                                             {
                                                if (_metadataLength + _offset <= _read)
                                                {
                                                   byte[] metaDataBuffer = new byte[_metadataLength];

                                                   System.Array.Copy(_buffer, ii, metaDataBuffer, 0, _metadataLength);

                                                   if (codepage == 0)
                                                   {
                                                      codepage = 65001; //UTF8

                                                      Ude.CharsetDetector cdet = new Ude.CharsetDetector();
                                                      cdet.Feed(metaDataBuffer, 0, metaDataBuffer.Length);
                                                      cdet.DataEnd();
                                                      if (cdet.Charset != null)
                                                      {
                                                         //Debug.Log($"Charset: {cdet.Charset}, confidence: {cdet.Confidence}, codepage: {cdet.CodePage}");

                                                         if (cdet.Confidence > 0.5f)
                                                            codepage = cdet.CodePage;
                                                      }
                                                   }

                                                   //var codepages = System.Text.Encoding.GetEncodings().Select(e => new {e.DisplayName, e.Name, e.CodePage}).ToList();
                                                   //Debug.Log(codepages.CTDump());

                                                   _nextRecordInfo = new RecordInfo(System.Text.Encoding.GetEncoding(codepage).GetString(metaDataBuffer));
                                                   //_nextRecordInfo = new RecordInfo(System.Text.Encoding.UTF8.GetString(metaDataBuffer));

                                                   if (!_isFirsttime)
                                                   {
                                                      _nextRecordDelay = (float)(_ms.Length - _ms.Position) / (_station.Bitrate * 125);
                                                   }
                                                   else
                                                   {
                                                      _isFirsttime = false;
                                                   }

                                                   if (Constants.DEV_DEBUG)
                                                      Debug.Log($"Record: {_nextRecordInfo} - {_nextRecordDelay}", this);

                                                   ii += _metadataLength;
                                                   _offset += _metadataLength;

                                                   if (Constants.DEV_DEBUG)
                                                      Debug.LogWarning("RecordInfo read: " + _nextRecordInfo, this);
                                                }
                                                else
                                                {
                                                   if (Constants.DEV_DEBUG)
                                                      Debug.LogError("Info-frame outside of the buffer!", this);

                                                   ii = _read;
                                                   _status = _read - (_metadataLength + _offset);
                                                }
                                             }
                                          }
                                          else
                                          {
                                             _status++;
                                             ii++;
                                          }
                                       }

                                       if (_offset < _read)
                                          _ms.Write(_buffer, _offset, _read - _offset);
                                    }
                                    else
                                    {
                                       _status += _read;
                                       _ms.Write(_buffer, 0, _read);
                                    }

                                    //_nextRecordDelay -= _pitchedTime; //TODO add _pitchedTime to _nextRecordDelay in the assignment (to be more precise)?

                                    if (_nextRecordDelay < Constants.FLOAT_TOLERANCE && recordInfo?.Equals(_nextRecordInfo) == false)
                                    {
                                       if (!string.IsNullOrEmpty(_nextRecordInfo.Info))
                                       {
                                          if (!station.PlayedRecords.Contains(_nextRecordInfo))
                                             station.PlayedRecords.Add(_nextRecordInfo);

                                          if (!Context.AllPlayedRecords.Contains(_nextRecordInfo))
                                          {
                                             Context.AllPlayedRecords.Add(_nextRecordInfo);
                                          }
                                          else
                                          {
                                             _nextRecordInfo = Context.AllPlayedRecords[Context.AllPlayedRecords.IndexOf(_nextRecordInfo)];
                                          }
                                       }

                                       recordInfo.Duration = RecordPlayTime; //update the duration of the last entry!

                                       recordInfo = _nextRecordInfo;

                                       RecordPlayTime = 0f;
                                    }
                                 }
                              } while (_playback);
                           }
                        }
                     }
                  }
               }
               catch (System.Threading.ThreadAbortException)
               {
                  _playback = false;
               }
               catch (System.Exception ex)
               {
                  _error = true;
                  _errorMessage = _station + System.Environment.NewLine + "Could not read url after " + Helper.FormatSecondsToHRF(PlayTime) + "!" + System.Environment.NewLine + ex;
                  Debug.LogError(_errorMessage, this);

                  _playback = false;
               }
            }
            else
            {
               readStreamLegacy(ref _station, ref _playback, ref _ms, ref _error, ref _errorMessage);
            }
         }
      }

      private void readStreamLegacy(ref RadioStation _station, ref bool _playback, ref System.IO.Stream _ms, ref bool _error, ref string _errorMessage)
      {
         if (_station != null)
         {
            if (_station.Url.CTStartsWith("file://"))
               Debug.LogWarning(Constants.ASSET_NAME + " is intended for streams and not for files! Please consider using 'DJ' instead: " + Constants.ASSET_DJ, this);

            try
            {
               System.Net.ServicePointManager.ServerCertificateValidationCallback = Crosstales.Common.Util.NetworkHelper.RemoteCertificateValidationCallback;

               using (Crosstales.Common.Util.CTWebClient client = new Crosstales.Common.Util.CTWebClient(int.MaxValue))
               {
                  System.Net.WebRequest _request = client.CTGetWebRequest(_station.Url.Trim());

                  // clear old request header and build own header
                  if (_station.RequestHeaders?.Count > 0)
                  {
                     _request.Headers.Clear();

                     foreach (RequestHeaderTuple rht in _station.RequestHeaders)
                     {
                        if (!string.IsNullOrEmpty(rht.Key) && !string.IsNullOrEmpty(rht.Value))
                           _request.Headers.Add(rht.Key, rht.Value);
                     }
                  }

                  using (System.Net.WebResponse _response = _request.GetResponse())
                  {
                     using (System.IO.Stream _stream = _response.GetResponseStream())
                     {
                        if (_stream != null)
                        {
                           byte[] _buffer = new byte[_station.ChunkSize * Constants.FACTOR_KB];
                           _playback = true;
                           Context.TotalDataRequests++;
                           _station.TotalDataRequests++;

                           //System.DateTime lastDate = System.DateTime.Now;

                           do
                           {
                              int _read;
                              if ((_read = _stream.Read(_buffer, 0, _buffer.Length)) > 0)
                              {
                                 Context.TotalDataSize += _read;
                                 _station.TotalDataSize += _read;
/*
                                 if (!isBuffering)
                                 {
                                    float difference = (float)(System.DateTime.Now - lastDate).TotalSeconds;
                                    float _pitchedTime = difference * pitch;

                                    Context.TotalPlayTime += _pitchedTime;
                                    _station.TotalPlayTime += _pitchedTime;
                                    PlayTime += _pitchedTime;
                                    RecordPlayTime += _pitchedTime;
                                 }

                                 lastDate = System.DateTime.Now;
*/
                                 if (_playback)
                                    _ms.Write(_buffer, 0, _read);
                              }
                           } while (_playback);
                        }
                     }
                  }
               }
            }
            catch (System.Threading.ThreadAbortException)
            {
               _playback = false;
            }
            catch (System.Exception ex)
            {
               _error = true;
               _errorMessage = _station + System.Environment.NewLine + "Could not read url after " + Helper.FormatSecondsToHRF(PlayTime) + "!" + System.Environment.NewLine + ex;
               Debug.LogError(_errorMessage, this);

               _playback = false;
            }
         }
      }
#endif

      private void logNoMoreData()
      {
         error = true;
         errorMessage = station + System.Environment.NewLine + "No more data to read after " + Helper.FormatSecondsToHRF(PlayTime) + "! Please restart this station or choose another one.";
         Debug.LogError(errorMessage, this);

         playback = false;
      }

      private void logDataError(System.Exception ex)
      {
         error = true;
         errorMessage = station + System.Environment.NewLine + "Could not read audio after " + Helper.FormatSecondsToHRF(PlayTime) + "! This is typically a sign of a buffer underun -> Please try to increment the 'ChunkSize' and 'BufferSize':" + System.Environment.NewLine + ex;
         Debug.LogError(errorMessage, this);

         playback = false;
      }

      private void logUnsupportedPlatform()
      {
         errorMessage = "'Radio' is not supported on the current platform!";
         Debug.LogWarning(errorMessage, this);
         onErrorInfo(station, errorMessage);
      }

      private void killWorker()
      {
#if (!UNITY_WSA && !UNITY_WEBGL && !UNITY_XBOXONE) || UNITY_EDITOR
         worker.CTAbort();
         timer.CTAbort();
#endif
      }

      #endregion


      #region Event-trigger methods

      protected override void onPlaybackStart(RadioStation _station)
      {
         stopped = false;
         playCounter++;

         if (Config.DEBUG)
            Debug.Log("onPlaybackStart: " + _station, this);

         base.onPlaybackStart(_station);
      }

      protected override void onPlaybackEnd(RadioStation _station)
      {
         stopped = true;
         playCounter--;

         if (Config.DEBUG)
            Debug.Log("onPlaybackEnd: " + _station, this);

         if (recordInfo != null)
         {
            recordInfo.Duration = RecordPlayTime; //update the duration of the last entry!

            recordInfo = new RecordInfo();
         }

         base.onPlaybackEnd(_station);
      }

      protected override void onBufferingStart(RadioStation _station)
      {
         if (Config.DEBUG)
            Debug.Log("onBufferingStart: " + _station, this);

         base.onBufferingStart(_station);
      }

      protected override void onBufferingEnd(RadioStation _station)
      {
         if (Config.DEBUG)
            Debug.Log("onBufferingEnd: " + _station, this);

         base.onBufferingEnd(_station);
      }

      protected override void onAudioStart(RadioStation _station)
      {
         audioCounter++;

         if (Config.DEBUG)
            Debug.Log("onAudioStart: " + _station, this);

         base.onAudioStart(_station);
      }

      protected override void onAudioEnd(RadioStation _station)
      {
         audioCounter--;

         if (Config.DEBUG)
            Debug.Log("onAudioEnd: " + _station, this);

         base.onAudioEnd(_station);
      }

      protected override void onErrorInfo(RadioStation _station, string info)
      {
         if (Config.DEBUG)
            Debug.Log("onErrorInfo: " + _station + " - " + info, this);

         base.onErrorInfo(_station, info);
      }

      protected override void onRecordChange(RadioStation _station, RecordInfo newRecord)
      {
         if (Config.DEBUG)
            Debug.Log($"onRecordChange: {_station} - {newRecord}", this);

         base.onRecordChange(_station, newRecord);
      }

      protected override void onRecordPlayTimeUpdate(RadioStation _station, RecordInfo record, float playtime)
      {
         //if (Config.DEBUG)
         //   Debug.Log($"onRecordPlayTimeUpdate: {_station} - {record} - {playtime}", this);

         base.onRecordPlayTimeUpdate(_station, record, playtime);
      }

/*
      protected override void onRecordChange(RadioStation _station, RecordInfo newRecord)
      {
         if (recordInfo != null && !recordInfo.Equals(newRecord))
         {
            if (Config.DEBUG)
               Debug.Log("onRecordChange: " + _station + " - " + newRecord, this);

            recordInfo.Duration = RecordPlayTime; //update the duration of the last entry!

            recordInfo = newRecord;

            //RecordPlayTime = 0f;

            if (!string.IsNullOrEmpty(recordInfo.Info))
            {
               if (!station.PlayedRecords.Contains(recordInfo))
                  station.PlayedRecords.Add(recordInfo);

               if (!Context.AllPlayedRecords.Contains(recordInfo))
                  Context.AllPlayedRecords.Add(recordInfo);
            }

            base.onRecordChange(_station, newRecord);
         }
      }
*/
      protected override void onNextRecordChange(RadioStation _station, RecordInfo nextRecord, float _delay)
      {
         if (Config.DEBUG)
            Debug.Log("onNextRecordChange: " + _station + " - " + nextRecord, this);

         base.onNextRecordChange(_station, nextRecord, _delay);
      }

      protected override void onNextRecordDelayUpdate(RadioStation _station, RecordInfo nextRecord, float _delay)
      {
         //Debug.Log($"onNextRecordDelayUpdate: {_station} - {nextRecord} - {_delay}");

         if (_delay > 0f)
         {
            base.onNextRecordDelayUpdate(_station, nextRecord, _delay);
         }
         else
         {
            onRecordChange(_station, nextRecord);
         }
      }

      #endregion


      #region Overridden methods

      public override string ToString()
      {
         System.Text.StringBuilder result = new System.Text.StringBuilder();

         result.Append(GetType().Name);
         result.Append(Constants.TEXT_TOSTRING_START);

         result.Append("Station='");
         result.Append(station);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("PlayOnStart='");
         result.Append(playOnStart);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("HandleFocus='");
         result.Append(handleFocus);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("CacheStreamSize='");
         result.Append(cacheStreamSize);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("CaptureDataStream='");
         result.Append(captureDataStream);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER_END);

         result.Append(Constants.TEXT_TOSTRING_END);

         return result.ToString();
      }

      #endregion


      #region Editor-only methods

#if UNITY_EDITOR

      /// <summary>Plays the radio-station (Editor only).</summary>
      /// <param name="channels">Number of audio channels (default: 2, optional)</param>
      /// <param name="sampleRate">Sample rate of the audio (default: 44100, optional)</param>
      public virtual void PlayInEditor(int channels = 2, int sampleRate = 44100)
      {
         if (Helper.isEditorMode)
         {
            if (stopped)
            {
               if (Crosstales.Common.Util.NetworkHelper.isInternetAvailable)
               {
                  if (Helper.isSane(ref station))
                  {
                     if (channels > 0)
                     {
                        if (sampleRate >= 8000)
                        {
                           Codec = Helper.AudioCodecForAudioFormat(station.Format);

                           if (Codec == AudioCodec.None)
                           {
                              errorMessage = station + System.Environment.NewLine + "Audio format not supported - cant play station: " + station.Format;
                              Debug.LogError(errorMessage, this);

                              return;
                           }

                           if (station.ExcludedCodec == Codec)
                           {
                              errorMessage = station + System.Environment.NewLine + "Excluded codec matched - can't play station: " + Codec;
                              Debug.LogError(errorMessage, this);
                           }
                           else
                           {
                              maxPlayTime = int.MaxValue / sampleRate - 120; //reserve of 2 minutes

                              Source.clip = AudioClip.Create(station.Name, int.MaxValue, channels, sampleRate, true, readPCMData);

                              killWorker();
                              worker = new System.Threading.Thread(playAudioFromUrlInEditor);
                              worker.Start();

                              Source.Play();
                           }
                        }
                        else
                        {
                           errorMessage = station + System.Environment.NewLine + "The 'sampleRate' must be greater than 8000!";
                           Debug.LogError(errorMessage, this);
                        }
                     }
                     else
                     {
                        errorMessage = station + System.Environment.NewLine + "The number of 'channels' must be greater than 0!";
                        Debug.LogError(errorMessage, this);
                     }
                  }
                  else
                  {
                     errorMessage = station + System.Environment.NewLine + "Could not start playback. Please verify the station settings.";
                     Debug.LogError(errorMessage);
                  }
               }
               else
               {
                  errorMessage = "No internet connection available! Can't play (stream) any stations!";
                  Debug.LogError(errorMessage, this);
               }
            }
            else
            {
               errorMessage = station + System.Environment.NewLine + "Station is already playing!";
               Debug.LogWarning(errorMessage, this);
            }
         }
         else
         {
            Debug.LogWarning("'PlayInEditor()' works only inside the Unity Editor!", this);
         }
      }

      private void playAudioFromUrlInEditor()
      {
         RadioStation _station = station; //make reference to original (it could change)

         playback = false;
         stopped = false;
         restarted = false;
         error = false;
         errorMessage = string.Empty;

         PlayTime = 0f;
         RecordPlayTime = 0f;
         BufferProgress = 0f;
         bufferAvailable = false;
         float _bufferCurrentProgress = 0f;

         recordInfo = new RecordInfo();
         nextRecordInfo = new RecordInfo();
         nextRecordDelay = 0f;

         onPlaybackStart(_station);

         onBufferingStart(_station);
         onBufferingProgressUpdate(_station, BufferProgress);

         using (ms = new Crosstales.Common.Util.MemoryCacheStream(CacheStreamSize, Config.MAX_CACHESTREAMSIZE * Constants.FACTOR_KB))
         {
            worker = legacyMode ? new System.Threading.Thread(() => readStreamLegacy(ref _station, ref playback, ref ms, ref error, ref errorMessage)) : new System.Threading.Thread(() => readStream(ref _station, ref playback, ref ms, ref error, ref errorMessage, ref nextRecordInfo, ref nextRecordDelay));
            worker.Start();

            // Waiting for stream
            do
            {
               System.Threading.Thread.Sleep(30);
            } while (!playback && !stopped && !error);

            int _bufferSize = _station.BufferSize * Constants.FACTOR_KB + _station.ChunkSize * Constants.FACTOR_KB;

            // Pre-buffering some data to allow start playing
            if (SkipPreBuffering)
            {
               System.Threading.Thread.Sleep(500);
            }
            else
            {
               do
               {
                  BufferProgress = (float)ms.Length / _bufferSize;

                  if (Mathf.Abs(BufferProgress - _bufferCurrentProgress) > Constants.FLOAT_TOLERANCE)
                  {
                     onBufferingProgressUpdate(_station, BufferProgress);
                     _bufferCurrentProgress = BufferProgress;
                  }

                  System.Threading.Thread.Sleep(50);
               } while (playback && !stopped && ms.Length < _bufferSize);
            }

            BufferProgress = 1f;
            onBufferingProgressUpdate(_station, BufferProgress);

            bufferAvailable = true;
            onBufferingEnd(_station);

            if (playback && !stopped)
            {
               timer = new System.Threading.Thread(() => timerTask(ref _station, ref playback, ref nextRecordDelay));
               timer.Start();

               try
               {
                  if (Codec == AudioCodec.MP3_NLayer)
                  {
                     nLayerReader = new NLayer.MpegFile(ms);
                     if (nLayerReader.SampleRate < 32000 || nLayerReader.SampleRate > 48000)
                     {
                        error = true;
                        playback = false;

                        errorMessage = $"Only MP3 with layer 3 specs is supported (current {_station.SampleRate}Hz)! MPEG-1 (Audio Layer III) allows the following sample rates: 32000Hz, 44100Hz and 48000Hz!";
                        Debug.LogError(errorMessage, this);
                        onErrorInfo(_station, errorMessage);

                        return;
                     }
                  }
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN) && !UNITY_EDITOR_OSX && !UNITY_EDITOR_LINUX
                  else if (Codec == AudioCodec.MP3_NAudio)
                  {
                     nAudioReader = new NAudio.Wave.Mp3FileReader(ms);
                  }
#endif
                  else if (Codec == AudioCodec.OGG_NVorbis)
                  {
                     nVorbisReader = new NVorbis.VorbisReader(ms, false);
                  }
                  else
                  {
                     error = true;
                     playback = false;

                     errorMessage = _station + System.Environment.NewLine + "Unsupported codec: " + Codec;
                     Debug.LogError(errorMessage, this);

                     onErrorInfo(_station, errorMessage);

                     return;
                  }

                  if (Codec == AudioCodec.OGG_NVorbis)
                     ms.Position = 0;

                  onAudioStart(_station);

                  int _iterations = 0;
                  do
                  {
                     if (Codec == AudioCodec.OGG_NVorbis && _iterations % oggCacheCleanFrameCount == 0)
                     {
                        if (Constants.DEV_DEBUG)
                           Debug.Log("Clean cache: " + oggCacheCleanFrameCount + " - " + PlayTime, this);

                        NVorbis.Mdct.ClearSetupCache();
                     }

                     _iterations++;

                     System.Threading.Thread.Sleep(50);
                  } while (playback && !stopped);

                  onAudioEnd(_station);
               }
               catch (System.Exception)
               {
                  error = true;
                  errorMessage = _station + System.Environment.NewLine + "Could not play the stream -> Please try another station!";
                  Debug.LogError(errorMessage, this);
                  onErrorInfo(_station, errorMessage);

                  playback = false;
               }
               finally
               {
                  if (Codec == AudioCodec.MP3_NLayer)
                  {
                     if (nLayerReader != null)
                     {
                        nLayerReader.Dispose();
                        nLayerReader = null;
                     }
                  }
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN) && !UNITY_EDITOR_OSX && !UNITY_EDITOR_LINUX
                  else if (Codec == AudioCodec.MP3_NAudio)
                  {
                     if (nAudioReader != null)
                     {
                        nAudioReader.Dispose();
                        nAudioReader = null;
                     }
                  }
#endif
                  else if (Codec == AudioCodec.OGG_NVorbis)
                  {
                     if (nVorbisReader != null)
                     {
                        nVorbisReader.Dispose();
                        nVorbisReader = null;

                        NVorbis.Mdct.ClearSetupCache();
                     }
                  }
               }
            }
         }

         onPlaybackEnd(_station);
      }

#endif

      #endregion
   }
}
// © 2015-2024 crosstales LLC (https://www.crosstales.com)