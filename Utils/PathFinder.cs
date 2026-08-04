using UnityEngine;

using System.Collections.Generic;


public class PlatformStep
{
    public StepPoint startPlatform;
    public StepPoint targetPlatform;

    public enum Type
    {
        None,
        NoJump,
        Jump
    }


    public PlatformStep(StepPoint startPlatform, StepPoint targetPlatform)
    {
        this.startPlatform  = startPlatform;
        this.targetPlatform = targetPlatform;
    }
}

public class PathFinder
{
    private static float distance(GameObject platform, GameObject other)
    {
        // TODO: Fix distance calculation
        // Should use SpriteRenderer bound size
        return Mathf.Sqrt(
            Mathf.Pow(platform.transform.position.x - other.transform.position.x, 2) +
            Mathf.Pow(platform.transform.position.y - other.transform.position.y, 2));
    }

    class PathStep
    {
        public PathStep basePlatform;

        public float distanceToTarget;
        public float distanceToCurrentPlatform;
        public float baseDistance;

        public GameObject gameobject;

        public PathStep(PathStep basePlatform, GameObject platform, Transform target)
        {
            this.basePlatform = basePlatform;

            distanceToTarget = distance(platform, target.gameObject);

            gameobject = platform;

            if (basePlatform == null)
            {
                distanceToCurrentPlatform = 0;
                baseDistance = 0;

                return;
            }
            
            distanceToCurrentPlatform = distance(platform, basePlatform.gameobject);
            baseDistance = basePlatform.baseDistance + basePlatform.distanceToCurrentPlatform;
        }

        public float cost()
        {
            return distanceToTarget + distanceToCurrentPlatform + baseDistance;
        }


        public class Comparer : IComparer<PathStep>
        {
            public int Compare(PathStep p1, PathStep p2)
            {
                float res = p1.cost() - p2.cost();

                return (int)res;
            }
        }
    }

    public static PlatformStep[] findPath(GameObject start, GameObject end)
    {
        if (start == end)
        {
            return new PlatformStep[0];
        }

        Dictionary<GameObject, bool> searched = new Dictionary<GameObject, bool>();

        List<PathStep> mainQueue = new List<PathStep>();

        mainQueue.Insert(0, new PathStep(null, start, end.transform));

        while (mainQueue.Count > 0)
        {
            PathStep basePlatform = mainQueue[0];
            mainQueue.RemoveAt(0);

            if (searched.ContainsKey(basePlatform.gameobject))
            {
                continue;
            }

            searched.Add(basePlatform.gameobject, true);

            ReachNetwork network = basePlatform.gameobject.GetComponent<ReachNetwork>();

            // TODO: Should get from somewhere instead of defining it every time
            ContactFilter2D filter = new ContactFilter2D();

            filter.useTriggers = true;
            filter.useLayerMask = true;
            filter.layerMask = LayerMask.GetMask("StepPoint");

            StepPoint[] reachables = network.InReach(filter);

            //PathStep[] platforms =
            //    new PathStep[singleReachables.Length + noJumpReachables.Length + jumpBackReachables.Length];

            // TODO: Add platforms should be unique
            Dictionary<GameObject, PathStep> platforms = new Dictionary<GameObject, PathStep>();

            for (int i = 0; i < reachables.Length; ++i)
            {
                platforms.Add(reachables[i].gameObject, new PathStep(basePlatform, reachables[i].gameObject, end.transform));
            }

            PathStep.Comparer comparer = new PathStep.Comparer();

            foreach (PathStep platform in platforms.Values)
            {
                // TODO: Remove this. As I added it above
                //if (searched.ContainsKey(platform.gameobject))
                //{
                //    continue;
                //}

                if (platform.gameobject == end)
                {
                    //Debug.Log("==============");
                    //Debug.Log("Found through:");

                    List<PlatformStep> pathList = new List<PlatformStep>();

                    PathStep parent = platform;

                    while (parent.basePlatform != null)
                    {
                        StepPoint currentPlatform    = parent.basePlatform.gameobject.GetComponent<StepPoint>();
                        StepPoint nextPlatform       = parent.gameobject.GetComponent<StepPoint>();

                        PlatformStep platformStep   = new PlatformStep(currentPlatform, nextPlatform);

                        Debug.Log(parent.gameobject);

                        pathList.Insert(0, platformStep);

                        parent = parent.basePlatform;
                    }

                    return pathList.ToArray();
                }

                int index = mainQueue.BinarySearch(platform, comparer);

                if (index < 0)
                {
                    mainQueue.Insert(~index, platform);
                }
            }
        }

        return null;
    }
}
