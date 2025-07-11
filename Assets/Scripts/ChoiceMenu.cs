using System;
using UnityEngine;

public class ChoiceMenu : MonoBehaviour
{
    private Player p;
    public event Action OnChoiceMade;
    
    private void Awake()
    {
        p = Player.instance;
    }

    public void IncDamageCounter() { 
        p.damage_counter += p.current_level;
        p.UpdateUI();
        OnChoiceMade?.Invoke();
    }

    public void IncHealCounter() 
    { 
        p.heal_counter += p.current_level;
        p.UpdateUI(); 
        OnChoiceMade?.Invoke();
    }

    public void IncShieldCounter() 
    { 
        p.shield_counter += p.current_level; 
        p.UpdateUI();
        OnChoiceMade?.Invoke();
    }
}
