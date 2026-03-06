using UnityEngine;
using Crosstales.Radio.Util;
using Crosstales.Radio.Model.Entry;
using Crosstales.Radio.Model.Enum;

namespace Crosstales.Radio.Provider
{
   /// <summary>Provider for users of Radio. This enables the possibility to manage the desired stations with a given initial set of stations.</summary>
   [HelpURL("https://www.crosstales.com/media/data/assets/radio/api/class_crosstales_1_1_radio_1_1_provider_1_1_radio_provider_user.html")]
   public class RadioProviderUser : BaseRadioProvider
   {
      #region Variables

      [UnityEngine.Serialization.FormerlySerializedAsAttribute("SaveOnDisable")] [Header("Save Behaviour"), Tooltip("Call 'Save' OnDisable (default: true)."), SerializeField]
      private bool saveOnDisable = true;


      [UnityEngine.Serialization.FormerlySerializedAsAttribute("Entry")] [Header("Source Settings"), Tooltip("User radio station entry."), SerializeField]
      private RadioEntryUser entry;

      #endregion


      #region Properties

      /// <summary>Call 'Save' OnDisable.</summary>
      public bool SaveOnDisable
      {
         get => saveOnDisable;
         set => saveOnDisable = value;
      }

      /// <summary>User radio station entry.</summary>
      public RadioEntryUser Entry
      {
         get => entry;
         private set => entry = value;
      }

      public override System.Collections.Generic.List<BaseRadioEntry> RadioEntries => new System.Collections.Generic.List<BaseRadioEntry> { Entry };

      protected override StationsChangeEvent onStationsChanged => OnStationsChanged;

      protected override ProviderReadyEvent onProviderReadyEvent => OnProviderReadyEvent;

      #endregion


      #region Events

      [Header("Events")] public StationsChangeEvent OnStationsChanged;
      public ProviderReadyEvent OnProviderReadyEvent;

      #endregion


      #region MonoBehaviour methods

      private void OnDisable()
      {
         //Debug.Log("OnDisable");

         if (!Helper.isEditorMode)
         {
            //Debug.Log("OnDisable");
            if (SaveOnDisable)
               Save(Entry.FinalPath);
         }
      }
/*
      private void OnApplicationQuit()
      {
         //Debug.Log("OnApplicationQuit");

         if (!Helper.isEditorMode)
         {
            //Debug.Log("OnApplicationQuit");
            if (SaveOnDisable)
               Save(Entry.FinalPath);
         }
      }
*/
      protected override void OnValidate()
      {
         if (Entry?.isInitialized == false)
            Entry.LoadOnlyOnce = true;

         base.OnValidate();
      }

      #endregion


      #region Public methods

      /// <summary>Deletes the user text-file.</summary>
      public void Delete()
      {
         if (Crosstales.Common.Util.FileHelper.ExistsFile(Entry.FinalPath))
         {
            try
            {
               Crosstales.Common.Util.FileHelper.DeleteFile(Entry.FinalPath);
            }
            catch (System.IO.IOException ex)
            {
               Debug.LogError("Could not delete file: " + Entry.FinalPath + System.Environment.NewLine + ex, this);
            }
         }
      }

      /// <summary>Shows the location of the user text-file in OS file browser.</summary>
      public void ShowFile()
      {
         Crosstales.Common.Util.FileHelper.ShowFile(Entry.FinalPath);
      }

      /// <summary>Edits the user text-file with the OS default application.</summary>
      public void EditFile()
      {
         Crosstales.Common.Util.FileHelper.OpenFile(Entry.FinalPath);
      }

      public void Save()
      {
         Save(Entry.FinalPath);
      }

      #endregion


      #region Private methods

      protected override void init()
      {
         base.init();

         if (Entry?.EnableSource == true)
         {
            if (!string.IsNullOrEmpty(Entry.FinalPath) && Crosstales.Common.Util.FileHelper.ExistsFile(Entry.FinalPath))
               StartCoroutine(loadWeb(addCoRoutine(), new RadioEntryURL(Entry, Crosstales.Common.Util.NetworkHelper.GetURLFromFile(Entry.FinalPath), DataFormatURL.Text), true));

            if (Entry.Resource != null)
            {
               if (!Entry.LoadOnlyOnce || Entry.LoadOnlyOnce && !Crosstales.Common.Util.FileHelper.ExistsFile(Entry.FinalPath))
               {
                  StartCoroutine(loadResource(addCoRoutine(), new RadioEntryResource(Entry, Entry.Resource, Entry.DataFormat, Entry.ReadNumberOfStations), true));

                  if (!Crosstales.Common.Util.FileHelper.ExistsFile(Entry.FinalPath))
                  {
                     //always store file first
                     Invoke(nameof(Save), 2f);
                  }
               }
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

            if (Entry?.EnableSource == true)
            {
               if (!string.IsNullOrEmpty(Entry.FinalPath) && Crosstales.Common.Util.FileHelper.ExistsFile(Entry.FinalPath))
                  loadWebInEditor(new RadioEntryURL(Entry, Crosstales.Common.Util.NetworkHelper.GetURLFromFile(Entry.FinalPath), DataFormatURL.Text), true);

               if (Entry.Resource != null)
               {
                  if (!Entry.LoadOnlyOnce || Entry.LoadOnlyOnce && !Crosstales.Common.Util.FileHelper.ExistsFile(Entry.FinalPath))
                     loadResourceInEditor(new RadioEntryResource(Entry, Entry.Resource, Entry.DataFormat, Entry.ReadNumberOfStations), true);
               }
            }
         }
      }

#endif

      #endregion
   }
}
// © 2016-2024 crosstales LLC (https://www.crosstales.com)