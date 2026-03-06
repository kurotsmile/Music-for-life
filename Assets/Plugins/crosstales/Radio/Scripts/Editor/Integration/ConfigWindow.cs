#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Crosstales.Radio.EditorUtil;
using Crosstales.Radio.Util;
using Crosstales.Radio.Model.Enum;

namespace Crosstales.Radio.EditorIntegration
{
   /// <summary>Editor window extension.</summary>
   public class ConfigWindow : ConfigBase
   {
      #region Variables

      private int tab;
      private int lastTab;

      private string tdUrl = "http://185.33.21.112:11010";

      private readonly AudioFormat[] formats = { AudioFormat.MP3, AudioFormat.OGG };
      private int formatIndex;

      private int channels = 2;
      private int sampleRate = 44100;

      private Vector2 scrollPosPrefabs;
      private Vector2 scrollPosTD;

      #endregion


      #region EditorWindow methods

      [MenuItem("Tools/" + Constants.ASSET_NAME + "/Configuration...", false, EditorHelper.MENU_ID + 1)]
      public static void ShowWindow()
      {
         GetWindow(typeof(ConfigWindow));
      }

      public static void ShowWindow(int tab)
      {
         ConfigWindow window = GetWindow(typeof(ConfigWindow)) as ConfigWindow;
         if (window != null) window.tab = tab;
      }

      private void OnEnable()
      {
         titleContent = new GUIContent(Constants.ASSET_NAME, EditorHelper.Logo_Asset_Small);
      }

      private void OnInspectorUpdate()
      {
         Repaint();
      }

      private void OnGUI()
      {
         tab = GUILayout.Toolbar(tab, new[] { "Config", "Prefabs", "TD", "Help", "About" });

         if (tab != lastTab)
         {
            lastTab = tab;
            GUI.FocusControl(null);
         }

         switch (tab)
         {
            case 0:
            {
               showConfiguration();

               EditorHelper.SeparatorUI(6);

               GUILayout.BeginHorizontal();
               {
                  if (GUILayout.Button(new GUIContent(" Save", EditorHelper.Icon_Save, "Saves the configuration settings for this project")))
                  {
                     save();
                  }

                  if (GUILayout.Button(new GUIContent(" Reset", EditorHelper.Icon_Reset, "Resets the configuration settings for this project.")))
                  {
                     if (EditorUtility.DisplayDialog("Reset configuration?", "Reset the configuration of " + Constants.ASSET_NAME + "?", "Yes", "No"))
                     {
                        Config.Reset();
                        EditorConfig.Reset();
                        save();
                     }
                  }
               }
               GUILayout.EndHorizontal();

               GUILayout.Space(6);
               break;
            }
            case 1:
               showPrefabs();
               break;
            case 2:
               showTestDrive();
               break;
            case 3:
               showHelp();
               break;
            default:
               showAbout();
               break;
         }
      }

      #endregion


      #region Private methods

      private void showPrefabs()
      {
         EditorHelper.BannerOC();

         scrollPosPrefabs = EditorGUILayout.BeginScrollView(scrollPosPrefabs, false, false);
         {
            GUILayout.Label("Available Prefabs", EditorStyles.boldLabel);

            GUILayout.Space(6);

            GUILayout.Label("RadioPlayer");

            if (GUILayout.Button(new GUIContent(" Add", EditorHelper.Icon_Plus, "Adds a 'RadioPlayer'-prefab to the scene.")))
               EditorHelper.InstantiatePrefab("RadioPlayer");

            EditorHelper.SeparatorUI();

            GUILayout.Label("RadioProviderResource");

            if (GUILayout.Button(new GUIContent(" Add", EditorHelper.Icon_Plus, "Adds a 'RadioProviderResource'-prefab to the scene.")))
               EditorHelper.InstantiatePrefab("RadioProviderResource");

            GUILayout.Space(6);

            GUILayout.Label("RadioProviderShoutcast");

            if (GUILayout.Button(new GUIContent(" Add", EditorHelper.Icon_Plus, "Adds a 'RadioProviderShoutcast'-prefab to the scene.")))
               EditorHelper.InstantiatePrefab("RadioProviderShoutcast");

            GUILayout.Space(6);

            GUILayout.Label("RadioProviderURL");

            if (GUILayout.Button(new GUIContent(" Add", EditorHelper.Icon_Plus, "Adds a 'RadioProviderURL'-prefab to the scene.")))
               EditorHelper.InstantiatePrefab("RadioProviderURL");

            GUILayout.Space(6);
            GUILayout.Label("RadioProviderUser");

            if (GUILayout.Button(new GUIContent(" Add", EditorHelper.Icon_Plus, "Adds a 'RadioProviderUser'-prefab to the scene.")))
               EditorHelper.InstantiatePrefab("RadioProviderUser");

            EditorHelper.SeparatorUI();

            GUILayout.Label("RadioSet");

            if (GUILayout.Button(new GUIContent(" Add", EditorHelper.Icon_Plus, "Adds a 'RadioSet'-prefab to the scene.")))
               EditorHelper.InstantiatePrefab("RadioSet");

            EditorHelper.SeparatorUI();

            GUILayout.Label("RadioManager");

            if (GUILayout.Button(new GUIContent(" Add", EditorHelper.Icon_Plus, "Adds a 'RadioManager'-prefab to the scene.")))
               EditorHelper.InstantiatePrefab("RadioManager");

            EditorHelper.SeparatorUI();

            GUILayout.Label("SimplePlayer");

            if (GUILayout.Button(new GUIContent(" Add", EditorHelper.Icon_Plus, "Adds a 'SimplePlayer'-prefab to the scene.")))
               EditorHelper.InstantiatePrefab("SimplePlayer");

            EditorHelper.SeparatorUI();

            GUILayout.Label("Loudspeaker");

            if (GUILayout.Button(new GUIContent(" Add", EditorHelper.Icon_Plus, "Adds a 'Loudspeaker'-prefab to the scene.")))
               EditorHelper.InstantiatePrefab("Loudspeaker", $"{EditorConfig.ASSET_PATH}Extras/Loudspeaker/Resources/Prefabs/");

            GUILayout.Space(6);

            GUILayout.Label("StreamSaver");

            if (GUILayout.Button(new GUIContent(" Add", EditorHelper.Icon_Plus, "Adds a 'StreamSaver'-prefab to the scene.")))
               EditorHelper.InstantiatePrefab("StreamSaver", $"{EditorConfig.ASSET_PATH}Extras/StreamSaver/Resources/Prefabs/");

            GUILayout.Space(6);

            GUILayout.Label("CrossFader");

            if (GUILayout.Button(new GUIContent(" Add", EditorHelper.Icon_Plus, "Adds a 'CrossFader'-prefab to the scene.")))
               EditorHelper.InstantiatePrefab("CrossFader", $"{EditorConfig.ASSET_PATH}Extras/CrossFader/Resources/Prefabs/");

            GUILayout.Space(6);
         }
         EditorGUILayout.EndScrollView();
      }

      private void showTestDrive()
      {
         EditorHelper.BannerOC();

         GUILayout.Space(3);
         GUILayout.Label("Test-Drive", EditorStyles.boldLabel);

         if (Helper.isEditorMode)
         {
            if (RadioPlayer.Instance != null)
            {
               scrollPosTD = EditorGUILayout.BeginScrollView(scrollPosTD, false, false);
               {
                  //GUI.enabled = !RadioPlayer.Instance.isPlayback;
                  tdUrl = EditorGUILayout.TextField(new GUIContent("URL", "URL of the radio station."), tdUrl);
                  formatIndex = EditorGUILayout.Popup("Format", formatIndex, System.Array.ConvertAll(formats, x => x.ToString()));
                  channels = EditorGUILayout.IntSlider(new GUIContent("Channels", "Audio channels of the radio station (default: 2)."), channels, 1, 2);
                  sampleRate = Mathf.Clamp(EditorGUILayout.IntField(new GUIContent("Sample Rate", "Audio sample rate of the radio station (default: 44100)."), sampleRate), 8000, 192000);


                  if (RadioPlayer.Instance.Station != null)
                  {
                     EditorHelper.SeparatorUI();

                     GUILayout.Label("Station Information", EditorStyles.boldLabel);
                     GUILayout.Label("Stats:", EditorStyles.boldLabel);
                     GUILayout.Label($"Current playtime:\t{Helper.FormatSecondsToHRF(RadioPlayer.Instance.Source != null ? RadioPlayer.Instance.Source.time : 0f)}");
                     GUILayout.Label($"Total download:{Constants.TAB}{Helper.FormatBytesToHRF(RadioPlayer.Instance.Station.TotalDataSize)}");
                  }

                  EditorHelper.SeparatorUI();

                  GUILayout.Label("Global Information", EditorStyles.boldLabel);
                  GUILayout.Label($"Total download:{Constants.TAB}{Helper.FormatBytesToHRF(Context.TotalDataSize)}");
                  GUILayout.Label($"Total requests:{Constants.TAB}{Context.TotalDataRequests}");
               }
               EditorGUILayout.EndScrollView();

               EditorHelper.SeparatorUI(6);

               if (RadioPlayer.Instance.Source != null && RadioPlayer.Instance.isPlayback)
               {
                  if (RadioPlayer.Instance.isBuffering)
                  {
                     GUILayout.Label("Buffering: " + RadioPlayer.Instance.BufferProgress.ToString(Constants.FORMAT_PERCENT));
                  }

                  if (GUILayout.Button(new GUIContent(" Stop", EditorHelper.Icon_Stop, "Stops the radio station.")))
                     RadioPlayer.Instance.Stop();
               }
               else
               {
                  if (!string.IsNullOrEmpty(tdUrl))
                  {
                     if (GUILayout.Button(new GUIContent(" Play", EditorHelper.Icon_Play, "Plays the radio station.")))
                     {
                        RadioPlayer.Instance.Station = new Crosstales.Radio.Model.RadioStation("TD-Radio", tdUrl, formats[formatIndex]);

                        RadioPlayer.Instance.PlayInEditor(channels, sampleRate);
                     }
                  }
                  else
                  {
                     EditorGUILayout.HelpBox("Please add an 'URL' for the radio station!", MessageType.Warning);
                  }
               }

               //GUI.enabled = true;

               GUILayout.Space(6);
            }
            else
            {
               EditorGUILayout.HelpBox("RadioPlayer not available!", MessageType.Warning);

               EditorGUILayout.HelpBox($"Did you add the 'RadioPlayer'-prefab to the scene?", MessageType.Info);

               GUILayout.Space(8);

               if (GUILayout.Button(new GUIContent($" Add RadioPlayer", EditorHelper.Icon_Plus, $"Add the 'RadioPlayer'-prefab to the current scene.")))
                  EditorHelper.InstantiatePrefab("RadioPlayer");
            }
         }
         else
         {
            EditorGUILayout.HelpBox("Disabled in Play-mode!", MessageType.Info);
         }
      }

      #endregion
   }
}
#endif
// © 2016-2024 crosstales LLC (https://www.crosstales.com)