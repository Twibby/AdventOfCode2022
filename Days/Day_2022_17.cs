using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Day_2022_17 : DayScript2022
{
    protected override string part_1()
    {
        StartCoroutine(auxDay17(2022));
        return "";
    }

    protected override string part_2()
    {
        //  1.000.000.000.000
        StartCoroutine(auxDay17(1000000000000));
        return "";
    }

    IEnumerator auxDay17(double totalRockWanted)
    { 
        int gridWidth = 7;
        int indexIteration = 0;
        string instruction = _input;

        List<List<char>> cavern = new List<List<char>>(); // represents columns
        for (int i = 0; i < gridWidth; i++)
        {
            cavern.Add(new List<char>());
        }

        printCavern(cavern);
        Debug.LogWarning(cavern.Count);

        Debug.LogWarning("Instruction length is : " + instruction.Length);
        float t0 = Time.realtimeSinceStartup;
        

        // following variables are here to make easier pattern recognition, i'm pretty sure this can be optimized
        List<(int, double)> instructionIndexOnRock0 = new List<(int, double)>();        
        int startPatternIndex = -1;
        double diffPattern = -1;
        double startPatternRockCount = -1;        
        double heightOffset = 0;
        bool isPatternvalidated = false;


        for (double rockCount = 0; rockCount < totalRockWanted; rockCount++)
        {
            Debug.Log("Starting rock " + (rockCount + 1).ToString());

            string[] rockShape = getRockShape(rockCount);

            if (rockCount > 1 && rockCount % 5 == 0)
            {
                yield return new WaitForEndOfFrame();

                // first of 5 rock shapes. Let's try to find a pattern, did we already meet that rock spawn being at same spot in instruction? 
                Debug.Log("At instruction : " + indexIteration + System.Environment.NewLine + System.String.Join("-", instructionIndexOnRock0.Select(x => x.Item1 + " (" + x.Item2 + ")")));
                if (!isPatternvalidated && instructionIndexOnRock0.Exists(x => x.Item1 == indexIteration) && (startPatternIndex < 0  || indexIteration == startPatternIndex))
                {
                    Debug.LogWarning("MAY WE HAVE A PATTERN ?? (" + startPatternIndex + ") at rock " + rockCount );
                    double patternStart = instructionIndexOnRock0.FindLast(x => x.Item1 == indexIteration).Item2;
                    double tallPoint = cavern.Select(x => x.LastIndexOf('#')).Max() + 1;
                    Debug.Log("With index at " + indexIteration + " | start : " + patternStart + " | end : " + tallPoint + " | Diff : " + (tallPoint - patternStart).ToString());

                    if (startPatternIndex >= 0)
                    { // pattern confirmation
                        Debug.LogWarning("And old pattern gap was " + diffPattern);
                        if (diffPattern == (tallPoint - patternStart))
                        {
                            Debug.LogWarning("YES");

                            double period = rockCount - startPatternRockCount;
                            double cycleNumber = System.Math.Floor((totalRockWanted - rockCount) / period) -1;      // period remaining (-1 to finish manually)

                            heightOffset = cycleNumber * diffPattern;       // Store the height of all 
                            rockCount += cycleNumber * period;
                            Debug.LogWarning("Period is : " + period + " | total Rock wanted is " + totalRockWanted + " | Cycles remaining : " + cycleNumber);
                            Debug.LogWarning("And now rockcount is at " + rockCount + " with offset " + heightOffset + "; just finish now");

                            isPatternvalidated = true;
                        }
                    }

                    startPatternIndex = indexIteration;
                    diffPattern = (tallPoint - patternStart);
                    startPatternRockCount = rockCount;
                }
                instructionIndexOnRock0.Add((indexIteration,cavern.Select(x => x.LastIndexOf('#')).Max() + 1));
            }


            int heighestPoint = cavern.Select(x => Mathf.Max(x.LastIndexOf('@'), x.LastIndexOf('#'))).Max() + 4;
            Vector2Int botLeftPos = new Vector2Int(2, heighestPoint);

            if (rockCount < 10)
            {
                string logR = "";
                for (int i = rockShape.Length -1; i >= 0; i--)
                {
                    logR += rockShape[i] + System.Environment.NewLine;
                }
                Debug.Log(logR);
            }
            bool canFall = true;
            int safetyCount = heighestPoint + 10;
            while (canFall && safetyCount > 0)
            {
                safetyCount--;

                //gas jet instruction
                if (instruction[indexIteration] == '<' && (botLeftPos.x > 0))
                {
                    bool canMoveLeft = botLeftPos.x > 0;
                    for (int i = 0; i < rockShape.Length; i++)
                    {
                        if (!canMoveLeft)
                            break;

                        int firstIndexFull = rockShape[i].IndexOf('#');
                        if (firstIndexFull < 0)
                            Debug.LogError("WTF can't happen " + rockShape[i]);
                        else if (botLeftPos.x + firstIndexFull - 1 < 0)
                            Debug.LogError("WTF can't happen " + botLeftPos.ToString() + " | " + rockShape[i] + " | " + firstIndexFull + "," + i + ", " + gridWidth);

                        canMoveLeft &= (cavern[botLeftPos.x + firstIndexFull - 1].Count <= botLeftPos.y +i || cavern[botLeftPos.x +firstIndexFull - 1][botLeftPos.y +i] == '.');
                    }
                    if (canMoveLeft)
                        botLeftPos.x -= 1;
                    //else
                    //    Debug.Log("cant move left");
                }
                else if (instruction[indexIteration] == '>')
                {
                    bool canMoveRight = botLeftPos.x + rockShape[0].Length < gridWidth;
                    for (int i = 0; i < rockShape.Length; i++)
                    {
                        if (!canMoveRight)
                            break;

                        int lastIndexFull = rockShape[i].LastIndexOf('#');
                        if (lastIndexFull < 0)
                            Debug.LogError("WTF can't happen " + rockShape[i]);
                        else if (botLeftPos.x + lastIndexFull+1 >= gridWidth)
                            Debug.LogError("WTF can't happen " + botLeftPos.ToString() + " | " + rockShape[i] + " | " + lastIndexFull + "," + i + ", " + gridWidth);

                        canMoveRight &= (botLeftPos.x + lastIndexFull + 1 < gridWidth) && (cavern[botLeftPos.x + lastIndexFull+1].Count <= botLeftPos.y+i || cavern[botLeftPos.x + lastIndexFull+1][botLeftPos.y +i] == '.'); 
                    }
                    if (canMoveRight)
                        botLeftPos.x += 1;
                    //else
                    //    Debug.Log("cant move right");
                }

                indexIteration += 1;
                indexIteration %= instruction.Length;


                // falling bot
                for (int i = 0; i < rockShape[0].Length; i++)
                {
                    if (rockShape[0][i] == '.')
                    {   // test with line 1, happens only with rock + form
                        if (cavern[botLeftPos.x + i].Count >= botLeftPos.y + 1 && cavern[botLeftPos.x + i][botLeftPos.y] != '.')
                        {
                            canFall = false;
                            break;
                        }
                        else
                            continue;
                    }

                    if (botLeftPos.y == 0 || (cavern[botLeftPos.x + i].Count >= botLeftPos.y && cavern[botLeftPos.x + i][botLeftPos.y - 1] != '.'))
                    {
                        // Stop falling
                        canFall = false;
                        break;
                    }
                }
                if (canFall)
                    botLeftPos.y -= 1;
                //Debug.Log(botLeftPos + " - " + canFall.ToString());
            }

            if (canFall)
            {
                Debug.LogError("Weird, shouldnt be here - " + safetyCount + " - " + botLeftPos.ToString());
                continue;
            }

            for (int rockHeight = 0; rockHeight < rockShape.Length; rockHeight++)
            {
                for (int i = 0; i < gridWidth; i++)
                {
                    char c = '.';
                    if (i >= botLeftPos.x && i < botLeftPos.x + rockShape[rockHeight].Length)
                        c = rockShape[rockHeight][i - botLeftPos.x]; //!= '.' ? (rockCount % 2 == 0 ? '#' : '@') : '.';

                    if (botLeftPos.y + rockHeight < cavern[i].Count)
                    {
                        if (i >= botLeftPos.x && i < botLeftPos.x + rockShape[rockHeight].Length)
                            cavern[i][botLeftPos.y + rockHeight] = c;
                    }
                    else
                    {
                        if (i < botLeftPos.x)
                            cavern[i].Add('.');
                        else if (i < botLeftPos.x + rockShape[rockHeight].Length)
                            cavern[i].Add(c);
                        else
                            cavern[i].Add('.');
                    }
                }
            }

            if (rockCount < 10)
                Debug.Log(printCavern(cavern));
        }

        double tallestPoint = cavern.Select(x => Mathf.Max(x.LastIndexOf('#'), x.LastIndexOf('@'))).Max() + 1;
        Debug.LogWarning(tallestPoint);
        Debug.LogWarning("Tallest Point " + tallestPoint + " with height offset " + heightOffset + " => total : " + (tallestPoint + heightOffset).ToString());

        //Debug.Log(printCavern(cavern));

        //return tallestPoint.ToString();
    }



    string[] getRockShape(double rockCount)
    {
        switch (rockCount % 5)
        {
            case 0: return new string[] { "####" };
            case 1: return new string[] { ".#.", "###", ".#." };
            case 2: return new string[] { "###", "..#", "..#" };
            case 3: return new string[] { "#", "#", "#", "#" };
            case 4: return new string[] { "##", "##" };
        }

        Debug.LogError("getRockShape, wtf ! " + rockCount);
        return new string[0];
    }

    string printCavern(List<List<char>> cavern)
    {
        string log = System.Environment.NewLine + "|.......|" + System.Environment.NewLine;
        int min = cavern.Select(x => x.Count).Min();
        for (int i = min - 1; i >= 0; i--)
        {
            log += "|" + String.Join("", cavern.Select(x => x[i])) + "|" + System.Environment.NewLine;
        }
        log += "|-------|";

        return log;
    }
}
