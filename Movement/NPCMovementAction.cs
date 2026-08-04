using UnityEngine;


public class NPCMovementAction : NPCAbstractAction
{
    private AbstractBody _body;
    public AbstractBody body
    {
        get
        {
            return _body;
        }
        protected set
        {
            _body = value;
        }
    }


    protected virtual Vector2 PositionMargin()
    {
        return Vector2.zero;
    }


    public NPCMovementAction(MovementController controller): base(controller)
    {   
        body = controller.GetBody();
    }

    public override Status GetStatus()
    {
        // TODO: Might want to change the is valid function to a bool or simething from RealtimeMovemnentAction
        return controller.IsValid() ? Status.DONE : Status.ERROR;
    }

    public MovementController getController()
    {
        return controller;
    }
}
