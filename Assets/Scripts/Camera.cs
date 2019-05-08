﻿using UnityEngine;

namespace myCamera
{
    public class Camera : MonoBehaviour
    {
        private void Awake()
        {
            //Disables all unnecessary script icons in the Scene view.
#if UNITY_EDITOR
            CustomEditorUtilities.ToggleGizmos(false);
#endif
            
            Screen.SetResolution(1920, 1080, true); //Ensures the proper resolution is set when the game is built and run.
            Cursor.visible = false;
        }
    }
}