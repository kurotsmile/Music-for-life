#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Crosstales.Radio.EditorUtil;
using Crosstales.Radio.Util;

namespace Crosstales.Radio.EditorExtension
{
   /// <summary>Custom editor for the 'RadioPlayer'-class.</summary>
   [InitializeOnLoad]
   [CustomEditor(typeof(RadioPlayer))]
   public class RadioPlayerEditor : Editor
   {
      #region Variables

      public delegate void StopPlayback();

      public static event StopPlayback OnStopPlayback;

      private RadioPlayer script;

      private int channels = 2;
      private int sampleRate = 44100;

      private static bool showTD;
      private static bool showStationInfo;
      private static bool showGlobalInfo;

      private bool showRecords;
      private bool showAllRecords;

      #endregion


      #region Static constructor

      static RadioPlayerEditor()
      {
         EditorApplication.hierarchyWindowItemOnGUI += hierarchyItemCB;
      }

      #endregion


      #region Editor methods

      private void OnEnable()
      {
         EditorApplication.update += onEditorUpdate;

         script = (RadioPlayer)target;

         if (Helper.isEditorMode)
         {
            OnStopPlayback += stop;
         }
      }

      private void OnDisable()
      {
         EditorApplication.update -= onEditorUpdate;

         if (Helper.isEditorMode)
         {
            stop();

            OnStopPlayback -= stop;
         }
      }

      public override bool RequiresConstantRepaint()
      {
         return true;
      }

      public override void OnInspectorGUI()
      {
         EditorHelper.BannerOC();

         if (!string.IsNullOrEmpty(script.Station.Url) && script.Station.Url.StartsWith("file:", System.StringComparison.OrdinalIgnoreCase))
            EditorHelper.BannerDJ();

         DrawDefaultInspector();

         if (Constants.DEV_DEBUG)
         {
            GUILayout.Label("BufferSize:\t" + Helper.FormatBytesToHRF(script.CurrentBufferSize));
            GUILayout.Label("Download speed:\t" + Helper.FormatBytesToHRF(script.CurrentDownloadSpeed) + "/s");
         }

         EditorHelper.SeparatorUI();

         if (script.isActiveAndEnabled)
         {
            if (!string.IsNullOrEmpty(script.Station.Url))
            {
               EditorStyles.foldout.fontStyle = FontStyle.Bold;
               showTD = EditorGUILayout.Foldout(showTD, "Test-Drive");
               EditorStyles.foldout.fontStyle = FontStyle.Normal;

               if (showTD)
               {
                  EditorGUI.indentLevel++;
                  if (Helper.isEditorMode)
                  {
                     GUI.enabled = !script.isPlayback;
                     channels = EditorGUILayout.IntSlider(new GUIContent("Channels", "Audio channels of the station (default: 2)."), channels, 1, 2);
                     sampleRate = Mathf.Clamp(EditorGUILayout.IntField(new GUIContent("Sample Rate", "Audio sample rate (default: 44100)."), sampleRate), 8000, 192000);
                     GUI.enabled = true;

                     if (script.isPlayback && script.Source != null)
                     {
                        GUILayout.Space(8);

                        if (GUILayout.Button(new GUIContent(" Stop", EditorHelper.Icon_Stop, "Stops the radio station.")))
                        {
                           stop();
                        }

                        if (script.isBuffering)
                        {
                           GUILayout.Label("Buffering: " + script.BufferProgress.ToString(Constants.FORMAT_PERCENT));
                        }
                     }
                     else
                     {
                        GUILayout.Space(8);

                        if (GUILayout.Button(new GUIContent(" Play", EditorHelper.Icon_Play, "Plays the radio station.")))
                        {
                           script.PlayInEditor(channels, sampleRate);
                        }
                     }
                  }
                  else
                  {
                     EditorGUILayout.HelpBox("Disabled in Play-mode!", MessageType.Info);
                  }

                  EditorGUI.indentLevel--;
               }

               EditorHelper.SeparatorUI();
               EditorStyles.foldout.fontStyle = FontStyle.Bold;
               showStationInfo = EditorGUILayout.Foldout(showStationInfo, "Station Information");
               EditorStyles.foldout.fontStyle = FontStyle.Normal;

               if (showStationInfo)
               {
                  EditorGUI.indentLevel++;

                  if (!Helper.isEditorMode && !script.LegacyMode)
                  {
                     GUILayout.Label("Current Record:", EditorStyles.boldLabel);

                     if (!string.IsNullOrEmpty(script.RecordInfo.Info))
                     {
                        GUILayout.Label("Title:\t\t" + script.RecordInfo.Title);
                        GUILayout.Label("Artist:\t\t" + script.RecordInfo.Artist);
                     }

                     GUILayout.Label("Time:\t\t" + Helper.FormatSecondsToHRF(script.RecordPlayTime));

                     GUILayout.Space(6);

                     showRecords = EditorGUILayout.Foldout(showRecords, "Played records (" + script.Station.PlayedRecords.Count + ")");
                     if (showRecords)
                     {
                        EditorGUI.indentLevel++;

                        foreach (Crosstales.Radio.Model.RecordInfo ri in script.Station.PlayedRecords)
                        {
                           EditorGUILayout.SelectableLabel(ri.ToShortString(), GUILayout.Height(16), GUILayout.ExpandHeight(false));
                        }

                        EditorGUI.indentLevel--;
                     }

                     GUILayout.Space(6);
                  }

                  GUILayout.Label("Stats:", EditorStyles.boldLabel);

                  if (Helper.isEditorMode)
                     GUILayout.Label("Current Playtime:\t" + Helper.FormatSecondsToHRF(script.Source != null ? script.Source.time : 0f));
                  if (!Helper.isEditorMode)
                     GUILayout.Label($"Total Time:{Constants.TAB}{Helper.FormatSecondsToHRF(script.Station.TotalPlayTime)}");
                  GUILayout.Label($"Total Download:{Constants.TAB}{Helper.FormatBytesToHRF(script.Station.TotalDataSize)}");
                  GUILayout.Label($"Total Requests:{Constants.TAB}{script.Station.TotalDataRequests}");

                  EditorGUI.indentLevel--;
               }

               EditorHelper.SeparatorUI();

               EditorStyles.foldout.fontStyle = FontStyle.Bold;
               showGlobalInfo = EditorGUILayout.Foldout(showGlobalInfo, "Global Information");
               EditorStyles.foldout.fontStyle = FontStyle.Normal;

               if (showGlobalInfo)
               {
                  EditorGUI.indentLevel++;

                  GUILayout.Space(6);

                  showAllRecords = EditorGUILayout.Foldout(showAllRecords, "All Played records (" + Context.AllPlayedRecords.Count + ")");
                  if (showAllRecords)
                  {
                     EditorGUI.indentLevel++;

                     foreach (Crosstales.Radio.Model.RecordInfo ri in Context.AllPlayedRecords)
                     {
                        EditorGUILayout.SelectableLabel(ri.ToShortString(), GUILayout.Height(16), GUILayout.ExpandHeight(false));
                     }

                     EditorGUI.indentLevel--;
                  }

                  GUILayout.Space(6);

                  GUILayout.Label($"Total Time:{Constants.TAB}{Helper.FormatSecondsToHRF(Context.TotalPlayTime)}");
                  GUILayout.Label($"Total Download:{Constants.TAB}{Helper.FormatBytesToHRF(Context.TotalDataSize)}");
                  GUILayout.Label($"Total Requests:{Constants.TAB}{Context.TotalDataRequests}");

                  EditorGUI.indentLevel--;
               }
            }
            else
            {
               EditorGUILayout.HelpBox("Please add an 'URL' for the radio station!", MessageType.Warning);
            }
         }
         else
         {
            EditorGUILayout.HelpBox("Script is disabled!", MessageType.Info);
         }
      }

      #endregion


      #region Private methods

      private static void onEditorUpdate()
      {
         if (EditorApplication.isCompiling || BuildPipeline.isBuildingPlayer)
            onStopPlayback();
      }

      private static void onStopPlayback()
      {
         OnStopPlayback?.Invoke();
      }

      private void stop()
      {
         if (script != null)
            script.Stop();
      }

      private static void hierarchyItemCB(int instanceID, Rect selectionRect)
      {
         if (EditorConfig.HIERARCHY_ICON)
         {
            GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

            if (go != null && go.GetComponent<RadioPlayer>())
            {
               Rect r = new Rect(selectionRect);
               r.x = r.width - 4;

               GUI.Label(r, EditorHelper.Logo_Asset_Small);
            }
         }
      }

      #endregion
   }
}
#endif
// © 2016-2024 crosstales LLC (https://www.crosstales.com)