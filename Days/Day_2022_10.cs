using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Day_2022_10 : DayScript2022
{
    Dictionary<int, int> cyclesToRegister = new Dictionary<int, int>();

    protected override string part_1()
    {
        int cycleCount = 0;
        int X = 1;

        cyclesToRegister = new Dictionary<int, int>();
        cyclesToRegister.Add(20, 0);
        cyclesToRegister.Add(60, 0);
        cyclesToRegister.Add(100, 0);
        cyclesToRegister.Add(140, 0);
        cyclesToRegister.Add(180, 0);
        cyclesToRegister.Add(220, 0);

        foreach (string instruction in _input.Split('\n'))
        {
            cycleCount++;
            registerValue(cycleCount, X);
            
            if (instruction.StartsWith("addx"))
            {
                cycleCount++;
                registerValue(cycleCount, X);
                X += int.Parse(instruction.Substring(5));
            }
        }

        foreach (var tmp in cyclesToRegister)
        {
            Debug.Log(tmp.Key + "->" + tmp.Value + "  ===>  " + (tmp.Key * tmp.Value).ToString());
        }
        int res = cyclesToRegister.Select(x => x.Key * x.Value).Sum();

        return res.ToString();
    }

    void registerValue(int cycle, int val)
    {
        if (IsTestInput)
            Debug.Log("During cycle " + cycle + ", X is " + val);

        if (cyclesToRegister.ContainsKey(cycle))
        {
            cyclesToRegister[cycle] = val;
        }
    }

    string drawing = "";

    protected override string part_2()
    {
        int cycleCount = 0;
        int X = 1;

        foreach (string instruction in _input.Split('\n'))
        {
            cycleCount++;
            draw(cycleCount, X);

            if (instruction.StartsWith("addx"))
            {
                cycleCount++;
                draw(cycleCount, X);
                X += int.Parse(instruction.Substring(5));
            }
        }
        Debug.Log(drawing);
        return base.part_2();
    }

    void draw(int cnt, int val)
    {
        if (IsTestInput)
            Debug.Log("Cycle " + cnt + ", draw at pos " + (cnt - 1).ToString() + " | X val is " + val.ToString());

        if (Mathf.Abs(((cnt-1)%40) - val) < 2)
            drawing += "#";
        else
            drawing += ".";

        if (cnt % 40 == 0)
        {
            drawing += System.Environment.NewLine;
        }
    }
}
