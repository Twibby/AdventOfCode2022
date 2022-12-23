using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Day_2022_23 : DayScript2022
{
    static List<Elf> elves = new List<Elf>();

    protected override string part_1()
    {
        string[] input = _input.Split('\n');
        elves = new List<Elf>();
        for (int y = 0; y < input.Length; y++)
        {
            for (int x = 0; x < input[y].Length; x++)
            {
                if (input[y][x] == '#')
                    elves.Add(new Elf(new Vector2Int(x, y)));
            }
        }
        Debug.Log("Elves are at pos : " + System.String.Join("; ", elves.Select(x => x.currentPos)));

        if (IsTestInput)
        {
            string grid = "";
            for (int i = 0; i < 12; i++)
            {
                for (int j = 0; j < 14; j++)
                {
                    grid += (elves.Exists(x => x.currentPos.x == j && x.currentPos.y == i) ? '#' : '.').ToString();
                }
                grid += System.Environment.NewLine;
            }
            Debug.LogWarning(grid);
        }

        int roundCount = 10;
        for (int cnt = 0; cnt < roundCount; cnt++)
        {
            if (IsTestInput)
                Debug.LogWarning("ROUND " + (cnt + 1).ToString());

            Dictionary<Vector2Int, List<int>> proposedPos = new Dictionary<Vector2Int, List<int>>();
            for (int i=0; i < elves.Count; i++)
            {
                Vector2Int? pos = elves[i].NextPos();
                if (pos == null)        // elf doesn't want to move bc he has 8 free spaces around him
                    continue;

                if (!proposedPos.ContainsKey(pos.Value))
                    proposedPos.Add(pos.Value, new List<int>());

                proposedPos[pos.Value].Add(i);
            }

            foreach (var pos in proposedPos)
            {
                Debug.Log("Proposed pos " + pos.Key.ToString() + " by " + System.String.Join(" & ", pos.Value));
                if (pos.Value.Count == 1)
                    elves[pos.Value[0]].currentPos = pos.Key;
            }

            if (IsTestInput)
            {
                string grid = "== End of Round " + (cnt+1).ToString() + " ==" + System.Environment.NewLine;
                for (int i = 0; i < 12; i++)
                {
                    for (int j = 0; j < 14; j++)
                    {
                        grid += (elves.Exists(x => x.currentPos.x == j && x.currentPos.y == i) ? '#' : '.').ToString();
                    }
                    grid += System.Environment.NewLine;
                }
                Debug.LogWarning(grid);
                Debug.LogWarning("AFTER ROUND, elves are at pos : " + System.String.Join("; ", elves.Select(x => x.currentPos)));
            }
        }

        // Seach for min rectangle and count empty spaces
        int minX = elves.Select(x => x.currentPos.x).Min();
        int maxX = elves.Select(x => x.currentPos.x).Max();
        int minY = elves.Select(x => x.currentPos.y).Min();
        int maxY = elves.Select(x => x.currentPos.y).Max();

        long area = (maxX - minX + 1) * (maxY - minY + 1);
        long total = area - elves.Count;

        return total.ToString();


    }

    protected override string part_2()
    {
        string[] input = _input.Split('\n');
        elves = new List<Elf>();
        for (int y = 0; y < input.Length; y++)
        {
            for (int x = 0; x < input[y].Length; x++)
            {
                if (input[y][x] == '#')
                    elves.Add(new Elf(new Vector2Int(x, y)));
            }
        }
        Debug.Log("Elves are at pos : " + System.String.Join("; ", elves.Select(x => x.currentPos)));

        if (IsTestInput)
        {
            string grid = "";
            for (int i = 0; i < 12; i++)
            {
                for (int j = 0; j < 14; j++)
                {
                    grid += (elves.Exists(x => x.currentPos.x == j && x.currentPos.y == i) ? '#' : '.').ToString();
                }
                grid += System.Environment.NewLine;
            }
            Debug.LogWarning(grid);
        }
        StartCoroutine(coDay23_2());

        return "";
    }

    IEnumerator coDay23_2()
    { 
        int safetyCount = 100000;
        for (int cnt = 0; cnt < safetyCount; cnt++)
        {

            if (cnt % 100 == 0)
            {

                int minX = elves.Select(x => x.currentPos.x).Min();
                int maxX = elves.Select(x => x.currentPos.x).Max();
                int minY = elves.Select(x => x.currentPos.y).Min();
                int maxY = elves.Select(x => x.currentPos.y).Max();

                string grid = "== End of Round " + (cnt).ToString() + " ==" + System.Environment.NewLine;
                for (int i = minY; i <= maxY; i++)
                {
                    for (int j = minX; j < maxX; j++)
                    {
                        grid += (elves.Exists(x => x.currentPos.x == j && x.currentPos.y == i) ? '#' : '.').ToString();
                    }
                    grid += System.Environment.NewLine;
                }
                Debug.LogWarning(grid);
                Debug.LogWarning("AFTER ROUND, elves are at pos : " + System.String.Join("; ", elves.Select(x => x.currentPos)));
            }

            Debug.LogWarning("ROUND " + (cnt + 1).ToString());
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();



            Dictionary<Vector2Int, List<int>> proposedPos = new Dictionary<Vector2Int, List<int>>();
            for (int i = 0; i < elves.Count; i++)
            {
                Vector2Int? pos = elves[i].NextPos();
                if (pos == null)        // elf doesn't want to move bc he has 8 free spaces around him
                    continue;

                if (!proposedPos.ContainsKey(pos.Value))
                    proposedPos.Add(pos.Value, new List<int>());

                proposedPos[pos.Value].Add(i);
            }

            Debug.Log(proposedPos.Count);
            if (proposedPos.Count == 0)
            {
                Debug.LogWarning("RESULT IS " + (cnt+1).ToString());
                break;
            }

            foreach (var pos in proposedPos)
            {
                //Debug.Log("Proposed pos " + pos.Key.ToString() + " by " + System.String.Join(" & ", pos.Value));
                if (pos.Value.Count == 1)
                    elves[pos.Value[0]].currentPos = pos.Key;
            }
        }
    }

    public enum Directions { North, South, West, East}
    public class Elf
    {
        public Vector2Int currentPos;
        public int consideredDirection;

        public Elf(Vector2Int pos)
        {
            this.currentPos = pos;
            this.consideredDirection = 0;
        }

        public Vector2Int? NextPos()
        {
            //Debug.Log("Elf at pos " + currentPos.ToString() + " is choosing new pos, curDir is " + consideredDirection.ToString());
            if (allNeighboursEmpty())
            {
                this.consideredDirection += 1;

                return null;
            }

            //Debug.Log("\t -> Has neighbour, wants to move");
            int directionsCount = System.Enum.GetValues(typeof(Directions)).Length;
            for (int i = 0; i < directionsCount; i++)
            {
                Directions checkDir = (Directions)((consideredDirection + i) % directionsCount);

                if (hasDirNeighboursEmpty(checkDir))
                {
                    Vector2Int res = new Vector2Int(currentPos.x, currentPos.y);
                    switch (checkDir)
                    {
                        case Directions.North: res.y -= 1; break;
                        case Directions.South: res.y += 1; break;
                        case Directions.West: res.x -= 1; break;
                        case Directions.East: res.x += 1; break;
                    }
                    this.consideredDirection += 1;

                    //Debug.Log("\t -> checked " + checkDir.ToString() + " and MATCH ! res is " + res.ToString());
                    return res;
                }
                //Debug.Log("\t -> checked " + checkDir.ToString() + " and doesn't match");
            }

            //Debug.Log("\t -> Nothing match, Doesn't move");
            this.consideredDirection += 1;
            return null;
        }

        bool allNeighboursEmpty()
        {
            if (elves.Exists(e => e.currentPos.x == this.currentPos.x -1 && e.currentPos.y == this.currentPos.y -1))
                return false;

            if (elves.Exists(e => e.currentPos.x == this.currentPos.x && e.currentPos.y == this.currentPos.y -1))
                return false;

            if (elves.Exists(e => e.currentPos.x == this.currentPos.x +1 && e.currentPos.y == this.currentPos.y -1))
                return false;

            if (elves.Exists(e => e.currentPos.x == this.currentPos.x -1 && e.currentPos.y == this.currentPos.y))
                return false;

            if (elves.Exists(e => e.currentPos.x == this.currentPos.x +1 && e.currentPos.y == this.currentPos.y))
                return false;

            if (elves.Exists(e => e.currentPos.x == this.currentPos.x -1 && e.currentPos.y == this.currentPos.y+1))
                return false;

            if (elves.Exists(e => e.currentPos.x == this.currentPos.x && e.currentPos.y == this.currentPos.y+1))
                return false;

            if (elves.Exists(e => e.currentPos.x == this.currentPos.x+1 && e.currentPos.y == this.currentPos.y+1))
                return false;

            return true;
        }

        bool hasDirNeighboursEmpty(Directions dir)
        {
            switch (dir)
            {
                case Directions.North:
                    return !elves.Exists(e => e.currentPos.y == this.currentPos.y - 1 && e.currentPos.x >= this.currentPos.x - 1 && e.currentPos.x <= this.currentPos.x + 1);
                case Directions.South:
                    return !elves.Exists(e => e.currentPos.y == this.currentPos.y + 1 && e.currentPos.x >= this.currentPos.x - 1 && e.currentPos.x <= this.currentPos.x + 1);
                case Directions.West:
                    return !elves.Exists(e => e.currentPos.x == this.currentPos.x - 1 && e.currentPos.y >= this.currentPos.y - 1 && e.currentPos.y <= this.currentPos.y + 1);
                case Directions.East:
                    return !elves.Exists(e => e.currentPos.x == this.currentPos.x + 1 && e.currentPos.y >= this.currentPos.y - 1 && e.currentPos.y <= this.currentPos.y + 1);
            }

            return false;
        }
    }
}
