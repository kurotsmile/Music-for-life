using UnityEngine;

namespace Crosstales.Radio.Tool
{
   /// <summary>Cross fade two AudioSource.</summary>
   [ExecuteInEditMode]
   [HelpURL("https://www.crosstales.com/media/data/assets/radio/api/class_crosstales_1_1_radio_1_1_tool_1_1_cross_fader.html")]
   public class CrossFader : MonoBehaviour
   {
      #region Variables

      /// <summary>Audio source A (e.g. left) to fade.</summary>
      [Tooltip("Audio source A (e.g. left) to fade.")] public AudioSource SourceA;

      /// <summary>Audio source B (e.g. right) to fade.</summary>
      [Tooltip("Audio source B (e.g. right) to fade.")] public AudioSource SourceB;

      private float volumeA = 1f;
      private float volumeB = 1f;

      #endregion


      #region Properties

      /// <summary>The current fader position in percent (-/+).</summary>
      public float FaderPosition
      {
         get
         {
            float positionA = (SourceA != null ? SourceA.volume : 1f) / volumeA;
            float positionB = (SourceB != null ? SourceB.volume : 1f) / volumeB;
            float resultA = 0f;
            float resultB = 0f;

            if (positionA < 1f)
            {
               resultA = 1f - positionA;
            }

            if (positionB < 1f)
            {
               resultB = -(1f - positionB);
            }

            if (resultA > 0 && !(resultB < 0))
            {
               return resultA;
            }

            if (!(resultA > 0) && resultB < 0)
            {
               return resultB;
            }

            return 0f;
         }

         set
         {
            float tempValue = Mathf.Clamp(value, -1f, 1f);

            if (tempValue > 0f)
            {
               float positionA = -(tempValue - 1f);

               if (SourceA != null)
                  SourceA.volume = volumeA * positionA;
               if (SourceB != null)
                  SourceB.volume = volumeB;
            }
            else if (tempValue < 0f)
            {
               float positionB = 1f - -tempValue;

               if (SourceA != null)
                  SourceA.volume = volumeA;
               if (SourceB != null)
                  SourceB.volume = volumeB * positionB;
            }
            else
            {
               if (SourceA != null)
                  SourceA.volume = volumeA;
               if (SourceB != null)
                  SourceB.volume = volumeB;
            }
         }
      }

      #endregion


      #region MonoBehaviour methods

      private void Start()
      {
         if (SourceA != null && SourceB != null && SourceA != SourceB)
         {
            volumeA = SourceA.volume;
            volumeB = SourceB.volume;
         }
         else
         {
            if (!Crosstales.Radio.Util.Helper.isEditorMode)
            {
               Debug.LogError("'SourceA' or 'SourceB' are null or equals!", this);
            }
         }
      }

      #endregion
   }
}
// © 2017-2024 crosstales LLC (https://www.crosstales.com)