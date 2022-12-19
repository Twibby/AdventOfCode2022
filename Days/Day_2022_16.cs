using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Day_2022_16 : DayScript2022
{

    // ******************************* \\
    // *********** WARNING *********** \\
    // ******************************* \\

    // Ce code est immonde et dégueulasse, il m'a pris la tête sur plusieurs heures donc si tu lis ce truc, t'attends pas à avoir qqch de beau et bien clair
    // La p1 est "ok" mais p2 est un foutoir sans nom

    protected override string part_1()
    {
        CaveNetwork graph = new CaveNetwork();

        foreach (string instruction in _input.Split('\n'))
        {
            string valveName = instruction.Substring(6, 2);
            int valveFlow = int.Parse(instruction.Split(';')[0].Substring(23));
            graph.nodes.Add(valveName, new Valve(valveName, valveFlow));

            foreach (string strTunnel in instruction.Substring(instruction.IndexOf(',') > 0 ? instruction.IndexOf(',') - 2 : instruction.Length - 2).Split(", ", System.StringSplitOptions.RemoveEmptyEntries))
            {
                Tunnel tun = new Tunnel(valveName, strTunnel);
                if (!graph.edges.Contains(tun))
                    graph.edges.Add(tun);
            }
        }

        graph.ReduceGraph();
        Debug.Log(graph.ToString());

        Debug.LogWarning("*****************");
        graph.SetNodesNeighbours();
        foreach (var node in graph.nodes)
        {
            Debug.Log(node.Value.ToString() + "  -->  " + String.Join(" | ", node.Value.neighbours.Select(x => x.Key + "-" + x.Value)));
        }

        // little hack because startNode "AA" has 0 flowrate but is kept in graph
        graph.nodes["AA"].isOpen = true;

        int remainingTime = 30;
        GraphState startState = new GraphState(graph, remainingTime, 0, "AA", "AA");

        StartCoroutine(coGraphState(startState));
        return "";
    }

    IEnumerator coGraphState(GraphState startState)
    { 
        int bestScore = 0;
        Queue<GraphState> queue = new Queue<GraphState>();
        queue.Enqueue(startState);

        Dictionary<string, int> encounteredConf = new Dictionary<string, int>();
        float t0 = Time.realtimeSinceStartup;
        int safetyCount = 1000000;
        while (safetyCount > 0 && queue.Count > 0)
        {
            safetyCount--;
            if (safetyCount % 100000 == 0)
            {
                Debug.Log(safetyCount + " - Queue Count: " + queue.Count + " - bestScore: " + bestScore);
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
            }
            GraphState current = queue.Dequeue();

            string conf = current.GetConf();
            if (!encounteredConf.ContainsKey(conf))
                encounteredConf.Add(conf, current.pressureReleased);
            else if (current.pressureReleased == 0 || encounteredConf[conf] > current.pressureReleased)
                continue; // we already got same conf with more score earlier
            else
                encounteredConf[conf] = current.pressureReleased;   //update
            

            if (current.timeRemaining <= 0)
            {
                bestScore = Mathf.Max(bestScore, current.pressureReleased);
                continue;
            }

            if (current.graph.nodes.Values.All( x => x.isOpen))
            {   // all valves are opened, just wait til timer ends
                //Debug.Log("All valves are opened, just wait til timer ends => " + current.timeRemaining);
                current.pressureReleased += current.graph.GetCurrentPressureReleased() * current.timeRemaining;
                bestScore = Mathf.Max(bestScore, current.pressureReleased);

                continue;
            }

            if (current.pressureReleased < 10*(25-current.timeRemaining))
                continue;   // some cleanup; if you did nothing for first 10 min on 30, it's useless

            bool hasAddedNewState = false;
            // if currentPos valve is closed (and flowrate > 0), try open it
            if (!current.graph.nodes[current.currentPos].isOpen)
            {
                //Debug.Log("Add state where i'm opening valve in cave " + current.currentPos);
                CaveNetwork newGraph = new CaveNetwork(current.graph);
                newGraph.nodes[current.currentPos].isOpen = true;

                int newScore = current.pressureReleased + current.graph.GetCurrentPressureReleased();

                GraphState newState = new GraphState(newGraph, current.timeRemaining -1, newScore, current.currentPos, current.currentPos);
                queue.Enqueue(newState);
                hasAddedNewState = true;
            }


            // Then add states where you take a tunnel
            foreach (var neighbour in current.graph.nodes[current.currentPos].neighbours)
            {
                if (neighbour.Key == current.lastPos)
                    continue;   // don't single loop

                if (neighbour.Value > current.timeRemaining)
                    continue;   // neighbour is too far


                CaveNetwork newGraph = new CaveNetwork(current.graph);
                int newScore = current.pressureReleased + current.graph.GetCurrentPressureReleased() + newGraph.GetCurrentPressureReleased() * (neighbour.Value -1);

                GraphState newState = new GraphState(newGraph, current.timeRemaining - neighbour.Value, newScore, neighbour.Key, current.currentPos);
                queue.Enqueue(newState);
                hasAddedNewState = true;
            }

            if (!hasAddedNewState)
            {   // no new state, so let's just not move and wait until timer ends
                //Debug.Log("No new state enqueued, so let's just not move and wait until timer ends");
                current.pressureReleased += current.graph.GetCurrentPressureReleased() * current.timeRemaining;
                bestScore = Mathf.Max(bestScore, current.pressureReleased);
            }
        }

        Debug.Log(safetyCount + " - " + queue.Count);
        Debug.Log((Time.realtimeSinceStartup - t0).ToString());
        Debug.LogWarning(bestScore);
        //return bestScore.ToString();
    }

    protected override string part_2()
    {
        CaveNetwork graph = new CaveNetwork();

        foreach (string instruction in _input.Split('\n'))
        {
            string valveName = instruction.Substring(6, 2);
            int valveFlow = int.Parse(instruction.Split(';')[0].Substring(23));
            graph.nodes.Add(valveName, new Valve(valveName, valveFlow));

            foreach (string strTunnel in instruction.Substring(instruction.IndexOf(',') > 0 ? instruction.IndexOf(',') - 2 : instruction.Length - 2).Split(", ", System.StringSplitOptions.RemoveEmptyEntries))
            {
                Tunnel tun = new Tunnel(valveName, strTunnel);
                if (!graph.edges.Contains(tun))
                    graph.edges.Add(tun);
            }
        }

        graph.ReduceGraph();
        Debug.Log(graph.ToString());

        Debug.LogWarning("*****************");
        graph.SetNodesNeighbours();
        foreach (var node in graph.nodes)
        {
            if (node.Value.flowRate == 0)
                node.Value.isOpen = true;

            Debug.Log(node.Value.ToString() + "  -->  " + String.Join(" | ", node.Value.neighbours.Select(x => x.Key + "-" + x.Value)));
        }

        // little hack because startNode "AA" has 0 flowrate but is kept in graph
        graph.nodes["AA"].isOpen = true;

        int remainingTime = 26;
        GraphStateP2 startState = new GraphStateP2(graph, remainingTime, 0, "AA", "AA", "AA", "AA", 0, 0);

        StartCoroutine(coGraphStateP2(startState));
        return "";
    }

    IEnumerator coGraphStateP2(GraphStateP2 startState)
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        int bestScore = 0;
        Queue<GraphStateP2> queue = new Queue<GraphStateP2>();
        queue.Enqueue(startState);

        Dictionary<string, int> encounteredConf = new Dictionary<string, int>();
        float t0 = Time.realtimeSinceStartup;
        int safetyCount = 10000000;
        while (safetyCount > 0 && queue.Count > 0)
        {
            safetyCount--;
            //if (queue.Count > 1200000)
            //    break;

            if (safetyCount % 10000 == 0)
            {
                Debug.Log(safetyCount + " - Queue Count: " + queue.Count + " - bestScore: " + bestScore);
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
            }
            GraphStateP2 current = queue.Dequeue();

            string conf = current.GetConf();
            string altConf = current.GetAltConf();
            if (!encounteredConf.ContainsKey(conf))
            {
                encounteredConf.Add(conf, current.timeRemaining);
                if (altConf != conf)
                    encounteredConf.Add(altConf, current.timeRemaining);
            }
            else if (current.pressureReleased == 0 || encounteredConf[conf] >= current.timeRemaining)
                continue; // we already got same conf with more score earlier
            else
                encounteredConf[conf] = current.timeRemaining;   //update

            if (current.timeRemaining <= 0)
            {
                bestScore = Mathf.Max(bestScore, current.pressureReleased);
                continue;
            }

            if (current.graph.nodes.Values.All(x => x.isOpen))
            {   // all valves are opened, just wait til timer ends
                //Debug.Log("All valves are opened, just wait til timer ends => " + current.timeRemaining);
                current.pressureReleased += current.graph.GetCurrentPressureReleased() * current.timeRemaining;
                bestScore = Mathf.Max(bestScore, current.pressureReleased);

                continue;
            }

            if (current.pressureReleased < 34 * ((startState.timeRemaining - 4) - current.timeRemaining))
                continue;   // some cleanup; if you did nothing for firsts mins on 30, it's useless (numbers chosen are arbitrary)

            bool hasAddedNewState = false;

            if (current.movingTime > 0)
            {
                // player moving, jsut deal with elephant cases
                if (current.movingElephantTime > 0)
                {
                    // both moving
                    GraphStateP2 newState = new GraphStateP2(current.graph, current.timeRemaining - 1, current.pressureReleased + current.graph.GetCurrentPressureReleased(), current.currentPos, current.lastPos, current.currentElephantPos, current.lastElephantPos, current.movingTime-1, current.movingElephantTime-1);
                    queue.Enqueue(newState);
                    hasAddedNewState = true;
                }
                else
                {
                    // only player moving, deal elephant cases
                    if (!current.graph.nodes[current.currentElephantPos].isOpen)        // open his valve
                    {
                        CaveNetwork newGraph = new CaveNetwork(current.graph);
                        newGraph.nodes[current.currentElephantPos].isOpen = true;

                        int newScore = current.pressureReleased + current.graph.GetCurrentPressureReleased();

                        GraphStateP2 newState = new GraphStateP2(newGraph, current.timeRemaining - 1, newScore, current.currentPos, current.lastPos, current.currentElephantPos, current.currentElephantPos, current.movingTime-1, 0);
                        queue.Enqueue(newState);
                        hasAddedNewState = true;
                    }

                    foreach (var neighbourEl in current.graph.nodes[current.currentElephantPos].neighbours)   // elephant moves to his next location
                    {
                        if (neighbourEl.Key == current.lastElephantPos)
                            continue;   // don't single loop

                        CaveNetwork newGraph = new CaveNetwork(current.graph);
                        int newScore = current.pressureReleased + current.graph.GetCurrentPressureReleased();

                        GraphStateP2 newState = new GraphStateP2(newGraph, current.timeRemaining - 1, newScore, current.currentPos, current.lastPos, neighbourEl.Key, current.currentElephantPos, current.movingTime-1, neighbourEl.Value-1);
                        queue.Enqueue(newState);
                        hasAddedNewState = true;
                    }
                }                
            }
            else   // PLAYER NOT MOVING
            {
                if (current.movingElephantTime > 0) //elephant moving jsut deal with player cases
                {
                    if (!current.graph.nodes[current.currentPos].isOpen)        // open his valve
                    {
                        CaveNetwork newGraph = new CaveNetwork(current.graph);
                        newGraph.nodes[current.currentPos].isOpen = true;

                        int newScore = current.pressureReleased + current.graph.GetCurrentPressureReleased();

                        GraphStateP2 newState = new GraphStateP2(newGraph, current.timeRemaining - 1, newScore, current.currentPos, current.currentPos, current.currentElephantPos, current.lastElephantPos, 0, current.movingElephantTime- 1);
                        queue.Enqueue(newState);
                        hasAddedNewState = true;
                    }

                    foreach (var neighbour in current.graph.nodes[current.currentPos].neighbours)   // player moves to his next location
                    {
                        if (neighbour.Key == current.lastPos)
                            continue;   // don't single loop

                        CaveNetwork newGraph = new CaveNetwork(current.graph);
                        int newScore = current.pressureReleased + current.graph.GetCurrentPressureReleased();

                        GraphStateP2 newState = new GraphStateP2(newGraph, current.timeRemaining - 1, newScore, neighbour.Key, current.currentPos, current.currentElephantPos, current.lastElephantPos, neighbour.Value - 1, current.movingElephantTime - 1);
                        queue.Enqueue(newState);
                        hasAddedNewState = true;
                    }
                }
                else
                {   // BOTH NOT MOVING DUUUH

                    // if currentPos valve is closed (and flowrate > 0), try open it
                    if (!current.graph.nodes[current.currentPos].isOpen)
                    {
                        CaveNetwork newGraph = new CaveNetwork(current.graph);
                        newGraph.nodes[current.currentPos].isOpen = true;


                        // same possibilities for elephant
                        {
                            if (!current.graph.nodes[current.currentElephantPos].isOpen)        // open his valve
                            {
                                CaveNetwork newNewGraph = new CaveNetwork(newGraph);
                                newNewGraph.nodes[current.currentElephantPos].isOpen = true;

                                int newScore = current.pressureReleased + current.graph.GetCurrentPressureReleased();

                                GraphStateP2 newState = new GraphStateP2(newNewGraph, current.timeRemaining - 1, newScore, current.currentPos, current.currentPos, current.currentElephantPos, current.currentElephantPos, 0, 0);
                                queue.Enqueue(newState);
                                hasAddedNewState = true;
                            }

                            foreach (var neighbourEl in current.graph.nodes[current.currentElephantPos].neighbours)   // elephant moves to his next location
                            {
                                if (neighbourEl.Key == current.lastElephantPos)
                                    continue;   // don't single loop

                                CaveNetwork newNewGraph = new CaveNetwork(newGraph);
                                int newScore = current.pressureReleased + current.graph.GetCurrentPressureReleased();

                                GraphStateP2 newState = new GraphStateP2(newNewGraph, current.timeRemaining - 1, newScore, current.currentPos, current.currentPos, neighbourEl.Key, current.currentElephantPos, 0, neighbourEl.Value-1);
                                queue.Enqueue(newState);
                                hasAddedNewState = true;
                            }
                        }
                    }


                    // Then add states where you take a tunnel
                    foreach (var neighbour in current.graph.nodes[current.currentPos].neighbours)
                    {
                        if (neighbour.Key == current.lastPos)
                            continue;   // don't single loop

                        CaveNetwork newGraph = new CaveNetwork(current.graph);

                        // same possibilities for elephant
                        {
                            if (!current.graph.nodes[current.currentElephantPos].isOpen)        // open his valve
                            {
                                CaveNetwork newNewGraph = new CaveNetwork(newGraph);
                                newNewGraph.nodes[current.currentElephantPos].isOpen = true;

                                int newScore = current.pressureReleased + current.graph.GetCurrentPressureReleased();

                                GraphStateP2 newState = new GraphStateP2(newNewGraph, current.timeRemaining - 1, newScore, neighbour.Key, current.currentPos, current.currentElephantPos, current.currentElephantPos, neighbour.Value-1, 0);
                                queue.Enqueue(newState);
                                hasAddedNewState = true;
                            }

                            foreach (var neighbourEl in current.graph.nodes[current.currentElephantPos].neighbours)   // elephant moves to his next location
                            {
                                if (neighbourEl.Key == current.lastElephantPos)
                                    continue;   // don't single loop

                                CaveNetwork newNewGraph = new CaveNetwork(newGraph);
                                int newScore = current.pressureReleased + current.graph.GetCurrentPressureReleased();

                                GraphStateP2 newState = new GraphStateP2(newNewGraph, current.timeRemaining - 1, newScore, neighbour.Key, current.currentPos, neighbourEl.Key, current.currentElephantPos, neighbour.Value-1, neighbourEl.Value-1);
                                queue.Enqueue(newState);
                                hasAddedNewState = true;
                            }
                        }
                    }
                }
            }

            if (!hasAddedNewState)
            {   // no new state, so let's just not move and wait until timer ends
                //Debug.Log("No new state enqueued, so let's just not move and wait until timer ends");
                current.pressureReleased += current.graph.GetCurrentPressureReleased() * current.timeRemaining;
                bestScore = Mathf.Max(bestScore, current.pressureReleased);
            }
        }

        Debug.Log(safetyCount + " - " + queue.Count);

        int maxTime = 0;
        Debug.LogWarning(bestScore);

        while (queue.Count > 0)
        {
            bestScore = Mathf.Max(bestScore, queue.Peek().GetHypotheticalScore());
            maxTime = Mathf.Max(queue.Dequeue().timeRemaining, maxTime);

            safetyCount++;
            if (safetyCount % 10000 == 0)
                yield return new WaitForEndOfFrame();
        }
        Debug.Log("max Time remaining " + maxTime);
        Debug.Log((Time.realtimeSinceStartup - t0).ToString());
        Debug.LogWarning(bestScore);
        Debug.Log("+ " + startState.graph.nodes.Select(x => x.Value.flowRate).Sum());
        //return bestScore.ToString();
        GC.Collect();
    }

    class Valve
    {
        public string name;
        public int flowRate;
        public bool isOpen = false;
        public Dictionary<string, int> neighbours = new Dictionary<string, int>();

        public Valve(string name, int flowRate)
        {
            this.name = name;
            this.flowRate = flowRate;
            
            this.isOpen = false;
            this.neighbours = new Dictionary<string, int>();

            //Debug.Log("New valve '" + name + "' with flow rate " + flowRate);
        }

        public Valve(Valve copy)
        {
            this.name = copy.name;
            this.flowRate = copy.flowRate;
            this.isOpen = copy.isOpen;
            this.neighbours = copy.neighbours;
        }

        public override string ToString()
        {
            return name + " (" + flowRate + ")" + (isOpen ? "o" : "x");
        }
    }

    class Tunnel
    {
        public string  X;
        public string Y;
        public int weight;

        public Tunnel(string valve1, string valve2)
        {
            if (String.Compare(valve1, valve2) <= 0)
            {
                this.X = valve1;
                this.Y = valve2;
            }
            else
            {
                this.X = valve2;
                this.Y = valve1;
            }
            this.weight = 1;
        }

        public Tunnel(string valve1, string valve2, int p_weight = 1)
        {
            if (String.Compare(valve1, valve2) <= 0)
            {
                this.X = valve1;
                this.Y = valve2;
            }
            else
            {
                this.X = valve2;
                this.Y = valve1;
            }
            this.weight = p_weight;

            Debug.Log("New Tunnel " + X + " - " + Y + " with weight " + weight);
        }

        public Tunnel (Tunnel copy)
        {
            this.X = copy.X;
            this.Y = copy.Y;
            this.weight = copy.weight;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(Tunnel))
                return false;

            Tunnel other = (Tunnel)obj;
            return (X == other.X && Y == other.Y);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() + Y.GetHashCode();
        }

        public override string ToString()
        {
            return X + "-" + Y + " " + weight;
        }
    }

    class CaveNetwork
    {
        public Dictionary<string, Valve> nodes;
        public List<Tunnel> edges;

        public CaveNetwork()
        {
            nodes = new Dictionary<string, Valve>();
            edges = new List<Tunnel>();
        }

        public CaveNetwork(CaveNetwork copy)
        {
            nodes = new Dictionary<string, Valve>();
            foreach (var tmp in copy.nodes) { nodes.Add(tmp.Key, new Valve(tmp.Value)); }

            edges = new List<Tunnel>();
            copy.edges.ForEach(x => this.edges.Add(new Tunnel(x)));
        }

        public override string ToString()
        {
            string log = "Graph with " + nodes.Count + " nodes and " + edges.Count + " edges";
            log += System.Environment.NewLine + String.Join(", ", nodes.Values);
            log += System.Environment.NewLine + String.Join( "  |  ", edges);

            return log;
        }

        public void ReduceGraph()
        {
            int safetyCount = 1000;
            while (this.nodes.Values.ToList().Exists(x => x.flowRate == 0 && x.name != "AA") && safetyCount > 0)
            {
                safetyCount--;

                Valve nulValve = this.nodes.Values.ToList().Find(x => x.flowRate == 0 && x.name != "AA");

                List<KeyValuePair<string, int>> linkedTunnels = edges.FindAll(x => x.X == nulValve.name || x.Y == nulValve.name).Select(x => new KeyValuePair<string,int>(x.X == nulValve.name ? x.Y : x.X, x.weight)).ToList();

                Debug.Log("Valve null is " + nulValve.name + ", linked to " + System.String.Join("; ", linkedTunnels.Select(x => x.Key + "-" + x.Value)));

                for (int i = 0; i < linkedTunnels.Count; i++)
                {
                    for (int j = i + 1; j < linkedTunnels.Count; j++)
                    {
                        Tunnel newTun = new Tunnel(linkedTunnels[i].Key, linkedTunnels[j].Key, linkedTunnels[i].Value + linkedTunnels[j].Value);
                        if (edges.Exists(x => x.X == newTun.X && x.Y == newTun.Y))
                        {
                            Tunnel existingTun = edges.Find(x => x.X == newTun.X && x.Y == newTun.Y);
                            newTun.weight = Mathf.Min(newTun.weight, existingTun.weight);
                            edges.Remove(existingTun);
                        }
                            edges.Add(newTun);
                    }
                }

                edges.RemoveAll(x => x.X == nulValve.name || x.Y == nulValve.name);
                nodes.Remove(nulValve.name);
            }

            if (safetyCount <= 0)
                Debug.LogError("Infinite loop, that doesnt work");

            edges.Sort(delegate (Tunnel t1, Tunnel t2) { return String.Compare(t1.X, t2.X); });
        }

        public void SetNodesNeighbours()
        {
            nodes.Values.ToList().ForEach(x => x.neighbours = new Dictionary<string, int>());
            foreach (Tunnel t in this.edges)
            {
                this.nodes[t.X].neighbours.Add(t.Y, t.weight);
                this.nodes[t.Y].neighbours.Add(t.X, t.weight);
            }
        }


        public int GetCurrentPressureReleased() { return nodes.Select(x => x.Value.isOpen ? x.Value.flowRate : 0).Sum(); }

    }

    class GraphState
    {
        public CaveNetwork graph;
        public int pressureReleased;
        public string currentPos;
        public string lastPos;  // to avoid circle

        public int timeRemaining;

        public GraphState(CaveNetwork graph, int remainingTime, int score, string newPos, string oldPos = "AA")
        {
            this.graph = new CaveNetwork(graph);
            this.timeRemaining = remainingTime;
            this.pressureReleased = score;
            this.currentPos = newPos;
            this.lastPos = oldPos;
        }

        public override string ToString()
        {
            return "At pos " + currentPos + " from " + lastPos + ", score is " + pressureReleased + " with " + timeRemaining + " minutes remaining";
        }

        public string GetConf()
        {
            return currentPos + "-" + String.Join("", graph.nodes.Values.Select(x => x.name + (x.isOpen ? "1" : "0")));
        }
    }

    class GraphStateP2
    {
        public CaveNetwork graph;
        public int pressureReleased;
        public string currentPos;
        public string lastPos;  // to avoid circle
        public string currentElephantPos;
        public string lastElephantPos;  // to avoid circle
        public int movingTime;
        public int movingElephantTime;

        public int timeRemaining;

        public GraphStateP2(CaveNetwork graph, int remainingTime, int score, string newPos, string oldPos, string newEPos, string oldEPos, int movingTime, int movingElTime)
        {
            this.graph = new CaveNetwork(graph);
            this.timeRemaining = remainingTime;
            this.pressureReleased = score;
            this.currentPos = newPos;
            this.lastPos = oldPos;
            this.currentElephantPos = newEPos;
            this.lastElephantPos = oldEPos;
            this.movingTime = movingTime;
            this.movingElephantTime = movingElTime;
        }

        public override string ToString()
        {
            return "At pos " + currentPos + "(" + (movingTime > 0 ? "moving" + movingTime : "") + lastPos + ") and eleph pos " + currentElephantPos + "(" + (movingElephantTime > 0 ? "moving" + movingElephantTime : "") + lastElephantPos +"), score is " + pressureReleased + " with " + timeRemaining + " minutes remaining";
        }

        public string GetConf()
        {
            return currentPos + movingTime + "-" + currentElephantPos + movingElephantTime + "-" + String.Join("", graph.nodes.Values.Select(x => x.name + (x.isOpen ? "1" : "0")));
        }

        public string GetAltConf()
        {
            return currentElephantPos + movingElephantTime + "-" + currentPos + movingTime + "-" + String.Join("", graph.nodes.Values.Select(x => x.name + (x.isOpen ? "1" : "0")));
        }

        public int GetHypotheticalScore()
        {
            return pressureReleased + timeRemaining * graph.GetCurrentPressureReleased();
        }
    }
}
