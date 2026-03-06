#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Crosstales.Radio.EditorUtil;
using Crosstales.Radio.Util;
using Crosstales.Radio.Provider;

namespace Crosstales.Radio.EditorExtension
{
   /// <summary>Custom editor for the 'RadioProviderUser'-class.</summary>
   [CustomEditor(typeof(RadioProviderUser))]
   public class RadioProviderUserEditor : Editor
   {
      #region Variables

      private RadioProviderUser script;

      private bool showStations;
      protected static bool showData;

      #endregion


      #region Editor methods

      private void OnEnable()
      {
         script = (RadioProviderUser)target;
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
            if (!string.IsNullOrEmpty(script.Entry.Path))
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
                     if (GUILayout.Button(new GUIContent(" Load", EditorHelper.Icon_Refresh, "Loads all radio stations from the user text-file and resource.")))
                     {
                        script.Load();
                     }

                     GUI.enabled = script.Stations.Count > 0;

                     GUILayout.BeginHorizontal();
                     {
                        if (GUILayout.Button(new GUIContent(" Save", EditorHelper.Icon_Save, "Saves all loaded radio stations as user text-file.")))
                        {
                           script.Save(script.Entry.FinalPath);
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

                     if (Crosstales.Common.Util.FileHelper.ExistsFile(script.Entry.FinalPath))
                     {
                        GUILayout.Space(8);

                        GUILayout.BeginHorizontal();
                        {
                           if (GUILayout.Button(new GUIContent(" Show File", EditorHelper.Icon_Show, "Shows the location of the user text-file in OS file browser.")))
                           {
                              script.ShowFile();
                           }

                           if (GUILayout.Button(new GUIContent(" Edit File", EditorHelper.Icon_Edit, "Edits the user text-file with the OS default application.")))
                           {
                              script.EditFile();
                           }

                           if (GUILayout.Button(new GUIContent(" Delete File", EditorHelper.Icon_Delete, "Deletes the user text-file.")))
                           {
                              if (EditorUtility.DisplayDialog("Delete user text-file?", "Delete the user text-file?", "Yes", "No"))
                              {
                                 script.Delete();
                              }
                           }
                        }
                        GUILayout.EndHorizontal();
                     }
                  }

                  EditorGUI.indentLevel--;
               }
            }
            else
            {
               EditorGUILayout.HelpBox("Please add a 'Path'!", MessageType.Warning);
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