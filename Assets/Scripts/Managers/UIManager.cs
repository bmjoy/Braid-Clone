using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [SerializeField]
    private GameObject _menuBackground, _audioMenuUI, _pauseMenuUI;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            StateUpdate();
            AudioManager.Instance.Play("menu");
        }
    }

    private void StateUpdate()
    {
        switch (CurrentState)
        {
            case UIState.GAME:
                {
                    _menuBackground.SetActive(true);
                    _pauseMenuUI.SetActive(true);
                    _audioMenuUI.SetActive(false);
                    GameManager.Instance.Pause();
                    AudioManager.Instance.PauseMusic();                   
                    break;
                }
            case UIState.MAIN_MENU:
                {
                    _menuBackground.SetActive(false);
                    _audioMenuUI.SetActive(false);
                    _pauseMenuUI.SetActive(false);
                    GameManager.Instance.Resume();
                    AudioManager.Instance.ResumeMusic();
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

    public enum UIState
    {
        MAIN_MENU,
        AUDIO,
        GAME,
    }

    public UIState CurrentState
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
