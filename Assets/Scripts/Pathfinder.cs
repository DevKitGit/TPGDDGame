using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Mathematics;
using UnityEngine;


public class Tile
{
    
    // Change this depending on what the desired size is for each element in the grid
    public IndicatorTile IndicatorTile;
    
    public Tile Parent;
    public Vector3Int Position;
    public float DistanceToTarget;
    public float Cost;
    public float Weight;
    public float F
    {
        get
        {
            if (DistanceToTarget != -1 && Cost != -1)
                return DistanceToTarget + Cost;
            else
                return -1;
        }
    }
    public bool Walkable;
    public bool WithinReach = false;

    public Tile(Vector3Int pos, bool walkable, IndicatorTile indicatorTile,  float weight = 1)
    {
        Parent = null;
        Position = pos;
        DistanceToTarget = -1;
        Cost = 1;
        Weight = weight;
        Walkable = walkable;
        this.IndicatorTile = indicatorTile;
    }
}

public class Pathfinder
{
    List<Tile> Grid;
    private Vector2Int Dimensions;
    public Pathfinder(List<Tile> grid, Vector2Int dimensions)
    {
        Grid = grid;
        Dimensions = dimensions;

    }

    private int Index(Vector3Int pos)
    {
        return pos.x + pos.y * Dimensions.x;
    }

    public Stack<Tile> FindPath(Tile Start, Tile End)
    {
        return FindPath(Start.Position, End.Position);
    }

    public List<Tile> FindWalkableTiles(Vector3Int Start, float cost)
    {
        Tile start = Grid[Index(Start)];
        List<Tile> nodeList = new List<Tile>{start};

        for (int i = 0; i < cost; i++)
        {
            var stepList = nodeList.ToList();
            foreach (var node in nodeList)
            {
                stepList.AddRange(Neighbors(node).Where(n => !nodeList.Contains(n) && !stepList.Contains(n) && n.Walkable));
            }
            nodeList.AddRange(stepList);
        }

        return nodeList;
    }
    public Stack<Tile> FindPath(Vector3Int Start, Vector3Int End)
    {
        Tile start = Grid[Index(Start)];
        Tile end = Grid[Index(End)];
        //Node start = new Node(new Vector3Int(Start.x, Start.y,0), true);
        ///Node end = new Node(new Vector3Int(End.x, End.y,0), true);

        Stack<Tile> Path = new Stack<Tile>();
        List<Tile> OpenList = new List<Tile>();
        List<Tile> ClosedList = new List<Tile>();
        List<Tile> adjacencies;
        Tile current = start;
       
        // add start node to Open List
        OpenList.Add(start);

        while(OpenList.Count != 0 && !ClosedList.Exists(x => x.Position == end.Position))
        {
            current = OpenList[0];
            OpenList.Remove(current);
            ClosedList.Add(current);
            adjacencies = Neighbors(current);
            foreach (var n in adjacencies.Where(n => !ClosedList.Contains(n) && n.Walkable && n.WithinReach).Where(n => !OpenList.Contains(n)))
            {
                n.Parent = current;
                n.DistanceToTarget = Math.Abs(n.Position.x - end.Position.x) + Math.Abs(n.Position.y - end.Position.y);
                if (Math.Abs(n.Position.y - end.Position.y) % 2 == 1)
                {
                    n.DistanceToTarget++;
                }
                n.Cost = n.Weight + n.Parent.Cost;
                OpenList.Add(n);
                OpenList = OpenList.OrderBy(node => node.F).ToList();
            }
        }
        
        // construct path, if end was not closed return null
        if(!ClosedList.Exists(x => x.Position == end.Position))
        {
            Debug.Log("no path exists");
            return null;
        }

        // if all good, return path
        Tile temp = ClosedList[ClosedList.IndexOf(current)];
        if (temp == null) return null;
        do
        {
            Path.Push(temp);
            temp = temp.Parent;
        } while (temp != start && temp != null) ;
        
        return Path;
    }

    public List<Tile> Neighbors(Tile n)
    {
        var neighbors = new List<Tile>();
        Vector3Int neighbor;
        var directions = (n.Position.y % 2) == 1? 
            evenDirs: 
            oddDirs;
        foreach (var direction in directions)
        {
            neighbor = n.Position + direction;
            if (InBounds(neighbor))
            {
                neighbors.Add(Grid[Index(neighbor)]);
            }
        }
        return neighbors;
    }
    
    private static Vector3Int
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
    
    private static Vector3Int[] oddDirs = 
        {DEG0, DEG60_ODD, DEG120_ODD, DEG180, DEG240_ODD, DEG300_ODD};
    private static Vector3Int[] evenDirs = 
        {DEG0, DEG60_EVEN, DEG120_EVEN, DEG180, DEG240_EVEN, DEG300_EVEN};
    private bool InBounds(Vector3Int gridPos)
    {
        /*if (gridPos.x == Dimensions.x && Dimensions.y % 2 == 1)
        {
            return false;
        }*/
        return gridPos.x > -1 && gridPos.y > -1 && gridPos.x < Dimensions.x && gridPos.y < Dimensions.y;
    }
}