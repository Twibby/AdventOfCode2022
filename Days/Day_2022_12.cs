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


        IntVector2 startPos = IntVector2.zero;
        for (int raw = 0; raw < grid.Count; raw++)
        {
            int colIndex = grid[raw].IndexOf('S');
            if (colIndex >= 0)
            {
                startPos = new IntVector2(colIndex, raw);
                Debug.Log("Start found at pos : " + startPos.ToString());
                break;
            }
        }

        IntVector2 endPos = IntVector2.zero;
        for (int raw = 0; raw < grid.Count; raw++)
        {
            int colIndex = grid[raw].IndexOf('E');
            if (colIndex >= 0)
            {
                endPos = new IntVector2(colIndex, raw);
                Debug.Log("End found at pos : " + endPos.ToString());
                break;
            }
        }
        Debug.Log(grid[endPos.y]);
        grid[endPos.y] = grid[endPos.y].Substring(0, endPos.x) + 'z' + grid[endPos.y].Substring(endPos.x +1);
        Debug.Log(grid[endPos.y]);

        int width = grid[0].Length;
        int height = grid.Count;

        List <KeyValuePair<IntVector2, int>> treatedPoints = new List<KeyValuePair<IntVector2, int>>();
        List<KeyValuePair<IntVector2,int>> queuePoints = new List<KeyValuePair<IntVector2, int>>() { new KeyValuePair<IntVector2, int>(startPos,0) };

        int safetyCount = width*height;
        while (queuePoints.Count > 0 && safetyCount > 0)
        {
            IntVector2 curPos = queuePoints[0].Key;
            int curWeight = queuePoints[0].Value;
            queuePoints.RemoveAt(0);

            char curChar = grid[curPos.y][curPos.x];
            if (curChar == 'S')
                curChar = 'a';

            List<IntVector2> neighbours = new List<IntVector2>() { new IntVector2(curPos.x - 1, curPos.y), new IntVector2(curPos.x + 1, curPos.y), new IntVector2(curPos.x, curPos.y - 1), new IntVector2(curPos.x, curPos.y + 1) };
            foreach (IntVector2 nextPos in neighbours)
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
                                queuePoints.Add(new KeyValuePair<IntVector2, int>(nextPos, curWeight + 1));
                            }
                        }
                        else
                            queuePoints.Add(new KeyValuePair<IntVector2, int>(nextPos, curWeight + 1));
                    }
                }
            }
            treatedPoints.Add(new KeyValuePair<IntVector2, int>(curPos, curWeight));
            queuePoints.Sort(delegate (KeyValuePair<IntVector2, int> a, KeyValuePair<IntVector2, int> b) { return a.Value.CompareTo(b.Value); });

            safetyCount--;
        }


        return base.part_1();
    }

    protected override string part_2()
    {
        // Principle : do same as part1 but in reverse, start from end and look for 'a' pos with constraint that you can go down at most 1 char 


        List<string> grid = _input.Split('\n').ToList();

        IntVector2 startPos = IntVector2.zero;
        for (int raw = 0; raw < grid.Count; raw++)
        {
            int colIndex = grid[raw].IndexOf('S');
            if (colIndex >= 0)
            {
                startPos = new IntVector2(colIndex, raw);
                Debug.Log("Start found at pos : " + startPos.ToString());
                break;
            }
        }
        Debug.Log(grid[startPos.y]);
        grid[startPos.y] = grid[startPos.y].Substring(0, startPos.x) + 'a' + grid[startPos.y].Substring(startPos.x + 1);
        Debug.Log(grid[startPos.y]);

        IntVector2 endPos = IntVector2.zero;
        for (int raw = 0; raw < grid.Count; raw++)
        {
            int colIndex = grid[raw].IndexOf('E');
            if (colIndex >= 0)
            {
                endPos = new IntVector2(colIndex, raw);
                Debug.Log("End found at pos : " + endPos.ToString());
                break;
            }
        }
        Debug.Log(grid[endPos.y]);
        grid[endPos.y] = grid[endPos.y].Substring(0, endPos.x) + 'z' + grid[endPos.y].Substring(endPos.x + 1);
        Debug.Log(grid[endPos.y]);

        int width = grid[0].Length;
        int height = grid.Count;

        List<KeyValuePair<IntVector2, int>> treatedPoints = new List<KeyValuePair<IntVector2, int>>();
        List<KeyValuePair<IntVector2, int>> queuePoints = new List<KeyValuePair<IntVector2, int>>() { new KeyValuePair<IntVector2, int>(endPos, 0) };

        int safetyCount = width * height;
        while (queuePoints.Count > 0 && safetyCount > 0)
        {
            IntVector2 curPos = queuePoints[0].Key;
            int curWeight = queuePoints[0].Value;
            queuePoints.RemoveAt(0);

            char curChar = grid[curPos.y][curPos.x];

            List<IntVector2> neighbours = new List<IntVector2>() { new IntVector2(curPos.x - 1, curPos.y), new IntVector2(curPos.x + 1, curPos.y), new IntVector2(curPos.x, curPos.y - 1), new IntVector2(curPos.x, curPos.y + 1) };
            foreach (IntVector2 nextPos in neighbours)
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
                                queuePoints.Add(new KeyValuePair<IntVector2, int>(nextPos, curWeight + 1));
                            }
                        }
                        else
                            queuePoints.Add(new KeyValuePair<IntVector2, int>(nextPos, curWeight + 1));
                    }
                }
            }
            treatedPoints.Add(new KeyValuePair<IntVector2, int>(curPos, curWeight));
            queuePoints.Sort(delegate (KeyValuePair<IntVector2, int> a, KeyValuePair<IntVector2, int> b) { return a.Value.CompareTo(b.Value); });

            safetyCount--;
        }
        return base.part_2();
    }
}
