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
            totalScore += (int)(round.Last() - 'W');    // item Score, X=1 Y=2 Z=3

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
            totalScore += (int)(round.Last() - 'X') * 3; // battle Issue, X=win=0 Y=draw=3 Z=win=6

            int itemPicked = (int)(round[0] + round[2] - 'Y' +3 -'A') %3;

            switch (itemPicked)
            {
                case 0: //rock  +1point
                    totalScore += 1; break;
                case 1: //paper  +2points
                    totalScore += 2; break;
                case 2: //scissors +3points
                    totalScore += 3; break;
            }
        }

        return totalScore.ToString();
    }
}
