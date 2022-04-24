using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Mathematics;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;


public class Tile
{
    
    // Change this depending on what the desired size is for each element in the grid
    public IndicatorTile IndicatorTile;
    public Unit UnitOnTile;
    public Tile Parent;
    public Vector3Int Position_grid;
    public Vector3 Position_world;

    public float DistanceToTarget;
    public float Cost;
    public float Weight;
    public bool Walkable;
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

    public Tile(Vector3Int pos_grid,Vector3 pos_world, bool walkable, IndicatorTile indicatorTile,  float weight = 1)
    {
        Parent = null;
        Position_grid = pos_grid;
        Position_world = pos_world;
        DistanceToTarget = -1;
        Cost = 1;
        Weight = weight;
        Walkable = walkable;
        IndicatorTile = indicatorTile;
    }

    public void ResetTile()
    {
        Cost = 1;
        DistanceToTarget = -1;
        Parent = null;
    }
    
    public void SetUnit(Unit unit)
    {
        UnitOnTile = unit;
    }
}

public class Pathfinder
{
    private List<Tile> Grid;
    private Vector2Int Dimensions;
    public Pathfinder(List<Tile> grid, Vector2Int dimensions)
    {
        Grid = grid;
        Dimensions = dimensions;

    }

    public int Index(Vector3Int pos)
    {
        return pos.x + pos.y * Dimensions.x;
    }

    public Stack<Tile> FindPathDFS(Tile Start, Tile End)
    {
        return FindPathDFS(Start.Position_grid, End.Position_grid);
    }

    public List<Tile> FindWalkableTiles(Vector3Int Start, float cost)
    {
        Tile start = Grid[Index(Start)];
        List<Tile> nodeList = new List<Tile>{start};
        
        for (int i = 0; i <= cost; i++)
        {
            var stepList = nodeList.ToList();
            foreach (var node in nodeList)
            {
                stepList.AddRange(Neighbors(node).Where(n => !nodeList.Contains(n) && !stepList.Contains(n) && n.Walkable && n.UnitOnTile == null));
            }
            nodeList.AddRange(stepList);
        }

        return nodeList;
    }
    public Stack<Tile> FindPathDFS(Vector3Int Start, Vector3Int End)
    {
        foreach (var tile in Grid.Where(tile => tile != null))
        {
            tile.ResetTile();
        }
        Tile start = Grid[Index(Start)];
        Tile end = Grid[Index(End)];
        Stack<Tile> Path = new Stack<Tile>();
        List<Tile> OpenList = new List<Tile>();
        List<Tile> ClosedList = new List<Tile>();
        List<Tile> adjacencies;
        Tile current = start;

        
        // reset all costs to their default;
        // add start node to Open List
        OpenList.Add(start);

        while(OpenList.Count != 0 && !ClosedList.Exists(x => x.Position_grid == end.Position_grid))
        {
            current = OpenList[0];
            OpenList.Remove(current);
            ClosedList.Add(current);
            adjacencies = Neighbors(current);
            foreach (var n in adjacencies.Where(n => !ClosedList.Contains(n) && n.Walkable && (n.UnitOnTile == null || n.Position_grid == end.Position_grid)).Where(n => !OpenList.Contains(n)))
            {
                n.Parent = current;
                n.DistanceToTarget = Math.Abs(n.Position_grid.x - end.Position_grid.x) + Math.Abs(n.Position_grid.y - end.Position_grid.y);
                if (Math.Abs(n.Position_grid.y - end.Position_grid.y) % 2 == 1)
                {
                    n.DistanceToTarget++;
                }
                n.Cost = n.Weight + n.Parent.Cost;
                OpenList.Add(n);
                OpenList = OpenList.OrderBy(node => node.F).ToList();
            }
        }
        
        // construct path, if end was not closed return null
        if(!ClosedList.Exists(x => x.Position_grid == end.Position_grid))
        {
            ClosedList.ForEach(tile => Debug.Log(tile.Position_grid));
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

    
    public Queue<Tile> FindWalkableBFS(Vector3Int Start, float cost)
    {
        foreach (var tile in Grid.Where(tile => tile != null))
        {
            tile.ResetTile();
        }
        Tile start = Grid[Index(Start)];
        Queue<Tile> Path = new Queue<Tile>();
        List<Tile> OpenList = new List<Tile>();
        List<Tile> ClosedList = new List<Tile>();
        List<Tile> adjacencies;
        Tile current = start;

        
        // reset all costs to their default;
        // add start node to Open List
        OpenList.Add(start);

        while(OpenList.Count != 0)
        {
            current = OpenList[0];
            OpenList.Remove(current);
            ClosedList.Add(current);
            adjacencies = Neighbors(current);
            foreach (var n in adjacencies.Where(n => n.Walkable && n.UnitOnTile == null && !OpenList.Contains(n) && n.Weight + current.Cost <= cost))
            {
                n.Parent = current;
                n.Cost = n.Weight + n.Parent.Cost;
                OpenList.Add(n);
                /*if (n.Cost <= cost)
                {
                    
                    //OpenList = OpenList.OrderBy(node => node.F).ToList();
                }*/
            }
        }
        
        /*// construct path, if end was not closed return null
        if(!ClosedList.Exists(x => x.Position_grid == end.Position_grid))
        {
            ClosedList.ForEach(tile => Debug.Log(tile.Position_grid));
            Debug.Log("no path exists");
            return null;
        }*/
        // if all good, return path
        /*Debug.Log(ClosedList.Count);
        Tile temp = ClosedList[ClosedList.IndexOf(current)];*/
        foreach (var tile in ClosedList)
        {
            Path.Enqueue(tile);
        }
        return Path;
    }
    public List<Tile> Neighbors(Tile n)
    {
        var neighbors = new List<Tile>();
        Vector3Int neighbor;
        var directions = (n.Position_grid.y % 2) == 1? 
            evenDirs: 
            oddDirs;
        foreach (var direction in directions)
        {
            neighbor = n.Position_grid + direction;
            if (InBounds(neighbor))
            {
                neighbors.Add(Grid[Index(neighbor)]);
            }
        }

        neighbors.RemoveAll(ne => ne == null);
        return neighbors;
    }

    //Recursive searching, find neighbours of neighbours n times, yields an area of tiles around a center tile.
    //Calling this with a radius of 1 will yield a a list containing tiles from origin *radius* steps in every direction
    public List<Tile> FindRangedNeighbours(Tile origin, int radius)
    {
        var neighbors = new List<Tile>{origin};
        Vector3Int neighbor;
        var directions = (origin.Position_grid.y % 2) == 1? 
            evenDirs: 
            oddDirs;
        while (radius > 0)
        {
            foreach (var n in neighbors)
            {
                foreach (var direction in directions)
                {
                    neighbor = n.Position_grid + direction;
                    if (!InBounds(neighbor) && !neighbors.Contains(Grid[Index(neighbor)])) continue;
                    neighbors.Add(Grid[Index(neighbor)]);
                }
            }
            radius--;
        }
        return neighbors;
    }

    public List<Tile> FindMeleeNeighbours(Tile origin, Tile target, int radius)
    {
        var neighbors = new List<Tile>{target};
        Vector3Int neighbor;
        var directions = (origin.Position_grid.y % 2) == 1? 
            evenDirs: 
            oddDirs;
        while (radius > 0)
        {
            foreach (var n in neighbors)
            {
                foreach (var direction in directions)
                {
                    neighbor = n.Position_grid + direction;
                    //dont add if out of bounds, or list already contains neighbour, or nei
                    if (!InBounds(neighbor) && 
                        neighbors.Contains(Grid[Index(neighbor)]) && 
                        !Neighbors(Grid[Index(neighbor)]).Contains(origin)) continue;
                    
                    neighbors.Add(Grid[Index(neighbor)]);
                }
            }
            radius--;
        }
        return neighbors;
    }
    
    public Tile FindNearestAngleTile(Tile origin, Vector2 direction, List<Tile> target,float AngleThreshhold)
    {
        var smallestVal = float.PositiveInfinity;
        Tile nearestTile = null;
        foreach (var t in target)
        {
            var tileXY = 
                new Vector2(t.Position_world.x, t.Position_world.y) - 
                new Vector2(origin.Position_world.x, origin.Position_world.y);
            
            var angleDiff = Vector2.Angle(direction, tileXY.normalized);
            
            if (angleDiff <= smallestVal && angleDiff <= AngleThreshhold)
            {
                smallestVal = angleDiff;
                nearestTile = t;
            }
        }
        return nearestTile;
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