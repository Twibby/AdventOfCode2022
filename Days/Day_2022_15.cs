using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Day_2022_15 : DayScript2022
{
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.K))
            part_1();
    }
    protected override string part_1()
    {
        List<Vector2Int> impossiblePosInterval = new List<Vector2Int>();
        int targetRow = (IsTestInput ? 10 : 2000000);

        // Parse input and create range where there can't be a beacon
        foreach (string sensorInstruction in _input.Split('\n'))
        {
            string[] coords = sensorInstruction.Substring(12, sensorInstruction.IndexOf(':') - 12).Split(", y=", System.StringSplitOptions.RemoveEmptyEntries);
            Vector2Int sCoord = new Vector2Int(int.Parse(coords[0]), int.Parse(coords[1]));

            coords = sensorInstruction.Substring(sensorInstruction.LastIndexOf("x=") + 2).Split(", y=", System.StringSplitOptions.RemoveEmptyEntries);
            Vector2Int bCoord = new Vector2Int(int.Parse(coords[0]), int.Parse(coords[1]));

            int distance = Mathf.Abs(bCoord.x - sCoord.x) + Mathf.Abs(bCoord.y - sCoord.y);
            int distToRow = Mathf.Abs(sCoord.y - targetRow);

            if (distToRow > distance)
                continue;

            impossiblePosInterval.Add(new Vector2Int(sCoord.x - (distance - distToRow), sCoord.x + (distance - distToRow)));
        }

        impossiblePosInterval.Sort(delegate (Vector2Int a, Vector2Int b) { return a.x.CompareTo(b.x); });

        long count = 0;
        count += impossiblePosInterval[0].y - impossiblePosInterval[0].x + 1;
        long maxVal = impossiblePosInterval[0].y;

        for (int i = 1; i < impossiblePosInterval.Count; i++)
        {
            if (impossiblePosInterval[i].x > maxVal)
            {
                count += impossiblePosInterval[i].y - impossiblePosInterval[i].x + 1;
                maxVal = impossiblePosInterval[i].y;
            }
            else if (impossiblePosInterval[i].y <= maxVal)
            {
                continue;   //interval in included in previous one
            }
            else
            {
                count += impossiblePosInterval[i].y - maxVal;
                maxVal = impossiblePosInterval[i].y;
            }
        }

        return count.ToString();
    }

    protected override string part_2()
    {
        int maxCoord = (IsTestInput ? 20 : 4000000);
        Dictionary<Vector2Int, Vector2Int> sensors = new Dictionary<Vector2Int, Vector2Int>();

        // Parse input and create dictionary Sensor Pos => closest Beacon pos
        foreach (string sensorInstruction in _input.Split('\n'))
        {
            string[] coords = sensorInstruction.Substring(12, sensorInstruction.IndexOf(':') - 12).Split(", y=", System.StringSplitOptions.RemoveEmptyEntries);
            Vector2Int sCoord = new Vector2Int(int.Parse(coords[0]), int.Parse(coords[1]));

            coords = sensorInstruction.Substring(sensorInstruction.LastIndexOf("x=") + 2).Split(", y=", System.StringSplitOptions.RemoveEmptyEntries);
            Vector2Int bCoord = new Vector2Int(int.Parse(coords[0]), int.Parse(coords[1]));

            sensors.Add(sCoord, bCoord);
        }

        // Compute for each row if one spot is possible
        for (int y = 0; y <= maxCoord; y++)
        {
            // First let find all impossible range (as in part1) for each sensor
            List<Vector2Int> impossiblePosInterval = new List<Vector2Int>();
            foreach (var sensor in sensors)
            {
                int distance = Mathf.Abs(sensor.Value.x - sensor.Key.x) + Mathf.Abs(sensor.Value.y - sensor.Key.y);
                int distToRow = Mathf.Abs(sensor.Key.y - y);

                if (distToRow > distance)
                    continue;

                impossiblePosInterval.Add(new Vector2Int(Mathf.Max(0,sensor.Key.x - (distance - distToRow)), Mathf.Min(maxCoord, sensor.Key.x + (distance - distToRow))));
            }

            // Sort these imposssible range and see if they match [0; maxCoord] or if there is an empty space somewhere...
            impossiblePosInterval.Sort(delegate (Vector2Int a, Vector2Int b) { return a.x.CompareTo(b.x); });
            if (impossiblePosInterval.Count == 0 || impossiblePosInterval[0].x != 0)
                return "Eeeeet c'est le fail : " + System.String.Join(" / ", impossiblePosInterval);

            // ... To do that, just track maxVal that is impossible and see if next interval is starting higher than maxVal
            int maxVal = impossiblePosInterval[0].y;
            for (int i = 1; i < impossiblePosInterval.Count; i++)   
            {
                if (impossiblePosInterval[i].x > maxVal + 1)
                {
                    long res = (long)(maxVal + 1) * 4000000 + y;
                    return "Point found : " + (maxVal + 1).ToString() + ", " + y.ToString() + " => " + res;
                }

                // interval are sorted by minBound so just look at maxBound to see if it's higher than previous max. 
                maxVal = Mathf.Max(maxVal, impossiblePosInterval[i].y);
                if (maxVal >= maxCoord) 
                    break;
            }
        }

        return "OUPS, point pas trouvé :'(";
    }
}
