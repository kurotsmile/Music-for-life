#if UNITY_EDITOR
using UnityEditor;
using Crosstales.Radio.EditorUtil;

namespace Crosstales.Radio.EditorIntegration
{
   /// <summary>Editor component for the "Tools"-menu.</summary>
   public static class StreamSaverMenu
   {
      [MenuItem("Tools/" + Util.Constants.ASSET_NAME + "/Prefabs/StreamSaver", false, EditorHelper.MENU_ID + 160)]
      private static void AddStreamSaver()
      {
         EditorHelper.InstantiatePrefab("StreamSaver", $"{EditorConfig.ASSET_PATH}Extras/StreamSaver/Resources/Prefabs/");
      }
   }
}
#endif
// © 2021 crosstales LLC (https://www.crosstales.com)