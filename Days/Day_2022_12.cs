using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Just putting some tags for next years to retrieve this A* algo :
// A star A* dijsktra djisktra path graph


public class Day_2022_12 : DayScript2022
{
    protected override string part_1()
    {
        List<string> grid = _input.Split('\n').ToList();


        Vector2Int startPos = Vector2Int.zero;
        for (int raw = 0; raw < grid.Count; raw++)
        {
            int colIndex = grid[raw].IndexOf('S');
            if (colIndex >= 0)
            {
                startPos = new Vector2Int(colIndex, raw);
                Debug.Log("Start found at pos : " + startPos.ToString());
                break;
            }
        }

        Vector2Int endPos = Vector2Int.zero;
        for (int raw = 0; raw < grid.Count; raw++)
        {
            int colIndex = grid[raw].IndexOf('E');
            if (colIndex >= 0)
            {
                endPos = new Vector2Int(colIndex, raw);
                Debug.Log("End found at pos : " + endPos.ToString());
                break;
            }
        }
        Debug.Log(grid[endPos.y]);
        grid[endPos.y] = grid[endPos.y].Substring(0, endPos.x) + 'z' + grid[endPos.y].Substring(endPos.x +1);
        Debug.Log(grid[endPos.y]);

        int width = grid[0].Length;
        int height = grid.Count;

        List <KeyValuePair<Vector2Int, int>> treatedPoints = new List<KeyValuePair<Vector2Int, int>>();
        List<KeyValuePair<Vector2Int,int>> queuePoints = new List<KeyValuePair<Vector2Int, int>>() { new KeyValuePair<Vector2Int, int>(startPos,0) };

        int safetyCount = width*height;
        while (queuePoints.Count > 0 && safetyCount > 0)
        {
            Vector2Int curPos = queuePoints[0].Key;
            int curWeight = queuePoints[0].Value;
            queuePoints.RemoveAt(0);

            char curChar = grid[curPos.y][curPos.x];
            if (curChar == 'S')
                curChar = 'a';

            List<Vector2Int> neighbours = new List<Vector2Int>() { new Vector2Int(curPos.x - 1, curPos.y), new Vector2Int(curPos.x + 1, curPos.y), new Vector2Int(curPos.x, curPos.y - 1), new Vector2Int(curPos.x, curPos.y + 1) };
            foreach (Vector2Int nextPos in neighbours)
            {
                if (treatedPoints.Exists(x => x.Key.x == nextPos.x && x.Key.y == nextPos.y))
                    continue;

                if (nextPos.x >= 0 && nextPos.x < width && nextPos.y >= 0 && nextPos.y < height)
                {
                    if ((int)(grid[nextPos.y][nextPos.x] - curChar) < 2)
                    {
                        if (nextPos.x == endPos.x && nextPos.y == endPos.y)
                            return (curWeight + 1).ToString();

                        if (queuePoints.Exists(x => x.Key.x == nextPos.x && x.Key.y == nextPos.y))
                        {
                            int nextWeight = queuePoints.Find(x => x.Key.x == nextPos.x && x.Key.y == nextPos.y).Value;
                            if (nextWeight <= curWeight + 1)
                            {
                                // nothing
                            }
                            else
                            {
                                queuePoints.RemoveAll(x => x.Key.x == nextPos.x && x.Key.y == nextPos.y);
                                queuePoints.Add(new KeyValuePair<Vector2Int, int>(nextPos, curWeight + 1));
                            }
                        }
                        else
                            queuePoints.Add(new KeyValuePair<Vector2Int, int>(nextPos, curWeight + 1));
                    }
                }
            }
            treatedPoints.Add(new KeyValuePair<Vector2Int, int>(curPos, curWeight));
            queuePoints.Sort(delegate (KeyValuePair<Vector2Int, int> a, KeyValuePair<Vector2Int, int> b) { return a.Value.CompareTo(b.Value); });

            safetyCount--;
        }


        return base.part_1();
    }

    protected override string part_2()
    {
        // Principle : do same as part1 but in reverse, start from end and look for 'a' pos with constraint that you can go down at most 1 char 


        List<string> grid = _input.Split('\n').ToList();

        Vector2Int startPos = Vector2Int.zero;
        for (int raw = 0; raw < grid.Count; raw++)
        {
            int colIndex = grid[raw].IndexOf('S');
            if (colIndex >= 0)
            {
                startPos = new Vector2Int(colIndex, raw);
                Debug.Log("Start found at pos : " + startPos.ToString());
                break;
            }
        }
        Debug.Log(grid[startPos.y]);
        grid[startPos.y] = grid[startPos.y].Substring(0, startPos.x) + 'a' + grid[startPos.y].Substring(startPos.x + 1);
        Debug.Log(grid[startPos.y]);

        Vector2Int endPos = Vector2Int.zero;
        for (int raw = 0; raw < grid.Count; raw++)
        {
            int colIndex = grid[raw].IndexOf('E');
            if (colIndex >= 0)
            {
                endPos = new Vector2Int(colIndex, raw);
                Debug.Log("End found at pos : " + endPos.ToString());
                break;
            }
        }
        Debug.Log(grid[endPos.y]);
        grid[endPos.y] = grid[endPos.y].Substring(0, endPos.x) + 'z' + grid[endPos.y].Substring(endPos.x + 1);
        Debug.Log(grid[endPos.y]);

        int width = grid[0].Length;
        int height = grid.Count;

        List<KeyValuePair<Vector2Int, int>> treatedPoints = new List<KeyValuePair<Vector2Int, int>>();
        List<KeyValuePair<Vector2Int, int>> queuePoints = new List<KeyValuePair<Vector2Int, int>>() { new KeyValuePair<Vector2Int, int>(endPos, 0) };

        int safetyCount = width * height;
        while (queuePoints.Count > 0 && safetyCount > 0)
        {
            Vector2Int curPos = queuePoints[0].Key;
            int curWeight = queuePoints[0].Value;
            queuePoints.RemoveAt(0);

            char curChar = grid[curPos.y][curPos.x];

            List<Vector2Int> neighbours = new List<Vector2Int>() { new Vector2Int(curPos.x - 1, curPos.y), new Vector2Int(curPos.x + 1, curPos.y), new Vector2Int(curPos.x, curPos.y - 1), new Vector2Int(curPos.x, curPos.y + 1) };
            foreach (Vector2Int nextPos in neighbours)
            {
                if (treatedPoints.Exists(x => x.Key.x == nextPos.x && x.Key.y == nextPos.y))
                    continue;

                if (nextPos.x >= 0 && nextPos.x < width && nextPos.y >= 0 && nextPos.y < height)
                {
                    if ((int)(curChar - grid[nextPos.y][nextPos.x]) < 2)
                    {
                        if (grid[nextPos.y][nextPos.x] == 'a')
                            return (curWeight + 1).ToString();

                        if (queuePoints.Exists(x => x.Key.x == nextPos.x && x.Key.y == nextPos.y))
                        {
                            int nextWeight = queuePoints.Find(x => x.Key.x == nextPos.x && x.Key.y == nextPos.y).Value;
                            if (nextWeight <= curWeight + 1)
                            {
                                // nothing
                            }
                            else
                            {
                                queuePoints.RemoveAll(x => x.Key.x == nextPos.x && x.Key.y == nextPos.y);
                                queuePoints.Add(new KeyValuePair<Vector2Int, int>(nextPos, curWeight + 1));
                            }
                        }
                        else
                            queuePoints.Add(new KeyValuePair<Vector2Int, int>(nextPos, curWeight + 1));
                    }
                }
            }
            treatedPoints.Add(new KeyValuePair<Vector2Int, int>(curPos, curWeight));
            queuePoints.Sort(delegate (KeyValuePair<Vector2Int, int> a, KeyValuePair<Vector2Int, int> b) { return a.Value.CompareTo(b.Value); });

            safetyCount--;
        }
        return base.part_2();
    }
}
