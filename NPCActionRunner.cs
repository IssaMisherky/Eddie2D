public class NPCActionRunner : RunnerSingleAction
{
    private NPCAbstractAction _npcAction;


    public new void setAction(ScriptedActionV1 _action)
    {
        if (_action != null && _action is not NPCAbstractAction)
        {
            throw new System.Exception("Action must be (NPCAbstractAction)");
        }

        if (_npcAction != null)
        {
            MovementController controller = _npcAction.GetController();

            controller.ResetInputs();
        }

        base.setAction(_action);

        _npcAction = (NPCAbstractAction)_action;
    }

    public new NPCAbstractAction currentAction()
    {
        return _npcAction;
    }
}