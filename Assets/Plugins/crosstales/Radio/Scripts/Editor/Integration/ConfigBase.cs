#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Crosstales.Radio.EditorUtil;
using Crosstales.Radio.EditorTask;
using Crosstales.Radio.Util;

namespace Crosstales.Radio.EditorIntegration
{
   /// <summary>Base class for editor windows.</summary>
   public abstract class ConfigBase : EditorWindow
   {
      #region Variables

      private static string updateText = UpdateCheck.TEXT_NOT_CHECKED;
      private static UpdateStatus updateStatus = UpdateStatus.NOT_CHECKED;

      private System.Threading.Thread worker;

      private Vector2 scrollPosConfig;
      private Vector2 scrollPosHelp;
      private Vector2 scrollPosAboutUpdate;
      private Vector2 scrollPosAboutReadme;
      private Vector2 scrollPosAboutVersions;

      private static string readme;
      private static string versions;

      private int aboutTab;

      private static readonly System.Random rnd = new System.Random();

      private readonly int adRnd1 = rnd.Next(0, 8);

      #endregion


      #region Protected methods

      protected void showConfiguration()
      {
         EditorHelper.BannerOC();

         GUI.skin.label.wordWrap = true;

         scrollPosConfig = EditorGUILayout.BeginScrollView(scrollPosConfig, false, false);
         {
            GUILayout.Label("General Settings", EditorStyles.boldLabel);

            //EditorConfig.PREFAB_AUTOLOAD = EditorGUILayout.Toggle(new GUIContent("Prefab Auto-Load", "Enable or disable auto-loading of the prefabs to the scene (default: " + EditorConstants.DEFAULT_PREFAB_AUTOLOAD + ")."), EditorConfig.PREFAB_AUTOLOAD);

            Config.DEBUG = EditorGUILayout.Toggle(new GUIContent("Debug", "Enable or disable debug logs (default: " + Constants.DEFAULT_DEBUG + ")"), Config.DEBUG);

            EditorConfig.UPDATE_CHECK = EditorGUILayout.Toggle(new GUIContent("Update Check", "Enable or disable the update-checks for the asset (default: " + EditorConstants.DEFAULT_UPDATE_CHECK + ")"), EditorConfig.UPDATE_CHECK);

            //EditorConfig.COMPILE_DEFINES = EditorGUILayout.Toggle(new GUIContent("Compile Defines", "Enable or disable adding compile defines 'CT_RADIO' for the asset (default: " + EditorConstants.DEFAULT_COMPILE_DEFINES + ")"), EditorConfig.COMPILE_DEFINES);

            EditorHelper.SeparatorUI();

            GUILayout.Label("UI Settings", EditorStyles.boldLabel);
            EditorConfig.HIERARCHY_ICON = EditorGUILayout.Toggle(new GUIContent("Show Hierarchy Icon", "Show hierarchy icon (default: " + EditorConstants.DEFAULT_HIERARCHY_ICON + ")."), EditorConfig.HIERARCHY_ICON);

            EditorHelper.SeparatorUI();

            GUILayout.Label("Default Audio Settings", EditorStyles.boldLabel);

            Config.DEFAULT_BITRATE = EditorGUILayout.IntField(new GUIContent("Bitrate", "Default bitrate in kbit/s (default: " + Constants.DEFAULT_DEFAULT_BITRATE + ")."), Config.DEFAULT_BITRATE);
            Config.DEFAULT_CHUNKSIZE = EditorGUILayout.IntField(new GUIContent("Chunk Size", "Default size of the streaming-chunk in KB (default: " + Constants.DEFAULT_DEFAULT_CHUNKSIZE + ")."), Config.DEFAULT_CHUNKSIZE);
            Config.DEFAULT_BUFFERSIZE = EditorGUILayout.IntField(new GUIContent("Buffer Size", "Default size of the local buffer in KB (default: " + Constants.DEFAULT_DEFAULT_BUFFERSIZE + ")."), Config.DEFAULT_BUFFERSIZE);
            Config.DEFAULT_CACHESTREAMSIZE = EditorGUILayout.IntField(new GUIContent("Cache Stream Size", $"Default size of the buffer in KB (default: {Constants.DEFAULT_DEFAULT_CACHESTREAMSIZE}, max: {Constants.DEFAULT_MAX_CACHESTREAMSIZE})."), Config.DEFAULT_CACHESTREAMSIZE);

            EditorHelper.SeparatorUI();

            GUILayout.Label("iOS Settings", EditorStyles.boldLabel);
            UnityEditor.PlayerSettings.iOS.allowHTTPDownload = EditorGUILayout.Toggle(new GUIContent("HTTP Download", "Allow HTTP download of data (recommended)."), UnityEditor.PlayerSettings.iOS.allowHTTPDownload);
         }
         EditorGUILayout.EndScrollView();

         validate();
      }

      protected void showHelp()
      {
         EditorHelper.BannerOC();

         scrollPosHelp = EditorGUILayout.BeginScrollView(scrollPosHelp, false, false);
         {
            GUILayout.Label("Resources", EditorStyles.boldLabel);

            GUILayout.BeginHorizontal();
            {
               GUILayout.BeginVertical();
               {
                  if (GUILayout.Button(new GUIContent(" Manual", EditorHelper.Icon_Manual, "Show the manual.")))
                     Crosstales.Common.Util.NetworkHelper.OpenURL(Constants.ASSET_MANUAL_URL);

                  GUILayout.Space(6);

                  if (GUILayout.Button(new GUIContent(" Forum", EditorHelper.Icon_Forum, "Visit the forum page.")))
                     Crosstales.Common.Util.NetworkHelper.OpenURL(Constants.ASSET_FORUM_URL);
               }
               GUILayout.EndVertical();

               GUILayout.BeginVertical();
               {
                  if (GUILayout.Button(new GUIContent(" API", EditorHelper.Icon_API, "Show the API.")))
                     Crosstales.Common.Util.NetworkHelper.OpenURL(Constants.ASSET_API_URL);

                  GUILayout.Space(6);

                  if (GUILayout.Button(new GUIContent(" Product", EditorHelper.Icon_Product, "Visit the product page.")))
                     Crosstales.Common.Util.NetworkHelper.OpenURL(Constants.ASSET_WEB_URL);
               }
               GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();

            EditorHelper.SeparatorUI();

            GUILayout.Label("Videos", EditorStyles.boldLabel);

            GUILayout.BeginHorizontal();
            {
               if (GUILayout.Button(new GUIContent(" Promo", EditorHelper.Video_Promo, "View the promotion video on 'Youtube'.")))
                  Crosstales.Common.Util.NetworkHelper.OpenURL(Constants.ASSET_VIDEO_PROMO);

               if (GUILayout.Button(new GUIContent(" Tutorial", EditorHelper.Video_Tutorial, "View the tutorial video on 'Youtube'.")))
                  Crosstales.Common.Util.NetworkHelper.OpenURL(Constants.ASSET_VIDEO_TUTORIAL);
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(6);

            if (GUILayout.Button(new GUIContent(" All Videos", EditorHelper.Icon_Videos, "Visit our 'Youtube'-channel for more videos.")))
               Crosstales.Common.Util.NetworkHelper.OpenURL(Constants.ASSET_SOCIAL_YOUTUBE);

            EditorHelper.SeparatorUI();

            GUILayout.Label("3rd Party Assets", EditorStyles.boldLabel);

            GUILayout.BeginVertical();
            {
               GUILayout.BeginHorizontal();
               {
                  if (GUILayout.Button(new GUIContent(string.Empty, EditorHelper.Asset_PlayMaker, "More information about 'PlayMaker'.")))
                     Crosstales.Common.Util.NetworkHelper.OpenURL(Constants.ASSET_3P_PLAYMAKER);

                  if (GUILayout.Button(new GUIContent(string.Empty, EditorHelper.Store_CompleteSoundSuite, "More information about 'Complete Sound Suite'.")))
                     Crosstales.Common.Util.NetworkHelper.OpenURL(Constants.ASSET_3P_SOUND_SUITE);

                  if (GUILayout.Button(new GUIContent(string.Empty, EditorHelper.Store_AudioVisualizer, "More information about 'Audio Visualizer'.")))
                     Crosstales.Common.Util.NetworkHelper.OpenURL(Constants.ASSET_3P_AUDIO_VISUALIZER);

                  if (GUILayout.Button(new GUIContent(string.Empty, EditorHelper.Store_VisualizerStudio, "More information about 'Visualizer Studio'.")))
                     Crosstales.Common.Util.NetworkHelper.OpenURL(Constants.ASSET_3P_VISUALIZER_STUDIO);
               }
               GUILayout.EndHorizontal();

               GUILayout.BeginHorizontal();
               {
                  if (GUILayout.Button(new GUIContent(string.Empty, EditorHelper.Store_ApolloVisualizerKit, "More information about 'Apollo Visualizer Kit'.")))
                     Crosstales.Common.Util.NetworkHelper.OpenURL(Constants.ASSET_3P_APOLLO_VISUALIZER);

                  if (GUILayout.Button(new GUIContent(string.Empty, EditorHelper.Store_RhythmVisualizator, "More information about 'Rhythm Visualizator'.")))
                     Crosstales.Common.Util.NetworkHelper.OpenURL(Constants.ASSET_3P_RHYTHM_VISUALIZATOR);

                  if (GUILayout.Button(new GUIContent(string.Empty, EditorHelper.Asset_VolumetricAudio, "More information about 'Volumetric Audio'.")))
                     Crosstales.Common.Util.NetworkHelper.OpenURL(Constants.ASSET_3P_VOLUMETRIC_AUDIO);

                  //CT Ads
                  switch (adRnd1)
                  {
                     case 0:
                     {
                        if (GUILayout.Button(new GUIContent(string.Empty, EditorHelper.Logo_Asset_BWF, "More information about 'Bad Word Filter'.")))
                           Crosstales.Common.Util.NetworkHelper.OpenURL(Constants.ASSET_BWF);

                        break;
                     }
                     case 1:
                     {
                        if (GUILayout.Button(new GUIContent(string.Empty, EditorHelper.Logo_Asset_DJ, "More information about 'DJ'.")))
                           Crosstales.Common.Util.NetworkHelper.OpenURL(Constants.ASSET_DJ);

                        break;
                     }
                     case 2:
                     {
                        if (GUILayout.Button(new GUIContent(string.Empty, EditorHelper.Logo_Asset_FB, "More information about 'File Browser'.")))
                           Crosstales.Common.Util.NetworkHelper.OpenURL(Constants.ASSET_FB);

                        break;
                     }
                     case 3:
                     {
                        if (GUILayout.Button(new GUIContent(string.Empty, EditorHelper.Logo_Asset_RTV, "More information about 'RT-Voice'.")))
                           Crosstales.Common.Util.NetworkHelper.OpenURL(Constants.ASSET_RTV);

                        break;
                     }
                     case 4:
                     {
                        if (GUILayout.Button(new GUIContent(string.Empty, EditorHelper.Logo_Asset_TB, "More information about 'Turbo Backup'.")))
                           Crosstales.Common.Util.NetworkHelper.OpenURL(Constants.ASSET_TB);

                        break;
                     }
                     case 5:
                     {
                        if (GUILayout.Button(new GUIContent(string.Empty, EditorHelper.Logo_Asset_TPS, "More information about 'Turbo Switch'.")))
                           Crosstales.Common.Util.NetworkHelper.OpenURL(Constants.ASSET_TPS);

                        break;
                     }
                     case 6:
                     {
                        if (GUILayout.Button(new GUIContent(string.Empty, EditorHelper.Logo_Asset_TPB, "More information about 'Turbo Builder'.")))
                           Crosstales.Common.Util.NetworkHelper.OpenURL(Constants.ASSET_TPB);

                        break;
                     }
                     case 7:
                     {
                        if (GUILayout.Button(new GUIContent(string.Empty, EditorHelper.Logo_Asset_OC, "More information about 'Online Check'.")))
                           Crosstales.Common.Util.NetworkHelper.OpenURL(Constants.ASSET_OC);


                        break;
                     }
                     default:
                     {
                        if (GUILayout.Button(new GUIContent(string.Empty, EditorHelper.Logo_Asset_TR, "More information about 'True Random'.")))
                           Crosstales.Common.Util.NetworkHelper.OpenURL(Constants.ASSET_TR);

                        break;
                     }
                  }
               }
               GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();

            GUILayout.Space(6);

            if (GUILayout.Button(new GUIContent(" All Supported Assets", EditorHelper.Icon_3p_Assets, "More information about the all supported assets.")))
               Crosstales.Common.Util.NetworkHelper.OpenURL(Constants.ASSET_3P_URL);
         }
         EditorGUILayout.EndScrollView();

         GUILayout.Space(6);
      }

      protected void showAbout()
      {
         EditorHelper.BannerOC();

         GUILayout.Space(3);
         GUILayout.Label(Constants.ASSET_NAME, EditorStyles.boldLabel);

         GUILayout.BeginHorizontal();
         {
            GUILayout.BeginVertical(GUILayout.Width(60));
            {
               GUILayout.Label("Version:");

               GUILayout.Space(12);

               GUILayout.Label("Web:");

               GUILayout.Space(2);

               GUILayout.Label("Email:");
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.Width(170));
            {
               GUILayout.Space(0);

               GUILayout.Label(Constants.ASSET_VERSION);

               GUILayout.Space(12);

               EditorGUILayout.SelectableLabel(Constants.ASSET_AUTHOR_URL, GUILayout.Height(16), GUILayout.ExpandHeight(false));

               GUILayout.Space(2);

               EditorGUILayout.SelectableLabel(Constants.ASSET_CONTACT, GUILayout.Height(16), GUILayout.ExpandHeight(false));
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            {
               //GUILayout.Space(0);
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.Width(64));
            {
               if (GUILayout.Button(new GUIContent(string.Empty, EditorHelper.Logo_Asset, "Visit asset website")))
                  Crosstales.Common.Util.NetworkHelper.OpenURL(EditorConstants.ASSET_URL);
            }
            GUILayout.EndVertical();
         }
         GUILayout.EndHorizontal();

         GUILayout.Label("© 2015-2024 by " + Constants.ASSET_AUTHOR);

         EditorHelper.SeparatorUI();

         GUILayout.BeginHorizontal();
         {
            if (GUILayout.Button(new GUIContent(" AssetStore", EditorHelper.Logo_Unity, "Visit the 'Unity AssetStore' website.")))
               Crosstales.Common.Util.NetworkHelper.OpenURL(Constants.ASSET_CT_URL);

            if (GUILayout.Button(new GUIContent(" " + Constants.ASSET_AUTHOR, EditorHelper.Logo_CT, "Visit the '" + Constants.ASSET_AUTHOR + "' website.")))
               Crosstales.Common.Util.NetworkHelper.OpenURL(Constants.ASSET_AUTHOR_URL);
         }
         GUILayout.EndHorizontal();

         EditorHelper.SeparatorUI();

         aboutTab = GUILayout.Toolbar(aboutTab, new[] { "Readme", "Versions", "Update" });

         switch (aboutTab)
         {
            case 2:
            {
               scrollPosAboutUpdate = EditorGUILayout.BeginScrollView(scrollPosAboutUpdate, false, false);
               {
                  Color fgColor = GUI.color;

                  GUI.color = Color.yellow;

                  switch (updateStatus)
                  {
                     case UpdateStatus.NO_UPDATE:
                        GUI.color = Color.green;
                        GUILayout.Label(updateText);
                        break;
                     case UpdateStatus.UPDATE:
                     {
                        GUILayout.Label(updateText);

                        if (GUILayout.Button(new GUIContent(" Download", "Visit the 'Unity AssetStore' to download the latest version.")))
                           UnityEditorInternal.AssetStore.Open("content/" + EditorConstants.ASSET_ID);

                        break;
                     }
                     case UpdateStatus.UPDATE_VERSION:
                     {
                        GUILayout.Label(updateText);

                        if (GUILayout.Button(new GUIContent(" Upgrade", "Upgrade to the newer version in the 'Unity AssetStore'")))
                           Crosstales.Common.Util.NetworkHelper.OpenURL(Constants.ASSET_CT_URL);

                        break;
                     }
                     case UpdateStatus.DEPRECATED:
                     {
                        GUILayout.Label(updateText);

                        if (GUILayout.Button(new GUIContent(" More Information", "Visit the 'crosstales'-site for more information.")))
                           Crosstales.Common.Util.NetworkHelper.OpenURL(Constants.ASSET_AUTHOR_URL);

                        break;
                     }
                     default:
                        GUI.color = Color.cyan;
                        GUILayout.Label(updateText);
                        break;
                  }

                  GUI.color = fgColor;
               }
               EditorGUILayout.EndScrollView();

               if (updateStatus == UpdateStatus.NOT_CHECKED || updateStatus == UpdateStatus.NO_UPDATE)
               {
                  bool isChecking = !(worker == null || worker?.IsAlive == false);

                  GUI.enabled = Crosstales.Common.Util.NetworkHelper.isInternetAvailable && !isChecking;

                  if (GUILayout.Button(new GUIContent(isChecking ? "Checking... Please wait." : " Check For Update", EditorHelper.Icon_Check, "Checks for available updates of " + Constants.ASSET_NAME)))
                  {
                     worker = new System.Threading.Thread(() => UpdateCheck.UpdateCheckForEditor(out updateText, out updateStatus));
                     worker.Start();
                  }

                  GUI.enabled = true;
               }

               break;
            }
            case 0:
            {
               if (readme == null)
               {
                  string path = Application.dataPath + EditorConfig.ASSET_PATH + "README.txt";

                  try
                  {
                     readme = Crosstales.Common.Util.FileHelper.ReadAllText(path);
                  }
                  catch (System.Exception)
                  {
                     readme = "README not found: " + path;
                  }
               }

               scrollPosAboutReadme = EditorGUILayout.BeginScrollView(scrollPosAboutReadme, false, false);
               {
                  GUILayout.Label(readme);
               }
               EditorGUILayout.EndScrollView();
               break;
            }
            default:
            {
               if (versions == null)
               {
                  string path = Application.dataPath + EditorConfig.ASSET_PATH + "Documentation/VERSIONS.txt";

                  try
                  {
                     versions = Crosstales.Common.Util.FileHelper.ReadAllText(path);
                  }
                  catch (System.Exception)
                  {
                     versions = "VERSIONS not found: " + path;
                  }
               }

               scrollPosAboutVersions = EditorGUILayout.BeginScrollView(scrollPosAboutVersions, false, false);
               {
                  GUILayout.Label(versions);
               }

               EditorGUILayout.EndScrollView();
               break;
            }
         }

         EditorHelper.SeparatorUI(6);

         GUILayout.BeginHorizontal();
         {
            if (GUILayout.Button(new GUIContent(string.Empty, EditorHelper.Social_Discord, "Communicate with us via 'Discord'.")))
               Crosstales.Common.Util.NetworkHelper.OpenURL(Constants.ASSET_SOCIAL_DISCORD);

            if (GUILayout.Button(new GUIContent(string.Empty, EditorHelper.Social_Facebook, "Follow us on 'Facebook'.")))
               Crosstales.Common.Util.NetworkHelper.OpenURL(Constants.ASSET_SOCIAL_FACEBOOK);

            if (GUILayout.Button(new GUIContent(string.Empty, EditorHelper.Social_Twitter, "Follow us on 'Twitter'.")))
               Crosstales.Common.Util.NetworkHelper.OpenURL(Constants.ASSET_SOCIAL_TWITTER);

            if (GUILayout.Button(new GUIContent(string.Empty, EditorHelper.Social_Linkedin, "Follow us on 'LinkedIn'.")))
               Crosstales.Common.Util.NetworkHelper.OpenURL(Constants.ASSET_SOCIAL_LINKEDIN);
         }
         GUILayout.EndHorizontal();

         GUILayout.Space(6);
      }

      protected static void save()
      {
         Config.Save();
         EditorConfig.Save();

         if (Config.DEBUG)
            Debug.Log("Config data saved");
      }

      #endregion


      #region Private methods

      private static void validate()
      {
         Config.DEFAULT_BITRATE = Config.DEFAULT_BITRATE <= 0 ? Constants.DEFAULT_DEFAULT_BITRATE : Helper.NearestMP3Bitrate(Config.DEFAULT_BITRATE);

         if (Config.DEFAULT_CHUNKSIZE <= 0)
         {
            Config.DEFAULT_CHUNKSIZE = Constants.DEFAULT_DEFAULT_CHUNKSIZE;
         }
         else if (Config.DEFAULT_CHUNKSIZE > Config.MAX_CACHESTREAMSIZE)
         {
            Config.DEFAULT_CHUNKSIZE = Config.MAX_CACHESTREAMSIZE;
         }

         if (Config.DEFAULT_BUFFERSIZE <= 0)
         {
            Config.DEFAULT_BUFFERSIZE = Constants.DEFAULT_DEFAULT_BUFFERSIZE;
         }
         else
         {
            if (Config.DEFAULT_BUFFERSIZE < 16)
            {
               Config.DEFAULT_BUFFERSIZE = 16;
            }

            if (Config.DEFAULT_BUFFERSIZE < Config.DEFAULT_CHUNKSIZE)
            {
               Config.DEFAULT_BUFFERSIZE = Config.DEFAULT_CHUNKSIZE;
            }
            else if (Config.DEFAULT_BUFFERSIZE > Config.MAX_CACHESTREAMSIZE)
            {
               Config.DEFAULT_BUFFERSIZE = Config.MAX_CACHESTREAMSIZE;
            }
         }

         if (Config.DEFAULT_CACHESTREAMSIZE <= 0)
         {
            Config.DEFAULT_CACHESTREAMSIZE = Constants.DEFAULT_DEFAULT_CACHESTREAMSIZE;
         }
         else if (Config.DEFAULT_CACHESTREAMSIZE <= Config.DEFAULT_BUFFERSIZE)
         {
            Config.DEFAULT_CACHESTREAMSIZE = Config.DEFAULT_BUFFERSIZE;
         }
         else if (Config.DEFAULT_CACHESTREAMSIZE > Config.MAX_CACHESTREAMSIZE)
         {
            Config.DEFAULT_CACHESTREAMSIZE = Config.MAX_CACHESTREAMSIZE;
         }
      }

      #endregion
   }
}
#endif
// © 2016-2024 crosstales LLC (https://www.crosstales.com)