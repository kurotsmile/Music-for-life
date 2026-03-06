using UnityEngine;
using System.Linq;
using Crosstales.Radio.Util;
using Crosstales.Radio.Model.Entry;

namespace Crosstales.Radio.Provider
{
   /// <summary>Provider for resources of radio stations in various formats.</summary>
   [HelpURL("https://www.crosstales.com/media/data/assets/radio/api/class_crosstales_1_1_radio_1_1_provider_1_1_radio_provider_resource.html")]
   public class RadioProviderResource : BaseRadioProvider
   {
      #region Variables

      [UnityEngine.Serialization.FormerlySerializedAsAttribute("Entries")] [Header("Source Settings"), Tooltip("All source radio station entries."), SerializeField] private System.Collections.Generic.List<RadioEntryResource> entries = new System.Collections.Generic.List<RadioEntryResource>();

      #endregion


      #region Properties

      /// <summary>All source radio station entries.</summary>
      public System.Collections.Generic.List<RadioEntryResource> Entries
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

         foreach (RadioEntryResource entry in Entries.Where(entry => entry?.EnableSource == true))
         {
            if (entry.Resource != null)
            {
               StartCoroutine(loadResource(addCoRoutine(), entry));
            }
            else
            {
               Debug.LogWarning(entry + ": resource field 'Resource' is null or empty!" + System.Environment.NewLine + "Please add a valid resource.", this);
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

            foreach (RadioEntryResource entry in Entries.Where(entry => entry?.EnableSource == true))
            {
               if (entry.Resource != null)
               {
                  loadResourceInEditor(entry);
               }
               else
               {
                  Debug.LogWarning(entry + ": resource field 'Resource' is null or empty!" + System.Environment.NewLine + "Please add a valid resource.", this);
               }
            }
         }
      }

#endif

      #endregion
   }
}
// © 2016-2024 crosstales LLC (https://www.crosstales.com)