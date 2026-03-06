#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Crosstales.Radio.Util;

namespace Crosstales.Radio.EditorUtil
{
   /// <summary>Editor helper class.</summary>
   public abstract class EditorHelper : Crosstales.Common.EditorUtil.BaseEditorHelper
   {
      #region Static variables

      /// <summary>Start index inside the "GameObject"-menu.</summary>
      public const int GO_ID = 32;

      /// <summary>Start index inside the "Tools"-menu.</summary>
      public const int MENU_ID = 11801; // 1, R = 18, A = 01

      private static Texture2D logo_asset;
      private static Texture2D logo_asset_small;

      private static Texture2D icon_play;
      private static Texture2D icon_stop;
      private static Texture2D icon_next;
      private static Texture2D icon_previous;
      private static Texture2D icon_edit;
      private static Texture2D icon_show;
      private static Texture2D icon_clear;

      private static Texture2D store_AudioVisualizer;
      private static Texture2D store_CompleteSoundSuite;
      private static Texture2D store_VisualizerStudio;
      private static Texture2D store_ApolloVisualizerKit;
      private static Texture2D store_RhythmVisualizator;

      #endregion


      #region Static properties

      public static Texture2D Logo_Asset => loadImage(ref logo_asset, "logo_asset_pro.png");

      public static Texture2D Logo_Asset_Small => loadImage(ref logo_asset_small, "logo_asset_small_pro.png");

      public static Texture2D Icon_Play => loadImage(ref icon_play, "icon_play.png");

      public static Texture2D Icon_Stop => loadImage(ref icon_stop, "icon_stop.png");

      public static Texture2D Icon_Next => loadImage(ref icon_next, "icon_next.png");

      public static Texture2D Icon_Previous => loadImage(ref icon_previous, "icon_previous.png");

      public static Texture2D Icon_Edit => loadImage(ref icon_edit, "icon_edit.png");

      public static Texture2D Icon_Show => loadImage(ref icon_show, "icon_show.png");

      public static Texture2D Icon_Clear => loadImage(ref icon_clear, "icon_clear.png");

      public static Texture2D Store_AudioVisualizer => loadImage(ref store_AudioVisualizer, "Store_AudioVisualizer.png");

      public static Texture2D Store_CompleteSoundSuite => loadImage(ref store_CompleteSoundSuite, "Store_CompleteSoundSuite.png");

      public static Texture2D Store_VisualizerStudio => loadImage(ref store_VisualizerStudio, "Store_VisualizerStudio.png");

      public static Texture2D Store_ApolloVisualizerKit => loadImage(ref store_ApolloVisualizerKit, "Store_ApolloVisualizerKit.png");

      public static Texture2D Store_RhythmVisualizator => loadImage(ref store_RhythmVisualizator, "Store_RhythmVisualizator.png");

      #endregion


      #region Static methods


      /// <summary>Instantiates a prefab.</summary>
      /// <param name="prefabName">Name of the prefab.</param>
      public static void InstantiatePrefab(string prefabName)
      {
         InstantiatePrefab(prefabName, EditorConfig.PREFAB_PATH);
      }

      /// <summary>Shows a banner for "Online Check".</summary>
      public static void BannerOC()
      {
#if !CT_OC
         if (Util.Constants.SHOW_OC_BANNER)
         {
            GUILayout.BeginHorizontal();
            {
               EditorGUILayout.HelpBox("'Online Check' is not installed!" + System.Environment.NewLine + "For reliable Internet availability tests, please install or get it from the Unity AssetStore.", MessageType.Info);

               GUILayout.BeginVertical(GUILayout.Width(32));
               {
                  GUILayout.Space(4);

                  if (GUILayout.Button(new GUIContent(string.Empty, Logo_Asset_OC, "Visit Online Check in the Unity AssetStore")))
                     Crosstales.Common.Util.NetworkHelper.OpenURL(Util.Constants.ASSET_OC);
               }
               GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
         }
#endif
      }

      /// <summary>Shows a banner for "DJ".</summary>
      public static void BannerDJ()
      {
#if !CT_DJ
         if (Constants.SHOW_DJ_BANNER)
         {
            GUILayout.BeginHorizontal();
            {
               EditorGUILayout.HelpBox("'DJ' is not installed!" + System.Environment.NewLine + "To play audio files, please install or get it from the Unity AssetStore.", MessageType.Info);

               GUILayout.BeginVertical(GUILayout.Width(32));
               {
                  GUILayout.Space(4);

                  if (GUILayout.Button(new GUIContent(string.Empty, Logo_Asset_DJ, "Visit DJ in the Unity AssetStore")))
                     Crosstales.Common.Util.NetworkHelper.OpenURL(Constants.ASSET_DJ);
               }
               GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
         }
#endif
      }

      /// <summary>Loads an image as Texture2D from 'Editor Default Resources'.</summary>
      /// <param name="logo">Logo to load.</param>
      /// <param name="fileName">Name of the image.</param>
      /// <returns>Image as Texture2D from 'Editor Default Resources'.</returns>
      private static Texture2D loadImage(ref Texture2D logo, string fileName)
      {
         if (logo == null)
         {
#if CT_DEVELOP
            logo = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets" + EditorConfig.ASSET_PATH + "Icons/" + fileName, typeof(Texture2D));
#else
                logo = (Texture2D)EditorGUIUtility.Load("crosstales/Radio/" + fileName);
#endif

            if (logo == null)
               Debug.LogWarning("Image not found: " + fileName);
         }

         return logo;
      }

      #endregion
   }
}
#endif
// © 2016-2024 crosstales LLC (https://www.crosstales.com)