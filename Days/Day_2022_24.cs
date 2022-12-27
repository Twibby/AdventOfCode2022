using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Day_2022_24 : DayScript2022
{
    static int valleyWidth;
    static int valleyHeight;
    static Vector2Int startPos;
    static Vector2Int endPos;

    static List<Dictionary<Vector2Int, List<char>>> blizzardsAtStep = new List<Dictionary<Vector2Int, List<char>>>();

    public int blizzardPrecount = 300;

    protected override string part_1()
    {
        StartCoroutine(coDay24_1());
        return "";
    }

    protected override string part_2()
    {
        StartCoroutine(coDay24_2());
        return "";
    }



    IEnumerator coDay24_1()
    {
        yield return new WaitForEndOfFrame();

        List<string> grid = _input.Split('\n').ToList();

        startPos = new Vector2Int(grid[0].IndexOf('.'), 0);
        endPos = new Vector2Int(grid[grid.Count - 1].IndexOf('.'), grid.Count - 1);

        int bestScore = int.MaxValue;

        valleyHeight = grid.Count;
        valleyWidth = grid[0].Length;

        Dictionary<Vector2Int, List<char>> startBlizzardPos = new Dictionary<Vector2Int, List<char>>();
        for (int y=1; y < grid.Count-1; y++)
        {
            for (int x = 1; x < grid[y].Length -1; x++)
            {
                if (grid[y][x] != '.')
                {
                    Vector2Int pos = new Vector2Int(x, y);
                    if (!startBlizzardPos.ContainsKey(pos))
                        startBlizzardPos.Add(pos, new List<char>());

                    startBlizzardPos[pos].Add(grid[y][x]);
                }
            }
        }

        Debug.Log("Start pos is " + startPos.ToString() + " - End Pos is " + endPos.ToString());
        printValley(startBlizzardPos, 0);
        yield return new WaitForSeconds(0.2f);

        // Let simulate all blizzard grids for step 0 to 1000
        blizzardsAtStep = new List<Dictionary<Vector2Int, List<char>>>();
        blizzardsAtStep.Add(startBlizzardPos);
        for (int i = 1; i < blizzardPrecount; i++)
        {
            Dictionary<Vector2Int, List<char>> previousBlizzard = blizzardsAtStep[i - 1];

            // Compute new blizzards pos
            Dictionary<Vector2Int, List<char>> newBlizzardPos = new Dictionary<Vector2Int, List<char>>();
            foreach (var blizzardPos in previousBlizzard)
            {
                foreach (char blizzard in blizzardPos.Value)
                {
                    Vector2Int newBlizzPos = blizzardPos.Key;
                    switch (blizzard)
                    {
                        case '<': newBlizzPos.x = (newBlizzPos.x - 1 > 0 ? newBlizzPos.x - 1 : valleyWidth - 2); break;
                        case '>': newBlizzPos.x = (newBlizzPos.x + 1 < valleyWidth-1 ? newBlizzPos.x + 1 : 1); break;
                        case '^': newBlizzPos.y = (newBlizzPos.y - 1 > 0 ? newBlizzPos.y - 1 : valleyHeight - 2); break;
                        case 'v': newBlizzPos.y = (newBlizzPos.y + 1 < valleyHeight-1 ? newBlizzPos.y + 1 : 1); break;
                        default: Debug.LogError("Impossible ?? " + blizzard); break;
                    }

                    //Debug.Log("Blizzard " + blizzard + " at pos " + blizzardPos.Key.ToString() + " will be at pos " + newBlizzPos);

                    if (!newBlizzardPos.ContainsKey(newBlizzPos))
                        newBlizzardPos.Add(newBlizzPos, new List<char>());

                    newBlizzardPos[newBlizzPos].Add(blizzard);
                }
            }
            blizzardsAtStep.Add(newBlizzardPos);

            if (i < 20)
            {
                printValley(newBlizzardPos, i);
                yield return new WaitForSeconds(0.1f);
            }
        }
        Debug.Log("TOTO");

        Dictionary<int, List<Vector2Int>> globalHistory = new Dictionary<int, List<Vector2Int>>();

        Stack<StateBlizzard> openStates = new Stack<StateBlizzard>();
        openStates.Push(new StateBlizzard(startPos, 0, new List<Vector2Int>()));

        long safetyCount = 100000000000000;
        while (openStates.Count > 0)
        {

            if (safetyCount % 5000 == 0)
            {
                yield return new WaitForEndOfFrame();
                Debug.Log(safetyCount + " - " + openStates.Count + " | best score is currently " + bestScore + System.Environment.NewLine + "First elmt history : " + System.String.Join("; ", openStates.Peek().posHistory));
            }

            StateBlizzard currentState = openStates.Pop();

            if (globalHistory.ContainsKey(currentState.stepCount) && globalHistory[currentState.stepCount].Contains(currentState.currentPos))
            {
                // Already been there
                continue;
            }

            if (!globalHistory.ContainsKey(currentState.stepCount))
                globalHistory.Add(currentState.stepCount, new List<Vector2Int>());

            globalHistory[currentState.stepCount].Add(currentState.currentPos);

            //if (openStates.Count > 10 && currentState.stepCount - openStates.Peek().stepCount > 10)
            //{
            //    StateBlizzard invState = openStates.Pop(); // let's explore 2 paths at same time, because lol why not ??
            //    openStates.Push(currentState);
            //    openStates.Push(invState);          
            //}

            if (currentState.stepCount >= blizzardPrecount-1)
            {
                //Debug.LogError("Didn't calculte enough blizzards conf");
                continue;
            }

            if (currentState.stepCount > bestScore)
                continue;

            if (currentState.stepCount + dist(currentState.currentPos, endPos) >= bestScore)
                continue;

            if (currentState.currentPos == startPos && currentState.stepCount > 5)
                continue;

            if (currentState.currentPos == endPos)
            {
                Debug.Log(currentState.ToString());
                if (currentState.stepCount < bestScore)
                {
                    bestScore = currentState.stepCount;
                    Debug.LogWarning("New best path found with score " + bestScore + " : " + currentState.ToString());
                }
                
                continue;
            }

            if (currentState.posHistory.FindAll(x => x == currentState.currentPos).Count > 2)   // passes 5 times same spot
            {
                //Debug.Log("let's say it's not here");
                continue;
            }

            //if (currentState.stepCount == 16 && currentState.currentPos == new Vector2Int(6,3))
            //{
            //    Debug.LogError("pause");
            //}
            Dictionary<Vector2Int, List<char>> newBlizzardPos = blizzardsAtStep[currentState.stepCount +1];

            List <Vector2Int> newHistory = new List<Vector2Int>(currentState.posHistory);
            newHistory.Add(currentState.currentPos);

            Vector2Int newPos;
            newPos = new Vector2Int(currentState.currentPos.x, currentState.currentPos.y);
            if (!newBlizzardPos.ContainsKey(newPos))
                openStates.Push(new StateBlizzard(newPos, currentState.stepCount + 1, newHistory));

            newPos = new Vector2Int(currentState.currentPos.x, currentState.currentPos.y - 1);
            if (isInGrid(newPos) && !newBlizzardPos.ContainsKey(newPos))
                openStates.Push(new StateBlizzard(newPos, currentState.stepCount + 1, newHistory));

            newPos = new Vector2Int(currentState.currentPos.x - 1, currentState.currentPos.y);
            if (isInGrid(newPos) && !newBlizzardPos.ContainsKey(newPos))
                openStates.Push(new StateBlizzard(newPos, currentState.stepCount + 1, newHistory));

            newPos = new Vector2Int(currentState.currentPos.x + 1, currentState.currentPos.y);
            if (isInGrid(newPos) && !newBlizzardPos.ContainsKey(newPos))
                openStates.Push(new StateBlizzard(newPos, currentState.stepCount + 1, newHistory));

            newPos = new Vector2Int(currentState.currentPos.x, currentState.currentPos.y + 1);
            if (isInGrid(newPos) && !newBlizzardPos.ContainsKey(newPos))
                openStates.Push(new StateBlizzard(newPos, currentState.stepCount + 1, newHistory));
            






            openStates.Peek();
            safetyCount--;
        }
        if (safetyCount <= 0)
            Debug.LogError("Exited earlier, remainings states count : " + openStates.Count);

            
        Debug.LogWarning("Final score is : " + bestScore);
        //return bestScore.ToString();
    }


    IEnumerator coDay24_2()
    {
        yield return new WaitForEndOfFrame();

        List<string> grid = _input.Split('\n').ToList();

        startPos = new Vector2Int(grid[0].IndexOf('.'), 0);
        endPos = new Vector2Int(grid[grid.Count - 1].IndexOf('.'), grid.Count - 1);

        int bestScore = int.MaxValue;

        valleyHeight = grid.Count;
        valleyWidth = grid[0].Length;

        Dictionary<Vector2Int, List<char>> startBlizzardPos = new Dictionary<Vector2Int, List<char>>();
        for (int y = 1; y < grid.Count - 1; y++)
        {
            for (int x = 1; x < grid[y].Length - 1; x++)
            {
                if (grid[y][x] != '.')
                {
                    Vector2Int pos = new Vector2Int(x, y);
                    if (!startBlizzardPos.ContainsKey(pos))
                        startBlizzardPos.Add(pos, new List<char>());

                    startBlizzardPos[pos].Add(grid[y][x]);
                }
            }
        }

        Debug.Log("Start pos is " + startPos.ToString() + " - End Pos is " + endPos.ToString());
        printValley(startBlizzardPos, 0);
        yield return new WaitForSeconds(0.2f);

        // Let simulate all blizzard grids for step 0 to 1000
        blizzardsAtStep = new List<Dictionary<Vector2Int, List<char>>>();
        blizzardsAtStep.Add(startBlizzardPos);
        for (int i = 1; i < blizzardPrecount; i++)
        {
            Dictionary<Vector2Int, List<char>> previousBlizzard = blizzardsAtStep[i - 1];

            // Compute new blizzards pos
            Dictionary<Vector2Int, List<char>> newBlizzardPos = new Dictionary<Vector2Int, List<char>>();
            foreach (var blizzardPos in previousBlizzard)
            {
                foreach (char blizzard in blizzardPos.Value)
                {
                    Vector2Int newBlizzPos = blizzardPos.Key;
                    switch (blizzard)
                    {
                        case '<': newBlizzPos.x = (newBlizzPos.x - 1 > 0 ? newBlizzPos.x - 1 : valleyWidth - 2); break;
                        case '>': newBlizzPos.x = (newBlizzPos.x + 1 < valleyWidth - 1 ? newBlizzPos.x + 1 : 1); break;
                        case '^': newBlizzPos.y = (newBlizzPos.y - 1 > 0 ? newBlizzPos.y - 1 : valleyHeight - 2); break;
                        case 'v': newBlizzPos.y = (newBlizzPos.y + 1 < valleyHeight - 1 ? newBlizzPos.y + 1 : 1); break;
                        default: Debug.LogError("Impossible ?? " + blizzard); break;
                    }

                    //Debug.Log("Blizzard " + blizzard + " at pos " + blizzardPos.Key.ToString() + " will be at pos " + newBlizzPos);

                    if (!newBlizzardPos.ContainsKey(newBlizzPos))
                        newBlizzardPos.Add(newBlizzPos, new List<char>());

                    newBlizzardPos[newBlizzPos].Add(blizzard);
                }
            }
            blizzardsAtStep.Add(newBlizzardPos);

            if (i < 20)
            {
                printValley(newBlizzardPos, i);
                yield return new WaitForSeconds(0.1f);
            }
        }
        Debug.Log("TOTO");

        Dictionary<int, List<(Vector2Int, int)>> globalHistory = new Dictionary<int, List<(Vector2Int, int)>>();

        Stack<StateBlizzard> openStates = new Stack<StateBlizzard>();
        openStates.Push(new StateBlizzard(startPos, 0, new List<Vector2Int>(), endPos, 0));

        long safetyCount = 100000000000000;
        while (openStates.Count > 0)
        {

            if (safetyCount % 5000 == 0)
            {
                yield return new WaitForEndOfFrame();
                Debug.Log(safetyCount + " - " + openStates.Count + " | best score is currently " + bestScore + System.Environment.NewLine + "First elmt history : " + System.String.Join("; ", openStates.Peek().posHistory));
            }

            StateBlizzard currentState = openStates.Pop();

            if (globalHistory.ContainsKey(currentState.stepCount) && globalHistory[currentState.stepCount].Exists( h => h.Item1 == currentState.currentPos && h.Item2 == currentState.targetReached))
            {
                // Already been there
                continue;
            }

            if (!globalHistory.ContainsKey(currentState.stepCount))
                globalHistory.Add(currentState.stepCount, new List<(Vector2Int, int)>());

            globalHistory[currentState.stepCount].Add((currentState.currentPos, currentState.targetReached));

            if (currentState.stepCount >= blizzardPrecount - 1)
            {
                //Debug.LogError("Didn't calculte enough blizzards conf");
                continue;
            }

            if (currentState.stepCount > bestScore)
                continue;

            //if (currentState.stepCount + dist(currentState.currentPos, endPos) >= bestScore)
            //    continue;

            //if (currentState.currentPos == startPos && currentState.stepCount > 5)
            //    continue;

            if (currentState.currentPos == currentState.targetPos)
            {
                currentState.targetReached += 1;

                if (currentState.targetReached >= 3)
                {
                    Debug.Log(currentState.ToString());
                    if (currentState.stepCount < bestScore)
                    {
                        bestScore = currentState.stepCount;
                        Debug.LogWarning("New best path found with score " + bestScore + " : " + currentState.ToString());
                    }
                    continue;
                }
                else
                {
                    if (currentState.targetReached == 1)
                        currentState.targetPos = startPos;
                    else
                        currentState.targetPos = endPos;
                }
            }

            Dictionary<Vector2Int, List<char>> newBlizzardPos = blizzardsAtStep[currentState.stepCount + 1];

            List<Vector2Int> newHistory = new List<Vector2Int>(currentState.posHistory);
            newHistory.Add(currentState.currentPos);

            Vector2Int newPos;
            newPos = new Vector2Int(currentState.currentPos.x, currentState.currentPos.y);
            if (!newBlizzardPos.ContainsKey(newPos))
                openStates.Push(new StateBlizzard(newPos, currentState.stepCount + 1, newHistory, currentState.targetPos, currentState.targetReached));

            if (currentState.targetReached == 1)
            {
                newPos = new Vector2Int(currentState.currentPos.x + 1, currentState.currentPos.y);
                if (isInGrid(newPos) && !newBlizzardPos.ContainsKey(newPos))
                    openStates.Push(new StateBlizzard(newPos, currentState.stepCount + 1, newHistory, currentState.targetPos, currentState.targetReached));

                newPos = new Vector2Int(currentState.currentPos.x, currentState.currentPos.y + 1);
                if (isInGrid(newPos) && !newBlizzardPos.ContainsKey(newPos))
                    openStates.Push(new StateBlizzard(newPos, currentState.stepCount + 1, newHistory, currentState.targetPos, currentState.targetReached));
            }

            newPos = new Vector2Int(currentState.currentPos.x, currentState.currentPos.y - 1);
            if (isInGrid(newPos) && !newBlizzardPos.ContainsKey(newPos))
                openStates.Push(new StateBlizzard(newPos, currentState.stepCount + 1, newHistory, currentState.targetPos, currentState.targetReached));

            newPos = new Vector2Int(currentState.currentPos.x - 1, currentState.currentPos.y);
            if (isInGrid(newPos) && !newBlizzardPos.ContainsKey(newPos))
                openStates.Push(new StateBlizzard(newPos, currentState.stepCount + 1, newHistory, currentState.targetPos, currentState.targetReached));

            if (currentState.targetReached != 1)
            {
                newPos = new Vector2Int(currentState.currentPos.x + 1, currentState.currentPos.y);
                if (isInGrid(newPos) && !newBlizzardPos.ContainsKey(newPos))
                    openStates.Push(new StateBlizzard(newPos, currentState.stepCount + 1, newHistory, currentState.targetPos, currentState.targetReached));

                newPos = new Vector2Int(currentState.currentPos.x, currentState.currentPos.y + 1);
                if (isInGrid(newPos) && !newBlizzardPos.ContainsKey(newPos))
                    openStates.Push(new StateBlizzard(newPos, currentState.stepCount + 1, newHistory, currentState.targetPos, currentState.targetReached));
            }

            openStates.Peek();
            safetyCount--;
        }
        if (safetyCount <= 0)
            Debug.LogError("Exited earlier, remainings states count : " + openStates.Count);


        Debug.LogWarning("Final score is : " + bestScore);
        //return bestScore.ToString();
    }




    bool isInGrid(Vector2Int pos)
    {
        if (pos == startPos || pos == endPos)
            return true;

        return pos.x >= 1 && pos.x < valleyWidth-1 && pos.y >= 1 && pos.y < valleyHeight-1;
    }


    class StateBlizzard
    {
        public Vector2Int currentPos;
        public int stepCount;
        
        public List<Vector2Int> posHistory = new List<Vector2Int>();

        public Vector2Int targetPos;
        public int targetReached = 0;

        public StateBlizzard(Vector2Int p_currentPos, int p_stepCount, List<Vector2Int> history)
        {
            this.currentPos = p_currentPos;
            this.stepCount = p_stepCount;
            this.posHistory = new List<Vector2Int>(history);

            this.targetPos = Vector2Int.zero;
            this.targetReached = 0;
        }

        public StateBlizzard(Vector2Int p_currentPos, int p_stepCount, List<Vector2Int> history, Vector2Int targetPos, int targetReached)
        {
            this.currentPos = p_currentPos;
            this.stepCount = p_stepCount;
            this.posHistory = new List<Vector2Int>(history);

            this.targetPos = targetPos;
            this.targetReached = targetReached;
        }

        public override string ToString()
        {
            return "At pos " + currentPos.ToString() + ", after " + stepCount + " steps. History: " + System.String.Join("; ", posHistory);
        }
    }

    public TMPro.TMP_Text gridLabel;
    public void printValley(Dictionary<Vector2Int, List<char>> blizzards, int min)
    {
        Debug.Log("valleyHeight = " + valleyHeight + " - ValleyWidth = " + valleyWidth);

        string result = "Minute " + min + " : "+ System.Environment.NewLine;
        for (int j=0; j < valleyHeight; j++)
        {
            for (int i=0; i < valleyWidth; i++)
            {
                Vector2Int pos = new Vector2Int(i, j);
                if (isInGrid(pos))
                    result += (blizzards.ContainsKey(pos) ? (blizzards[pos].Count > 1 ? blizzards[pos].Count.ToString() : blizzards[pos][0].ToString()) : ".");
                else
                    result += (pos == startPos || pos == endPos ? "." : "#");
            }
            result += System.Environment.NewLine;
        }

        Debug.LogWarning(result);

        if (IsTestInput && gridLabel != null)
        {
            gridLabel.text = result;
        }
    }

    int dist(Vector2Int startPos, Vector2Int endPos)
    {
        return (Mathf.Abs(startPos.x - endPos.x) + Mathf.Abs(startPos.y - endPos.y));
    }
}
