using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField]
    private GameObject _deathPrefab, _gate, _gateMarkPrefab, _smokeParticlePrefab, _flashingPuzzleOutline, _puzzlePiece;

    [SerializeField]
    private Transform[] _gateCells;

    private int _enemyDeathCount, _totalNumOfEnemies;

    public event Func<IEnumerator> OnAllEnemiesKilled;

    protected override void Awake()
    {
        base.Awake();

        Player.OnDeath += DeathSequence;
    }

    private void OnDestroy()
    {
        Player.OnDeath -= DeathSequence;
    }

    private void Start()
    {
        _totalNumOfEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
    }

    private void DeathSequence(Transform player)
    {
        var go = Instantiate(_deathPrefab, player.position, Quaternion.identity);
        Destroy(go, 2f);
        StartCoroutine(Rumble());
    }

    public int EnemyDeathCount
    {
        get => _enemyDeathCount;

        set
        {
            _enemyDeathCount = value;

            UIManager.Instance.UpdateEnemyDeathCount(_enemyDeathCount - 1);
            Instantiate(_gateMarkPrefab, _gateCells[_enemyDeathCount - 1]);
            Instantiate(_smokeParticlePrefab, _gateCells[_enemyDeathCount - 1]);

            if (_enemyDeathCount == _totalNumOfEnemies)
            {
                if (OnAllEnemiesKilled != null)
                {
                    StartCoroutine(OnAllEnemiesKilled());
                }
            }
            AudioManager.Instance.Play(Sound.MONSTAR_HURT);
        }
    }

    public void CapturePuzzle()
    {
        _flashingPuzzleOutline.SetActive(true);
        _puzzlePiece.SetActive(true);
        AudioManager.Instance.Play(Sound.PUZZLE_CAPTURED);
    }

    private IEnumerator Rumble()
    {
        GamePad.SetVibration(PlayerIndex.One, .2f, .2f);
        yield return new WaitForSeconds(.5f);
        GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
    }

}
