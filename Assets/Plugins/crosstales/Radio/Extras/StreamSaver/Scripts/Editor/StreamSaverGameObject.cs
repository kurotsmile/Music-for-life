#if UNITY_EDITOR
using UnityEditor;
using Crosstales.Radio.EditorUtil;

namespace Crosstales.Radio.EditorIntegration
{
   /// <summary>Editor component for the "Hierarchy"-menu.</summary>
   public static class StreamSaverGameObject
   {
      [MenuItem("GameObject/" + Util.Constants.ASSET_NAME + "/StreamSaver", false, EditorHelper.GO_ID + 9)]
      private static void AddStreamSaver()
      {
         EditorHelper.InstantiatePrefab("StreamSaver", $"{EditorConfig.ASSET_PATH}Extras/StreamSaver/Resources/Prefabs/");
      }
   }
}
#endif
// © 2021 crosstales LLC (https://www.crosstales.com)