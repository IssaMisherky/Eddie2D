using UnityEngine;


// TODO: Throw error in case multiple exist for one gameobject
public class RunnerSingleAction : MonoBehaviour
{
    private ScriptedActionV1 _action;


    private void FixedUpdate()
    {
        if (_action == null)
        {
            return;
        }

        _action.FixedUpdate();
    }

    private void LateUpdate()
    {
        if (_action == null)
        {
            return;
        }

        _action.LateUpdate();
    }


    public void setAction(ScriptedActionV1 action)
    {
        _action = action;

        // FixedUpdate();
    }

    public ScriptedActionV1 currentAction()
    {
        return _action;
    }
}