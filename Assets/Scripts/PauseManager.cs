using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager instance;

    public static bool isPaused = false;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(gameObject);
    }
    
    public static void Pause()
    {
        isPaused = true;
    }

    public static void Resume()
    {
        isPaused = false;
    }

    public static void TogglePause()
    {
        isPaused = !isPaused;
    }
}
