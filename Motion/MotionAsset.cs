using UnityEngine;


public abstract class MotionAsset : ScriptableObject
{
    public abstract MotionInstance Instance();


    public abstract void Setup(MovementController movementController, MotionInstance instance);

    public virtual void TimeUpdate(MovementController movementController, MotionInstance instance, float time) { }
}