﻿using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField]
    private GameObject _menuBackground, _audioMenuUI, _pauseMenuUI;

    [SerializeField]
    private Sprite _killedEnemyIcon;

    [SerializeField]
    private GameObject[] _monstarIcons;

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            StateUpdate();
            AudioManager.Instance.Play(Sound.MENU);
        }
    }

    public void UpdateEnemyDeathCount(int index)
    {
        _monstarIcons[index].GetComponent<Image>().sprite = _killedEnemyIcon;
    }

    private void StateUpdate()
    {
        switch (GetCurrentState)
        {
            case UIState.GAME:
                {
                    PauseGame();
                    break;
                }
            case UIState.MAIN_MENU:
                {
                    ResumeGame();
                    break;
                }
            case UIState.AUDIO:
                {
                    _audioMenuUI.SetActive(false);
                    _pauseMenuUI.SetActive(true);
                    break;
                }
        }
    }

    private void PauseGame()
    {
        _menuBackground.SetActive(true);
        _pauseMenuUI.SetActive(true);
        _audioMenuUI.SetActive(false);
        GameManager.Instance.PauseTime();
        AudioManager.Instance.PauseMusic();
    }

    public void ResumeGame()
    {
        _menuBackground.SetActive(false);
        _audioMenuUI.SetActive(false);
        _pauseMenuUI.SetActive(false);
        GameManager.Instance.ResumeTime();
        AudioManager.Instance.ResumeMusic();
    }

    public enum UIState
    {
        MAIN_MENU,
        AUDIO,
        GAME,
    }

    public UIState GetCurrentState
    {
        get
        {
            if (_pauseMenuUI.activeSelf)
                return UIState.MAIN_MENU;

            else if (_audioMenuUI.activeSelf)
                return UIState.AUDIO;

            else return UIState.GAME;
        }
    }

}
