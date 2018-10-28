using UnityEngine;
using System;
using System.Collections;
using XInputDotNetPure;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField]
    private GameObject _deathPrefab, _monstarIcons, _gate, _puzzleOutline, _puzzlePiece;

    [SerializeField]
    private Sprite _darkened;

    private int _enemyDeathCount, _totalNumOfEnemies;

    public Func<IEnumerator> OnAllEnemiesKilled;

    protected override void Awake()
    {
        base.Awake();

    }

    private void Start()
    {
        _totalNumOfEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
    }

    public void Kill(GameObject player)
    {
        Destroy(player);
        GameObject go = Instantiate(_deathPrefab, player.transform.position, Quaternion.identity);
        Destroy(go, 2f);
        StartCoroutine(Rumble());
        AudioManager.Instance.Play("playerHurt");
    }

    public int EnemyDeathCount
    {
        get
        {
            return _enemyDeathCount;
        }
        set
        {
            _monstarIcons.transform.GetChild(value - 1).gameObject.GetComponent<SpriteRenderer>().sprite = _darkened;
            _monstarIcons.transform.GetChild(value - 1).transform.GetChild(0).gameObject.SetActive(true);
            _gate.transform.GetChild(value - 1).gameObject.SetActive(true);
            _gate.transform.GetChild(value - 1).transform.GetChild(0).gameObject.SetActive(true);

            _enemyDeathCount = value;

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
