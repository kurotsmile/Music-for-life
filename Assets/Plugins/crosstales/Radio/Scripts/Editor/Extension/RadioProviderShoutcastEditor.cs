#if UNITY_EDITOR
using UnityEditor;
using Crosstales.Radio.EditorUtil;
using Crosstales.Radio.Provider;

namespace Crosstales.Radio.EditorExtension
{
   /// <summary>Custom editor for the 'RadioProviderShoutcast'-class.</summary>
   [CustomEditor(typeof(RadioProviderShoutcast))]
   public class RadioProviderShoutcastEditor : BaseRadioProviderEditor
   {
      #region Variables

      private RadioProviderShoutcast _script;

      #endregion


      #region Editor methods

      protected override void OnEnable()
      {
         base.OnEnable();
         _script = (RadioProviderShoutcast)target;
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