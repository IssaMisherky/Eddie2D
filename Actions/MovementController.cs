public abstract class MovementController : ScriptedAction
{
    protected AbstractBody body;

    //private PlayerMechanicsBody _playerMechanicsBody;


    public MovementController(AbstractBody body)
    {
        this.body = body;

        //_playerMechanicsBody = playerMechanicsBody;
    }


    public AbstractBody GetBody()
    {
        return body;
    }

    public abstract void ResetInputs();
}
