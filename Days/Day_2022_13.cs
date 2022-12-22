using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Day_2022_13 : DayScript2022
{
    protected override string part_1()
    {
        int index = 1;
        int result = 0;
        foreach (string instruction in _input.Split(new string[] { "\n\n" }, System.StringSplitOptions.RemoveEmptyEntries))
        {
            Packet p1 = new Packet(instruction.Split('\n')[0]);
            Packet p2 = new Packet(instruction.Split('\n')[1]);

            Debug.Log(p1.Compare(p2));
            if (p1.Compare(p2) < 0)
                result += index;

            index++;
        }

        return result.ToString();
    }

    protected override string part_2()
    {
        List<Packet> allPackets = new List<Packet>();
        foreach (string instruction in _input.Split(new string[] { "\n\n" }, System.StringSplitOptions.RemoveEmptyEntries))
        {
            Packet p1 = new Packet(instruction.Split('\n')[0]);
            Packet p2 = new Packet(instruction.Split('\n')[1]);

            allPackets.Add(p1);
            allPackets.Add(p2);
        }

        allPackets.Add(new Packet("[[2]]", true));
        allPackets.Add(new Packet("[[6]]", true));

        allPackets.Sort(delegate (Packet p1, Packet p2) { return p1.Compare(p2); });

        int index1 = allPackets.FindIndex(x => x.isLookedPacket) +1;
        int index2 = allPackets.FindLastIndex(x => x.isLookedPacket) +1;

        int total = index1 * index2;

        Debug.Log(index1 + " x " + index2 + " => " + total);

        return total.ToString();
    }

    class Packet
    {
        public bool isInt;
        public int intVal;
        public List<Packet> listVal;

        public bool isLookedPacket = false;

        public Packet(string rawInput, bool isLookedPacket = false)
        {
            this.isLookedPacket = isLookedPacket;
            //Debug.Log("Init packet : " + rawInput);
            if (rawInput[0] == '[')
            {
                
                this.isInt = false;

                listVal = new List<Packet>();
                int safetyCount = 1000;

                rawInput = rawInput.Substring(1, rawInput.Length - 2);

                int index = 0;
                while (index < rawInput.Length && safetyCount > 0 )
                {
                    if (rawInput[index] == ']')
                        break;  // end of list

                    if (rawInput[index] == '[')
                    {
                        int startIndex = index;
                        int cnt = 1;
                        index++;
                        while (cnt > 0 && index < rawInput.Length)
                        {
                            if (rawInput[index] == '[')
                                cnt++;
                            else if (rawInput[index] == ']')
                                cnt--;

                            index++;
                        }
                        listVal.Add(new Packet(rawInput.Substring(startIndex, index - startIndex)));
                        index++;
                    }
                    else if (rawInput[index] == ',' || rawInput[index] == ' ')
                    {
                        index++;
                    }
                    else
                    {
                        int comaIndex = rawInput.IndexOf(',', index);
                        if (comaIndex < 0)
                        {
                            //Debug.LogError("pezozerzer | " + rawInput + " | " + index);
                            comaIndex = rawInput.Length;
                        }
                        listVal.Add(new Packet(rawInput.Substring(index, comaIndex - index)));
                        index = comaIndex;
                    }

                }

                if (safetyCount <= 0)
                    Debug.LogError("incorrect parsing");
            }
            else
            {
                this.isInt = true;
                intVal = int.Parse(rawInput);
            }
        }

        public override string ToString()
        {
            if (this.isInt)
                return this.intVal + "(i)";

            return "[" + String.Join(", ", listVal.Select(x => x.ToString())) + "]";
        }

        public int Compare(Packet other)
        {
            if (this.isInt && other.isInt)
                return this.intVal.CompareTo(other.intVal);

            if (!this.isInt && !other.isInt)
            {
                for (int i = 0; i < Mathf.Min(this.listVal.Count, other.listVal.Count); i++)
                {
                    int cmpVal = this.listVal[i].Compare(other.listVal[i]);
                    if (cmpVal != 0)
                        return cmpVal;
                }

                return this.listVal.Count.CompareTo(other.listVal.Count);
            }

            if (this.isInt)
                return new Packet("[" + this.intVal + "]").Compare(other);
            else
                return this.Compare(new Packet("[" + other.intVal + "]"));
        }
    }
}
