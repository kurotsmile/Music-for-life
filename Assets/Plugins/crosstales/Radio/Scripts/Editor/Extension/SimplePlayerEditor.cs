#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Crosstales.Radio.EditorUtil;
using Crosstales.Radio.Util;

namespace Crosstales.Radio.EditorExtension
{
   /// <summary>Custom editor for the 'SimplePlayer'-class.</summary>
   //[InitializeOnLoad]
   [CustomEditor(typeof(SimplePlayer))]
   public class SimplePlayerEditor : Editor
   {
      #region Variables

      private SimplePlayer script;

      private bool showStations;
      private bool showRecords;
      private bool showAllRecords;

      private static bool showData;
      private static bool showTD;
      private static bool showGlobalInfo;

      public delegate void StopPlayback();

      public static event StopPlayback OnStopPlayback;

      #endregion

/*
        #region Static constructor

        static SimplePlayerEditor()
        {
            EditorApplication.update += onEditorUpdate;
        }

        #endregion
*/

      #region Editor methods

      private void OnEnable()
      {
         EditorApplication.update += onEditorUpdate;

         script = (SimplePlayer)target;

         if (Helper.isEditorMode)
         {
            OnStopPlayback += stopRadio;
         }
      }

      private void OnDisable()
      {
         EditorApplication.update -= onEditorUpdate;

         if (Helper.isEditorMode)
         {
            stopRadio();

            OnStopPlayback -= stopRadio;
         }
      }

      public override bool RequiresConstantRepaint()
      {
         return true;
      }

      public override void OnInspectorGUI()
      {
         DrawDefaultInspector();

         EditorHelper.SeparatorUI();

         if (script.isActiveAndEnabled)
         {
            if (script.Player != null && script.Set != null)
            {
               EditorStyles.foldout.fontStyle = FontStyle.Bold;
               showData = EditorGUILayout.Foldout(showData, "Data");
               EditorStyles.foldout.fontStyle = FontStyle.Normal;

               if (showData)
               {
                  EditorGUI.indentLevel++;

                  GUILayout.Label("Ready:\t" + (script.isReady ? "Yes" : "No"));

                  GUILayout.Space(6);

                  showStations = EditorGUILayout.Foldout(showStations, "Stations (" + script.CountStations() + "/" + script.Stations.Count + ")");
                  if (showStations)
                  {
                     EditorGUI.indentLevel++;

                     foreach (Crosstales.Radio.Model.RadioStation station in script.GetStations(script.PlayRandom))
                     {
                        EditorGUILayout.SelectableLabel(station.ToShortString(), GUILayout.Height(16), GUILayout.ExpandHeight(false));
                     }

                     EditorGUI.indentLevel--;
                  }

                  GUILayout.Space(8);

                  if (Helper.isEditorMode)
                  {
                     if (GUILayout.Button(new GUIContent(" Load", EditorHelper.Icon_Refresh, "Loads all radio stations from the given providers.")))
                     {
                        script.Load();
                     }

                     GUI.enabled = script.Stations.Count > 0;

                     GUILayout.BeginHorizontal();
                     {
                        if (GUILayout.Button(new GUIContent(" Save TXT", EditorHelper.Icon_Save, "Saves all loaded radio stations as a text-file with streams.")))
                        {
                           string path = EditorUtility.SaveFilePanel("Save radio stations as text-file", string.Empty, "Radio.txt", "txt");
                           if (!string.IsNullOrEmpty(path))
                           {
                              script.Save(path, script.Filter);
                           }
                        }

                        if (GUILayout.Button(new GUIContent(" Save M3U", EditorHelper.Icon_Save, "Saves the list of all loaded radio stations as an M3U-file.")))
                        {
                           string path = EditorUtility.SaveFilePanel("Save radio stations as M3U-file", string.Empty, "Radio.m3u", "m3u");
                           if (!string.IsNullOrEmpty(path))
                           {
                              Helper.SaveAsM3U(path, script.GetStations(false, script.Filter));
                           }
                        }

                        if (GUILayout.Button(new GUIContent(" Save PLS", EditorHelper.Icon_Save, "Saves the list of all loaded radio stations as a PLS-file.")))
                        {
                           string path = EditorUtility.SaveFilePanel("Save radio stations as PLS-file", string.Empty, "Radio.pls", "pls");
                           if (!string.IsNullOrEmpty(path))
                           {
                              Helper.SaveAsPLS(path, script.GetStations(false, script.Filter));
                           }
                        }

                        if (GUILayout.Button(new GUIContent(" Save XSPF", EditorHelper.Icon_Save, "Saves the list of all loaded radio stations as a XSPF-file.")))
                        {
                           string path = EditorUtility.SaveFilePanel("Save radio stations as XSPF-file", string.Empty, "Radio.xspf", "xspf");
                           if (!string.IsNullOrEmpty(path))
                           {
                              Helper.SaveAsXSPF(path, script.GetStations(false, script.Filter));
                           }
                        }
                     }
                     GUILayout.EndHorizontal();

                     GUI.enabled = true;
                  }

                  EditorGUI.indentLevel--;
               }

               EditorHelper.SeparatorUI();

               EditorStyles.foldout.fontStyle = FontStyle.Bold;
               showTD = EditorGUILayout.Foldout(showTD, "Test-Drive");
               EditorStyles.foldout.fontStyle = FontStyle.Normal;

               if (showTD)
               {
                  EditorGUI.indentLevel++;

                  if (Helper.isEditorMode)
                  {
                     GUI.enabled = script.Stations.Count > 0;

                     GUILayout.BeginHorizontal();
                     {
                        if (GUILayout.Button(new GUIContent(" Previous", EditorHelper.Icon_Previous, "Plays the previous radio station.")))
                        {
                           script.Previous();
                        }

                        if (script.isPlayback)
                        {
                           if (GUILayout.Button(new GUIContent(" Stop", EditorHelper.Icon_Stop, "Stops the radio station.")))
                           {
                              script.Stop();
                           }
                        }
                        else
                        {
                           if (GUILayout.Button(new GUIContent(" Play", EditorHelper.Icon_Play, "Plays the radio station.")))
                           {
                              script.Play();
                           }
                        }

                        if (GUILayout.Button(new GUIContent(" Next", EditorHelper.Icon_Next, "Plays the next radio station.")))
                        {
                           script.Next();
                        }
                     }
                     GUILayout.EndHorizontal();

                     GUI.enabled = true;

                     if (script.isPlayback)
                     {
                        GUILayout.Space(8);

                        GUILayout.BeginHorizontal();
                        {
                           if (script.isBuffering)
                           {
                              GUILayout.Label($"Buffering station '{script.Station?.Name}':");

                              GUI.skin.label.alignment = TextAnchor.MiddleRight;
                              GUILayout.Label(script.BufferProgress.ToString(Constants.FORMAT_PERCENT));
                              GUI.skin.label.alignment = TextAnchor.MiddleLeft;
                           }
                           else
                           {
                              GUILayout.Label(script.Station?.Name);

                              GUI.skin.label.alignment = TextAnchor.MiddleRight;
                              GUILayout.Label(Helper.FormatSecondsToHRF(script.Source != null ? script.Source.time : 0f));
                              GUI.skin.label.alignment = TextAnchor.MiddleLeft;
                           }
                        }
                        GUILayout.EndHorizontal();
                     }
                  }
                  else
                  {
                     EditorGUILayout.HelpBox("Disabled in Play-mode!", MessageType.Info);
                  }

                  if (script.Player != null && script.isPlayback)
                  {
                     EditorHelper.SeparatorUI();

                     GUILayout.Label("Station Information", EditorStyles.boldLabel);

                     GUILayout.Label($"Name:\t\t{script.Station?.Name}");
                     GUILayout.Label($"Genres:\t\t{script.Station?.Genres}");
                     GUILayout.Label($"Format:\t\t{script.Station?.Format}");
                     GUILayout.Label($"Bitrate:\t\t{script.Station?.Bitrate}kb/s");
                     GUILayout.Label($"Rating:\t\t{script.Station?.Rating:N2}");

                     if (!Helper.isEditorMode && !script.Player.LegacyMode)
                     {
                        GUILayout.Space(4);

                        GUILayout.Label("Current Record:", EditorStyles.boldLabel);

                        if (!string.IsNullOrEmpty(script.RecordInfo?.Info))
                        {
                           GUILayout.Label("Title:\t\t" + script.RecordInfo?.Title);
                           GUILayout.Label("Artist:\t\t" + script.RecordInfo?.Artist);
                        }

                        GUILayout.Label("Time:\t\t" + Helper.FormatSecondsToHRF(script.RecordPlayTime));

                        GUILayout.Space(6);

                        showRecords = EditorGUILayout.Foldout(showRecords, $"Played records ({script.Station?.PlayedRecords.Count})");
                        if (showRecords)
                        {
                           EditorGUI.indentLevel++;

                           foreach (Crosstales.Radio.Model.RecordInfo ri in script.Station?.PlayedRecords)
                           {
                              EditorGUILayout.SelectableLabel(ri.ToShortString(), GUILayout.Height(16), GUILayout.ExpandHeight(false));
                           }

                           EditorGUI.indentLevel--;
                        }

                        GUILayout.Space(6);
                     }
                     else
                     {
                        GUILayout.Space(4);
                     }

                     GUILayout.Label("Stats:", EditorStyles.boldLabel);

                     if (script.Station != null)
                     {
                        if (!Helper.isEditorMode)
                           GUILayout.Label($"Total Time:{Constants.TAB}{Helper.FormatSecondsToHRF(script.Station.TotalPlayTime)}");

                        if (script.Station != null)
                        {
                           GUILayout.Label($"Total Download:{Constants.TAB}{Helper.FormatBytesToHRF(script.Station.TotalDataSize)}");
                           GUILayout.Label($"Total Requests:{Constants.TAB}{script.Station.TotalDataRequests}");
                        }
                     }
                  }

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

                  showAllRecords = EditorGUILayout.Foldout(showAllRecords, $"All Played records ({Context.AllPlayedRecords.Count})");
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
               EditorGUILayout.HelpBox("Please add a 'Set'!", MessageType.Warning);
               /*
               if (script.Turntable == null && script.Set != null)
               {
                  EditorGUILayout.HelpBox("Please add a 'Player'!", MessageType.Warning);
               }
               else if (script.Turntable != null && script.Set == null)
               {
                  EditorGUILayout.HelpBox("Please add a 'Set'!", MessageType.Warning);
               }
               else
               {
                  EditorGUILayout.HelpBox("Please add a 'Player' and a 'Set'!", MessageType.Warning);
               }
               */
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

      private void stopRadio()
      {
         script.Stop();
      }

      #endregion
   }
}
#endif
// © 2016-2024 crosstales LLC (https://www.crosstales.com)