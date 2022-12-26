using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Day_2022_25 : DayScript2022
{
    // Already answered :   2=000=22-0-1=1-=22-1       for number  30508250245921
                            
    protected override string part_1()
    {
        double result = 0;

        foreach (string fuel in _input.Split('\n'))
        {
            result += snafuToInt(fuel);
        }

        Debug.Log("Final result : " + result + " => " + intToSnafu(result));
        return intToSnafu(result).ToString();
    }

    protected override string part_2()
    {
        return base.part_2();
    }

    double snafuToInt(string snafu)
    {
        double result = 0;
        for (int i = 0; i <snafu.Length; i++)
        {
            char c = snafu[snafu.Length - 1 - i];
            double power = System.Math.Pow(5, i);
            int val = -1;
            if (int.TryParse(c.ToString(), out val))
                result += val * power;
            else if (c == '-')
                result -= power;
            else if (c == '=')
                result -= 2 * power;
            else
                Debug.LogError("wtf ? " + snafu + " - '" + c + "'");
        }
        return result;
    }

    string intToSnafu(double number)
    {

        List<double> remainders = new List<double>();
        double copy = number;
        while (copy > 0)
        {
            double remain = copy - System.Math.Floor(copy / 5) * 5;
            copy = System.Math.Floor(copy/5);

            remainders.Add(remain);
        }
        
        List<string> snafuChar = new List<string>();
        for (int i = 0; i < remainders.Count; i++)
        {
            if (remainders[i] <= 2)
            {
                snafuChar.Add(remainders[i].ToString());
            }
            else
            {
                if (remainders[i] == 3)
                    snafuChar.Add("=");
                else if (remainders[i] == 4)
                    snafuChar.Add("-");
                else if (remainders[i] == 5)
                    snafuChar.Add("0");
                else
                    Debug.LogError("WTF ?? " + remainders[i]);

                if (i >= remainders.Count - 1)
                    snafuChar.Add("1");
                else
                    remainders[i + 1] += 1;
            }
        }
        snafuChar.Reverse();
        return System.String.Join("", snafuChar);
    }
}
