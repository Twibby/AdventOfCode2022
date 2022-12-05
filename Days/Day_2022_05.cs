using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Day_2022_05 : DayScript2022
{
    protected override string part_1()
    {
        List<string> pilesInput = _input.Split(new string[] { "\n\n" }, System.StringSplitOptions.RemoveEmptyEntries)[0].Split('\n').ToList();
        pilesInput.Reverse();

        int pileCount = int.Parse(pilesInput[0].Split(' ', System.StringSplitOptions.RemoveEmptyEntries).Last());
        pilesInput.RemoveAt(0);

        List<Stack<char>> piles = new List<Stack<char>>();
        for (int i = 0; i < pileCount; i++)
        {
            piles.Add(new Stack<char>());
        }

        // init piles
        foreach (string level in pilesInput)
        {
            for (int i = 0; i < pileCount; i++)
            {
                int index = i * 4 + 1;
                if (level.Length > index && level[index] != ' ')
                    piles[i].Push(level[index]);
            }
        }

        foreach (string instruction in _input.Split(new string[] { "\n\n" }, System.StringSplitOptions.RemoveEmptyEntries)[1].Split('\n'))
        {
            string[] splits = instruction.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
            if (splits.Length != 6)
                Debug.LogError("instruction not correctly formated : " + instruction);

            int cnt = int.Parse(splits[1]);
            int startPile = int.Parse(splits[3]) - 1;
            int endPile = int.Parse(splits[5]) - 1;

            for (int i = 0; i < cnt; i++)
            {
                char c = piles[startPile].Pop();
                piles[endPile].Push(c);
            }
        }
        return String.Join("", piles.Select(x => x.Peek()));
    }

    protected override string part_2()
    {
        List<string> pilesInput = _input.Split(new string[] { "\n\n" }, System.StringSplitOptions.RemoveEmptyEntries)[0].Split('\n').ToList();
        pilesInput.Reverse();
        
        int pileCount = int.Parse(pilesInput[0].Split(' ', System.StringSplitOptions.RemoveEmptyEntries).Last());
        pilesInput.RemoveAt(0);

        List<Stack<char>> piles = new List<Stack<char>>();
        for (int i = 0; i < pileCount; i++)
        {
            piles.Add(new Stack<char>());
        }

        // init piles
        foreach (string level in pilesInput)
        {
            for (int i = 0; i < pileCount; i++)
            {
                int index = i * 4 + 1;
                if (level.Length > index && level[index] != ' ')
                    piles[i].Push( level[index] );
            }
        }

        // start to make instructions
        foreach (string instruction in _input.Split(new string[] { "\n\n" }, System.StringSplitOptions.RemoveEmptyEntries)[1].Split('\n'))
        {
            string[] splits = instruction.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
            if (splits.Length != 6)
                Debug.LogError("instruction not correctly formated : " + instruction);

            int cnt = int.Parse(splits[1]);
            int startPile = int.Parse(splits[3]) - 1;
            int endPile = int.Parse(splits[5]) - 1;

            List<char> crates = new List<char>();
            for (int i = 0; i < cnt; i++)
            {
                crates.Add(piles[startPile].Pop());
            }

            crates.Reverse();
            foreach (char c in crates)
            {
                piles[endPile].Push(c);
            }
        }
        return String.Join("", piles.Select(x => x.Peek()));
    }
}
