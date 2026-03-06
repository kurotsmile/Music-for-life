#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Crosstales.Radio.EditorUtil;
using Crosstales.Radio.Util;

namespace Crosstales.Radio.EditorExtension
{
   /// <summary>Base-class for custom editors of children of the 'BaseRadioProvider'-class.</summary>
   public abstract class BaseRadioProviderEditor : Editor
   {
      #region Variables

      private Crosstales.Radio.Provider.BaseRadioProvider script;

      private bool showStations;
      protected static bool showData;

      #endregion


      #region Editor methods

      protected virtual void OnEnable()
      {
         script = (Crosstales.Radio.Provider.BaseRadioProvider)target;
      }

      #endregion

      protected void showDataUI()
      {
         EditorStyles.foldout.fontStyle = FontStyle.Bold;
         showData = EditorGUILayout.Foldout(showData, "Data");
         EditorStyles.foldout.fontStyle = FontStyle.Normal;

         if (showData)
         {
            EditorGUI.indentLevel++;
            GUILayout.Label("Ready:\t" + (script.isReady ? "Yes" : "No"));

            GUILayout.Space(6);

            showStations = EditorGUILayout.Foldout(showStations, "Stations (" + script.Stations.Count + ")");
            if (showStations)
            {
               EditorGUI.indentLevel++;

               foreach (Crosstales.Radio.Model.RadioStation station in script.Stations)
               {
                  EditorGUILayout.SelectableLabel(station.ToShortString(), GUILayout.Height(16), GUILayout.ExpandHeight(false));
               }

               EditorGUI.indentLevel--;
            }

            GUILayout.Space(8);

            if (Helper.isEditorMode)
            {
               if (GUILayout.Button(new GUIContent(" Load", EditorHelper.Icon_Refresh, "Loads all radio stations from the given sources.")))
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
                        script.Save(path);
                     }
                  }

                  if (GUILayout.Button(new GUIContent(" Save M3U", EditorHelper.Icon_Save, "Saves the list of all loaded radio stations as an M3U-file.")))
                  {
                     string path = EditorUtility.SaveFilePanel("Save radio stations as M3U-file", string.Empty, "Radio.m3u", "m3u");
                     if (!string.IsNullOrEmpty(path))
                     {
                        Helper.SaveAsM3U(path, script.Stations);
                     }
                  }

                  if (GUILayout.Button(new GUIContent(" Save PLS", EditorHelper.Icon_Save, "Saves the list of all loaded radio stations as a PLS-file.")))
                  {
                     string path = EditorUtility.SaveFilePanel("Save radio stations as PLS-file", string.Empty, "Radio.pls", "pls");
                     if (!string.IsNullOrEmpty(path))
                     {
                        Helper.SaveAsPLS(path, script.Stations);
                     }
                  }

                  if (GUILayout.Button(new GUIContent(" Save XSPF", EditorHelper.Icon_Save, "Saves the list of all loaded radio stations as a XSPF-file.")))
                  {
                     string path = EditorUtility.SaveFilePanel("Save radio stations as XSPF-file", string.Empty, "Radio.xspf", "xspf");
                     if (!string.IsNullOrEmpty(path))
                     {
                        Helper.SaveAsXSPF(path, script.Stations);
                     }
                  }
               }
               GUILayout.EndHorizontal();

               GUI.enabled = true;
            }

            EditorGUI.indentLevel--;
         }
      }
   }
}
#endif
// © 2016-2024 crosstales LLC (https://www.crosstales.com)