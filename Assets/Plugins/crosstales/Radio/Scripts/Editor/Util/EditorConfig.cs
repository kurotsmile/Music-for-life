#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Crosstales.Radio.EditorUtil
{
   /// <summary>Editor configuration for the asset.</summary>
   [InitializeOnLoad]
   public static class EditorConfig
   {
      #region Changable variables

      /// <summary>Enable or disable update-checks for the asset.</summary>
      public static bool UPDATE_CHECK = EditorConstants.DEFAULT_UPDATE_CHECK;

      /// <summary>Enable or disable adding compile defines "CT_RADIO" for the asset.</summary>
      public static bool COMPILE_DEFINES = EditorConstants.DEFAULT_COMPILE_DEFINES;

      /// <summary>Automatically load and add the prefabs to the scene.</summary>
      public static bool PREFAB_AUTOLOAD = EditorConstants.DEFAULT_PREFAB_AUTOLOAD;

      /// <summary>Enable or disable the icon in the hierarchy.</summary>
      public static bool HIERARCHY_ICON = EditorConstants.DEFAULT_HIERARCHY_ICON;

      /// <summary>Is the configuration loaded?</summary>
      public static bool isLoaded;

      private static string assetPath;
      private const string idPath = "Documentation/id/";
      private static readonly string idName = EditorConstants.ASSET_UID + ".txt";

      #endregion


      #region Constructor

      static EditorConfig()
      {
         if (!isLoaded)
         {
            Load();
         }
      }

      #endregion


      #region Properties

      /// <summary>Returns the path to the asset inside the Unity project.</summary>
      /// <returns>The path to the asset inside the Unity project.</returns>
      public static string ASSET_PATH
      {
         get
         {
            if (assetPath == null)
            {
               try
               {
                  if (Crosstales.Common.Util.FileHelper.ExistsFile(Application.dataPath + EditorConstants.DEFAULT_ASSET_PATH + idPath + idName))
                  {
                     assetPath = EditorConstants.DEFAULT_ASSET_PATH;
                  }
                  else
                  {
                     string[] files = System.IO.Directory.GetFiles(Application.dataPath, idName, System.IO.SearchOption.AllDirectories);

                     if (files.Length > 0)
                     {
                        string name = files[0].Substring(Application.dataPath.Length);
                        assetPath = name.Substring(0, name.Length - idPath.Length - idName.Length).Replace("\\", "/");
                     }
                     else
                     {
                        Debug.LogWarning("Could not locate the asset! File not found: " + idName);
                        assetPath = EditorConstants.DEFAULT_ASSET_PATH;
                     }
                  }
               }
               catch (System.Exception ex)
               {
                  Debug.LogWarning("Could not locate asset: " + ex);
               }
            }

            return assetPath;
         }
      }

      /// <summary>Returns the path of the prefabs.</summary>
      /// <returns>The path of the prefabs.</returns>
      public static string PREFAB_PATH => ASSET_PATH + EditorConstants.PREFAB_SUBPATH;

      #endregion


      #region Public static methods

      /// <summary>Resets all changeable variables to their default value.</summary>
      public static void Reset()
      {
         assetPath = null;

         UPDATE_CHECK = EditorConstants.DEFAULT_UPDATE_CHECK;
         COMPILE_DEFINES = EditorConstants.DEFAULT_COMPILE_DEFINES;
         PREFAB_AUTOLOAD = EditorConstants.DEFAULT_PREFAB_AUTOLOAD;
         HIERARCHY_ICON = EditorConstants.DEFAULT_HIERARCHY_ICON;
      }

      /// <summary>Loads all changeable variables.</summary>
      public static void Load()
      {
         assetPath = null;

         if (Crosstales.Common.Util.CTPlayerPrefs.HasKey(EditorConstants.KEY_UPDATE_CHECK))
            UPDATE_CHECK = Crosstales.Common.Util.CTPlayerPrefs.GetBool(EditorConstants.KEY_UPDATE_CHECK);

         if (Crosstales.Common.Util.CTPlayerPrefs.HasKey(EditorConstants.KEY_COMPILE_DEFINES))
            COMPILE_DEFINES = Crosstales.Common.Util.CTPlayerPrefs.GetBool(EditorConstants.KEY_COMPILE_DEFINES);

         if (Crosstales.Common.Util.CTPlayerPrefs.HasKey(EditorConstants.KEY_PREFAB_AUTOLOAD))
            PREFAB_AUTOLOAD = Crosstales.Common.Util.CTPlayerPrefs.GetBool(EditorConstants.KEY_PREFAB_AUTOLOAD);

         if (Crosstales.Common.Util.CTPlayerPrefs.HasKey(EditorConstants.KEY_HIERARCHY_ICON))
            HIERARCHY_ICON = Crosstales.Common.Util.CTPlayerPrefs.GetBool(EditorConstants.KEY_HIERARCHY_ICON);

         isLoaded = true;
      }

      /// <summary>Saves all changeable variables.</summary>
      public static void Save()
      {
         Crosstales.Common.Util.CTPlayerPrefs.SetBool(EditorConstants.KEY_UPDATE_CHECK, UPDATE_CHECK);
         Crosstales.Common.Util.CTPlayerPrefs.SetBool(EditorConstants.KEY_COMPILE_DEFINES, COMPILE_DEFINES);
         Crosstales.Common.Util.CTPlayerPrefs.SetBool(EditorConstants.KEY_PREFAB_AUTOLOAD, PREFAB_AUTOLOAD);
         Crosstales.Common.Util.CTPlayerPrefs.SetBool(EditorConstants.KEY_HIERARCHY_ICON, HIERARCHY_ICON);

         Crosstales.Common.Util.CTPlayerPrefs.Save();
      }

      #endregion
   }
}
#endif
// © 2017-2024 crosstales LLC (https://www.crosstales.com)