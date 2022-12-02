using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Day_2022_02 : DayScript2022
{
    protected override string part_1()
    {
        int totalScore = 0;

        foreach (string round in _input.Split('\n'))
        {
            totalScore += (int)(round[2] - 'W');    // item Score, X=1 Y=2 Z=3

            int issue = (int)(round[2] - round[0]) %3;
            switch (issue)
            {
                case 0: //win  +6points
                    totalScore += 6; break;
                case 1: //loss  +0points
                    totalScore += 0; break;
                case 2: //draw +3points
                    totalScore += 3; break;
                default:
                    Debug.LogWarning("wtf! " + round + " | " + issue);
                    break;
            }
        }

        return totalScore.ToString();        
    }

    protected override string part_2()
    {
        int totalScore = 0;

        foreach (string round in _input.Split('\n'))
        {
            totalScore += (int)(round[2] - 'X') * 3; // battle Issue, X=win=0 Y=draw=3 Z=win=6

            int itemPicked = (int)(round[0] + round[2] - 'Y' +3 -'A') %3;
            totalScore += itemPicked + 1;   // 0 means rock so 1 point, etc..
        }

        return totalScore.ToString();
    }
}
