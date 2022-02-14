using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    //The current turn number
    public int TurnNumber { get; private set; } = 1;
    
    //The TMP displaying the current turn number;
    public TextMeshProUGUI TurnNumberHolder;
    
    public static event Action OnTurnStart = delegate {  };
    public static event Action OnTurnEnd = delegate{  };
    
    private void StartTurn()
    {
        IncrementTurnText();
    }
    
    private void EndTurn()
    {
        
    }
    
    private void IncrementTurnText()
    {
        TurnNumber += 1;
        TurnNumberHolder.text = TurnNumber.ToString();
        TurnNumberHolder.ForceMeshUpdate();
    }

    private void OnEnable()
    {
        OnTurnStart += StartTurn;
        OnTurnEnd += EndTurn;
    }
    private void OnDestroy()
    {
        OnTurnStart -= StartTurn;
        OnTurnEnd -= EndTurn;
    }
    private void OnDisable()
    {
        OnTurnStart -= StartTurn;
        OnTurnEnd -= EndTurn;
    }
}
