using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Day_2022_14 : DayScript2022
{
    public List<Vector2Int> grid = new List<Vector2Int>();

    protected override string part_1()
    {
        StartCoroutine(coDay14_1());
        return "";
    }

    protected override string part_2()
    {
        StartCoroutine(coDay14_2());
        return "";
    }

    IEnumerator coDay14_1()
    { 
        foreach (string wallsLine in _input.Split('\n'))
        {
            string[] walls = wallsLine.Split(new string[] { " -> " }, System.StringSplitOptions.RemoveEmptyEntries);
            if (walls.Length < 2)
            {
                Debug.LogError("less than 2 walls ?");
                continue;
            }

            for (int i = 1; i < walls.Length; i++)
            {
                int[] fromCoords = walls[i - 1].Split(',').Select(x => int.Parse(x)).ToArray();
                Vector2Int from = new Vector2Int(fromCoords[0], fromCoords[1]);

                int[] toCoords = walls[i].Split(',').Select(x => int.Parse(x)).ToArray();
                Vector2Int to = new Vector2Int(toCoords[0], toCoords[1]);

                if (from.x == to.x)
                {
                    for (int cnt = Mathf.Min(from.y, to.y); cnt <= Mathf.Max(from.y, to.y); cnt++)
                    {
                        grid.Add(new Vector2Int(from.x, cnt));
                    }
                }
                else
                {
                    for (int cnt = Mathf.Min(from.x, to.x); cnt <= Mathf.Max(from.x, to.x); cnt++)
                    {
                        grid.Add(new Vector2Int(cnt, from.y));
                    }
                }
            }
        }
        yield return new WaitForEndOfFrame();

        int botGrid = grid.Select(x => x.y).Max();
        Debug.Log(botGrid);

        yield return new WaitForEndOfFrame();


        int safetyCount = 1000000;
        long result = 0;
        while(safetyCount > 0)
        {

            if (safetyCount % 100 == 0)
            {
                yield return new WaitForEndOfFrame();
                Debug.Log(safetyCount + " - Last sand " + grid.Last().ToString());
                yield return new WaitForEndOfFrame();

            }

            Vector2Int sandPos = new Vector2Int(500, 0);

            int subSafetyCount = 200;
            bool isResting = false;
            while (!isResting && sandPos.y <= botGrid && subSafetyCount > 0)
            {
                if (!grid.Exists(p => p.x == sandPos.x && p.y == sandPos.y + 1))  // falling down
                    sandPos.y += 1;
                else if (!grid.Exists(p => p.x == sandPos.x - 1 && p.y == sandPos.y + 1))  // falling left
                    sandPos = new Vector2Int(sandPos.x - 1, sandPos.y + 1);
                else if (!grid.Exists(p => p.x == sandPos.x + 1 && p.y == sandPos.y + 1))  // falling right
                    sandPos = new Vector2Int(sandPos.x + 1, sandPos.y + 1);
                else
                    isResting = true;

                    subSafetyCount--;
            }

            if (subSafetyCount <= 0)
                Debug.LogError("not enough subSafetyCount ??? " + sandPos.ToString());

            if (!isResting)
                break;
            else
            {
                result++;
                //Debug.Log("sand resting at " + sandPos.ToString());
                grid.Add(sandPos);
            }

            safetyCount--;
        }

        Debug.LogWarning("Result is " + result);
    }


    IEnumerator coDay14_2()
    {
        foreach (string wallsLine in _input.Split('\n'))
        {
            string[] walls = wallsLine.Split(new string[] { " -> " }, System.StringSplitOptions.RemoveEmptyEntries);
            if (walls.Length < 2)
            {
                Debug.LogError("less than 2 walls ?");
                continue;
            }

            for (int i = 1; i < walls.Length; i++)
            {
                int[] fromCoords = walls[i - 1].Split(',').Select(x => int.Parse(x)).ToArray();
                Vector2Int from = new Vector2Int(fromCoords[0], fromCoords[1]);

                int[] toCoords = walls[i].Split(',').Select(x => int.Parse(x)).ToArray();
                Vector2Int to = new Vector2Int(toCoords[0], toCoords[1]);

                if (from.x == to.x)
                {
                    for (int cnt = Mathf.Min(from.y, to.y); cnt <= Mathf.Max(from.y, to.y); cnt++)
                    {
                        grid.Add(new Vector2Int(from.x, cnt));
                    }
                }
                else
                {
                    for (int cnt = Mathf.Min(from.x, to.x); cnt <= Mathf.Max(from.x, to.x); cnt++)
                    {
                        grid.Add(new Vector2Int(cnt, from.y));
                    }
                }
            }
        }
        yield return new WaitForEndOfFrame();

        int botGrid = grid.Select(x => x.y).Max();
        Debug.Log(botGrid);

        yield return new WaitForEndOfFrame();


        int safetyCount = 1000000;
        long result = 0;
        while (safetyCount > 0)
        {

            if (safetyCount % 100 == 0)
            {
                yield return new WaitForEndOfFrame();
                Debug.Log(safetyCount + " - Last sand " + grid.Last().ToString());
                yield return new WaitForEndOfFrame();

            }

            Vector2Int sandPos = new Vector2Int(500, 0);

            int subSafetyCount = 200;
            bool isResting = false;
            while (!isResting && subSafetyCount > 0)
            {
                if (sandPos.y > botGrid + 1)
                    Debug.LogError("too deep ?? " + sandPos);
                if (sandPos.y == botGrid + 1)
                    isResting = true;
                else if (!grid.Exists(p => p.x == sandPos.x && p.y == sandPos.y + 1))  // falling down
                    sandPos.y += 1;
                else if (!grid.Exists(p => p.x == sandPos.x - 1 && p.y == sandPos.y + 1))  // falling left
                    sandPos = new Vector2Int(sandPos.x - 1, sandPos.y + 1);
                else if (!grid.Exists(p => p.x == sandPos.x + 1 && p.y == sandPos.y + 1))  // falling right
                    sandPos = new Vector2Int(sandPos.x + 1, sandPos.y + 1);
                else
                    isResting = true;

                subSafetyCount--;
            }

            if (subSafetyCount <= 0)
                Debug.LogError("not enough subSafetyCount ??? " + sandPos.ToString());

            result++;
            grid.Add(sandPos);
            if (sandPos == new Vector2Int(500, 0))
                break;

            safetyCount--;
        }

        Debug.LogWarning("Result is " + result);
    }
}
