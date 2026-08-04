using System.Collections.Generic;
using System.Net.NetworkInformation;


namespace GOAPSystem
{
    [System.Serializable]
    public class WorldState
    {
        public string key;
        public int value;
    }

    public class WorldStates
    {
        public Dictionary<string, int> states;

        public WorldStates()
        {
            states = new Dictionary<string, int>();
        }

        public bool hasState(string key)
        {
            return states.ContainsKey(key);
        }

        private void addState(string key, int value)
        {
            states.Add(key, value);
        }

        public void removeState(string key)
        {
            if (states.ContainsKey(key))
            {
                states.Remove(key);
            }
        }

        public void setState(string key, int value)
        {
            if (states.ContainsKey(key))
            {
                states[key] = value;
            }
            else
            {
                states.Add(key, value);
            }
        }

        public void ajustState(string key, int ajustment)
        {
            if (states.ContainsKey(key))
            {
                states[key] += ajustment;
                if (states[key] <= 0)
                {
                    removeState(key);
                }
            }
            else
            {
                states.Add(key, ajustment);
            }
        }

        public Dictionary<string, int> getStates()
        {
            return states;
        }

        public long Snapshot()
        {
            Dictionary<int, int> state = new Dictionary<int, int>();

            long worldValues = 0;
            long worldMask = 0;

            foreach (var entry in state)
            {
                int bitIndex = entry.Key; // The ID you mapped from a string earlier
                worldMask |= (1L << bitIndex);
                if (entry.Value > 0)
                { // If it's a "true" or positive value
                    worldValues |= (1L << bitIndex);
                }
            }

            return 0;
        }
    }
}