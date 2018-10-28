using UnityEngine;
using UnityEngine.EventSystems;

//The only purpose of this script is to make it so that if the top button of the menu is selected and the user pressed the 'up' key,
//then the bottom button is selected, and if the bottom key is selected and the user presses the 'down' key, the top button is selected, that's all.
public class UINavigation : MonoBehaviour
{
    private int _index;
    private bool _axisInUse;

    [SerializeField]
    private GameObject[] _buttons;

    private void OnEnable()
    {
        _index = 0;   
    }

    private void Start()
    {
        _index = Mathf.Clamp(_index, 0, _buttons.Length);
        _axisInUse = false;
    }

    private void Update()
    {
        FindCurrentButton();
        SelectCurrentButton();  
    }

    private void FindCurrentButton()
    {
        float vertical = Input.GetAxisRaw("Vertical");

        if (vertical < 0f)
        {
            if (!_axisInUse)
            {
                _index = (_index + 1) % _buttons.Length;
                _axisInUse = true;
            }
        }
        else if (vertical > 0f)
        {
            if (!_axisInUse)
            {
                _index--;

                if (_index < 0)
                    _index = _buttons.Length - 1;

                _axisInUse = true;
            }
        }
        else
        {
            _axisInUse = false;
        }
    }

    private void SelectCurrentButton()
    {
        EventSystem.current.SetSelectedGameObject(_buttons[_index]);
    }

}
