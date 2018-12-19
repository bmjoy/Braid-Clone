using System;
using System.Collections;
using UnityEngine;
using XInputDotNetPure;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField]
    private GameObject _deathPrefab, _monstarIcons, _gate, _puzzleOutline, _puzzlePiece;

    [SerializeField]
    private Sprite _darkened;

    private int _enemyDeathCount, _totalNumOfEnemies;

    /// <summary>
    /// Notice how I don't declare the Func as static. This is because it is already inside of a singleton - which itself contains a static reference to all public fields,
    /// properties and functions inside of the class.
    /// </summary>
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
        GameObject go = Instantiate(_deathPrefab, player.position, Quaternion.identity);
        Destroy(go, 2f);
        StartCoroutine(Rumble());
    }

    public int EnemyDeathCount
    {
        get
        {
            return _enemyDeathCount;
        }
        set
        {
            _enemyDeathCount = value;

            _monstarIcons.transform.GetChild(_enemyDeathCount - 1).gameObject.GetComponent<SpriteRenderer>().sprite = _darkened;
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
            AudioManager.Instance.Play("monstarHurt");
        }
    }

    public void CapturePuzzle()
    {
        _puzzleOutline.SetActive(true);
        _puzzlePiece.SetActive(true);
        AudioManager.Instance.Play("puzzleCaptured");
    }

    private IEnumerator Rumble()
    {
        GamePad.SetVibration(PlayerIndex.One, .2f, .2f);
        yield return new WaitForSeconds(.5f);
        GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
    }

}
