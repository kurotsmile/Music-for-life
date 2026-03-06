#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Crosstales.Radio.EditorUtil;
using Crosstales.Radio.Util;

namespace Crosstales.Radio.EditorExtension
{
   /// <summary>Custom editor for the 'RadioPlayer'-class.</summary>
   //[InitializeOnLoad]
   [CustomEditor(typeof(RadioManager))]
   public class RadioManagerEditor : Editor
   {
      #region Variables

      private RadioManager script;

      private bool showStations;
      private bool showPlayers;
      protected static bool showData;

      #endregion


      #region Editor methods

      private void OnEnable()
      {
         script = (RadioManager)target;
      }

      public override void OnInspectorGUI()
      {
         DrawDefaultInspector();

         EditorHelper.SeparatorUI();

         if (script.isActiveAndEnabled)
         {
            if (script.Set != null)
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

                     foreach (Crosstales.Radio.Model.RadioStation station in script.GetStations())
                     {
                        EditorGUILayout.SelectableLabel(station.ToShortString(), GUILayout.Height(16), GUILayout.ExpandHeight(false));
                     }

                     EditorGUI.indentLevel--;
                  }

                  showPlayers = EditorGUILayout.Foldout(showPlayers, "Players (" + script.CountPlayers() + "/" + script.Players.Count + ")");
                  if (showPlayers)
                  {
                     EditorGUI.indentLevel++;

                     foreach (RadioPlayer player in script.Players)
                     {
                        EditorGUILayout.SelectableLabel(player.ToShortString(), GUILayout.Height(16), GUILayout.ExpandHeight(false));
                     }

                     EditorGUI.indentLevel--;
                  }

                  GUILayout.Space(8);

                  if (Helper.isEditorMode)
                  {
                     if (GUILayout.Button(new GUIContent(" Load", EditorHelper.Icon_Refresh, "Loads all radio stations from the given set.")))
                     {
                        if (script.Set != null)
                        {
                           script.Load();
                        }
                        else
                        {
                           Debug.LogWarning("'Set' is null - please add at least one set in the Inspector!");
                        }
                     }

                     GUI.enabled = script.Stations?.Count > 0;

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
            }
            else
            {
               EditorGUILayout.HelpBox("Please add a 'Set'!", MessageType.Warning);
            }
         }
         else
         {
            EditorGUILayout.HelpBox("Script is disabled!", MessageType.Info);
         }
      }

      #endregion
   }
}
#endif
// © 2016-2024 crosstales LLC (https://www.crosstales.com)