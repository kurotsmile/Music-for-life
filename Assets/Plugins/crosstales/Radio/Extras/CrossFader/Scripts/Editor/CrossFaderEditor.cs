#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Crosstales.Radio.EditorUtil;

namespace Crosstales.Radio.EditorExtension
{
   /// <summary>Custom editor for the 'CrossFader'-class.</summary>
   [CustomEditor(typeof(Crosstales.Radio.Tool.CrossFader))]
   public class CrossFaderEditor : Editor
   {
      #region Variables

      private Crosstales.Radio.Tool.CrossFader script;

      #endregion


      #region Editor methods

      private void OnEnable()
      {
         script = (Crosstales.Radio.Tool.CrossFader)target;
      }

      public override void OnInspectorGUI()
      {
         DrawDefaultInspector();

         script.FaderPosition = EditorGUILayout.Slider(new GUIContent("Fader Position", "The current fader position in percent (-/+)."), script.FaderPosition, -1f, 1f);

         if (script.isActiveAndEnabled)
         {
            if (script.SourceA != null && script.SourceB != null)
            {
               if (script.SourceA != script.SourceB)
               {
                  //add stuff if needed
               }
               else
               {
                  EditorHelper.SeparatorUI();
                  EditorGUILayout.HelpBox("'SourceA' is equals 'SourceB'! Please add two different sources.", MessageType.Warning);
               }
            }
            else
            {
               EditorHelper.SeparatorUI();
               EditorGUILayout.HelpBox("Please add 'SourceA' and 'SourceB'!", MessageType.Warning);
            }
         }
         else
         {
            EditorHelper.SeparatorUI();
            EditorGUILayout.HelpBox("Script is disabled!", MessageType.Info);
         }
      }

      public override bool RequiresConstantRepaint()
      {
         return true;
      }

      #endregion
   }
}
#endif
// © 2017-2024 crosstales LLC (https://www.crosstales.com)