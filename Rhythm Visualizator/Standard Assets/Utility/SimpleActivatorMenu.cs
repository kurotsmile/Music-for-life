using System;
using UnityEngine;

namespace UnityStandardAssets.Utility
{
    public class SimpleActivatorMenu : MonoBehaviour
    {

        public GameObject[] objects;


        private int m_CurrentActiveObject;


        private void OnEnable()
        {
            // active object starts from first in array
            m_CurrentActiveObject = 0;
        }


        public void NextCamera()
        {
            int nextactiveobject = m_CurrentActiveObject + 1 >= objects.Length ? 0 : m_CurrentActiveObject + 1;

            for (int i = 0; i < objects.Length; i++)
            {
                objects[i].SetActive(i == nextactiveobject);
            }

            m_CurrentActiveObject = nextactiveobject;
        }
    }
}
