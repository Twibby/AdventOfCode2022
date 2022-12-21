using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Day_2022_17 : DayScript2022
{
    protected override string part_1()
    {
        return auxDay17(2022);
    }

    protected override string part_2()
    {
        //  1.000.000.000.000
        return auxDay17(1000000);
    }

    string auxDay17(int totalRockWanted)
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
        for (int rockCount = 0; rockCount < totalRockWanted; rockCount++)
        {
            //Debug.LogWarning("Starting rock " + (rockCount + 1).ToString());

            string[] rockShape = getRockShape(rockCount);
            int heighestPoint = cavern.Select(x => Mathf.Max(x.LastIndexOf('@'), x.LastIndexOf('#'))).Max() + 4;
            IntVector2 botLeftPos = new IntVector2(2, heighestPoint);

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

        int tallestPoint = cavern.Select(x => Mathf.Max(x.LastIndexOf('#'), x.LastIndexOf('@'))).Max() + 1;

        //Debug.Log(printCavern(cavern));

        Debug.Log(printCavern(cavern));

        return tallestPoint.ToString();
    }



    string[] getRockShape(int rockCount)
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