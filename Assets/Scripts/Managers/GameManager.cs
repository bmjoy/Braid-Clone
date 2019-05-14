using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public static bool IsGamePaused => Time.deltaTime == 0f;

    protected override void Awake()
    {
        base.Awake();

        Time.timeScale = 1f;
    }

    public void ResumeTime()
    {
        Time.timeScale = 1f;      
    }

    public void PauseTime()
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



