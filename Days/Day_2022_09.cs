using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Day_2022_09 : DayScript2022
{
    protected override string part_1()
    {
        return snake(2).ToString();
    }

    protected override string part_2()
    {
        return snake(10).ToString();
    }

    int snake (int numberOfNodes)
    { 
        // Init snake (list of nodes)
        List<Vector2> nodes = new List<Vector2>();
        for (int i = 0; i < numberOfNodes; i++)
        {
            nodes.Add(Vector2.zero);
        }

        List<Vector2> allPos = new List<Vector2>();
        allPos.Add(nodes[numberOfNodes-1]);

        // go through each instruction
        foreach (string instruction in _input.Split('\n'))
        {
            string[] inst = instruction.Split(' ');
            if (inst.Length != 2)
            {
                Debug.LogError("Incorrect instruction line ! '" + instruction + "'");
                continue;
            }

            for (int i = 0; i < int.Parse(inst[1]); i++)
            {
                // move head by one in wanted direction
                Vector2 head = nodes[0];
                switch (inst[0])
                {
                    case "L": head.x -= 1; break;
                    case "R": head.x += 1; break;
                    case "U": head.y += 1; break;
                    case "D": head.y -= 1; break;
                    default:
                        Debug.LogError("Incorrect instrcution SIDE line ! '" + instruction + "'");
                        break;
                }
                nodes[0] = head;

                // for each node, check if distance with previous node is too big and currentNode has to move to get closer
                for (int nodeCnt = 1; nodeCnt < numberOfNodes; nodeCnt++)
                {
                    if (Vector2.Distance(nodes[nodeCnt-1], nodes[nodeCnt]) > Mathf.Sqrt(2))
                    {
                        Vector2 prevNode = nodes[nodeCnt - 1];
                        Vector2 curNode = nodes[nodeCnt];
                        if (curNode.x == prevNode.x)
                            curNode.y = (curNode.y + prevNode.y) / 2;
                        else if (curNode.y == prevNode.y)
                            curNode.x = (curNode.x + prevNode.x) / 2;
                        else
                        {
                            curNode.x += (prevNode.x > curNode.x ? 1 : -1);
                            curNode.y += (prevNode.y > curNode.y ? 1 : -1);
                        }

                        nodes[nodeCnt] = curNode;
                    }
                    else
                        break;
                }

                if (!allPos.Contains(nodes[numberOfNodes-1]))
                    allPos.Add(nodes[numberOfNodes - 1]);

            }
        }

        return allPos.Count;
    }
}
