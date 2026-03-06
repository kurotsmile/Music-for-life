using UnityEngine;
using System.Linq;
using Crosstales.Radio.Util;
using Crosstales.Radio.Model;
using Crosstales.Radio.Model.Entry;
using Crosstales.Radio.Model.Enum;

namespace Crosstales.Radio.Provider
{
   /// <summary>Provider for URLs of radio stations in various formats.</summary>
   [HelpURL("https://www.crosstales.com/media/data/assets/radio/api/class_crosstales_1_1_radio_1_1_provider_1_1_radio_provider_u_r_l.html")]
   public class RadioProviderURL : BaseRadioProvider
   {
      #region Variables

      [UnityEngine.Serialization.FormerlySerializedAsAttribute("Entries")] [Header("Source Settings"), Tooltip("All source radio station entries."), SerializeField]
      private System.Collections.Generic.List<RadioEntryURL> entries = new System.Collections.Generic.List<RadioEntryURL>();

      #endregion


      #region Properties

      /// <summary>All source radio station entries.</summary>
      public System.Collections.Generic.List<RadioEntryURL> Entries
      {
         get => entries;
         private set => entries = value;
      }

      public override System.Collections.Generic.List<BaseRadioEntry> RadioEntries => Entries.Cast<BaseRadioEntry>().ToList();

      protected override StationsChangeEvent onStationsChanged => OnStationsChanged;

      protected override ProviderReadyEvent onProviderReadyEvent => OnProviderReadyEvent;

      #endregion


      #region Events

      [Header("Events")] public StationsChangeEvent OnStationsChanged;
      public ProviderReadyEvent OnProviderReadyEvent;

      #endregion


      #region Private methods

      protected override void init()
      {
         base.init();

         foreach (RadioEntryURL entry in Entries.Where(entry => entry?.EnableSource == true))
         {
            if (!string.IsNullOrEmpty(entry.FinalURL))
            {
               if (entry.DataFormat == DataFormatURL.Stream)
               {
                  RadioStation station = new RadioStation(entry.Name, entry.FinalURL, entry.Format, entry.Station, entry.Genres.ToLower(), entry.Bitrate, entry.Rating, entry.Description, entry.Icon, entry.IconUrl, entry.City, entry.Country, entry.Language, entry.ChunkSize, entry.BufferSize, entry.ExcludedCodec);

                  if (!Stations.Contains(station))
                  {
                     Stations.Add(station);
                  }
                  else
                  {
                     Debug.LogWarning("Station already added: '" + entry + "'", this);
                  }
               }
               else
               {
                  StartCoroutine(loadWeb(addCoRoutine(), entry));
               }
            }
            else
            {
               Debug.LogWarning(entry + ": 'URL' is null or empty!" + System.Environment.NewLine + "Please add a valid URL.", this);
            }
         }
      }

      #endregion


      #region Editor-only methods

#if UNITY_EDITOR

      protected override void initInEditor()
      {
         if (Helper.isEditorMode)
         {
            base.initInEditor();

            foreach (RadioEntryURL entry in Entries.Where(entry => entry?.EnableSource == true))
            {
               if (!string.IsNullOrEmpty(entry.FinalURL))
               {
                  if (entry.DataFormat == DataFormatURL.Stream)
                  {
                     RadioStation station = new RadioStation(entry.Name, entry.FinalURL, entry.Format, entry.Station, entry.Genres.ToLower(), entry.Bitrate, entry.Rating, entry.Description, entry.Icon, entry.IconUrl, entry.City, entry.Country, entry.Language, entry.ChunkSize, entry.BufferSize, entry.ExcludedCodec);

                     if (!Stations.Contains(station))
                     {
                        Stations.Add(station);
                     }
                     else
                     {
                        Debug.LogWarning("Station already added: '" + entry + "'", this);
                     }
                  }
                  else
                  {
                     loadWebInEditor(entry);
                  }
               }
               else
               {
                  Debug.LogWarning(entry + ": 'URL' is null or empty!" + System.Environment.NewLine + "Please add a valid URL.", this);
               }
            }
         }
      }

#endif

      #endregion
   }
}
// © 2016-2024 crosstales LLC (https://www.crosstales.com)