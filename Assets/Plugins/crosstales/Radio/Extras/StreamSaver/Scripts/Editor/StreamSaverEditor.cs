#if UNITY_EDITOR
using UnityEditor;
using Crosstales.Radio.EditorUtil;

namespace Crosstales.Radio.EditorExtension
{
   /// <summary>Custom editor for the 'StreamSaver'-class.</summary>
   [CustomEditor(typeof(Crosstales.Radio.Tool.StreamSaver))]
   public class StreamSaverEditor : Editor
   {
      #region Variables

      private Crosstales.Radio.Tool.StreamSaver script;

      private static readonly string musicCopyright = "Copyright laws for music are VERY STRICT and MUST BE respected!" + System.Environment.NewLine + "If you save music, make sure YOU have the RIGHT to do so! " + System.Environment.NewLine + "crosstales LLC denies any responsibility for YOUR actions with this tool - use it at your OWN RISK!" + System.Environment.NewLine + System.Environment.NewLine + "For more, see 'https://en.wikipedia.org/wiki/Radio_music_ripping' and the rights applying to your country.";

      #endregion


      #region Editor methods

      private void OnEnable()
      {
         script = (Crosstales.Radio.Tool.StreamSaver)target;
      }

      public override void OnInspectorGUI()
      {
         DrawDefaultInspector();

         if (script.isActiveAndEnabled)
         {
            if (script.Player != null)
            {
               if (string.IsNullOrEmpty(script.OutputPath) || !Crosstales.Common.Util.FileHelper.ExistsDirectory(script.OutputPath))
               {
                  EditorHelper.SeparatorUI();
                  EditorGUILayout.HelpBox("Please add a valid 'Output Path' for the saved audio records!", MessageType.Warning);
               }
               /*
               else
               {
                  //add stuff if needed
               }
               */
            }
            else
            {
               EditorHelper.SeparatorUI();
               EditorGUILayout.HelpBox("Please add a 'Player'!", MessageType.Warning);
            }

            EditorGUILayout.HelpBox(musicCopyright, MessageType.Error);
         }
         else
         {
            EditorHelper.SeparatorUI();
            EditorGUILayout.HelpBox("Script is disabled!", MessageType.Info);
         }
      }

      #endregion
   }
}
#endif
// © 2017-2024 crosstales LLC (https://www.crosstales.com)