using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonBehavior : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    ColorChanger _colorChanger;

    private void Awake()
    {
        _colorChanger = transform.GetChild(0).GetComponent<ColorChanger>();
    }

    private void OnDisable()
    {
        _colorChanger.IsCurrentlyActive = false;
    }

    public void OnSelect(BaseEventData eventData)
    {
        _colorChanger.IsCurrentlyActive = true;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        _colorChanger.IsCurrentlyActive = false;
    }

 
}
   
