using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    private Text _Text;
    private float _time, _lastTime;

    void Start()
    {
        _Text = GetComponent<Text>();
    }

    void Update()
    {
        _time += Time.deltaTime;
        if (_time > _lastTime)
        {
            _Text.text = string.Format("{0} fps    {1} ms", (int)(1.0f / Time.deltaTime), Time.deltaTime * 1000.0f);
            _lastTime = _time + 1.0f;
        }
    }
}
