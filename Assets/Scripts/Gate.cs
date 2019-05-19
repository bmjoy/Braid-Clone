using System.Collections;
using UnityEngine;

public class Gate : MonoBehaviour
{
    [SerializeField]
    private float _speed;

    [SerializeField]
    private Transform _endpoint;

    private void Start()
    {
        LevelManager.Instance.OnAllEnemiesKilled += OpenGate;
    }

    private IEnumerator OpenGate()
    {
        while (transform.localPosition.y >= _endpoint.localPosition.y)
        {
            yield return null;
            transform.Translate(Vector2.down * (_speed * Time.deltaTime));
            AudioManager.Instance.Play(Sound.GATE_OPEN);
        }
        Destroy(transform.parent.gameObject);
    }

    private void OnDestroy()
    {
        LevelManager.Instance.OnAllEnemiesKilled -= OpenGate;
    }

}