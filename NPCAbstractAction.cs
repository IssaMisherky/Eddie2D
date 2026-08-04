public class NPCAbstractAction : ScriptedActionV1
{
    protected MovementController controller;

    public enum Status
    {
        IN_ACTION,
        DONE,
        ERROR
    }


    public NPCAbstractAction(MovementController controller)
    {
        this.controller = controller;
    }


    public virtual Status GetStatus() { return Status.DONE; }

    protected void done(bool successfull)
    {
        if (OnDone != null)
        {
            OnDone.Invoke(successfull);
        }
    }

    public delegate void OnActionDone(bool successfull);
    public event OnActionDone OnDone;


    public MovementController GetController() { return controller; }
}