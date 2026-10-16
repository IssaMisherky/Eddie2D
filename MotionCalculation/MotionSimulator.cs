using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MotionSimulator
{
    public float simulationDeltaTime = 0.02f;
    public float stepTime = 0.1f;


    private float MaxDisplacement   = -5;

    private MotionAsset[] _motionAssets;
    private MovementController _movementManager;
    private IMotionSimulatorEvents _eventsHandler;

    private int stopIndex;
    private SimulationBody _body;
    private MotionInstance[] motions;


    private MotionGraphs motionGraphs;
    public MotionGraphs Graphs
    {
        get { return motionGraphs; }
    }

    private bool _isSimulated;
    public bool IsSimulated
    {
        get { return _isSimulated; }
    }


    public MotionSimulator(IMotionSimulatorEvents eventsHandler, MovementController movementManager, MotionAsset[] motionAssets)
    {
        _motionAssets       = motionAssets;
        _movementManager    = movementManager;
        _eventsHandler      = eventsHandler;

        AbstractBody abstractBody = _movementManager.GetBody();

        if (abstractBody is not SimulationBody)
        {
            throw new EddieException("MovementController can't have other than a SimulationBody when passed to MotionSimulator.");
        }

        _body = abstractBody as SimulationBody;
    }


    private IEnumerator SimulateMotion(MotionInstance motion, MovementController movementManager)
    {
        float timer = stepTime;

        while (true)
        {
            movementManager.FixedUpdate();
            _body.UpdateFrame(simulationDeltaTime);
            motion.asset.TimeUpdate(movementManager, motion, simulationDeltaTime);

            _eventsHandler.OnMotionLateUpdate();

            timer -= simulationDeltaTime;

            if (timer > 0)
            {
                continue;
            }

            timer = stepTime;

            yield return _body.Position;

            if (_body.Position.y <= MaxDisplacement)
            {
                break;
            }

            yield return null;
        }
    }

    private IEnumerator GraphMotion(List<Vector2> upwardsGraph, List<Vector2> downwardsGraph, MotionInstance motion)
    {
        upwardsGraph.Add(_body.Position);

        motion.asset.Setup(_movementManager, motion);

        _eventsHandler.OnMotionSetup();

        IEnumerator simulateMotion = SimulateMotion(motion, _movementManager);

        bool isStopped = false;

        while (simulateMotion.MoveNext())
        {
            var result = simulateMotion.Current;

            if (result != null)
            {
                if (_body.Position.y <= 0 && !isStopped)
                {
                    stopIndex = downwardsGraph.Count;

                    isStopped = true;
                }

                if (_body.Velocity.y <= 0)
                {
                    downwardsGraph.Add((Vector2)result);
                }
                else
                {
                    upwardsGraph.Add((Vector2)result);
                }
            }

            yield return null;
        }
    }

    public IEnumerator CalculateSimulations(OnSimulationFinish onFinish)
    {
        simulationDeltaTime = Time.fixedDeltaTime;

        MotionGraph[] graphs = new MotionGraph[_motionAssets.Length];
        motions = new MotionInstance[graphs.Length];

        for (int i = 0; i < _motionAssets.Length; i++)
        {
            MotionInstance motionInstance = _motionAssets[i].Instance();

            motions[i] = motionInstance;

            List<Vector2> jumpUpwardsGraph = new List<Vector2>();
            List<Vector2> JumpDownwardsGraph = new List<Vector2>();

            yield return GraphMotion(jumpUpwardsGraph, JumpDownwardsGraph, motionInstance);

            MotionGraph graph = new MotionGraph(jumpUpwardsGraph.ToArray(), JumpDownwardsGraph.ToArray(), stopIndex);

            graphs[i] = graph;
        }


        MotionGraphs motionGraphs = new MotionGraphs
        {
            orderedGraphs = graphs
        };

        this.motionGraphs = motionGraphs;

        onFinish.Invoke(motionGraphs);

        _isSimulated = true;
    }

    protected virtual void OnMotionSetup() { }
    protected virtual void OnMotionLateUpdate() { }

    public delegate void OnSimulationFinish(MotionGraphs graphs);


    public struct MotionGraphs
    {
        public MotionGraph[] orderedGraphs;
    }

    public class MotionGraph
    {
        public Vector2[] up;
        public Vector2[] down;
        public int stopIndex;

        public MotionGraph(Vector2[] up, Vector2[] down, int stopIndex)
        {
            this.up = up;
            this.down = down;
            this.stopIndex = stopIndex;
        }

        public Vector2[] DownStopped()
        {
            Vector2[] jumpStopped = new Vector2[stopIndex + 1];

            for (int vi = 0; vi < stopIndex; vi++)
            {
                jumpStopped[vi] = down[vi];
            }

            jumpStopped[jumpStopped.Length - 1] = new Vector2(down[stopIndex].x, 0);

            return jumpStopped;
        }
    }
}

public interface IMotionSimulatorEvents
{
    public void OnMotionSetup();
    public void OnMotionLateUpdate() { }
    public void OnMotionSimulated() { }
}