using System;
using UnityEngine;
using UnityEngine.Rendering;

public class EventBus : MonoBehaviour
{
    public static EventBus Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public Action<int> playerTakenDamage;
    public Action<int> enemyTakenDamage;
}
