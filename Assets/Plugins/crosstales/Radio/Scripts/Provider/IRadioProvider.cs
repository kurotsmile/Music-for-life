using Crosstales.Radio.Model;
using Crosstales.Radio.Model.Entry;

namespace Crosstales.Radio.Provider
{
   /// <summary>Interface for all radio providers.</summary>
   public interface IRadioProvider
   {
      #region Properties

      /// <summary>Returns the list of all RadioEntry.</summary>
      /// <returns>>List of all RadioEntry.</returns>
      System.Collections.Generic.List<BaseRadioEntry> RadioEntries { get; }

      /// <summary>Returns the list of all loaded RadioStation.</summary>
      /// <returns>List of all loaded RadioStation.</returns>
      System.Collections.Generic.List<RadioStation> Stations { get; }

      /// <summary>Is this provider ready (= data loaded)?</summary>
      /// <returns>True if this provider is ready.</returns>
      bool isReady { get; }

      #endregion


      #region Methods

      /// <summary>Loads all stations from this provider.</summary>
      void Load();

      /// <summary>Saves all stations from this provider as text-file with streams.</summary>
      /// <param name="path">Path to the text-file.</param>
      void Save(string path);

      #endregion
   }
}
// © 2018-2024 crosstales LLC (https://www.crosstales.com)