using System;
using System.Collections;
using UnityEngine;
using XInputDotNetPure;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField]
    private GameObject _deathPrefab, _monstarIcons, _gate, _puzzleOutline, _puzzlePiece;

    [SerializeField]
    private Sprite _darkenedEnemyIcon;

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

            _monstarIcons.transform.GetChild(_enemyDeathCount - 1).gameObject.GetComponent<SpriteRenderer>().sprite = _darkenedEnemyIcon;
            _monstarIcons.transform.GetChild(_enemyDeathCount - 1).transform.GetChild(0).gameObject.SetActive(true);
            _gate.transform.GetChild(_enemyDeathCount - 1).gameObject.SetActive(true);
            _gate.transform.GetChild(_enemyDeathCount - 1).transform.GetChild(0).gameObject.SetActive(true);

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
        _puzzleOutline.SetActive(true);
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
