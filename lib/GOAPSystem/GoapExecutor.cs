using GOAPSystem;
using UnityEngine;
using System.Collections.Generic;


[RequireComponent(typeof(GoapAgent))]
public class GoapExecuter : MonoBehaviour
{
    private GoapAgent _agent;
    private Queue<ActionInstance> _currentPlan = new Queue<ActionInstance>();
    private bool _isPlanning = false;
    private ActionInstance _activeAction;


    void Awake()
    {
        _agent = GetComponent<GoapAgent>();
    }

    private void Update()
    {
        if (_isPlanning)
        {
            return;
        }

        if ((_currentPlan == null || _currentPlan.Count == 0) && _activeAction == null)
        {
            //StartCoroutine(RequestPlanRoutine());
            RequestPlanRoutine();
            return;
        }

        ExecutePlan();
    }


    //private IEnumerator RequestPlanRoutine()
    private void RequestPlanRoutine()
    {
        _isPlanning = true;


        //var planRequest = GoapPlannerService.Instance.PlanAsync(this.gameObject, myGoals);

        //yield return planRequest;

        //_currentPlan = planRequest.Result;

        _currentPlan = GOAPPlanner.Instance.plan(gameObject, _agent.GetActions(), _agent.GetGoals().goals, _agent.GetWorldStates());

        _isPlanning = false;
    }

    private void ExecutePlan()
    {
        if (_activeAction == null && _currentPlan.Count > 0)
        {
            _activeAction = _currentPlan.Dequeue();
            _activeAction.ActionAsset.OnStart(gameObject, _activeAction);
        }

        if (_activeAction == null)
        {
            return;
        }

        ActionStatus status = _activeAction.ActionAsset.Perform(gameObject, _activeAction);//, Time.deltaTime);

        if (status == ActionStatus.Success)
        {
            _activeAction.ActionAsset.OnEnd(gameObject, _activeAction);
            _activeAction = null;
        }
        else if (status == ActionStatus.Failure)
        {
            AbortPlan();
        }
    }


    public void AbortPlan()
    {
        if (_activeAction != null)
        {
            _activeAction.ActionAsset.OnEnd(gameObject, _activeAction);
            _activeAction = null;
        }

        _currentPlan.Clear();
    }


    // TODO: Remove, this is only for debugging
    public string GetCurrentAction()
    {
        return _activeAction != null ? _activeAction.ActionAsset.name : "----None----";
    }
}