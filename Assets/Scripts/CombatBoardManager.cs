﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class CombatBoardManager : MonoBehaviour
{
    public Tilemap _tileMap;
    public List<Tile> board;
    public Vector2Int _tileMapDimensions;
    public IndicatorTile _TilePrefab;
    public Pathfinder Pathfinder;

    public static Vector3Int
        DEG0 = new(1, 0, 0),
        DEG60_EVEN = new(1, 1, 0),
        DEG60_ODD = new(0, 1, 0),
        DEG120_EVEN = new(0, 1, 0),
        DEG120_ODD = new(-1, 1, 0),
        DEG180 = new(-1, 0, 0),
        DEG240_EVEN = new(0, -1, 0),
        DEG240_ODD = new(-1, -1, 0),
        DEG300_EVEN = new(1, -1, 0),
        DEG300_ODD = new(0, -1, 0);


    public static Vector3Int[] oddDirs = 
        {DEG0, DEG60_ODD, DEG120_ODD, DEG180, DEG240_ODD, DEG300_ODD};
    public static Vector3Int[] evenDirs = 
        {DEG0, DEG60_EVEN, DEG120_EVEN, DEG180, DEG240_EVEN, DEG300_EVEN};



    private void Start()
    {
        CreateBoard();
    }
    
    private void CreateBoard()
    {
        _tileMap = GetComponentInChildren<Tilemap>();
        _tileMap.CompressBounds();
        _tileMapDimensions = new Vector2Int(_tileMap.size.x, _tileMap.size.y);
        board = new List<Tile>(_tileMapDimensions.x*_tileMapDimensions.y);
        _tileMap.origin.Set(Mathf.CeilToInt(_tileMapDimensions.x/2f)+1,Mathf.CeilToInt(_tileMapDimensions.y/2f)+1,0);
        //Make the pathfinding tiles

        for (int y = 0; y < _tileMapDimensions.y; y++)
        {
            for (int x = 0; x < _tileMapDimensions.x; x++)
            {
                var walkable = !(x == _tileMapDimensions.x-1 && y % 2 == 1);
                var tileGO = Instantiate(_TilePrefab.gameObject, _tileMap.CellToWorld(new Vector3Int(x, y, 0)), quaternion.identity,gameObject.transform);
                tileGO.name = $"Tile,{x},{y}";
                board.Add(new Tile(new Vector3Int(x,y,0),true,tileGO.GetComponent<IndicatorTile>()));
            }
        }
        Pathfinder = new Pathfinder(board, _tileMapDimensions);
        
    }
    
    public Tile GetTile(Vector3Int cellPos)
    {
        return InBounds(cellPos) ? null : board[cellPos.y + _tileMapDimensions.y*cellPos.x];
    }

    public bool InBounds(Vector3Int gridPos)
    {
        if (gridPos.x == _tileMapDimensions.x-1 && gridPos.y % 2 == 1)
        {
            return false;
        }
        return gridPos.x > -1 && gridPos.y > -1 && gridPos.x < _tileMapDimensions.x && gridPos.y < _tileMapDimensions.y;
    }
    
    public int Index(Vector3Int pos)
    {
        return pos.x + pos.y * _tileMapDimensions.x;
    }
}