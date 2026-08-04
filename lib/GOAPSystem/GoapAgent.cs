using UnityEngine;
using System.Collections.Generic;


namespace GOAPSystem
{
    public class GoalsLayer
    {
        public Dictionary<string, int> goals;
        public bool remove;

        public GoalsLayer(string key, int value, bool remove)
        {
            goals = new Dictionary<string, int>();
            goals.Add(key, value);
            this.remove = remove;
        }
    }


    public class GoapAgent : MonoBehaviour
    {
        [Header("Configuration")]
        public List<GoapActionAsset> actionLibrary;

        protected WorldStates agentStates;
        protected GoalsLayer goal;


        public WorldStates GetWorldStates()
        {
            return agentStates;
        }

        public GoalsLayer GetGoals()
        {
            return goal;
        }

        public List<GoapActionAsset> GetActions()
        {
            return actionLibrary;
        }
    }
}