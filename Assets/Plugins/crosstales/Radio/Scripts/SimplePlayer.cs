using UnityEngine;
using Crosstales.Radio.Util;
using Crosstales.Radio.Model;
using Crosstales.Radio.Model.Enum;
using Crosstales.Radio.Set;

namespace Crosstales.Radio
{
   /// <summary>Simple player.</summary>
   [ExecuteInEditMode]
   [HelpURL("https://www.crosstales.com/media/data/assets/radio/api/class_crosstales_1_1_radio_1_1_simple_player.html")]
   public class SimplePlayer : BasePlayer, ISet
   {
      #region Variables

      [UnityEngine.Serialization.FormerlySerializedAsAttribute("Player")] [Header("General Settings"), Tooltip("'RadioPlayer' from the scene (optional)."), SerializeField]
      private RadioPlayer player;

      [UnityEngine.Serialization.FormerlySerializedAsAttribute("Set")] [Tooltip("'RadioSet' from the scene."), SerializeField]
      private RadioSet set;

      [UnityEngine.Serialization.FormerlySerializedAsAttribute("Filter")] [Tooltip("Global RadioFilter (active if no explicit filter is given)."), SerializeField]
      private RadioFilter filter;

      [UnityEngine.Serialization.FormerlySerializedAsAttribute("RetryOnError")] [Header("Retry Settings"), Tooltip("Retry to start the radio on an error (default: false)."), SerializeField]
      private bool retryOnError;

      [UnityEngine.Serialization.FormerlySerializedAsAttribute("Retries")] [Tooltip("Defines how many times should the radio station restart after an error before giving up (default: 3)."), SerializeField]
      private int retries = 3;


      [UnityEngine.Serialization.FormerlySerializedAsAttribute("PlayOnStart")] [Header("Behaviour Settings"), Tooltip("Play a radio on start (default: false)."), SerializeField]
      private bool playOnStart;

      [UnityEngine.Serialization.FormerlySerializedAsAttribute("PlayEndless")] [Tooltip("Enable endless play (default: true)."), SerializeField]
      private bool playEndless = true;

      [UnityEngine.Serialization.FormerlySerializedAsAttribute("PlayRandom")] [Tooltip("Play the radio stations in random order (default: false)."), SerializeField]
      private bool playRandom;

        [Tooltip("In case 'Next' or 'Previous' is called, follow the logical direction through the playlist (default: true)."), SerializeField]
        private bool _followDirection = true;

        private bool playedOnStart;
      private bool stopped = true;
      private int invokeDelayCounter = 1;
      private bool started;
      private float lastPlaytime = float.MinValue;

      private bool isDirectionNext = true;

      private RadioFilter currentFilter;

      #endregion


      #region Properties

      /// <summary>'RadioPlayer' from the scene.</summary>
      public RadioPlayer Player
      {
         get
         {
            if (player == null)
               player = RadioPlayer.Instance;

            return player;
         }

         set => player = value;
      }

      /// <summary>'RadioSet' from the scene.</summary>
      public RadioSet Set
      {
         get => set;
         set => set = value;
      }

      /// <summary>Global RadioFilter (active if no explicit filter is given).</summary>
      public RadioFilter Filter
      {
         get => filter;
         set => filter = value;
      }

      /// <summary>Retry to start the radio on an error.</summary>
      public bool RetryOnError
      {
         get => retryOnError;
         set => retryOnError = value;
      }

      /// <summary>Defines how many times should the radio station restart after an error before giving up.</summary>
      public int Retries
      {
         get => retries;
         set => retries = value;
      }

      /// <summary>Play a radio on start.</summary>
      public bool PlayOnStart
      {
         get => playOnStart;
         set => playOnStart = value;
      }

      /// <summary>Enable endless play.</summary>
      public bool PlayEndless
      {
         get => playEndless;
         set => playEndless = value;
      }

      /// <summary>Play the radio stations in random order.</summary>
      public bool PlayRandom
      {
         get => playRandom;
         set => playRandom = value;
      }

        /// <summary>In case 'Next' or 'Previous' is called, follow the logical direction through the playlist.</summary>
        public bool FollowDirection
        {
            get => _followDirection;
            set => _followDirection = value;
        }

        protected override PlaybackStartEvent onPlaybackStarted => OnPlaybackStarted;

      protected override PlaybackEndEvent onPlaybackEnded => OnPlaybackEnded;

      protected override BufferingStartEvent onBufferingStarted => OnBufferingStarted;

      protected override BufferingEndEvent onBufferingEnded => OnBufferingEnded;

      protected override AudioStartEvent onAudioStarted => OnAudioStarted;

      protected override AudioEndEvent onAudioEnded => OnAudioEnded;

      protected override RecordChangeEvent onRecordChanged => OnRecordChanged;

      protected override ErrorEvent onError => OnError;

      #endregion


      #region Events

      [Header("Events")] public PlaybackStartEvent OnPlaybackStarted;
      public PlaybackEndEvent OnPlaybackEnded;
      public BufferingStartEvent OnBufferingStarted;
      public BufferingEndEvent OnBufferingEnded;
      public AudioStartEvent OnAudioStarted;
      public AudioEndEvent OnAudioEnded;
      public RecordChangeEvent OnRecordChanged;
      public StationChangeEvent OnStationChanged;
      public FilterChangeEvent OnFilterChanged;
      public StationsChangeEvent OnStationsChanged;
      public ProviderReadyEvent OnProviderReadyEvent;
      public ErrorEvent OnError;

      /// <summary>An event triggered whenever the filter changes.</summary>
      public event FilterChange OnFilterChange;

      /// <summary>An event triggered whenever the stations change.</summary>
      public event StationsChange OnStationsChange;

      /// <summary>An event triggered whenever all providers are ready.</summary>
      public event ProviderReady OnProviderReady;

      /// <summary>An event triggered whenever an radio station changes.</summary>
      public event StationChange OnStationChange;

      #endregion


      #region MonoBehaviour methods

      private void Start()
      {
         //Helper.ApplicationIsPlaying = Application.isPlaying; //needed to enforce the right mode

         if (Player != null && Set != null)
         {
            Player.OnPlaybackStart += onPlaybackStart;
            Player.OnPlaybackEnd += onPlaybackEnd;
            Player.OnAudioStart += onAudioStart;
            Player.OnAudioEnd += onAudioEnd;
            Player.OnAudioPlayTimeUpdate += onAudioPlayTimeUpdate;
            Player.OnBufferingStart += onBufferingStart;
            Player.OnBufferingEnd += onBufferingEnd;
            Player.OnBufferingProgressUpdate += onBufferingProgressUpdate;
            Player.OnErrorInfo += onErrorInfo;
            Player.OnRecordChange += onRecordChange;
            Player.OnRecordPlayTimeUpdate += onRecordPlayTimeUpdate;
            Player.OnNextRecordChange += onNextRecordChange;
            Player.OnNextRecordDelayUpdate += onNextRecordDelayUpdate;
            Set.OnProviderReady += onProviderReady;
            Set.OnStationsChange += onStationsChange;
            Set.OnFilterChange += onFilterChange;
         }
         else
         {
            if (!Helper.isEditorMode)
            {
               Debug.LogError("'Player' or 'Set' are null!", this);
            }
         }

         if (Filter != null)
            currentFilter = new RadioFilter(Filter);
      }

      private void Update()
      {
         if (Filter == null && currentFilter != null)
         {
            currentFilter = null;
            onFilterChange();
         }
         else if (Filter != null && currentFilter == null ||
                  currentFilter?.Equals(Filter) == false)
         {
            currentFilter = new RadioFilter(Filter);
            onFilterChange();
         }
      }

      private void OnDestroy()
      {
         if (Player != null) //don't use property! It will create a new instance of RadioPlayer.
         {
            Player.OnPlaybackStart -= onPlaybackStart;
            Player.OnPlaybackEnd -= onPlaybackEnd;
            Player.OnAudioStart += onAudioStart;
            Player.OnAudioEnd += onAudioEnd;
            Player.OnAudioPlayTimeUpdate -= onAudioPlayTimeUpdate;
            Player.OnBufferingStart -= onBufferingStart;
            Player.OnBufferingEnd -= onBufferingEnd;
            Player.OnBufferingProgressUpdate -= onBufferingProgressUpdate;
            Player.OnErrorInfo -= onErrorInfo;
            Player.OnRecordChange -= onRecordChange;
            Player.OnRecordPlayTimeUpdate -= onRecordPlayTimeUpdate;
            Player.OnNextRecordChange -= onNextRecordChange;
            Player.OnNextRecordDelayUpdate -= onNextRecordDelayUpdate;
         }

         if (Set != null)
         {
            Set.OnProviderReady -= onProviderReady;
            Set.OnStationsChange -= onStationsChange;
            Set.OnFilterChange -= onFilterChange;
         }
      }

      private void OnValidate()
      {
         if (Retries < 0)
            Retries = 0;
      }

      #endregion


      #region Public methods

      /// <summary>Plays the next radio station.</summary>
      public void Next()
      {
         Next(PlayRandom);
      }

      /// <summary>Plays the next (normal/random) radio station.</summary>
      /// <param name="random">Play a random radio station</param>
      /// <param name="_filter">Filter (default: null, optional)</param>
      /// <returns>Next RadioStation</returns>
      public RadioStation Next(bool random, RadioFilter _filter = null)
      {
         if (this != null && !isActiveAndEnabled)
            return null;

         isDirectionNext = true;

         RadioStation station = Set.NextStation(random, getFilter(_filter));

         if (station != null)
         {
            Player.Station = station;

            Play();

            return station;
         }

         return null;
      }

      /// <summary>Plays the previous radio station.</summary>
      public void Previous()
      {
         Previous(PlayRandom);
      }

      /// <summary>Plays the previous radio station.</summary>
      /// <param name="random">Play a random radio station</param>
      /// <param name="_filter">Filter (default: null, optional)</param>
      /// <returns>Previous RadioStation</returns>
      public RadioStation Previous(bool random, RadioFilter _filter = null)
      {
         if (this != null && !isActiveAndEnabled)
            return null;

         isDirectionNext = false;

         RadioStation station = Set.PreviousStation(random, getFilter(_filter));

         if (station != null)
         {
            Player.Station = station;

            Play();

            return station;
         }

         return null;
      }

      #endregion


      #region Private methods

      private void play()
      {
         Player.Play();

         onStationChange(Player.Station);
      }

      private void playInvoker()
      {
         if (started)
         {
            Play();
         }
      }

      private RadioFilter getFilter(RadioFilter _filter = null)
      {
         if (_filter?.isFiltering == true)
            return _filter;

         //return Filter.isFiltering ? Filter : (Set.Filter.isFiltering ? Set.Filter : null);
         return Filter.isFiltering ? Filter : null;
      }

      private void playEndlessly()
      {
         if (!stopped && PlayEndless)
         {
            invokeDelayCounter = 1;

            if (isDirectionNext || !FollowDirection)
            {
               Next(PlayRandom);
            }
            else
            {
               Previous(PlayRandom);
            }
         }
      }

      #endregion


      #region Callback & event-trigger methods

      protected override void onAudioStart(RadioStation station)
      {
         started = true;

         base.onAudioStart(station);
      }

      protected override void onAudioEnd(RadioStation station)
      {
         started = false;

         base.onAudioEnd(station);
      }

      protected override void onAudioPlayTimeUpdate(RadioStation station, float _playtime)
      {
         if (_playtime > 30f) //reset restartCounter after 30 seconds
            invokeDelayCounter = 1;

         base.onAudioPlayTimeUpdate(station, _playtime);
      }

      protected override void onErrorInfo(RadioStation station, string info)
      {
         //if (Helper.isInternetAvailable)
         //{
         if (RetryOnError && started)
         {
            if (invokeDelayCounter < Retries)
            {
               Stop();

               Debug.LogWarning("Error occurred -> Restarting station." + System.Environment.NewLine + info, this);

               Invoke(nameof(playInvoker), Constants.INVOKE_DELAY * invokeDelayCounter);

               invokeDelayCounter++;
            }
            else
            {
               if (PlayEndless)
               {
                  playEndlessly();
               }
               else
               {
                  Stop();

                  Debug.LogError("Restarting station failed more than " + Retries + " times - giving up!" + System.Environment.NewLine + info, this);
               }
            }
         }
         else
         {
            if (PlayEndless)
            {
               playEndlessly();
            }
            else
            {
               Stop();

               Debug.LogError("Could not start the station '" + station.Name + "'! Please try another station. " + System.Environment.NewLine + info, this);
            }
         }
         //}

         base.onErrorInfo(station, info);
      }

      private void onProviderReady()
      {
         if (Config.DEBUG)
            Debug.Log("Provider ready - all stations loaded.", this);

         if (!Helper.isEditorMode && PlayOnStart && !playedOnStart)
         {
            playedOnStart = true;
            Play();
         }

         if (!Helper.isEditorMode)
            OnProviderReadyEvent?.Invoke();

         OnProviderReady?.Invoke();
      }

      private void onStationsChange()
      {
         if (Config.DEBUG)
            Debug.Log("onStationsChange SP", this);

         if (!Helper.isEditorMode && PlayOnStart && !playedOnStart)
         {
            playedOnStart = true;
            Play();
         }

         if (!Helper.isEditorMode)
            OnStationsChanged?.Invoke();

         OnStationsChange?.Invoke();
      }

      private void onFilterChange()
      {
         /*
         if (isReady)
         {
            RadioStation station = Set.StationFromIndex(PlayRandom, -1, getFilter());

            if (station != null)
               Player.Station = station;
         }
         */
         if (!Helper.isEditorMode)
            OnFilterChanged.Invoke();

         OnFilterChange?.Invoke();
      }

      private void onStationChange(RadioStation newStation)
      {
         if (!Helper.isEditorMode)
            OnStationChanged?.Invoke(newStation.Name, newStation.GetHashCode());

         OnStationChange?.Invoke(newStation);
      }

      #endregion


      #region Set wrapper

      public System.Collections.Generic.List<RadioStation> Stations => Set != null ? Set.Stations : new System.Collections.Generic.List<RadioStation>();

      public System.Collections.Generic.List<RadioStation> RandomStations => Set != null ? Set.RandomStations : new System.Collections.Generic.List<RadioStation>();

      public bool isReady => Set != null && Set.isReady;

      public int CurrentStationIndex
      {
         get => Set != null ? Set.CurrentStationIndex : 0;
         set
         {
            if (Set != null)
               Set.CurrentStationIndex = value;
         }
      }

      public int CurrentRandomStationIndex
      {
         get => Set != null ? Set.CurrentRandomStationIndex : 0;
         set
         {
            if (Set != null)
               Set.CurrentRandomStationIndex = value;
         }
      }

      public System.Collections.Generic.List<RadioStation> GetStations(bool random = false, RadioFilter _filter = null)
      {
         return Set != null ? Set.GetStations(random, getFilter(_filter)) : new System.Collections.Generic.List<RadioStation>();
      }

      public int CountStations(RadioFilter _filter = null)
      {
         return Set != null ? Set.CountStations(getFilter(_filter)) : 0;
      }

      public RadioStation StationFromIndex(bool random = false, int index = -1, RadioFilter _filter = null)
      {
         return Set != null ? Set.StationFromIndex(random, index, getFilter(_filter)) : null;
      }

      public RadioStation StationFromHashCode(int hashCode)
      {
         return Set != null ? Set.StationFromHashCode(hashCode) : null;
      }

      public RadioStation NextStation(bool random = false, RadioFilter _filter = null)
      {
         return Set != null ? Set.NextStation(random, getFilter(_filter)) : null;
      }

      public RadioStation PreviousStation(bool random = false, RadioFilter _filter = null)
      {
         return Set != null ? Set.PreviousStation(random, getFilter(_filter)) : null;
      }

      public System.Collections.Generic.List<RadioStation> StationsByName(bool desc = false, RadioFilter _filter = null)
      {
         return Set != null ? Set.StationsByName(desc, getFilter(_filter)) : new System.Collections.Generic.List<RadioStation>();
      }

      public System.Collections.Generic.List<RadioStation> StationsByURL(bool desc = false, RadioFilter _filter = null)
      {
         return Set != null ? Set.StationsByURL(desc, getFilter(_filter)) : new System.Collections.Generic.List<RadioStation>();
      }

      public System.Collections.Generic.List<RadioStation> StationsByFormat(bool desc = false, RadioFilter _filter = null)
      {
         return Set != null ? Set.StationsByFormat(desc, getFilter(_filter)) : new System.Collections.Generic.List<RadioStation>();
      }

      public System.Collections.Generic.List<RadioStation> StationsByStation(bool desc = false, RadioFilter _filter = null)
      {
         return Set != null ? Set.StationsByStation(desc, getFilter(_filter)) : new System.Collections.Generic.List<RadioStation>();
      }

      public System.Collections.Generic.List<RadioStation> StationsByBitrate(bool desc = false, RadioFilter _filter = null)
      {
         return Set != null ? Set.StationsByBitrate(desc, getFilter(_filter)) : new System.Collections.Generic.List<RadioStation>();
      }

      public System.Collections.Generic.List<RadioStation> StationsByGenres(bool desc = false, RadioFilter _filter = null)
      {
         return Set != null ? Set.StationsByGenres(desc, getFilter(_filter)) : new System.Collections.Generic.List<RadioStation>();
      }

      public System.Collections.Generic.List<RadioStation> StationsByCities(bool desc = false, RadioFilter _filter = null)
      {
         return Set != null ? Set.StationsByCities(desc, getFilter(_filter)) : new System.Collections.Generic.List<RadioStation>();
      }

      public System.Collections.Generic.List<RadioStation> StationsByCountries(bool desc = false, RadioFilter _filter = null)
      {
         return Set != null ? Set.StationsByCountries(desc, getFilter(_filter)) : new System.Collections.Generic.List<RadioStation>();
      }

      public System.Collections.Generic.List<RadioStation> StationsByLanguages(bool desc = false, RadioFilter _filter = null)
      {
         return Set != null ? Set.StationsByLanguages(desc, getFilter(_filter)) : new System.Collections.Generic.List<RadioStation>();
      }

      public System.Collections.Generic.List<RadioStation> StationsByRating(bool desc = false, RadioFilter _filter = null)
      {
         return Set != null ? Set.StationsByRating(desc, getFilter(_filter)) : new System.Collections.Generic.List<RadioStation>();
      }

      public void Load()
      {
         if (Set != null)
            Set.Load();
      }

      public void Save(string path, RadioFilter _filter = null)
      {
         if (Set != null)
            Set.Save(path, _filter);
      }

      public void RandomizeStations(bool resetIndex = true)
      {
         if (Set != null)
            Set.RandomizeStations();
      }

      #endregion


      #region Turntable wrapper

      public override RadioStation Station
      {
         get => Player.Station;

         set => Player.Station = value;
      }

      public override bool HandleFocus
      {
         get => Player.HandleFocus;

         set => Player.HandleFocus = value;
      }

      public override int CacheStreamSize
      {
         get => Player.CacheStreamSize;

         set => Player.CacheStreamSize = value;
      }

      public override bool LegacyMode
      {
         get => Player.LegacyMode;

         set => Player.LegacyMode = value;
      }

      public override bool CaptureDataStream
      {
         get => Player.CaptureDataStream;

         set => Player.CaptureDataStream = value;
      }

      public override bool SkipPreBuffering
      {
         get => Player.SkipPreBuffering;

         set => Player.SkipPreBuffering = value;
      }

      public override AudioSource Source
      {
         get => Player.Source;

         protected set
         {
            //ignore
         }
      }

      public override AudioCodec Codec
      {
         get => Player.Codec;

         protected set
         {
            //ignore
         }
      }

      public override float PlayTime
      {
         get => Player.PlayTime;

         protected set
         {
            //ignore
         }
      }

      public override float BufferProgress
      {
         get => Player.BufferProgress;

         protected set
         {
            //ignore
         }
      }

      public override bool isPlayback => Player.isPlayback;

      public override bool isAudioPlaying => Player.isAudioPlaying;

      public override bool isBuffering => Player.isBuffering;

      public override float RecordPlayTime
      {
         get => Player.RecordPlayTime;

         protected set
         {
            //ignore
         }
      }

      public override RecordInfo RecordInfo => Player.RecordInfo;

      public override RecordInfo NextRecordInfo => Player.RecordInfo;

      public override float NextRecordDelay => Player.NextRecordDelay;

      public override long CurrentBufferSize => Player.CurrentBufferSize;

      public override long CurrentDownloadSpeed => Player.CurrentDownloadSpeed;

      public override Crosstales.Common.Util.MemoryCacheStream DataStream
      {
         get => Player.DataStream;

         protected set
         {
            //ignore
         }
      }

      public override int Channels => Player.Channels;

      public override int SampleRate => Player.SampleRate;

      public override float Volume
      {
         get => Player.Volume;

         set => Player.Volume = value;
      }

      public override float Pitch
      {
         get => Player.Pitch;

         set => Player.Pitch = value;
      }

      public override float StereoPan
      {
         get => Player.StereoPan;

         set => Player.StereoPan = value;
      }

      public override bool isMuted
      {
         get => Player.isMuted;

         set => Player.isMuted = value;
      }

      public override void Play()
      {
         if (this != null && !isActiveAndEnabled)
            return;

         if (Player != null)
         {
            float currentTime = Time.realtimeSinceStartup;

            if (lastPlaytime + Constants.PLAY_CALL_SPEED < currentTime)
            {
               lastPlaytime = currentTime;

               Stop();

               if (string.IsNullOrEmpty(Player.Station?.Url))
               {
                  RadioStation station = Set.NextStation(PlayRandom, getFilter());

                  if (station != null)
                  {
                     Player.Station = station;
                  }
                  else
                  {
                     Debug.LogWarning("No station set, 'Play' not possible.");
                     return;
                  }
               }

               stopped = false;

               if (Helper.isEditorMode)
               {
#if UNITY_EDITOR
                  Player.PlayInEditor();
#endif
               }
               else
               {
                  Invoke(nameof(play), Constants.INVOKE_DELAY);
               }
            }
            else
            {
               Debug.LogWarning("'Play' called too fast - please slow down!", this);
            }
         }
      }

      public override void Stop()
      {
         if (this != null && !isActiveAndEnabled)
            return;

         Player.Stop();

         stopped = true;
      }
/*
      public override void Silence()
      {
         if (player != null)
            player.Silence();
      }
*/

      public override void Restart(float invokeDelay = Constants.INVOKE_DELAY)
      {
         Player.Restart(invokeDelay);
      }

      public override void Mute()
      {
         Player.Mute();
      }

      public override void UnMute()
      {
         Player.UnMute();
      }

      #endregion
   }
}
// © 2016-2024 crosstales LLC (https://www.crosstales.com)