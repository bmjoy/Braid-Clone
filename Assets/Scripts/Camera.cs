using UnityEngine;

namespace myCamera
{
    public class Camera : MonoBehaviour
    {
        private void Awake()
        {
            //This makes sure that all of the annoying script icons in the Scene view are disabled (I have a lot of script icons ;)).
#if UNITY_EDITOR
            CustomEditorUtilities.ToggleGizmos(false);
#endif
            //This will ensure the proper resolution when the game is built and run.
            Screen.SetResolution(1920, 1080, true);

            Cursor.visible = false;
        }
    }
}