using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Day_2022_03 : DayScript2022
{
    protected override string part_1()
    {
        int totalScore = 0;
        foreach (string rucksack in _input.Split('\n'))
        {
            string leftItems = rucksack.Substring(0, rucksack.Length / 2);
            string rightItems = rucksack.Substring(rucksack.Length / 2);

            foreach (char c in leftItems)
            {
                if (rightItems.Contains(c))
                {
                    totalScore += getScoreItem(c);
                    break;
                }
            }
        }
        return totalScore.ToString();
    }

    protected override string part_2()
    {
        int totalScore = 0;

        int index = 0;
        List<string> groupSacks = new List<string>();
        foreach (string rucksack in _input.Split('\n'))
        {
            groupSacks.Add(rucksack);
            index++;
            if (index >= 3)
            {
                totalScore += getScoreItem(getCommonChar(groupSacks));

                groupSacks = new List<string>();
                index = 0;
            }
        }

        return totalScore.ToString();
    }

    int getScoreItem(char item)
    {
        if (item >= 'a')
            return (int)(item - 'a') + 1;
           
        return (int)(item - 'A') + 27;
    }

    char getCommonChar(List<string> strings)
    {
        if (strings.Count != 3)
        {
            Debug.LogError("weird");
            return '1';
        }

        char result = '1';

        foreach (char c in strings[0])
        {
            if (strings[1].Contains(c) && strings[2].Contains(c) )
            {
                result = c;
                break;
            }
        }

        return result;
    }
}
