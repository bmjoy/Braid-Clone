using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonBehavior : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public void OnSelect(BaseEventData eventData)
    {
        transform.GetChild(0).GetComponent<ColorChanger>().CurrentlyActive = true;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        transform.GetChild(0).GetComponent<ColorChanger>().CurrentlyActive = false;
    }

 
}
   
