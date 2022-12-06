using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Day_2022_06 : DayScript2022
{
    protected override string part_1()
    {
        return getNoDuplicationSubtring(4);
    }
    protected override string part_2()
    {
        return getNoDuplicationSubtring(14);
    }

    string getNoDuplicationSubtring(int length)
    { 
        int index = 0;
        while (index < _input.Length -(length-1))
        {
            if (hasDuplication(_input.Substring(index, length)))
                break;

            index++;
        }

        return (index + length).ToString();
    }

    bool hasDuplication(string packet)
    {
        for (int i = 0; i < packet.Length-1; i++)
        {
            if (packet.Substring(i + 1).Contains(packet[i]))
                return false;
        }

        return true;
    }
}
