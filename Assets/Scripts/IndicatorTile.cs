using System;
using UnityEngine;

public class IndicatorTile : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer; 
    [SerializeField] private Sprite tileDefault;
    [SerializeField] private Sprite tileWithinReach;
    [SerializeField] private Sprite tileChosenPath;
    [SerializeField] private Sprite tileEnemy;
    [SerializeField] private Sprite tileSelectedEnemy;
    [SerializeField] private Sprite tileAlly;
    
    public enum Indicator
    {
        WithinReach,
        ChosenPath,
        Enemy,
        SelectedEnemy,
        Ally,
        Default
    }
    public void SetIndicator(Indicator indicator)
    {
        spriteRenderer.sprite = indicator switch
        {
            Indicator.WithinReach => tileWithinReach,
            Indicator.ChosenPath => tileChosenPath,
            Indicator.Enemy => tileEnemy,
            Indicator.SelectedEnemy => tileSelectedEnemy,
            Indicator.Ally => tileAlly,
            Indicator.Default => tileDefault,
            _ => throw new ArgumentOutOfRangeException(nameof(indicator), indicator, null)
        };
    }
}