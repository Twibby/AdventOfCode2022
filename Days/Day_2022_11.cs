using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Day_2022_11 : DayScript2022
{
    public static List<Monkey> monkeys;
    public static int ppcm = 1;

    protected override string part_1()
    {
        monkeys = new List<Monkey>();
        foreach (string mkStr in _input.Split("\n\n", System.StringSplitOptions.RemoveEmptyEntries))
        {
            monkeys.Add(new Monkey(mkStr));
        }

        int roundCount = 20;
        for (int i = 0; i < roundCount; i++)
        {
            foreach (Monkey mk in monkeys)
            {
                mk.InspectItems();
            }

            Debug.LogWarning("After round " + (i + 1).ToString() + ":" + System.Environment.NewLine + String.Join(System.Environment.NewLine, monkeys.Select(x => "Monkey " + x.id + ": " + String.Join(", ", x.itemsHold))));
        }

        monkeys.Sort(
            delegate (Monkey a, Monkey b) { return b.inspectionCount.CompareTo(a.inspectionCount); }
            );

        long res = monkeys[0].inspectionCount * monkeys[1].inspectionCount;
        Debug.Log("res is " + monkeys[0].inspectionCount + " x " + monkeys[1].inspectionCount + " ==> " + (monkeys[0].inspectionCount * monkeys[1].inspectionCount).ToString());

        return res.ToString();
    }

    protected override string part_2()
    {
        monkeys = new List<Monkey>();
        ppcm = 1;
        foreach (string mkStr in _input.Split("\n\n", System.StringSplitOptions.RemoveEmptyEntries))
        {
            Monkey mk = new Monkey(mkStr);
            monkeys.Add(mk);
            ppcm *= mk.testDivisor;     // works because they are all primes in my input

        }

        int roundCount = 10000;
        for (int i = 0; i < roundCount; i++)
        {
            foreach (Monkey mk in monkeys)
            {
                mk.InspectItems(false);
            }

            if ((i+1)%1000 == 0)
                Debug.LogWarning("== After round " + (i + 1).ToString() + " ==" + System.Environment.NewLine + String.Join(System.Environment.NewLine, monkeys.Select(x => "Monkey " + x.id + " inspected items " + x.inspectionCount + " times.")));
        }

        monkeys.Sort(
            delegate (Monkey a, Monkey b) { return b.inspectionCount.CompareTo(a.inspectionCount); }
            );

        long res = monkeys[0].inspectionCount * monkeys[1].inspectionCount;
        Debug.Log("res is " + monkeys[0].inspectionCount + " x " + monkeys[1].inspectionCount + " ==> " + (monkeys[0].inspectionCount * monkeys[1].inspectionCount).ToString());

        return res.ToString();
    }

    public class Monkey
    {
        public int id;
        public List<long> itemsHold;
        public string formula;
        public int testDivisor;
        public int monkeyIdIfTrue;
        public int monkeyIdIfFalse;

        public long inspectionCount;

        public Monkey(string input)
        {
            string[] raws = input.Split('\n');
            this.id = int.Parse(raws[0].Substring(7, raws[0].Length - 8));
            this.itemsHold = raws[1].Substring(raws[1].IndexOf(':') + 1).Split( ", " ).Select(x => long.Parse(x)).ToList();
            this.formula = raws[2].Substring(raws[2].IndexOf('=') + 2);
            this.testDivisor = int.Parse(raws[3].Split(' ').Last());
            this.monkeyIdIfTrue = int.Parse(raws[4].Split(' ').Last());
            this.monkeyIdIfFalse = int.Parse(raws[5].Split(' ').Last());

            this.inspectionCount = 0;

            Debug.Log(this.ToString());
        }

        public override string ToString()
        {
            string res = "Monkey " + id + " has item : " + String.Join(", ", this.itemsHold) 
                + System.Environment.NewLine + "Formula : " + formula
                + System.Environment.NewLine + "Is divisible by " + this.testDivisor + " ? " + this.monkeyIdIfTrue + " : " + this.monkeyIdIfFalse;
            return res;
        }

        public void InspectItems(bool isGettingBored = true, bool debug = false)
        {
            if (debug)
                Debug.Log("Monkey " + this.id + ":");

            foreach (long item in this.itemsHold)
            {
                inspectionCount++;
                
                if (debug)
                    Debug.Log("Monkey inspects item with level of " + item);

                long newItemVal = -1;
                switch (this.formula)       // Flemme de faire un parser...
                {
                    case "old * 11": newItemVal = item * 11; break;
                    case "old + 1": newItemVal = item + 1; break;
                    case "old * 7": newItemVal = item * 7; break;
                    case "old + 3": newItemVal = item + 3; break;
                    case "old + 6": newItemVal = item + 6; break;
                    case "old + 5": newItemVal = item + 5; break;
                    case "old * old": newItemVal = item * item; break;
                    case "old + 7": newItemVal = item + 7; break;

                    case "old * 19": newItemVal = item * 19; break; // for test input
                    default:
                        Debug.LogWarning("weird : '" + this.formula + "'"); break;
                }

                if (debug)
                    Debug.Log("New Item val is " + newItemVal);

                if (isGettingBored)
                {
                    newItemVal /= 3;
                    if (debug)
                        Debug.Log("After getting bored -> " + newItemVal);
                }

                newItemVal %= ppcm;     // Doesn't change any result of tests because ppcm%any dividorTest is 0, and this allows to avoid too big numbers

                if (newItemVal % testDivisor == 0)
                {
                    monkeys[monkeyIdIfTrue].itemsHold.Add(newItemVal);
                    if (debug)
                        Debug.Log("Divisible, sent to " + monkeyIdIfTrue);                    
                }
                else
                {
                    monkeys[monkeyIdIfFalse].itemsHold.Add(newItemVal); 
                    if (debug)
                        Debug.Log("NOT Divisible, sent to " + monkeyIdIfFalse);                    
                }
            }

            this.itemsHold = new List<long>();      // No monkey send an item to himself so after inspecting everything he has no items left
        }
    }
}
