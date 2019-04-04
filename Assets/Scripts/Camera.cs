using UnityEngine;

namespace myCamera
{
    public class Camera : MonoBehaviour
    {
        private void Awake()
        {
            //Disable all unnecessary script icons in the Scene view.
#if UNITY_EDITOR
            CustomEditorUtilities.ToggleGizmos(false);
#endif
            //Ensure the proper resolution is set when the game is built and run.
            Screen.SetResolution(1920, 1080, true);

            Cursor.visible = false;
        }
    }
}