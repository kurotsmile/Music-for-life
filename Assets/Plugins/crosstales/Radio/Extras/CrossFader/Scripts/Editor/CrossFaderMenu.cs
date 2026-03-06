#if UNITY_EDITOR
using UnityEditor;
using Crosstales.Radio.EditorUtil;

namespace Crosstales.Radio.EditorIntegration
{
   /// <summary>Editor component for the "Tools"-menu.</summary>
   public static class CrossFaderMenu
   {
      [MenuItem("Tools/" + Crosstales.Radio.Util.Constants.ASSET_NAME + "/Prefabs/CrossFader", false, EditorHelper.MENU_ID + 170)]
      private static void AddCrossFader()
      {
         EditorHelper.InstantiatePrefab("CrossFader", $"{EditorConfig.ASSET_PATH}Extras/CrossFader/Resources/Prefabs/");
      }
   }
}
#endif
// © 2021-2024 crosstales LLC (https://www.crosstales.com)