using System.Linq;
using UnityEngine;

namespace Crosstales.Common.Util
{
   /// <summary>Enables or disable game objects on Android or iOS in the background.</summary>
   [DisallowMultipleComponent]
   public class BackgroundController : MonoBehaviour
   {
      #region Variables

      ///<summary>Selected objects to disable in the background for the controller.</summary>
      [Tooltip("Selected objects to disable in the background for the controller.")] public GameObject[] Objects;

      private bool _isFocused;

      #endregion


      #region MonoBehaviour methods

#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR //|| CT_DEVELOP
      private void Start()
      {
         _isFocused = Application.isFocused;
      }

      private void FixedUpdate()
      {
         if (Application.isFocused != _isFocused)
         {
            _isFocused = Application.isFocused;

            if (BaseHelper.isMobilePlatform && !TouchScreenKeyboard.visible)
            {
               foreach (GameObject go in Objects.Where(go => go != null))
               {
                  go.SetActive(_isFocused);
               }

               if (BaseConstants.DEV_DEBUG)
                  Debug.Log($"Application.isFocused: {_isFocused}", this);
            }
         }
      }
#endif

      #endregion
   }
}
// © 2018-2024 crosstales LLC (https://www.crosstales.com)