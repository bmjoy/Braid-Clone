using UnityEngine;

public class Cam : MonoBehaviour
{
    private void Awake()
    {       
        Screen.SetResolution(1920, 1080, true);
        Cursor.visible = false;
    }

}
