using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Day_2022_18 : DayScript2022
{
    protected override string part_1()
    {
        List<Vector3> droplets = new List<Vector3>();
        foreach (string instruction in _input.Split('\n'))
        {
            string[] sArray = instruction.Split(',');

            // store as a Vector3
            Vector3 droplet = new Vector3(
                int.Parse(sArray[0]),
                int.Parse(sArray[1]),
                int.Parse(sArray[2]));

            droplets.Add(droplet);
        }

        int count = 0;
        foreach (Vector3 droplet in droplets)
        {
            foreach (Vector3 neighbour in getNeighbours(droplet))
            {
                if (!droplets.Contains(neighbour))
                    count++;
            }
        }

        return count.ToString();
    }

    List<Vector3> getNeighbours(Vector3 pos)
    {
        List<Vector3> res = new List<Vector3>();
        res.Add(new Vector3(pos.x + 1, pos.y, pos.z));
        res.Add(new Vector3(pos.x - 1, pos.y, pos.z));
        res.Add(new Vector3(pos.x, pos.y + 1, pos.z));
        res.Add(new Vector3(pos.x, pos.y - 1, pos.z));
        res.Add(new Vector3(pos.x, pos.y, pos.z + 1));
        res.Add(new Vector3(pos.x, pos.y, pos.z - 1));

        return res;        
    }

    protected override string part_2()
    {
        List<Vector3> droplets = new List<Vector3>();
        Dictionary<Vector3, bool> treatedPos = new Dictionary<Vector3, bool>();
        foreach (string instruction in _input.Split('\n'))
        {
            string[] sArray = instruction.Split(',');

            // store as a Vector3
            Vector3 droplet = new Vector3(
                int.Parse(sArray[0]),
                int.Parse(sArray[1]),
                int.Parse(sArray[2]));

            droplets.Add(droplet);

            treatedPos.Add(droplet, false);
        }

        float xMin = droplets.Select(d => d.x).Min();
        float xMax = droplets.Select(d => d.x).Max();
        float yMin = droplets.Select(d => d.y).Min();
        float yMax = droplets.Select(d => d.y).Max();
        float zMin = droplets.Select(d => d.z).Min();
        float zMax = droplets.Select(d => d.z).Max();

        Debug.Log("X : " + xMin + " - " + xMax + " | Y : " + yMin + " - " + yMax + " | Z : " + zMin + " - " + zMax);
        float totalCases = (xMax - xMin) * (yMax - yMin) * (zMax - zMin);
        Debug.Log(totalCases);

        Queue<Vector3> openedCases = new Queue<Vector3>();
        Vector3 startPos = new Vector3(xMin - 1, yMin - 1, zMin - 1);
        openedCases.Enqueue(startPos);

        int safetyCount = 100000;
        while (openedCases.Count > 0 && safetyCount > 0 )
        {
            safetyCount--;
            Vector3 curPos = openedCases.Dequeue();
            if (treatedPos.ContainsKey(curPos))
                continue;

            treatedPos.Add(curPos, true);
            if (curPos.x >= xMin)
                openedCases.Enqueue(new Vector3(curPos.x -1, curPos.y, curPos.z));

            if (curPos.x <= xMax)
                openedCases.Enqueue(new Vector3(curPos.x +1, curPos.y, curPos.z));

            if (curPos.y >= yMin)
                openedCases.Enqueue(new Vector3(curPos.x, curPos.y -1, curPos.z));

            if (curPos.y <= yMax)
                openedCases.Enqueue(new Vector3(curPos.x, curPos.y +1, curPos.z));

            if (curPos.z >= zMin)
                openedCases.Enqueue(new Vector3(curPos.x, curPos.y, curPos.z - 1));
            
            if (curPos.z <= zMax)
                openedCases.Enqueue(new Vector3(curPos.x, curPos.y, curPos.z + 1));
        }

        if (safetyCount == 0)
            Debug.LogError("snif " + openedCases.Count);

        // pretty graphic part :D
        //foreach (var tmp in treatedPos)       
        //{
        //    GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //    go.transform.position = tmp.Key;
        //    go.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        //    go.GetComponent<MeshRenderer>().material.color = tmp.Value ? Color.green : Color.red;
        //}

        int count = 0;
        foreach (Vector3 droplet in droplets)
        {
            foreach (Vector3 neighbour in getNeighbours(droplet))
            {
                if (treatedPos.ContainsKey(neighbour) && treatedPos[neighbour])
                    count++;
            }
        }

        return count.ToString();
    }
}
