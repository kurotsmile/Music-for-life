using UnityEngine;
using System.Linq;
using Crosstales.Radio.Util;
using Crosstales.Radio.Model;
using Crosstales.Radio.Model.Enum;
using Crosstales.Radio.Set;

//TODO add "PlayerByCities", "PlayerByCountries" and "PlayerByLanguages, PlayerFromIndex"
namespace Crosstales.Radio
{
   /// <summary>Radio manager for multiple radio players.</summary>
   [ExecuteInEditMode]
   [HelpURL("https://www.crosstales.com/media/data/assets/radio/api/class_crosstales_1_1_radio_1_1_radio_manager.html")]
   public class RadioManager : MonoBehaviour, ISet
   {
      #region Variables

      /// <summary>'Set' from the scene.</summary>
      [Header("General Settings")] [Tooltip("'Set' from the scene.")] public RadioSet Set;

      /// <summary>Global RadioFilter (active if no explicit filter is given).</summary>
      [Tooltip("Global RadioFilter (active if no explicit filter is given).")] public RadioFilter Filter;


      /// <summary>Calls 'Load' on Start (default: false).</summary>
      [Header("Behaviour Settings")] [Tooltip("Calls 'Load' on Start (default: false).")] public bool LoadOnStart;

      /// <summary>Calls 'Load' on Start in Editor (default: false).</summary>
      [Tooltip("Calls 'Load' on Start in Editor (default: false).")] public bool LoadOnStartInEditor;


      /// <summary>Instantiate RadioPlayer (default: false).</summary>
      [Header("Player Settings")] [Tooltip("Instantiate RadioPlayers (default: false).")] public bool InstantiateRadioPlayers;

      /// <summary>Prefab of the RadioPlayer.</summary>
      [Tooltip("Prefab of the RadioPlayer.")] public GameObject RadioPrefab;

      private int playerIndex = -1;
      private readonly System.Collections.Generic.List<RadioPlayer> randomPlayers = new System.Collections.Generic.List<RadioPlayer>(Constants.INITIAL_LIST_SIZE);
      private int randomPlayerIndex = -1;

      private RadioPlayer currentRadioPlayer;

      private readonly System.Collections.Generic.List<RadioPlayer> players = new System.Collections.Generic.List<RadioPlayer>(Constants.INITIAL_LIST_SIZE);

      private bool clearedStation = true;
      private bool clearedStationRandom = true;


      private RadioFilter lastPlayerFilter;
      private RadioFilter lastRandomPlayerFilter;
      private System.Collections.Generic.List<RadioPlayer> lastFilteredPlayers;
      private System.Collections.Generic.List<RadioPlayer> lastFilteredRandomPlayers;


      // Filter specific fields
      private RadioFilter playersByNameFilterDesc;
      private RadioFilter playersByNameFilterAsc;
      private System.Collections.Generic.List<RadioPlayer> playersByNameDesc = new System.Collections.Generic.List<RadioPlayer>();
      private System.Collections.Generic.List<RadioPlayer> playersByNameAsc = new System.Collections.Generic.List<RadioPlayer>();

      private RadioFilter playersByURLFilterDesc;
      private RadioFilter playersByURLFilterAsc;
      private System.Collections.Generic.List<RadioPlayer> playersByURLDesc = new System.Collections.Generic.List<RadioPlayer>();
      private System.Collections.Generic.List<RadioPlayer> playersByURLAsc = new System.Collections.Generic.List<RadioPlayer>();

      private RadioFilter playersByFormatFilterDesc;
      private RadioFilter playersByFormatFilterAsc;
      private System.Collections.Generic.List<RadioPlayer> playersByFormatDesc = new System.Collections.Generic.List<RadioPlayer>();
      private System.Collections.Generic.List<RadioPlayer> playersByFormatAsc = new System.Collections.Generic.List<RadioPlayer>();

      private RadioFilter playersByStationFilterDesc;
      private RadioFilter playersByStationFilterAsc;
      private System.Collections.Generic.List<RadioPlayer> playersByStationDesc = new System.Collections.Generic.List<RadioPlayer>();
      private System.Collections.Generic.List<RadioPlayer> playersByStationAsc = new System.Collections.Generic.List<RadioPlayer>();

      private RadioFilter playersByBitrateFilterDesc;
      private RadioFilter playersByBitrateFilterAsc;
      private System.Collections.Generic.List<RadioPlayer> playersByBitrateDesc = new System.Collections.Generic.List<RadioPlayer>();
      private System.Collections.Generic.List<RadioPlayer> playersByBitrateAsc = new System.Collections.Generic.List<RadioPlayer>();

      private RadioFilter playersByGenresFilterDesc;
      private RadioFilter playersByGenresFilterAsc;
      private System.Collections.Generic.List<RadioPlayer> playersByGenresDesc = new System.Collections.Generic.List<RadioPlayer>();
      private System.Collections.Generic.List<RadioPlayer> playersByGenresAsc = new System.Collections.Generic.List<RadioPlayer>();

      private RadioFilter playersByRatingFilterDesc;
      private RadioFilter playersByRatingFilterAsc;
      private System.Collections.Generic.List<RadioPlayer> playersByRatingDesc = new System.Collections.Generic.List<RadioPlayer>();
      private System.Collections.Generic.List<RadioPlayer> playersByRatingAsc = new System.Collections.Generic.List<RadioPlayer>();

      private RadioFilter currentFilter;

      private Transform tf;

      //private static int count = 0;
      private bool isWorking;

      #endregion


      #region Properties

      /// <summary>List of all instantiated RadioPlayer.</summary>
      public System.Collections.Generic.List<RadioPlayer> Players => players;

      /// <summary>Is any of the RadioPlayers in playback-mode?</summary>
      /// <returns>True if any of the RadioPlayers is in playback-mode.</returns>
      public bool isPlayback
      {
         get { return Players.Any(rp => rp != null && rp.isPlayback); }
      }

      /// <summary>Is any of the RadioPlayers playing audio?</summary>
      /// <returns>True if any of the RadioPlayers is playing audio.</returns>
      public bool isAudioPlaying
      {
         get { return Players.Any(rp => rp != null && rp.isAudioPlaying); }
      }

      /// <summary>Is any of the RadioPlayers buffering?</summary>
      /// <returns>True if any of the RadioPlayers is buffering.</returns>
      public bool isBuffering
      {
         get { return Players.Any(rp => rp != null && rp.isBuffering); }
      }

      #endregion


      #region Events

      [Header("Events")] public FilterChangeEvent OnFilterChanged;
      public StationsChangeEvent OnStationsChanged;
      public ProviderReadyEvent OnProviderReadyEvent;

      /// <summary>An event triggered whenever the filter changes.</summary>
      public event FilterChange OnFilterChange;

      /// <summary>An event triggered whenever the stations change.</summary>
      public event StationsChange OnStationsChange;

      /// <summary>An event triggered whenever all providers are ready.</summary>
      public event ProviderReady OnProviderReady;

      #endregion


      #region MonoBehaviour methods

      private void Start()
      {
         tf = transform;

         if (LoadOnStart && !Helper.isEditorMode)
         {
            Invoke(nameof(Load), 0.1f);
         }

         if (LoadOnStartInEditor && Helper.isEditorMode)
         {
            Load();
         }

         if (Filter != null)
         {
            currentFilter = new RadioFilter(Filter);
         }
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

      private void OnEnable()
      {
         if (Set != null)
         {
            Set.OnProviderReady += onProviderReady;
            Set.OnStationsChange += onStationsChange;
         }
      }

      private void OnDisable()
      {
         if (Set != null)
         {
            Set.OnProviderReady -= onProviderReady;
            Set.OnStationsChange -= onStationsChange;
         }
      }

      #endregion


      #region Public methods

      /// <summary>Get all RadioPlayer for a given RadioFilter.</summary>
      /// <param name="random">Return random RadioPlayer (default: false, optional)</param>
      /// <param name="filter">Filter for the radio players (default: null, optional)</param>
      /// <returns>All RadioPlayer for a given RadioFilter.</returns>
      public System.Collections.Generic.List<RadioPlayer> GetPlayers(bool random = false, RadioFilter filter = null)
      {
         return filterPlayers(random, getFilter(filter)).ToList();
      }

      /// <summary>Count all RadioPlayer for a given RadioFilter.</summary>
      /// <param name="filter">Filter for the radio players (default: null, optional)</param>
      /// <returns>Number of all RadioPlayer for a given RadioFilter.</returns>
      public int CountPlayers(RadioFilter filter = null)
      {
         return getFilter(filter) == null ? Players.Count : filterPlayers(false, getFilter(filter)).ToList().Count;
      }

      /// <summary>Play all radios of this manager at once.</summary>
      public void PlayAll()
      {
         StopAll();

         foreach (RadioPlayer rp in Players.Where(rp => rp != null))
         {
            rp.Play();
         }
      }

      /// <summary>Radio player from a given index (normal/random) from this manager.</summary>
      /// <param name="random">Return a random radio player (default: false, optional)</param>
      /// <param name="index">Index of the radio player (default: -1, optional)</param>
      /// <param name="filter">Filter for the radio players (default: null, optional)</param>
      /// <returns>Radio player by index.</returns>
      public RadioPlayer PlayerFromIndex(bool random = false, int index = -1, RadioFilter filter = null)
      {
         return playerByIndex(random, index, filter);
      }

      /// <summary>Next (normal/random) radio from this manager.</summary>
      /// <param name="random">Return a random radio player (default: false, optional)</param>
      /// <param name="filter">Filter for the radio players (default: null, optional)</param>
      /// <param name="stopAll">Stops all radios of this manager (default: true, optional)</param>
      /// <param name="playImmediately">Plays the radio (default: true, optional)</param>
      /// <returns>Next radio station.</returns>
      public RadioPlayer Next(bool random = false, RadioFilter filter = null, bool stopAll = true, bool playImmediately = true)
      {
         if (Players?.Count > 0)
         {
            if (stopAll)
               StopAll();

            currentRadioPlayer = nextPlayer(random, getFilter(filter));

            if (stopAll && playImmediately)
            {
               Invoke(nameof(play), Constants.INVOKE_DELAY);
            }
            else
            {
               play();
            }
         }
         else
         {
            Debug.LogWarning("No 'Players' found. Can't play next radio station and returning null.", this);
         }

         return currentRadioPlayer;
      }

      /// <summary>Previous (normal/random) radio from this manager.</summary>
      /// <param name="random">Return a random radio player (default: false, optional)</param>
      /// <param name="filter">Filter for the radio players (default: null, optional)</param>
      /// <param name="stopAll">Stops all radios of this manager (default: true, optional)</param>
      /// <param name="playImmediately">Plays the radio (default: true, optional)</param>
      /// <returns>Previous radio station.</returns>
      public RadioPlayer Previous(bool random = false, RadioFilter filter = null, bool stopAll = true, bool playImmediately = true)
      {
         if (Players?.Count > 0)
         {
            if (stopAll)
               StopAll();

            currentRadioPlayer = previousPlayer(random, getFilter(filter));

            if (stopAll && playImmediately)
            {
               Invoke(nameof(play), Constants.INVOKE_DELAY);
            }
            else
            {
               play();
            }
         }
         else
         {
            Debug.LogWarning("No 'Players' found. Can't play previous radio station and returning null.", this);
         }

         return currentRadioPlayer;
      }


      /// <summary>Stops all radios of this manager at once.</summary>
      /// <param name="resetIndex">Reset the index of the radio stations (default: false)</param>
      public void StopAll(bool resetIndex)
      {
         foreach (RadioPlayer rp in Players.Where(rp => rp != null))
         {
            rp.Stop();
         }

         if (resetIndex)
            playerIndex = randomPlayerIndex = 0;
      }

      /// <summary>Stops all radios of this manager at once.</summary>
      public void StopAll()
      {
         StopAll(false);
      }

      /// <summary>Returns all radios of this manager ordered by name.</summary>
      /// <param name="desc">Descending order (default: false, optional)</param>
      /// <param name="filter">Filter for the radio players (default: null, optional)</param>
      /// <returns>All radios of this manager ordered by name.</returns>
      public System.Collections.Generic.List<RadioPlayer> PlayersByName(bool desc = false, RadioFilter filter = null)
      {
         RadioFilter _filter = getFilter(filter);

         if (desc)
         {
            if (_filter != null)
            {
               if (_filter.Equals(playersByNameFilterDesc))
               {
                  if (Constants.DEV_DEBUG)
                     Debug.Log("PlayersByName with filter DESC: CACHED!", this);

                  return playersByNameDesc;
               }

               if (Constants.DEV_DEBUG)
                  Debug.Log("PlayersByName with filter DESC: NOT cached!", this);

               playersByNameDesc = new System.Collections.Generic.List<RadioPlayer>(filterPlayers(false, _filter).OrderByDescending(entry => entry.Station.Name));
               playersByNameFilterDesc = new RadioFilter(_filter);

               return playersByNameDesc;
            }

            if (playersByNameDesc.Count > 0)
            {
               if (Constants.DEV_DEBUG)
                  Debug.Log("PlayersByName without filter DESC: CACHED!", this);

               return playersByNameDesc;
            }

            if (Constants.DEV_DEBUG)
               Debug.Log("PlayersByName without filter DESC: NOT cached!", this);

            playersByNameDesc = new System.Collections.Generic.List<RadioPlayer>(filterPlayers().OrderByDescending(entry => entry.Station.Name));
            playersByNameFilterDesc = null;

            return playersByNameDesc;
         }

         if (_filter != null)
         {
            if (_filter.Equals(playersByNameFilterAsc))
            {
               if (Constants.DEV_DEBUG)
                  Debug.Log("PlayersByName with filter ASC: CACHED!", this);

               return playersByNameAsc;
            }

            if (Constants.DEV_DEBUG)
               Debug.Log("PlayersByName with filter ASC: NOT cached!", this);

            playersByNameAsc = new System.Collections.Generic.List<RadioPlayer>(filterPlayers(false, _filter).OrderBy(entry => entry.Station.Name));
            playersByNameFilterAsc = new RadioFilter(_filter);

            return playersByNameAsc;
         }

         if (playersByNameAsc.Count > 0)
         {
            if (Constants.DEV_DEBUG)
               Debug.Log("PlayersByName without filter ASC: CACHED!", this);

            return playersByNameAsc;
         }

         if (Constants.DEV_DEBUG)
            Debug.Log("PlayersByName without filter ASC: NOT cached!", this);

         playersByNameAsc = new System.Collections.Generic.List<RadioPlayer>(filterPlayers().OrderBy(entry => entry.Station.Name));
         playersByNameFilterAsc = null;

         return playersByNameAsc;
      }

      /// <summary>Returns all radios of this manager ordered by URL.</summary>
      /// <param name="desc">Descending order (default: false, optional)</param>
      /// <param name="filter">Filter for the radio players (default: null, optional)</param>
      /// <returns>All radios of this manager ordered by URL.</returns>
      public System.Collections.Generic.List<RadioPlayer> PlayersByURL(bool desc = false, RadioFilter filter = null)
      {
         RadioFilter _filter = getFilter(filter);

         if (desc)
         {
            if (_filter != null)
            {
               if (_filter.Equals(playersByURLFilterDesc))
               {
                  if (Constants.DEV_DEBUG)
                     Debug.Log("PlayersByURL with filter DESC: CACHED!", this);

                  return playersByURLDesc;
               }

               if (Constants.DEV_DEBUG)
                  Debug.Log("PlayersByURL with filter DESC: NOT cached!", this);

               playersByURLDesc = new System.Collections.Generic.List<RadioPlayer>(filterPlayers(false, _filter).OrderByDescending(entry => entry.Station.Url).ThenBy(entry => entry.Station.Name));
               playersByURLFilterDesc = new RadioFilter(_filter);

               return playersByURLDesc;
            }

            if (playersByURLDesc.Count > 0)
            {
               if (Constants.DEV_DEBUG)
                  Debug.Log("PlayersByURL without filter DESC: CACHED!", this);

               return playersByURLDesc;
            }

            if (Constants.DEV_DEBUG)
               Debug.Log("PlayersByURL without filter DESC: NOT cached!", this);

            playersByURLDesc = new System.Collections.Generic.List<RadioPlayer>(filterPlayers().OrderByDescending(entry => entry.Station.Url).ThenBy(entry => entry.Station.Name));
            playersByURLFilterDesc = null;

            return playersByURLDesc;
         }

         if (_filter != null)
         {
            if (_filter.Equals(playersByURLFilterAsc))
            {
               if (Constants.DEV_DEBUG)
                  Debug.Log("PlayersByURL with filter ASC: CACHED!", this);

               return playersByURLAsc;
            }

            if (Constants.DEV_DEBUG)
               Debug.Log("PlayersByURL with filter ASC: NOT cached!", this);

            playersByURLAsc = new System.Collections.Generic.List<RadioPlayer>(filterPlayers(false, _filter).OrderBy(entry => entry.Station.Url).ThenBy(entry => entry.Station.Name));
            playersByURLFilterAsc = new RadioFilter(_filter);

            return playersByURLAsc;
         }

         if (playersByURLAsc.Count > 0)
         {
            if (Constants.DEV_DEBUG)
               Debug.Log("PlayersByURL without filter ASC: CACHED!", this);

            return playersByURLAsc;
         }

         if (Constants.DEV_DEBUG)
            Debug.Log("PlayersByURL without filter ASC: NOT cached!", this);

         playersByURLAsc = new System.Collections.Generic.List<RadioPlayer>(filterPlayers().OrderBy(entry => entry.Station.Url).ThenBy(entry => entry.Station.Name));
         playersByURLFilterAsc = null;

         return playersByURLAsc;
      }

      /// <summary>Returns all radios of this manager ordered by audio format.</summary>
      /// <param name="desc">Descending order (default: false, optional)</param>
      /// <param name="filter">Filter for the radio players (default: null, optional)</param>
      /// <returns>All radios of this manager ordered by audio format.</returns>
      public System.Collections.Generic.List<RadioPlayer> PlayersByFormat(bool desc = false, RadioFilter filter = null)
      {
         RadioFilter _filter = getFilter(filter);

         if (desc)
         {
            if (_filter != null)
            {
               if (_filter.Equals(playersByFormatFilterDesc))
               {
                  if (Constants.DEV_DEBUG)
                     Debug.Log("PlayersByFormat with filter DESC: CACHED!", this);

                  return playersByFormatDesc;
               }

               if (Constants.DEV_DEBUG)
                  Debug.Log("PlayersByFormat with filter DESC: NOT cached!", this);

               playersByFormatDesc = new System.Collections.Generic.List<RadioPlayer>(filterPlayers(false, _filter).OrderByDescending(entry => entry.Station.Format).ThenBy(entry => entry.Station.Name));
               playersByFormatFilterDesc = new RadioFilter(_filter);

               return playersByFormatDesc;
            }

            if (playersByFormatDesc.Count > 0)
            {
               if (Constants.DEV_DEBUG)
                  Debug.Log("PlayersByFormat without filter DESC: CACHED!", this);

               return playersByFormatDesc;
            }

            if (Constants.DEV_DEBUG)
               Debug.Log("PlayersByFormat without filter DESC: NOT cached!", this);

            playersByFormatDesc = new System.Collections.Generic.List<RadioPlayer>(filterPlayers().OrderByDescending(entry => entry.Station.Format).ThenBy(entry => entry.Station.Name));
            playersByFormatFilterDesc = null;

            return playersByFormatDesc;
         }

         if (_filter != null)
         {
            if (_filter.Equals(playersByFormatFilterAsc))
            {
               if (Constants.DEV_DEBUG)
                  Debug.Log("PlayersByFormat with filter ASC: CACHED!", this);

               return playersByFormatAsc;
            }

            if (Constants.DEV_DEBUG)
               Debug.Log("PlayersByFormat with filter ASC: NOT cached!", this);

            playersByFormatAsc = new System.Collections.Generic.List<RadioPlayer>(filterPlayers(false, _filter).OrderBy(entry => entry.Station.Format).ThenBy(entry => entry.Station.Name));
            playersByFormatFilterAsc = new RadioFilter(_filter);

            return playersByFormatAsc;
         }

         if (playersByFormatAsc.Count > 0)
         {
            if (Constants.DEV_DEBUG)
               Debug.Log("PlayersByFormat without filter ASC: CACHED!", this);

            return playersByFormatAsc;
         }

         if (Constants.DEV_DEBUG)
            Debug.Log("PlayersByFormat without filter ASC: NOT cached!", this);

         playersByFormatAsc = new System.Collections.Generic.List<RadioPlayer>(filterPlayers().OrderBy(entry => entry.Station.Format).ThenBy(entry => entry.Station.Name));
         playersByFormatFilterAsc = null;

         return playersByFormatAsc;
      }

      /// <summary>Returns all radios of this manager ordered by station.</summary>
      /// <param name="desc">Descending order (default: false, optional)</param>
      /// <param name="filter">Filter for the radio players (default: null, optional)</param>
      /// <returns>All radios of this manager ordered by station.</returns>
      public System.Collections.Generic.List<RadioPlayer> PlayersByStation(bool desc = false, RadioFilter filter = null)
      {
         RadioFilter _filter = getFilter(filter);

         if (desc)
         {
            if (_filter != null)
            {
               if (_filter.Equals(playersByStationFilterDesc))
               {
                  if (Constants.DEV_DEBUG)
                     Debug.Log("PlayersByStation with filter DESC: CACHED!", this);

                  return playersByStationDesc;
               }

               if (Constants.DEV_DEBUG)
                  Debug.Log("PlayersByStation with filter DESC: NOT cached!", this);

               playersByStationDesc = new System.Collections.Generic.List<RadioPlayer>(filterPlayers(false, _filter).OrderByDescending(entry => entry.Station.Station).ThenBy(entry => entry.Station.Name));
               playersByStationFilterDesc = new RadioFilter(_filter);

               return playersByStationDesc;
            }

            if (playersByStationDesc.Count > 0)
            {
               if (Constants.DEV_DEBUG)
                  Debug.Log("PlayersByStation without filter DESC: CACHED!", this);

               return playersByStationDesc;
            }

            if (Constants.DEV_DEBUG)
               Debug.Log("PlayersByStation without filter DESC: NOT cached!", this);

            playersByStationDesc = new System.Collections.Generic.List<RadioPlayer>(filterPlayers().OrderByDescending(entry => entry.Station.Station).ThenBy(entry => entry.Station.Name));
            playersByStationFilterDesc = null;

            return playersByStationDesc;
         }

         if (_filter != null)
         {
            if (_filter.Equals(playersByStationFilterAsc))
            {
               if (Constants.DEV_DEBUG)
                  Debug.Log("PlayersByStation with filter ASC: CACHED!", this);

               return playersByStationAsc;
            }

            if (Constants.DEV_DEBUG)
               Debug.Log("PlayersByStation with filter ASC: NOT cached!", this);

            playersByStationAsc = new System.Collections.Generic.List<RadioPlayer>(filterPlayers(false, _filter).OrderBy(entry => entry.Station.Station).ThenBy(entry => entry.Station.Name));
            playersByStationFilterAsc = new RadioFilter(_filter);

            return playersByStationAsc;
         }

         if (playersByStationAsc.Count > 0)
         {
            if (Constants.DEV_DEBUG)
               Debug.Log("PlayersByStation without filter ASC: CACHED!", this);

            return playersByStationAsc;
         }

         if (Constants.DEV_DEBUG)
            Debug.Log("PlayersByStation without filter ASC: NOT cached!", this);

         playersByStationAsc = new System.Collections.Generic.List<RadioPlayer>(filterPlayers().OrderBy(entry => entry.Station.Station).ThenBy(entry => entry.Station.Name));
         playersByStationFilterAsc = null;

         return playersByStationAsc;
      }

      /// <summary>Returns all radios of this manager ordered by bitrate.</summary>
      /// <param name="desc">Descending order (default: false, optional)</param>
      /// <param name="filter">Filter for the radio players (default: null, optional)</param>
      /// <returns>All radios of this manager ordered by bitrate.</returns>
      public System.Collections.Generic.List<RadioPlayer> PlayersByBitrate(bool desc = false, RadioFilter filter = null)
      {
         RadioFilter _filter = getFilter(filter);

         if (desc)
         {
            if (_filter != null)
            {
               if (_filter.Equals(playersByBitrateFilterDesc))
               {
                  if (Constants.DEV_DEBUG)
                     Debug.Log("PlayersByBitrate with filter DESC: CACHED!", this);

                  return playersByBitrateDesc;
               }

               if (Constants.DEV_DEBUG)
                  Debug.Log("PlayersByBitrate with filter DESC: NOT cached!", this);

               playersByBitrateDesc = new System.Collections.Generic.List<RadioPlayer>(filterPlayers(false, _filter).OrderByDescending(entry => entry.Station.Bitrate).ThenBy(entry => entry.Station.Name));
               playersByBitrateFilterDesc = new RadioFilter(_filter);

               return playersByBitrateDesc;
            }

            if (playersByBitrateDesc.Count > 0)
            {
               if (Constants.DEV_DEBUG)
                  Debug.Log("PlayersByBitrate without filter DESC: CACHED!", this);

               return playersByBitrateDesc;
            }

            if (Constants.DEV_DEBUG)
               Debug.Log("PlayersByBitrate without filter DESC: NOT cached!", this);

            playersByBitrateDesc = new System.Collections.Generic.List<RadioPlayer>(filterPlayers().OrderByDescending(entry => entry.Station.Bitrate).ThenBy(entry => entry.Station.Name));

            return playersByBitrateDesc;
         }

         if (_filter != null)
         {
            if (_filter.Equals(playersByBitrateFilterAsc))
            {
               if (Constants.DEV_DEBUG)
                  Debug.Log("PlayersByBitrate with filter ASC: CACHED!", this);

               return playersByBitrateAsc;
            }

            if (Constants.DEV_DEBUG)
               Debug.Log("PlayersByBitrate with filter ASC: NOT cached!", this);

            playersByBitrateAsc = new System.Collections.Generic.List<RadioPlayer>(filterPlayers(false, _filter).OrderBy(entry => entry.Station.Bitrate).ThenBy(entry => entry.Station.Name));
            playersByBitrateFilterAsc = new RadioFilter(_filter);

            return playersByBitrateAsc;
         }

         if (playersByBitrateAsc.Count > 0)
         {
            if (Constants.DEV_DEBUG)
               Debug.Log("PlayersByBitrate without filter ASC: CACHED!", this);

            return playersByBitrateAsc;
         }

         if (Constants.DEV_DEBUG)
            Debug.Log("PlayersByBitrate without filter ASC: NOT cached!", this);

         playersByBitrateAsc = new System.Collections.Generic.List<RadioPlayer>(filterPlayers().OrderBy(entry => entry.Station.Bitrate).ThenBy(entry => entry.Station.Name));
         playersByBitrateFilterAsc = null;

         return playersByBitrateAsc;
      }

      /// <summary>Returns all radios of this manager ordered by genres.</summary>
      /// <param name="desc">Descending order (default: false, optional)</param>
      /// <param name="filter">Filter for the radio players (default: null, optional)</param>
      /// <returns>All radios of this manager ordered by genre.</returns>
      public System.Collections.Generic.List<RadioPlayer> PlayersByGenres(bool desc = false, RadioFilter filter = null)
      {
         RadioFilter _filter = getFilter(filter);

         if (desc)
         {
            if (_filter != null)
            {
               if (_filter.Equals(playersByGenresFilterDesc))
               {
                  if (Constants.DEV_DEBUG)
                     Debug.Log("PlayersByGenres with filter DESC: CACHED!", this);

                  return playersByGenresDesc;
               }

               if (Constants.DEV_DEBUG)
                  Debug.Log("PlayersByGenres with filter DESC: NOT cached!", this);

               playersByGenresDesc = new System.Collections.Generic.List<RadioPlayer>(filterPlayers(false, _filter).OrderByDescending(entry => entry.Station.Genres).ThenBy(entry => entry.Station.Name));
               playersByGenresFilterDesc = new RadioFilter(_filter);

               return playersByGenresDesc;
            }

            if (playersByGenresDesc.Count > 0)
            {
               if (Constants.DEV_DEBUG)
                  Debug.Log("PlayersByGenres without filter DESC: CACHED!", this);

               return playersByGenresDesc;
            }

            if (Constants.DEV_DEBUG)
               Debug.Log("PlayersByGenres without filter DESC: NOT cached!", this);

            playersByGenresDesc = new System.Collections.Generic.List<RadioPlayer>(filterPlayers().OrderByDescending(entry => entry.Station.Genres).ThenBy(entry => entry.Station.Name));
            playersByGenresFilterDesc = null;

            return playersByGenresDesc;
         }

         if (_filter != null)
         {
            if (_filter.Equals(playersByGenresFilterAsc))
            {
               if (Constants.DEV_DEBUG)
                  Debug.Log("PlayersByGenres with filter ASC: CACHED!", this);

               return playersByGenresAsc;
            }

            if (Constants.DEV_DEBUG)
               Debug.Log("PlayersByGenres with filter ASC: NOT cached!", this);

            playersByGenresAsc = new System.Collections.Generic.List<RadioPlayer>(filterPlayers(false, _filter).OrderBy(entry => entry.Station.Genres).ThenBy(entry => entry.Station.Name));
            playersByGenresFilterAsc = new RadioFilter(_filter);

            return playersByGenresAsc;
         }

         if (playersByGenresAsc.Count > 0)
         {
            if (Constants.DEV_DEBUG)
               Debug.Log("PlayersByGenres without filter ASC: CACHED!", this);

            return playersByGenresAsc;
         }

         if (Constants.DEV_DEBUG)
            Debug.Log("PlayersByGenres without filter ASC: NOT cached!", this);

         playersByGenresAsc = new System.Collections.Generic.List<RadioPlayer>(filterPlayers().OrderBy(entry => entry.Station.Genres).ThenBy(entry => entry.Station.Name));
         playersByGenresFilterAsc = null;

         return playersByGenresAsc;
      }

      /// <summary>Returns all radios of this manager ordered by rating.</summary>
      /// <param name="desc">Descending order (default: false, optional)</param>
      /// <param name="filter">Filter for the radio players (default: null, optional)</param>
      /// <returns>All radios of this manager ordered by rating.</returns>
      public System.Collections.Generic.List<RadioPlayer> PlayersByRating(bool desc = false, RadioFilter filter = null)
      {
         RadioFilter _filter = getFilter(filter);

         if (desc)
         {
            if (_filter != null)
            {
               if (_filter.Equals(playersByRatingFilterDesc))
               {
                  if (Constants.DEV_DEBUG)
                     Debug.Log("PlayersByRating with filter DESC: CACHED!", this);

                  return playersByRatingDesc;
               }

               if (Constants.DEV_DEBUG)
                  Debug.Log("PlayersByRating with filter DESC: NOT cached!", this);

               playersByRatingDesc = new System.Collections.Generic.List<RadioPlayer>(filterPlayers(false, _filter).OrderByDescending(entry => entry.Station.Rating).ThenBy(entry => entry.Station.Name));
               playersByRatingFilterDesc = new RadioFilter(_filter);

               return playersByRatingDesc;
            }

            if (playersByRatingDesc.Count > 0)
            {
               if (Constants.DEV_DEBUG)
                  Debug.Log("PlayersByRating without filter DESC: CACHED!", this);

               return playersByRatingDesc;
            }

            if (Constants.DEV_DEBUG)
               Debug.Log("PlayersByRating without filter DESC: NOT cached!", this);

            playersByRatingDesc = new System.Collections.Generic.List<RadioPlayer>(filterPlayers().OrderByDescending(entry => entry.Station.Rating).ThenBy(entry => entry.Station.Name));
            playersByRatingFilterDesc = null;

            return playersByRatingDesc;
         }

         if (_filter != null)
         {
            if (_filter.Equals(playersByRatingFilterAsc))
            {
               if (Constants.DEV_DEBUG)
                  Debug.Log("PlayersByRating with filter ASC: CACHED!", this);

               return playersByRatingAsc;
            }

            if (Constants.DEV_DEBUG)
               Debug.Log("PlayersByRating with filter ASC: NOT cached!", this);

            playersByRatingAsc = new System.Collections.Generic.List<RadioPlayer>(filterPlayers(false, _filter).OrderBy(entry => entry.Station.Rating).ThenBy(entry => entry.Station.Name));
            playersByRatingFilterAsc = new RadioFilter(_filter);

            return playersByRatingAsc;
         }

         if (playersByRatingAsc.Count > 0)
         {
            if (Constants.DEV_DEBUG)
               Debug.Log("PlayersByRating without filter ASC: CACHED!", this);

            return playersByRatingAsc;
         }

         if (Constants.DEV_DEBUG)
            Debug.Log("PlayersByRating without filter ASC: NOT cached!", this);

         playersByRatingAsc = new System.Collections.Generic.List<RadioPlayer>(filterPlayers().OrderBy(entry => entry.Station.Rating).ThenBy(entry => entry.Station.Name));
         playersByRatingFilterAsc = null;

         return playersByRatingAsc;
      }

      /// <summary>Randomize all radio players.</summary>
      /// <param name="resetIndex">Reset the index of the random radio stations (default: true, optional)</param>
      public void RandomizePlayers(bool resetIndex = true)
      {
         int seed = Random.Range(0, int.MaxValue);
         randomPlayers.CTShuffle(seed);

         if (resetIndex)
            randomPlayerIndex = 0;
      }

      #endregion


      #region Private methods

      private System.Collections.Generic.IEnumerable<RadioPlayer> filterPlayers(bool random = false, RadioFilter filter = null)
      {
         if (random)
         {
            if (filter?.isFiltering == true)
            {
               clearedStationRandom = false;

               if (filter.Equals(lastRandomPlayerFilter) && lastFilteredRandomPlayers.Count > 0)
               {
                  if (Constants.DEV_DEBUG)
                     Debug.Log("filterPlayers RND: CACHED!", this);

                  return lastFilteredRandomPlayers;
               }

               if (Constants.DEV_DEBUG)
                  Debug.Log("filterPlayers RND: NOT Cached!", this);

               System.Collections.Generic.IEnumerable<RadioPlayer> _players = from entry in randomPlayers
                  where (string.IsNullOrEmpty(filter.Names) || entry.Station.Name.CTContainsAny(filter.Names)) &&
                        (string.IsNullOrEmpty(filter.Stations) || entry.Station.Station.CTContainsAny(filter.Stations)) &&
                        (string.IsNullOrEmpty(filter.Urls) || entry.Station.Url.CTContainsAll(filter.Urls)) &&
                        (string.IsNullOrEmpty(filter.Genres) || entry.Station.Genres.CTContainsAny(filter.Genres)) &&
                        (string.IsNullOrEmpty(filter.Cities) || entry.Station.City.CTContainsAny(filter.Cities)) &&
                        (string.IsNullOrEmpty(filter.Countries) || entry.Station.Country.CTContainsAny(filter.Countries)) &&
                        (string.IsNullOrEmpty(filter.Languages) || entry.Station.Language.CTContainsAny(filter.Languages)) &&
                        entry.Station.Format.ToString().CTContainsAny(filter.Format) &&
                        entry.Station.Bitrate >= filter.BitrateMin && entry.Station.Bitrate <= filter.BitrateMax &&
                        entry.Station.Rating >= filter.RatingMin && entry.Station.Rating <= filter.RatingMax &&
                        (!filter.ExcludeUnsupportedCodecs || entry.Station.ExcludedCodec == AudioCodec.None || entry.Station.ExcludedCodec != Helper.AudioCodecForAudioFormat(entry.Station.Format))
                  select entry;

               System.Collections.Generic.List<RadioPlayer> radioPlayers = _players.ToList();
               lastFilteredRandomPlayers = filter.Limit != 0 && radioPlayers.Count > filter.Limit ? radioPlayers.GetRange(0, filter.Limit) : radioPlayers;
               lastRandomPlayerFilter = new RadioFilter(filter);

               clearFilters(false, false);

               return lastFilteredRandomPlayers;
            }

            if (Constants.DEV_DEBUG)
               Debug.Log("filterPlayers RND: No filtering!", this);

            if (!clearedStationRandom)
            {
               clearFilters();
               clearedStationRandom = true;
            }

            return randomPlayers;
         }

         if (filter?.isFiltering == true)
         {
            clearedStation = false;

            if (filter.Equals(lastPlayerFilter) && lastFilteredPlayers.Count > 0)
            {
               if (Constants.DEV_DEBUG)
                  Debug.Log("filterPlayers: CACHED!", this);

               return lastFilteredPlayers;
            }

            if (Constants.DEV_DEBUG)
               Debug.Log("filterPlayers: NOT Cached!", this);

            System.Collections.Generic.IEnumerable<RadioPlayer> _players = from entry in Players
               where (string.IsNullOrEmpty(filter.Names) || entry.Station.Name.CTContainsAny(filter.Names)) &&
                     (string.IsNullOrEmpty(filter.Stations) || entry.Station.Station.CTContainsAny(filter.Stations)) &&
                     (string.IsNullOrEmpty(filter.Urls) || entry.Station.Url.CTContainsAll(filter.Urls)) &&
                     (string.IsNullOrEmpty(filter.Genres) || entry.Station.Genres.CTContainsAny(filter.Genres)) &&
                     (string.IsNullOrEmpty(filter.Cities) || entry.Station.City.CTContainsAny(filter.Cities)) &&
                     (string.IsNullOrEmpty(filter.Countries) || entry.Station.Country.CTContainsAny(filter.Countries)) &&
                     (string.IsNullOrEmpty(filter.Languages) || entry.Station.Language.CTContainsAny(filter.Languages)) &&
                     entry.Station.Format.ToString().CTContainsAny(filter.Format) &&
                     entry.Station.Bitrate >= filter.BitrateMin && entry.Station.Bitrate <= filter.BitrateMax &&
                     entry.Station.Rating >= filter.RatingMin && entry.Station.Rating <= filter.RatingMax &&
                     (!filter.ExcludeUnsupportedCodecs || entry.Station.ExcludedCodec == AudioCodec.None || entry.Station.ExcludedCodec != Helper.AudioCodecForAudioFormat(entry.Station.Format))
               select entry;

            System.Collections.Generic.List<RadioPlayer> radioPlayers = _players.ToList();
            lastFilteredPlayers = filter.Limit != 0 && radioPlayers.Count > filter.Limit ? radioPlayers.GetRange(0, filter.Limit) : radioPlayers;
            lastPlayerFilter = new RadioFilter(filter);

            clearFilters(false, false);

            return lastFilteredPlayers;
         }

         if (Constants.DEV_DEBUG)
            Debug.Log("filterPlayers: No filtering!", this);

         if (!clearedStation)
         {
            clearFilters();
            clearedStation = true;
         }

         return Players;
      }

      private void clearFilters(bool clearLastFilter = true, bool clearLastRandomFilter = true)
      {
         if (clearLastFilter)
         {
            lastFilteredPlayers = null;
            lastPlayerFilter = null;
         }

         if (clearLastRandomFilter)
         {
            lastFilteredRandomPlayers = null;
            lastRandomPlayerFilter = null;
         }

         playersByNameFilterDesc = null;
         playersByNameFilterAsc = null;
         playersByURLFilterDesc = null;
         playersByURLFilterAsc = null;
         playersByFormatFilterDesc = null;
         playersByFormatFilterAsc = null;
         playersByStationFilterDesc = null;
         playersByStationFilterAsc = null;
         playersByBitrateFilterDesc = null;
         playersByBitrateFilterAsc = null;
         playersByGenresFilterDesc = null;
         playersByGenresFilterAsc = null;
         playersByRatingFilterDesc = null;
         playersByRatingFilterAsc = null;
         resetFilterLists();
      }

      private void resetFilterLists()
      {
         playersByNameDesc.Clear();
         playersByNameAsc.Clear();
         playersByURLDesc.Clear();
         playersByURLAsc.Clear();
         playersByFormatDesc.Clear();
         playersByFormatAsc.Clear();
         playersByStationDesc.Clear();
         playersByStationAsc.Clear();
         playersByBitrateDesc.Clear();
         playersByBitrateAsc.Clear();
         playersByGenresDesc.Clear();
         playersByGenresAsc.Clear();
         playersByRatingDesc.Clear();
         playersByRatingAsc.Clear();
      }

      private void play()
      {
         currentRadioPlayer.Play();
      }

      private RadioPlayer playerByIndex(bool random = false, int index = -1, RadioFilter filter = null)
      {
         System.Collections.Generic.List<RadioPlayer> _players = new System.Collections.Generic.List<RadioPlayer>(filterPlayers(random, filter));

         if (_players.Count > 0)
         {
            if (random)
            {
               if (index > -1 && index < _players.Count)
               {
                  randomPlayerIndex = index;
               }
               else
               {
                  randomPlayerIndex = Random.Range(0, _players.Count - 1);
               }

               return _players[randomPlayerIndex];
            }

            if (index > -1 && index < _players.Count)
            {
               playerIndex = index;
            }
            else
            {
               playerIndex = 0;
            }

            return _players[playerIndex];
         }

         return null;
      }

      private RadioPlayer nextPlayer(bool random = false, RadioFilter filter = null)
      {
         System.Collections.Generic.List<RadioPlayer> _players = new System.Collections.Generic.List<RadioPlayer>(filterPlayers(random, filter));

         if (random)
         {
            if (randomPlayerIndex > -1 && randomPlayerIndex + 1 < _players.Count)
            {
               randomPlayerIndex++;
            }
            else
            {
               randomPlayerIndex = 0;
            }

            if (_players.Count > 0)
            {
               return _players[randomPlayerIndex];
            }
         }

         else
         {
            if (playerIndex > -1 && playerIndex + 1 < _players.Count)
            {
               playerIndex++;
            }
            else
            {
               playerIndex = 0;
            }

            if (_players.Count > 0)
               return _players[playerIndex];
         }

         return null;
      }

      private RadioPlayer previousPlayer(bool random = false, RadioFilter filter = null)
      {
         System.Collections.Generic.List<RadioPlayer> _players = new System.Collections.Generic.List<RadioPlayer>(filterPlayers(random, filter));
         if (random)
         {
            if (randomPlayerIndex > 0 && randomPlayerIndex < _players.Count)
            {
               randomPlayerIndex--;
            }
            else
            {
               randomPlayerIndex = _players.Count - 1;
            }

            if (_players.Count > 0)
            {
               return _players[randomPlayerIndex];
            }
         }

         else
         {
            if (playerIndex > 0 && playerIndex < _players.Count)
            {
               playerIndex--;
            }
            else
            {
               playerIndex = _players.Count - 1;
            }

            if (_players.Count > 0)
               return _players[playerIndex];
         }

         return null;
      }

      private RadioFilter getFilter(RadioFilter filter)
      {
         if (filter?.isFiltering == true)
            return filter;

         return Filter.isFiltering ? Filter : null;
      }

      #endregion


      #region Event-trigger methods

      private void onProviderReady()
      {
         if (!isWorking)
         {
            isWorking = true;

            if (Config.DEBUG)
               Debug.Log("onProviderReady", this);

            Players.Clear();
            randomPlayers.Clear();

            Transform ttf = transform;
            for (int ii = ttf.childCount - 1; ii >= 0; ii--)
            {
               Transform child = ttf.GetChild(ii);
               Destroy(child.gameObject);
            }

            if (!Helper.isEditorMode && InstantiateRadioPlayers)
            {
               if (RadioPrefab != null)
               {
                  foreach (RadioStation station in Stations)
                  {
                     GameObject go = Instantiate(RadioPrefab, tf, true);
                     RadioPlayer rp = go.GetComponent<RadioPlayer>();
                     rp.Station = station;

                     Players.Add(rp);
                     randomPlayers.Add(rp);

                     if (Config.DEBUG)
                        Debug.Log("Radio station found: " + rp, this);

                     //count++;
                  }
               }

               RandomizePlayers();
            }

            clearFilters();

            if (!Helper.isEditorMode)
               OnProviderReadyEvent?.Invoke();

            OnProviderReady?.Invoke();

            isWorking = false;
         }
      }

      private void onStationsChange()
      {
         if (!isWorking)
         {
            isWorking = true;

            if (Config.DEBUG)
               Debug.Log("onStationsChange", this);

            Players.Clear();
            randomPlayers.Clear();

            Transform ttf = transform;
            for (int ii = ttf.childCount - 1; ii >= 0; ii--)
            {
               Transform child = ttf.GetChild(ii);
               Destroy(child.gameObject);
            }

            if (!Helper.isEditorMode && InstantiateRadioPlayers)
            {
               if (RadioPrefab != null)
               {
                  foreach (RadioStation station in Stations)
                  {
                     GameObject go = Instantiate(RadioPrefab, tf, true);
                     RadioPlayer rp = go.GetComponent<RadioPlayer>();
                     rp.Station = station;

                     Players.Add(rp);
                     randomPlayers.Add(rp);

                     if (Config.DEBUG)
                        Debug.Log("Radio station found: " + rp, this);

                     //count++;
                  }
               }

               RandomizePlayers();
            }

            clearFilters();

            if (!Helper.isEditorMode)
               OnStationsChanged?.Invoke();

            OnStationsChange?.Invoke();

            isWorking = false;
         }
      }

      private void onFilterChange()
      {
         if (Config.DEBUG)
            Debug.Log("onFilterChange", this);

         clearFilters(false, false);

         if (!Helper.isEditorMode)
            OnFilterChanged?.Invoke();

         OnFilterChange?.Invoke();
      }

      #endregion


      #region Overridden methods

      public override string ToString()
      {
         System.Text.StringBuilder result = new System.Text.StringBuilder();
         result.Append(GetType().Name);
         result.Append(Constants.TEXT_TOSTRING_START);
         result.Append("Set='");
         result.Append(Set);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);
         result.Append("LoadOnStart='");
         result.Append(LoadOnStart);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);
         result.Append("LoadOnStartInEditor='");
         result.Append(LoadOnStartInEditor);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);
         result.Append("InstantiateRadioPlayers='");
         result.Append(InstantiateRadioPlayers);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER);
         result.Append("RadioPrefab='");
         result.Append(RadioPrefab);
         result.Append(Constants.TEXT_TOSTRING_DELIMITER_END);
         result.Append(Constants.TEXT_TOSTRING_END);
         return result.ToString();
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

      public System.Collections.Generic.List<RadioStation> GetStations(bool random = false, RadioFilter filter = null)
      {
         return Set != null ? Set.GetStations(random, getFilter(filter)) : new System.Collections.Generic.List<RadioStation>();
      }

      public int CountStations(RadioFilter filter = null)
      {
         return Set != null ? Set.CountStations(getFilter(filter)) : 0;
      }

      public RadioStation StationFromIndex(bool random = false, int index = -1, RadioFilter filter = null)
      {
         return Set != null ? Set.StationFromIndex(random, index, getFilter(filter)) : null;
      }

      public RadioStation StationFromHashCode(int hashCode)
      {
         return Set != null ? Set.StationFromHashCode(hashCode) : null;
      }

      public RadioStation NextStation(bool random = false, RadioFilter filter = null)
      {
         return Set != null ? Set.NextStation(random, getFilter(filter)) : null;
      }

      public RadioStation PreviousStation(bool random = false, RadioFilter filter = null)
      {
         return Set != null ? Set.PreviousStation(random, getFilter(filter)) : null;
      }

      public System.Collections.Generic.List<RadioStation> StationsByName(bool desc = false, RadioFilter filter = null)
      {
         return Set != null ? Set.StationsByName(desc, getFilter(filter)) : new System.Collections.Generic.List<RadioStation>();
      }

      public System.Collections.Generic.List<RadioStation> StationsByURL(bool desc = false, RadioFilter filter = null)
      {
         return Set != null ? Set.StationsByURL(desc, getFilter(filter)) : new System.Collections.Generic.List<RadioStation>();
      }

      public System.Collections.Generic.List<RadioStation> StationsByFormat(bool desc = false, RadioFilter filter = null)
      {
         return Set != null ? Set.StationsByFormat(desc, getFilter(filter)) : new System.Collections.Generic.List<RadioStation>();
      }

      public System.Collections.Generic.List<RadioStation> StationsByStation(bool desc = false, RadioFilter filter = null)
      {
         return Set != null ? Set.StationsByStation(desc, getFilter(filter)) : new System.Collections.Generic.List<RadioStation>();
      }

      public System.Collections.Generic.List<RadioStation> StationsByBitrate(bool desc = false, RadioFilter filter = null)
      {
         return Set != null ? Set.StationsByBitrate(desc, getFilter(filter)) : new System.Collections.Generic.List<RadioStation>();
      }

      public System.Collections.Generic.List<RadioStation> StationsByGenres(bool desc = false, RadioFilter filter = null)
      {
         return Set != null ? Set.StationsByGenres(desc, getFilter(filter)) : new System.Collections.Generic.List<RadioStation>();
      }

      public System.Collections.Generic.List<RadioStation> StationsByCities(bool desc = false, RadioFilter filter = null)
      {
         return Set != null ? Set.StationsByCities(desc, getFilter(filter)) : new System.Collections.Generic.List<RadioStation>();
      }

      public System.Collections.Generic.List<RadioStation> StationsByCountries(bool desc = false, RadioFilter filter = null)
      {
         return Set != null ? Set.StationsByCountries(desc, getFilter(filter)) : new System.Collections.Generic.List<RadioStation>();
      }

      public System.Collections.Generic.List<RadioStation> StationsByLanguages(bool desc = false, RadioFilter filter = null)
      {
         return Set != null ? Set.StationsByLanguages(desc, getFilter(filter)) : new System.Collections.Generic.List<RadioStation>();
      }

      public System.Collections.Generic.List<RadioStation> StationsByRating(bool desc = false, RadioFilter filter = null)
      {
         return Set != null ? Set.StationsByRating(desc, getFilter(filter)) : new System.Collections.Generic.List<RadioStation>();
      }

      public void Load()
      {
         if (Set != null)
            Set.Load();
      }

      public void Save(string path, RadioFilter filter = null)
      {
         if (Set != null)
            Set.Save(path, filter);
      }

      public void RandomizeStations(bool resetIndex = true)
      {
         if (Set != null)
            Set.RandomizeStations();
      }

      #endregion
   }
}
// © 2015-2024 crosstales LLC (https://www.crosstales.com)