using UnityEngine;


public abstract class NPCStepAction : NPCMovementAction
{
    protected NPCStepAction(MovementController controller) : base(controller)
    {
    }

    public abstract StepPoint GetTargetPlatform();
}