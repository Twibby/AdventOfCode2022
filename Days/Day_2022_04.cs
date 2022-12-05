using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Day_2022_04 : DayScript2022
{
    protected override string part_1()
    {
        int res = 0;
        foreach (string elves in _input.Split('\n'))
        {
            List<int> bounds = elves.Split(new char[] { '-', ',' }).Select(x => int.Parse(x)).ToList();

            if ((bounds[0] >= bounds[2] && bounds[1] <= bounds[3])
                ||
                (bounds[0] <= bounds[2] && bounds[1] >= bounds[3]))
                res++;

        }
        return res.ToString();
    }

    protected override string part_2()
    {
        int res = 0;
        foreach (string elves in _input.Split('\n'))
        {
            List<int> bounds = elves.Split(new char[] { '-', ',' }).Select(x => int.Parse(x)).ToList();

            if ((bounds[0] <= bounds[2] && bounds[1] >= bounds[2]) ||
                (bounds[0] <= bounds[3] && bounds[1] >= bounds[3]) ||
                (bounds[0] >= bounds[2] && bounds[1] <= bounds[3]))
                res++;

        }
        return res.ToString();
    }
}
