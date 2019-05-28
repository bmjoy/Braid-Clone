using UnityEngine;

public class ParticleSystemAutoDestroy : MonoBehaviour
{
    private ParticleSystem _ps;

    private void Start()
    {
        _ps = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        DestroyOnCompletion();

        void DestroyOnCompletion()
        {
            if (!_ps.IsAlive())
            {
                Destroy(gameObject);
            }
        }
    }

}