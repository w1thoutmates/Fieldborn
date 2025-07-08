using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;
    public Button endTunrBtn;

    private int currentTurn = 1; // 1 - 10 turns
                                 // (currentTurn % 2 == 0) - enemy turn
                                 // (currentTurn % 2 != 0) - player turn
    private int turnsPerRound = 10;
    private int currentRound = 1;
    private Base_enemy enemy;
    private ShapeStorage shapeStorage;

    [Header("UI")]
    public TextMeshProUGUI round_text;
    public TextMeshProUGUI turn_text;

    [HideInInspector]public bool IsPlayerTurn() => currentTurn % 2 == 1;
    [HideInInspector]public bool IsEnemyTurn() => currentTurn % 2 == 0;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else 
            Destroy(gameObject);

        enemy = FindObjectOfType<Base_enemy>();
        shapeStorage = FindObjectOfType<ShapeStorage>().GetComponent<ShapeStorage>();

        turn_text.text = $"Ход игрока {GetPlayerTurnNumber()}";
        turn_text.color = new Color(149f / 255f, 255f / 255f, 140f / 255f, 1f);
        round_text.text = "Раунд " + currentRound; 
    }

    private void Start()
    {
        
    }

    public void StartNewRound()
    {
        if (currentRound % 2 == 0)
        {
            Grid.instance.ClearBoard();
        }

        Debug.Log("<color=cyan>НОВЫЙ РАУНД</color>");
        currentTurn = 1;
        StartCurrentTurn();
    }

    public void EndTurn()
    {
        endTunrBtn.gameObject.SetActive(false);

        currentTurn++;
        
        if(currentTurn > turnsPerRound)
        {
            StartCoroutine(EndRound());
        }
        else
        {
            StartCurrentTurn();
        }
    }

    private void StartCurrentTurn()
    {
        if(IsPlayerTurn())
        {
            turn_text.text = $"Ход игрока {GetPlayerTurnNumber()}";
            turn_text.color = new Color(149f / 255f, 255f / 255f, 140f / 255f, 1f);
            PlayerTurnStart();
        }
        else if (IsEnemyTurn())
        {
            turn_text.text = $"Ход врага {GetEnemyTurnNumber()}";
            turn_text.color = new Color(255f / 255f, 91f / 255f, 76f / 255f, 1f);
            EnemyTurnStart();
        }
    }

    private void PlayerTurnStart()
    {
        Player.instance.StartTurn();
        shapeStorage.RequestNewShapes();
        endTunrBtn.gameObject.SetActive(true);
        Player.instance.TurnActions();  
    }

    private void EnemyTurnStart()
    {
        enemy.TurnActions();
    }

    private IEnumerator EndRound()
    {
        GameObject shapes_on_scene = GameObject.Find("Shapes");
        shapes_on_scene.gameObject.SetActive(false);

        Debug.Log("<color=yellow>КОНЕЦ РАУНДА</color>");
        yield return new WaitForSeconds(1f);

        // player attack
        yield return StartCoroutine(Player.instance.Attack(enemy));

        // player shield 
        yield return StartCoroutine(Player.instance.CastShield());

        // enemy attack
        yield return StartCoroutine(enemy.Attack());

        // player heal
        yield return StartCoroutine(Player.instance.HealSelf());

        // temp
        Player.instance.shield_counter = 0;
        Player.instance.UpdateUI();

        yield return new WaitForSeconds(0.1f);

        StartNewRound();
        shapes_on_scene.gameObject.SetActive(true);
        currentRound++;
        round_text.text = "Раунд " + currentRound;
    }

    private int GetPlayerTurnNumber() => (currentTurn + 1) / 2;
    private int GetEnemyTurnNumber() => currentTurn / 2;
}
