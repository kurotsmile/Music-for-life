using System.Linq;
using UnityEngine;

namespace Crosstales.Common.Util
{
   /// <summary>Allows any Unity gameobject to survive a scene switch. This is especially useful to keep the music playing while loading a new scene.</summary>
   [DisallowMultipleComponent]
   public class SurviveSceneSwitch : Crosstales.Common.Util.Singleton<SurviveSceneSwitch>
   {
      #region Variables

      ///<summary>Objects which have to survive a scene switch.</summary>
      [Tooltip("Objects which have to survive a scene switch.")] public GameObject[] Survivors; //any object, like a RadioPlayer

      private Transform _tf;
      private float _ensureParentTimer;

      private const float ENSURE_PARENT_TIME = 1.5f;

      #endregion


      #region MonoBehaviour methods

      private void Start()
      {
         _ensureParentTimer = ENSURE_PARENT_TIME;
         _tf = transform;
      }

      private void Update()
      {
         _ensureParentTimer += Time.deltaTime;

         if (Survivors != null && _ensureParentTimer > ENSURE_PARENT_TIME)
         {
            _ensureParentTimer = 0f;

            foreach (GameObject _go in Survivors.Where(_go => _go != null))
            {
               _go.transform.SetParent(_tf);
            }
         }
      }

      #endregion
   }
}
// © 2016-2024 crosstales LLC (https://www.crosstales.com)