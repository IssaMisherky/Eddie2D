using UnityEngine;

using System.Collections.Generic;


namespace GOAPSystem
{
    [RequireComponent(typeof(GoapAgent))]
    public abstract class GOAPAction : MonoBehaviour
    {
        public string _actionName = "[ Action Name ]";
        public string actionName
        {
            get { return _actionName; }
            private set { _actionName = value; }
        }

        [SerializeField]
        private float cost = 1.0f;
        [SerializeField]
        private WorldState[] preConditions;
        [SerializeField]
        private WorldState[] afterEffects;


        private Dictionary<string, int> _preConditions;
        private Dictionary<string, int> _afterEffects;


        public bool running = false;


        public GOAPAction()
        {
            _preConditions  = new Dictionary<string, int>();
            _afterEffects   = new Dictionary<string, int>();
        }


        private void addStatesToDictionary(WorldState[] states, Dictionary<string, int> dictionary)
        {
            if (states != null)
            {
                foreach (WorldState state in states)
                {
                    dictionary.Add(state.key, state.value);
                }
            }
        }

        protected virtual void Awake()
        {
            addStatesToDictionary(preConditions, _preConditions);
            addStatesToDictionary(afterEffects, _afterEffects);
        }


        public virtual bool IsAvailable()
        {
            return true;
        }

        public bool IsAchievable(Dictionary<string, int> conditions)
        {
            foreach (KeyValuePair<string, int> condition in _preConditions)
            {
                if (!conditions.ContainsKey(condition.Key))
                {
                    return false;
                }
            }

            return true;
        }


        public float getCost()
        {
            return cost;
        }

        public Dictionary<string, int> getAfterEffects()
        {
            return _afterEffects;
        }


        public virtual bool PrePerform() { return true; }
        public virtual bool PostPerform() { return true; }
    }
}