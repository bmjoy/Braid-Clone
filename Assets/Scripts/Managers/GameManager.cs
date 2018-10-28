using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{

    public bool GameIsPaused { get { return Time.deltaTime == 0f; } }

    protected override void Awake()
    {
        base.Awake();

        Time.timeScale = 1f;
    }

    public void Resume()
    {
        Time.timeScale = 1f;      
    }

    public void Pause()
    {
        Time.timeScale = 0f;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit()
    {
        Application.Quit();
    }

}



