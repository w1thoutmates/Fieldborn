using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleOutcomeManager : MonoBehaviour
{
    public static BattleOutcomeManager instance;

    public GameObject win_screen;
    public GameObject lose_screen;

    private void Awake()
    {
        if(instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void ShowWinScreen()
    {
        PauseManager.Pause();

        win_screen.SetActive(true);
    }

    public void ShowLoseScreen()
    {
        PauseManager.Pause();

        lose_screen.SetActive(true);
    }

    public void RertyButton()
    {
        PauseManager.instance = null;
        TurnManager.instance = null;
        Grid.instance = null;
        Player.instance = null;
        BattleOutcomeManager.instance = null;
        EventBus.Instance = null;
        
        PauseManager.Resume();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
