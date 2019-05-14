using TMPro;
using UnityEngine;

public class ColorChanger : MonoBehaviour
{   
    private TextMeshProUGUI _textMeshPro;
    private Color _myColor, _startColor, _targetColor;

    public bool IsCurrentlyActive { get; set; }

    private void Start()
    {
        _textMeshPro = GetComponent<TextMeshProUGUI>();
        _startColor = new Color32(215, 215, 215, 255);
    }

    private void Update()
    {
        ChangeColor();
    }

    private void ChangeColor()
    {
        if (IsCurrentlyActive)
        {
            var colorBlendValue = .7f;
            _myColor = Color.Lerp(_startColor, Color.red, Mathf.PingPong(Time.unscaledTime, colorBlendValue));
            _textMeshPro.color = _myColor;
        }
        else
            _textMeshPro.color = Color.black;
    }
}
