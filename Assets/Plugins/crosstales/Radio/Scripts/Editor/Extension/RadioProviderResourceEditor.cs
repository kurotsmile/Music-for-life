#if UNITY_EDITOR
using UnityEditor;
using Crosstales.Radio.EditorUtil;
using Crosstales.Radio.Provider;

namespace Crosstales.Radio.EditorExtension
{
   /// <summary>Custom editor for the 'RadioProviderResource'-class.</summary>
   [CustomEditor(typeof(RadioProviderResource))]
   public class RadioProviderResourceEditor : BaseRadioProviderEditor
   {
      #region Variables

      private RadioProviderResource _script;

      #endregion


      #region Editor methods

      protected override void OnEnable()
      {
         base.OnEnable();
         _script = (RadioProviderResource)target;
      }

      public override void OnInspectorGUI()
      {
         DrawDefaultInspector();

         EditorHelper.SeparatorUI();

         if (_script.isActiveAndEnabled)
         {
            if (_script.Entries?.Count > 0)
            {
               showDataUI();
            }
            else
            {
               EditorGUILayout.HelpBox("Please add 'Entries'!", MessageType.Warning);
            }
         }
         else
         {
            EditorGUILayout.HelpBox("Script is disabled!", MessageType.Info);
         }
      }

      #endregion
   }
}
#endif
// © 2016-2024 crosstales LLC (https://www.crosstales.com)