#if UNITY_EDITOR
using UnityEditor;
using Crosstales.Radio.EditorUtil;

namespace Crosstales.Radio.EditorIntegration
{
   /// <summary>Editor component for the "Hierarchy"-menu.</summary>
   public static class CrossFaderGameObject
   {
      [MenuItem("GameObject/" + Crosstales.Radio.Util.Constants.ASSET_NAME + "/CrossFader", false, EditorHelper.GO_ID + 10)]
      private static void AddCrossFader()
      {
         EditorHelper.InstantiatePrefab("CrossFader", $"{EditorConfig.ASSET_PATH}Extras/CrossFader/Resources/Prefabs/");
      }
   }
}
#endif
// © 2021-2024 crosstales LLC (https://www.crosstales.com)