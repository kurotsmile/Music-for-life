using Crosstales.Radio.Model;

namespace Crosstales.Radio.Set
{
   /// <summary>Interface for all sets.</summary>
   public interface ISet
   {
      #region Properties

      /// <summary>List of all loaded RadioStation from all providers.</summary>
      System.Collections.Generic.List<RadioStation> Stations { get; }

      /// <summary>Returns the list of all randomized RadioStation from this set.</summary>
      /// <returns>The list of all randomized RadioStation from this set.</returns>
      System.Collections.Generic.List<RadioStation> RandomStations { get; }

      /// <summary>Are all providers of this set ready (= data loaded)?</summary>
      /// <returns>True if all providers of this set are ready.</returns>
      bool isReady { get; }

      /// <summary>Current station index.</summary>
      int CurrentStationIndex { get; set; }

      /// <summary>Current random station index.</summary>
      int CurrentRandomStationIndex { get; set; }

      #endregion


      #region Methods

      /// <summary>Loads all stations from this set (via providers).</summary>
      void Load();

      /// <summary>Saves all stations from this set as text-file with streams.</summary>
      /// <param name="path">Path to the text-file.</param>
      /// <param name="filter">Filter for the radio stations (default: null, optional)</param>
      void Save(string path, RadioFilter filter = null);

      /// <summary>Get all RadioStation for a given RadioFilter.</summary>
      /// <param name="random">Return random RadioStation (default: false, optional)</param>
      /// <param name="filter">Filter for the radio stations (default: null, optional)</param>
      /// <returns>All RadioStation for a given RadioFilter.</returns>
      System.Collections.Generic.List<RadioStation> GetStations(bool random = false, RadioFilter filter = null);

      /// <summary>Count all RadioStation for a given RadioFilter.</summary>
      /// <param name="filter">Filter for the radio stations (default: null, optional)</param>
      /// <returns>Number of all RadioStation for a given RadioFilter.</returns>
      int CountStations(RadioFilter filter = null);

      /// <summary>Radio station from a given index (normal/random) from this set.</summary>
      /// <param name="random">Return a random Radio station (default: false, optional)</param>
      /// <param name="index">Index of the radio station (default: -1, optional)</param>
      /// <param name="filter">Filter for the radio stations (default: null, optional)</param>
      /// <returns>Record from index.</returns>
      RadioStation StationFromIndex(bool random = false, int index = -1, RadioFilter filter = null);

      /// <summary>Radio station from a hashcode from this set.</summary>
      /// <param name="hashCode">Hashcode of the radio station</param>
      /// <returns>Radio station from hashcode.</returns>
      RadioStation StationFromHashCode(int hashCode);

      /// <summary>Next (normal/random) radio station from this set.</summary>
      /// <param name="random">Return a random radio station (default: false, optional)</param>
      /// <param name="filter">Filter for the radio stations (default: null, optional)</param>
      /// <returns>Next radio station.</returns>
      RadioStation NextStation(bool random = false, RadioFilter filter = null);

      /// <summary>Previous (normal/random) radio station from this set.</summary>
      /// <param name="random">Return a random radio station (default: false, optional)</param>
      /// <param name="filter">Filter for the radio stations (default: null, optional)</param>
      /// <returns>Previous radio station.</returns>
      RadioStation PreviousStation(bool random = false, RadioFilter filter = null);

      /// <summary>Returns all radio stations of this set ordered by name.</summary>
      /// <param name="desc">Descending order (default: false, optional)</param>
      /// <param name="filter">Filter for the radio stations (default: null, optional)</param>
      /// <returns>All radios of this set ordered by name.</returns>
      System.Collections.Generic.List<RadioStation> StationsByName(bool desc = false, RadioFilter filter = null);

      /// <summary>Returns all radio stations of this set ordered by URL.</summary>
      /// <param name="desc">Descending order (default: false, optional)</param>
      /// <param name="filter">Filter for the radio stations (default: null, optional)</param>
      /// <returns>All radios of this set ordered by URL.</returns>
      System.Collections.Generic.List<RadioStation> StationsByURL(bool desc = false, RadioFilter filter = null);

      /// <summary>Returns all radio stations of this set ordered by audio format.</summary>
      /// <param name="desc">Descending order (default: false, optional)</param>
      /// <param name="filter">Filter for the radio stations (default: null, optional)</param>
      /// <returns>All radios of this set ordered by audio format.</returns>
      System.Collections.Generic.List<RadioStation> StationsByFormat(bool desc = false, RadioFilter filter = null);

      /// <summary>Returns all radio stations of this set ordered by station.</summary>
      /// <param name="desc">Descending order (default: false, optional)</param>
      /// <param name="filter">Filter for the radio stations (default: null, optional)</param>
      /// <returns>All radios of this set ordered by station.</returns>
      System.Collections.Generic.List<RadioStation> StationsByStation(bool desc = false, RadioFilter filter = null);

      /// <summary>Returns all radio stations of this set ordered by bitrate.</summary>
      /// <param name="desc">Descending order (default: false, optional)</param>
      /// <param name="filter">Filter for the radio stations (default: null, optional)</param>
      /// <returns>All radios of this set ordered by bitrate.</returns>
      System.Collections.Generic.List<RadioStation> StationsByBitrate(bool desc = false, RadioFilter filter = null);

      /// <summary>Returns all radio stations of this set ordered by genres.</summary>
      /// <param name="desc">Descending order (default: false, optional)</param>
      /// <param name="filter">Filter for the radio stations (default: null, optional)</param>
      /// <returns>All radios of this set ordered by genre.</returns>
      System.Collections.Generic.List<RadioStation> StationsByGenres(bool desc = false, RadioFilter filter = null);

      /// <summary>Returns all radio stations of this set ordered by cities.</summary>
      /// <param name="desc">Descending order (default: false, optional)</param>
      /// <param name="filter">Filter for the radio stations (default: null, optional)</param>
      /// <returns>All radios of this set ordered by cities.</returns>
      System.Collections.Generic.List<RadioStation> StationsByCities(bool desc = false, RadioFilter filter = null);

      /// <summary>Returns all radio stations of this set ordered by countries.</summary>
      /// <param name="desc">Descending order (default: false, optional)</param>
      /// <param name="filter">Filter for the radio stations (default: null, optional)</param>
      /// <returns>All radios of this set ordered by countries.</returns>
      System.Collections.Generic.List<RadioStation> StationsByCountries(bool desc = false, RadioFilter filter = null);

      /// <summary>Returns all radio stations of this set ordered by languages.</summary>
      /// <param name="desc">Descending order (default: false, optional)</param>
      /// <param name="filter">Filter for the radio stations (default: null, optional)</param>
      /// <returns>All radios of this set ordered by languages.</returns>
      System.Collections.Generic.List<RadioStation> StationsByLanguages(bool desc = false, RadioFilter filter = null);

      /// <summary>Returns all radio stations of this set ordered by rating.</summary>
      /// <param name="desc">Descending order (default: false, optional)</param>
      /// <param name="filter">Filter for the radio stations (default: null, optional)</param>
      /// <returns>All radios of this set ordered by rating.</returns>
      System.Collections.Generic.List<RadioStation> StationsByRating(bool desc = false, RadioFilter filter = null);

      /// <summary>Randomize all radio stations.</summary>
      /// <param name="resetIndex">Reset the index of the random radio stations (default: true, optional)</param>
      void RandomizeStations(bool resetIndex = true);

      #endregion
   }
}
// © 2020-2024 crosstales LLC (https://www.crosstales.com)