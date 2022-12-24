using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class DayAnimation_2022_17 : DayAnimationScript
{
    public List<GameObject> RocksPrefabs;
    public Transform RockParent;

    public TMP_Text instructionLabel;

    public TMP_Text RockCountLabel;
    public TMP_Text MaxHeightLabel;

    public float timerInterval = 0.01f;

    public override IEnumerator part_1()
    {
        for (int i = RockParent.childCount -1; i >= 0; i--)
        {
            Destroy(RockParent.GetChild(i).gameObject);
        }

        yield return new WaitForEndOfFrame();

        int maxHeight = 0;

        instructionLabel.text = input;
        RockCountLabel.text = "0";
        MaxHeightLabel.text = maxHeight.ToString();

        Debug.Log("Starting Animation");
        yield return new WaitForSeconds(1f);


        int gridWidth = 7;
        int indexIteration = 0;
        string instruction = input;

        List<List<char>> cavern = new List<List<char>>(); // represents columns
        for (int i = 0; i < gridWidth; i++)
        {
            cavern.Add(new List<char>());
        }

        Color[] colors = new Color[] { Color.grey, Color.white, Color.blue, Color.yellow, Color.green, Color.magenta, Color.cyan };

        printCavern(cavern);
        Debug.LogWarning(cavern.Count);
        for (int rockCount = 0; rockCount < 2022; rockCount++)
        {
            //Debug.LogWarning("Starting rock " + (rockCount + 1).ToString());
            RockCountLabel.text = (rockCount + 1).ToString();

            instructionLabel.text = instruction.Substring(0, indexIteration) + "<color=green><b>" + instruction[indexIteration] + "</b></color>" + instruction.Substring(indexIteration + 1);

            string[] rockShape = getRockShape(rockCount);
            int heighestPoint = cavern.Select(x => Mathf.Max(x.LastIndexOf('@'), x.LastIndexOf('#'))).Max() + 4;
            Vector2Int botLeftPos = new Vector2Int(2, heighestPoint);

            GameObject rock =  GameObject.Instantiate(RocksPrefabs[rockCount % RocksPrefabs.Count], new Vector3(2, heighestPoint, 0), Quaternion.identity, RockParent);
            //Color tmp = new Color(UnityEngine.Random.Range(0, 255f) / 255f, UnityEngine.Random.Range(0, 255f) / 255f, UnityEngine.Random.Range(0, 255f) / 255f);
            Color tmp = colors[rockCount % colors.Length];
            foreach (var cube in rock.GetComponentsInChildren<MeshRenderer>()) { cube.material.color = tmp; }

            adjustCamera(botLeftPos.y);

            yield return new WaitForSeconds(timerInterval);

            if (rockCount == 162)
                Debug.Log("toto");

            bool canFall = true;
            int safetyCount = heighestPoint + 10;
            while (canFall && safetyCount > 0)
            {
                safetyCount--;

                instructionLabel.text = instruction.Substring(0, indexIteration) + "<color=green><b>" + instruction[indexIteration] + "</b></color>" + instruction.Substring(indexIteration + 1);

                rock.GetComponentInChildren<TMP_Text>().text = instruction[indexIteration].ToString();
                bool canMoveLeft = false, canMoveRight = false;
                //gas jet instruction
                if (instruction[indexIteration] == '<' && (botLeftPos.x > 0))
                {
                    canMoveLeft = botLeftPos.x > 0;
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
                    {
                        botLeftPos.x -= 1;
                        rock.transform.DOMoveX(botLeftPos.x, timerInterval * 2f / 3f);  //rock.transform.Translate(new Vector3(-1, 0, 0));
                    }
                    //else
                    //    Debug.Log("cant move left");
                }
                else if (instruction[indexIteration] == '>')
                {
                    canMoveRight = botLeftPos.x + rockShape[0].Length < gridWidth;
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
                    {
                        botLeftPos.x += 1;
                        rock.transform.DOMoveX(botLeftPos.x, timerInterval*2f/3f); //.Translate(new Vector3(1, 0, 0));
                    }
                    //else
                    //    Debug.Log("cant move right");
                }

                rock.GetComponentInChildren<TMP_Text>().color = (canMoveLeft || canMoveRight ? Color.black : Color.red);


                indexIteration += 1;
                indexIteration %= instruction.Length;

                yield return new WaitForSeconds(timerInterval);


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
                {
                    botLeftPos.y -= 1;
                    //rock.transform.Translate(new Vector3(0, -1, 0));
                    rock.transform.DOMoveY(botLeftPos.y, timerInterval * 2f / 3f);
                    adjustCamera(botLeftPos.y);

                    yield return new WaitForSeconds(timerInterval);
                }
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

            maxHeight = Mathf.Max(maxHeight, botLeftPos.y + rockShape.Length);
            MaxHeightLabel.text = maxHeight.ToString();
            int tmpHeight = cavern.Select(x => x.LastIndexOf('#')).Max() + 1;
            if (tmpHeight != maxHeight)
            {
                Debug.LogError("wtf, " + tmpHeight + " , " + maxHeight + " | " + botLeftPos.ToString() + System.Environment.NewLine + printCavern(cavern));
            }
        }

        int tallestPoint = cavern.Select(x => Mathf.Max(x.LastIndexOf('#'), x.LastIndexOf('@'))).Max() + 1;

        //Debug.Log(printCavern(cavern));

        //return tallestPoint.ToString();

    }

    void adjustCamera(int rockY)
    {
        if (Camera.current == null)
        {
            Debug.LogWarning("wat");
            return;
        }
        float val = Mathf.Lerp(Camera.current.transform.position.y, rockY, 0.3f);
        Camera.current.transform.DOMoveY(val, 2*timerInterval);
        //if (Mathf.Abs(rockY - Camera.current.transform.position.y) > 4f)
        //    Camera.current.transform.DOMoveY(rockY, timerInterval);
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
