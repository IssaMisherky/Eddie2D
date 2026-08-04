using UnityEngine;
using System.Collections.Generic;


namespace GOAPSystem
{
    public class Node
    {
        public Node parent;
        public float cost;
        public Dictionary<string, int> needs;
        public GoapActionAsset action;

        public Node(Node parent, float cost, Dictionary<string, int> neededStates, GoapActionAsset action)
        {
            this.parent = parent;
            this.cost   = cost;
            needs = new Dictionary<string, int>(neededStates);
            this.action = action;
        }
    }

    public class GOAPPlanner
    {
        public static bool debug = false;


        public static readonly GOAPPlanner Instance = new GOAPPlanner();


        public Queue<ActionInstance> plan(GameObject npc, List<GoapActionAsset> actions, Dictionary<string, int> goal, WorldStates states)
        {
            //List<GoapActionAsset> availableActions = new List<GoapActionAsset>();
            //foreach (GOAPAction action in actions)
            //{
            //    if (action.IsAvailable())
            //    {
            //        availableActions.Add(action);
            //    }
            //}

            if (debug == true)
            {
                Debug.DebugBreak();
            }



            Dictionary<string, int> startStates = new Dictionary<string, int>(GOAPWorld.Instance.getStates().states);

            foreach (KeyValuePair<string, int> pair in states.states)
            {
                startStates.Add(pair.Key, pair.Value);
            }

            Dictionary<string, int> targetStates = new Dictionary<string, int>();

            foreach (KeyValuePair<string, int> pair in goal)
            {
                targetStates.Add(pair.Key, pair.Value);
            }






            /* ========================================= */
            //List<Node> leaves = new List<Node>();
            //Node start = new Node(null, 0, startStates, null);

            //bool success = BuildGraph(start, leaves, actions, goal);
            /* ========================================= */

            // List<Node> leaves = new List<Node>();
            Node target = new Node(null, 0, targetStates, null);


            Node planTip = BuildGraph(target, actions, startStates);


            if (planTip == null)
            {
                //Debug.Log("NO PLAN");

                return null;
            }

            //Node cheapest = null;
            //foreach (Node leaf in leaves)
            //{
            //    if (cheapest == null)
            //    {
            //        cheapest = leaf;
            //        continue;
            //    }

            //    if (leaf.cost < cheapest.cost)
            //    {
            //        cheapest = leaf;
            //    }
            //}

            List<GoapActionAsset> result = new List<GoapActionAsset>();
            Node node = planTip;
            while (node != null)
            {
                if (node.action != null)
                {
                    result.Add(node.action);
                    //result.Insert(0, node.action);
                }
                node = node.parent;
            }

            NPCActionRunner runner = npc.GetComponent<NPCActionRunner>();

            Queue<ActionInstance> queue = new Queue<ActionInstance>();
            foreach (GoapActionAsset action in result)
            {
                ActionInstance actionInstance = new ActionInstance();

                actionInstance.ActionAsset = action;
                actionInstance.NpcRunner = runner;

                queue.Enqueue(actionInstance);
            }

            Debug.Log("The plan is: ");
            foreach (ActionInstance action in queue)
            {
                Debug.Log("Q: " + action.ActionAsset.actionName);
            }

            return queue;
        }

        private bool achievesANeed(GoapActionAsset action, Dictionary<string, int> state)
        {
            foreach (WorldState condition in action.effects)
            {
                if (state.ContainsKey(condition.key) && state[condition.key] == condition.value)
                {
                    return true;
                }
            }

            return false;
        }

        private Dictionary<string, int> getPreConditions(GoapActionAsset action)
        {
            Dictionary<string, int> dictionary = new Dictionary<string, int>();

            if (action.preconditions != null)
            {
                foreach (WorldState state in action.preconditions)
                {
                    dictionary.Add(state.key, state.value);
                }
            }

            return dictionary;
        }

        private Dictionary<string, int> getEffects(GoapActionAsset action)
        {
            Dictionary<string, int> dictionary = new Dictionary<string, int>();

            if (action.effects != null)
            {
                foreach (WorldState state in action.effects)
                {
                    dictionary.Add(state.key, state.value);
                }
            }

            return dictionary;
        }

        private class CostComparer : IComparer<Node>
        {
            public int Compare(Node n1, Node n2)
            {
                float cost1 = n1.cost + n1.needs.Count;
                float cost2 = n2.cost + n2.needs.Count;

                return cost1 > cost2 ? 1 : -1;
            }
        }

        private Node BuildGraph(Node parent, List<GoapActionAsset> availableActions, Dictionary<string, int> currentStates)
        {
            List<Node> mainQueue = new List<Node>();

            mainQueue.Add(parent);

            while (mainQueue.Count > 0)
            {
                Node topLeave = mainQueue[0];
                mainQueue.RemoveAt(0);

                if (topLeave.needs.Keys.Count == 0)
                {
                    return topLeave;
                }

                // TODO: Sort actions by cost

                foreach (GoapActionAsset action in availableActions)
                {
                    if (achievesANeed(action, topLeave.needs))
                    {
                        Dictionary<string, int> preConditions = getPreConditions(action);
                        Dictionary<string, int> currentNeeds = new Dictionary<string, int>(preConditions);

                        Dictionary<string, int> effects = getEffects(action);

                        foreach (KeyValuePair<string, int> need in topLeave.needs)
                        {
                            if (!effects.ContainsKey(need.Key) || effects[need.Key] != need.Value)
                            {
                                if (!currentNeeds.ContainsKey(need.Key))
                                {
                                    currentNeeds.Add(need.Key, need.Value);
                                }
                            }
                        }

                        List<string> toRemove = new List<string>();

                        foreach (KeyValuePair<string, int> need in currentNeeds)
                        {
                            if (currentStates.ContainsKey(need.Key) && need.Value == currentStates[need.Key])
                            {
                                toRemove.Add(need.Key);
                            }
                        }

                        foreach (string key in toRemove)
                        {
                            if (currentNeeds.ContainsKey(key))
                            {
                                currentNeeds.Remove(key);
                            }
                        }

                        Node node = new Node(topLeave, topLeave.cost + action.baseCost, currentNeeds, action);


                        if (currentNeeds.Count == 0)
                        {
                            return node;
                        }


                        CostComparer comparer = new CostComparer();

                        if (mainQueue.Count == 0)
                        {
                            mainQueue.Add(node);
                        }
                        else
                        {
                            int index = mainQueue.BinarySearch(node, comparer);
                            mainQueue.Insert(index, node);
                        }

                        availableActions = ActionSubset(availableActions, action);
                    }
                }
            }

            return null;
        }

        private bool goalAchieved(Dictionary<string, int> goal, Dictionary<string, int> state)
        {
            foreach(KeyValuePair<string, int> g in goal)
            {
                if (!state.ContainsKey(g.Key) || state[g.Key] != g.Value)
                {
                    return false;
                }
            }
            return true;
        }

        private List<GoapActionAsset> ActionSubset(List<GoapActionAsset> actions, GoapActionAsset toRemove)
        {
            List<GoapActionAsset> subset = new List<GoapActionAsset>();
            foreach (GoapActionAsset action in actions)
            {
                if (!action.Equals(toRemove))
                {
                    subset.Add(action);
                }
            }
            return subset;
        }
    }
}
