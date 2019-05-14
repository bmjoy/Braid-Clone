using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class SetAsFirstSelected : MonoBehaviour
{

    private void OnEnable()
    {
        StartCoroutine(SelectButton());
    }

    /// <summary>
    /// Set the button as being currently selected.
    /// </summary>
    /// <returns></returns>
    private IEnumerator SelectButton()
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return null;
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

}
