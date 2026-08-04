using GOAPSystem;
using UnityEngine;


public abstract class GoapActionAsset : ScriptableObject
{
    public string actionName;
    public float baseCost;
    // public WorldStateRequirement[] preconditions;
    // public WorldStateRequirement[] effects;

    public WorldState[] preconditions;
    public WorldState[] effects;


    public virtual void OnStart(GameObject npc, ActionInstance instance) { }
    public abstract ActionStatus Perform(GameObject npc, ActionInstance instance);
    public virtual void OnEnd(GameObject npc, ActionInstance instance) { }
}