using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
public class CombatBoardManager : MonoBehaviour
{
    public List<Tile> board;
    public int width = 10;
    public int height = 5;
    public enum Pattern
    {
        Star = 0,
        Box = 1,
        Cone = 2
    }


    public Tile GetTile(int row, int col)
    {
        if (row >= height || col >= width || row < 0 || col < 0)
        {
            return null;
        }
        return board[col + height*row];
    }

    
    public List<Tile> GetNeighbours(int row, int col, Pattern pattern = Pattern.Box, int radius = 1)
    {
        var neighbours = new List<Tile>();
        for (int i = -1; i < 1; i++)
        {
            for (int j = -1; j < 1; j++)
            {
                if (pattern == Pattern.Box)
                {
                    
                }
            }
        }

        return neighbours;
    }
}