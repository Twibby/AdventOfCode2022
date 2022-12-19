using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Day_2022_19 : DayScript2022
{
    protected override string part_1()
    {
        StartCoroutine(coDay(24));
        return "";
    }

    IEnumerator coDay(int totalTime)
    { 
        Dictionary<int, int> bestSCores = new Dictionary<int, int>();
        foreach (string instruction in _input.Split('\n'))
        {
            Blueprint bp = new Blueprint(instruction);
            Debug.LogWarning("*********** STARTING BLUEPRINT " + bp.id + " ****************** ");

            StateMachine startState = new StateMachine(totalTime, MaterialType.Ore, new Dictionary<MaterialType, int>(), new Dictionary<MaterialType, int>() { { MaterialType.Ore, 1 } });
            StateMachine startState2 = new StateMachine(totalTime, MaterialType.Clay, new Dictionary<MaterialType, int>(), new Dictionary<MaterialType, int>() { { MaterialType.Ore, 1 } });

            Queue<StateMachine> queue = new Queue<StateMachine>();
            queue.Enqueue(startState);
            queue.Enqueue(startState2);

            int bestGeodeScore = 0;
            int safetyCount = 100000000;
            int time = 0;
            if (bp.id == 2)
                Debug.Log("toto");

            while (queue.Count > 0 && safetyCount > 0 )
            {
                safetyCount--;
                if (safetyCount %10000 == 0)
                {
                    Debug.Log("SafetyCount " + safetyCount + " - queue Count " + queue.Count + "  (" + time + ")");
                    yield return new WaitForEndOfFrame();
                }

                StateMachine curState = queue.Dequeue();

                if (curState.timeRemaining != time)
                    time = curState.timeRemaining;

                if (curState.timeRemaining <= 0)
                {
                    bestGeodeScore = Mathf.Max(bestGeodeScore, curState.materialsCount[MaterialType.Geode]);
                    continue;
                }

                // try to remove some bad cases
                //if (curState.timeRemaining <= 15 && curState.robotsCount[MaterialType.Clay] == 0)
                //    continue;       // arbitrary value to cut down cases to analyze
                //if ((curState.robotsCount[MaterialType.Geode]+1 * curState.timeRemaining)  <= bestGeodeScore+1)
                //    continue;   // this machine will never reach bestscore
                //if (curState.robotsCount[MaterialType.Obsi] == 0 && bp.geodeRobotCost[MaterialType.Obsi] >= curState.timeRemaining + 1)
                //    continue;
                if (curState.nextRobotTarget == MaterialType.Geode && curState.robotsCount[MaterialType.Obsi] * (curState.timeRemaining-1) < bp.geodeRobotCost[MaterialType.Obsi])
                {   // no more robot will be built
                    bestGeodeScore = Mathf.Max(bestGeodeScore, curState.materialsCount[MaterialType.Geode] + curState.robotsCount[MaterialType.Geode]*curState.timeRemaining);
                    continue;
                }

                // calc if can build this turn
                bool canBuild = bp.canBuild(curState.nextRobotTarget, curState.materialsCount);

                // Production phase
                foreach (KeyValuePair<MaterialType, int> mt in curState.robotsCount)
                {
                    curState.materialsCount[mt.Key] += mt.Value;
                }

                // reduce resources with cost, add robot to list and add new statemachines for each next target choice
                if (canBuild)
                {
                    foreach (KeyValuePair<MaterialType, int> mt in bp.getCostOf(curState.nextRobotTarget))
                    {
                        curState.materialsCount[mt.Key] -= mt.Value;
                    }
                    curState.robotsCount[curState.nextRobotTarget] += 1;

                    foreach (MaterialType mt in System.Enum.GetValues(typeof(MaterialType)))
                    {
                        if ( (mt == MaterialType.Ore && curState.robotsCount[mt] >= 4)         // never build more than 3 ore robots
                            || (mt == MaterialType.Ore && curState.timeRemaining <= 4)         // don't build a ore robot in 5 last minutes (arbitrary)
                            || (mt == MaterialType.Obsi && curState.robotsCount[MaterialType.Clay] == 0)    // can't build obsi robot without clay
                            || (mt == MaterialType.Geode && curState.robotsCount[MaterialType.Obsi] == 0)   // can't build geode robot without obsi
                            )
                            continue;      


                        queue.Enqueue(new StateMachine(curState.timeRemaining - 1, mt, new Dictionary<MaterialType, int>(curState.materialsCount), new Dictionary<MaterialType, int>(curState.robotsCount)));
                    }
                }
                else
                {
                    queue.Enqueue(new StateMachine(curState.timeRemaining - 1, curState.nextRobotTarget, new Dictionary<MaterialType, int>(curState.materialsCount), new Dictionary<MaterialType, int>(curState.robotsCount)));
                }
            }
            yield return new WaitForEndOfFrame();

            if (safetyCount <= 0)
            {
                int maxTime = 0;
                while (queue.Count > 0)
                {
                    maxTime = Mathf.Max(queue.Dequeue().timeRemaining, maxTime);
                }
                Debug.LogError("MEH... " + safetyCount + " - " + queue.Count + " | " + bestGeodeScore + " | maxTime " + maxTime);
                yield return new WaitForEndOfFrame();
            }
            else
                Debug.Log((10000000 - safetyCount).ToString() + " iterations done");

            Debug.LogWarning("Best score of Blueprint " + bp.id + " is " + bestGeodeScore);
            bestSCores.Add(bp.id, bestGeodeScore);

            yield return new WaitForSeconds(1f);
        }

        yield return new WaitForEndOfFrame();

        int totalScore = bestSCores.Select(x => x.Key*x.Value).Sum();
        Debug.LogWarning("Total score is " + totalScore);
    }

    protected override string part_2()
    {
        StartCoroutine(coDay_2(32));
        return "";
    }

    IEnumerator coDay_2(int totalTime)
    {
        Dictionary<int, int> bestScores = new Dictionary<int, int>();
        int index = 0;
        foreach (string instruction in _input.Split('\n'))
        {
            index++;
            if (index > 3)
                break;

            Blueprint bp = new Blueprint(instruction);
            Debug.LogWarning("*********** STARTING BLUEPRINT " + bp.id + " ****************** ");

            StateMachine startState = new StateMachine(totalTime, MaterialType.Ore, new Dictionary<MaterialType, int>(), new Dictionary<MaterialType, int>() { { MaterialType.Ore, 1 } });
            StateMachine startState2 = new StateMachine(totalTime, MaterialType.Clay, new Dictionary<MaterialType, int>(), new Dictionary<MaterialType, int>() { { MaterialType.Ore, 1 } });

            Stack<StateMachine> queue = new Stack<StateMachine>();
            queue.Push(startState2);
            queue.Push(startState);

            int maxOre = bp.getMaxMaterialNeededByStep(MaterialType.Ore);
            int maxClay = bp.getMaxMaterialNeededByStep(MaterialType.Clay);

            int bestGeodeScore = 0;
            int safetyCount = 100000000;
            int time = 0;
            if (bp.id == 2)
                Debug.Log("toto");

            while (queue.Count > 0 && safetyCount > 0)
            {
                safetyCount--;
                if (safetyCount % 10000 == 0)
                {
                    Debug.Log("SafetyCount " + safetyCount + " - queue Count " + queue.Count + "  (" + bestGeodeScore + ")");
                    yield return new WaitForEndOfFrame();
                }

                StateMachine curState = queue.Pop();

                if (curState.timeRemaining != time)
                    time = curState.timeRemaining;

                if (curState.timeRemaining <= 0)
                {
                    if (curState.materialsCount[MaterialType.Geode] > bestGeodeScore)
                    {
                        bestGeodeScore = curState.materialsCount[MaterialType.Geode];
                    }                    
                    continue;
                }

                // try to remove some bad cases
                //if (curState.timeRemaining <= 15 && curState.robotsCount[MaterialType.Clay] == 0)
                //    continue;       // arbitrary value to cut down cases to analyze
                if (curState.timeRemaining <= 8)
                {
                    int t = curState.timeRemaining;
                    if (curState.materialsCount[MaterialType.Geode] + t * curState.robotsCount[MaterialType.Geode] + ((t-1)*t/ 2) < bestGeodeScore)
                        continue;   // this machine will never reach bestscore
                }
                if (curState.robotsCount[MaterialType.Obsi] == 0 && Mathf.Sqrt(bp.geodeRobotCost[MaterialType.Obsi]) >= curState.timeRemaining + 1)
                    continue;
                if (curState.nextRobotTarget == MaterialType.Geode && curState.robotsCount[MaterialType.Obsi] * (curState.timeRemaining - 1) + curState.materialsCount[MaterialType.Obsi] < bp.geodeRobotCost[MaterialType.Obsi])
                {   // no more robot will be built
                    bestGeodeScore = Mathf.Max(bestGeodeScore, curState.materialsCount[MaterialType.Geode] + curState.robotsCount[MaterialType.Geode] * curState.timeRemaining);
                    continue;
                }
                if (curState.nextRobotTarget == MaterialType.Obsi && curState.robotsCount[MaterialType.Clay] * (curState.timeRemaining - 1) + curState.materialsCount[MaterialType.Clay] < bp.obsiRobotCost[MaterialType.Clay])
                {   // no more robot will be built
                    bestGeodeScore = Mathf.Max(bestGeodeScore, curState.materialsCount[MaterialType.Geode] + curState.robotsCount[MaterialType.Geode] * curState.timeRemaining);
                    continue;
                }

                // calc if can build this turn
                bool canBuild = bp.canBuild(curState.nextRobotTarget, curState.materialsCount);

                // Production phase
                foreach (KeyValuePair<MaterialType, int> mt in curState.robotsCount)
                {
                    curState.materialsCount[mt.Key] += mt.Value;
                }

                // reduce resources with cost, add robot to list and add new statemachines for each next target choice
                if (canBuild)
                {
                    foreach (KeyValuePair<MaterialType, int> mt in bp.getCostOf(curState.nextRobotTarget))
                    {
                        curState.materialsCount[mt.Key] -= mt.Value;
                    }
                    curState.robotsCount[curState.nextRobotTarget] += 1;

                    foreach (MaterialType mt in System.Enum.GetValues(typeof(MaterialType)))
                    {
                        if ((mt == MaterialType.Ore && curState.robotsCount[mt] >= maxOre)          // never build more robots that we can use in one step
                            || (mt == MaterialType.Clay && curState.robotsCount[mt] >= maxClay)     // never build more robots that we can use in one step
                            || (mt == MaterialType.Ore && curState.timeRemaining <= 10)             // don't build a ore robot in 10 last minutes (arbitrary)
                            || (mt == MaterialType.Clay && curState.timeRemaining <= 5)             // don't build a clay robot in 5 last minutes (arbitrary)
                            || (mt == MaterialType.Obsi && curState.robotsCount[MaterialType.Clay] == 0)    // can't build obsi robot without clay
                            || (mt == MaterialType.Geode && curState.robotsCount[MaterialType.Obsi] == 0)   // can't build geode robot without obsi
                            )
                            continue;


                        queue.Push(new StateMachine(curState.timeRemaining - 1, mt, new Dictionary<MaterialType, int>(curState.materialsCount), new Dictionary<MaterialType, int>(curState.robotsCount)));
                    }
                }
                else
                {
                    queue.Push(new StateMachine(curState.timeRemaining - 1, curState.nextRobotTarget, new Dictionary<MaterialType, int>(curState.materialsCount), new Dictionary<MaterialType, int>(curState.robotsCount)));
                }
            }
            yield return new WaitForEndOfFrame();

            if (safetyCount <= 0)
            {
                int maxTime = 0;
                while (queue.Count > 0)
                {
                    maxTime = Mathf.Max(queue.Pop().timeRemaining, maxTime);
                }
                Debug.LogError("MEH... " + safetyCount + " - " + queue.Count + " | " + bestGeodeScore + " | maxTime " + maxTime);
                yield return new WaitForEndOfFrame();
            }
            else
                Debug.Log((10000000 - safetyCount).ToString() + " iterations done");

            Debug.LogWarning("Best score of Blueprint " + bp.id + " is " + bestGeodeScore);
            bestScores.Add(bp.id, bestGeodeScore);

            yield return new WaitForSeconds(1f);
        }

        yield return new WaitForEndOfFrame();

        int totalScore = 1;
        foreach (int score in bestScores.Values) { totalScore *= score; }

        Debug.LogWarning("Total score is " + totalScore);
    }

    public enum MaterialType { Ore, Clay, Obsi, Geode}
    class Blueprint
    {
        public int id;
        public Dictionary<MaterialType, int> oreRobotCost;
        public Dictionary<MaterialType, int> clayRobotCost;
        public Dictionary<MaterialType, int> obsiRobotCost;
        public Dictionary<MaterialType, int> geodeRobotCost;

        public Blueprint(int id, Dictionary<MaterialType, int> oreRobotCost, Dictionary<MaterialType, int> clayRobotCost, Dictionary<MaterialType, int> obsiRobotCost, Dictionary<MaterialType, int> geodeRobotCost)
        {
            this.id = id;
            this.oreRobotCost = oreRobotCost;
            this.clayRobotCost = clayRobotCost;
            this.obsiRobotCost = obsiRobotCost;
            this.geodeRobotCost = geodeRobotCost;
        }

        public Blueprint(string rawInput)
        {
            string[] words = rawInput.Split(new char[] { ' ', ':' }, System.StringSplitOptions.RemoveEmptyEntries);
            this.id = int.Parse(words[1]);

            oreRobotCost = new Dictionary<MaterialType, int>();
            oreRobotCost.Add(MaterialType.Ore, int.Parse(words[6]));

            clayRobotCost = new Dictionary<MaterialType, int>();
            clayRobotCost.Add(MaterialType.Ore, int.Parse(words[12]));

            obsiRobotCost = new Dictionary<MaterialType, int>();
            obsiRobotCost.Add(MaterialType.Ore, int.Parse(words[18]));
            obsiRobotCost.Add(MaterialType.Clay, int.Parse(words[21]));

            geodeRobotCost = new Dictionary<MaterialType, int>();
            geodeRobotCost.Add(MaterialType.Ore, int.Parse(words[27]));
            geodeRobotCost.Add(MaterialType.Obsi, int.Parse(words[30]));

            foreach (MaterialType mt in System.Enum.GetValues(typeof(MaterialType)))
            {
                if (!oreRobotCost.ContainsKey(mt))  { oreRobotCost.Add(mt, 0); }
                if (!clayRobotCost.ContainsKey(mt)) { clayRobotCost.Add(mt, 0); }
                if (!obsiRobotCost.ContainsKey(mt)) { obsiRobotCost.Add(mt, 0); }
                if (!geodeRobotCost.ContainsKey(mt)){ geodeRobotCost.Add(mt, 0); }
            }
        }

        public bool canBuild(MaterialType robotType, Dictionary<MaterialType, int> resourcesAvailables)
        {
            Dictionary<MaterialType, int> cost = getCostOf(robotType);

            bool canBuild = true;
            foreach (var mtCost in cost)
            {
                canBuild &= resourcesAvailables.ContainsKey(mtCost.Key) && resourcesAvailables[mtCost.Key] >= mtCost.Value;
            }

            return canBuild;
        }

        public Dictionary<MaterialType, int> getCostOf(MaterialType robotType)
        {
            Dictionary<MaterialType, int> cost = new Dictionary<MaterialType, int>();
            switch (robotType)
            {
                case MaterialType.Ore: cost = oreRobotCost; break;
                case MaterialType.Clay: cost = clayRobotCost; break;
                case MaterialType.Obsi: cost = obsiRobotCost; break;
                case MaterialType.Geode: cost = geodeRobotCost; break;
            }
            return cost;
        }

        public int getMaxMaterialNeededByStep(MaterialType type)
        {
            return Mathf.Max(oreRobotCost[type], Mathf.Max(clayRobotCost[type], Mathf.Max(obsiRobotCost[type], geodeRobotCost[type])));
        }
    }

    class StateMachine
    {
        public int timeRemaining;
        public MaterialType nextRobotTarget;

        public Dictionary<MaterialType, int> materialsCount;
        public Dictionary<MaterialType, int> robotsCount;


        public StateMachine(int timeRemaining, MaterialType nextRobotTarget, Dictionary<MaterialType, int> materialsCount, Dictionary<MaterialType, int> robotsCount)
        {
            this.timeRemaining = timeRemaining;
            this.nextRobotTarget = nextRobotTarget;
            this.materialsCount = materialsCount;
            this.robotsCount = robotsCount;

            foreach (MaterialType mt in System.Enum.GetValues(typeof(MaterialType)))
            {
                if (!materialsCount.ContainsKey(mt))
                    materialsCount.Add(mt, 0);

                if (!robotsCount.ContainsKey(mt))
                    robotsCount.Add(mt, 0);
            }
        }

        public override string ToString()
        {
            return "Building " + nextRobotTarget.ToString() + " robot with " + timeRemaining + "min. Has " + String.Join(", ", robotsCount.Select(x => x.Value.ToString() + " " + x.Key.ToString())) + " robots AND " + String.Join(",", materialsCount.Select(x => x.Value.ToString() + " " + x.Key.ToString()));
        }
    }
}
