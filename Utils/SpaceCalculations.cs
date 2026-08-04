using UnityEngine;
using System.Collections.Generic;


public static class SpaceCalculations
{
    private static float[] collistion(float a, float b, float r)
    {
        float left = -2 * a * b;
        float right = Mathf.Sqrt(Mathf.Pow(2 * a * b, 2) - 4 * (1 + a * a) * (b * b - r * r));
        float div = 2 * (1 + a * a);

        float[] results = new float[]
        {
            (left - right) / div,
            (left + right) / div
        };

        return results;
    }

    public static Vector2? rayCollision(LinearGraph l1, LinearGraph l2)
    {
        if (l1.isVertical)
        {
            if (l2.isVertical)
            {
                return null;
            }

            return new Vector2(l1.a, l2.a * l1.a + l2.b);
        }
        else if (l2.isVertical)
        {
            return new Vector2(l2.a, l1.a * l2.a + l1.b);
        }

        float baseValue = l1.a - l2.a;

        if (baseValue == 0)
        {
            return null;
        }

        float x = (l2.b - l1.b) / baseValue;
        float y = l1.a * x + l1.b;

        return new Vector2(x, y);
    }

    public static Vector2? lineRayCollision(LinearGraph linearGraph, Line line)
    {
        LinearGraph lineGraph = new LinearGraph(line);

        Vector2? collision = rayCollision(linearGraph, lineGraph);

        if (collision == null)
        {
            return null;
        }

        Vector2 coll = (Vector2)collision;

        if (line.p1.x != line.p2.x)
        {
            if (coll.x >= Mathf.Min(line.p1.x, line.p2.x) && coll.x <= Mathf.Max(line.p1.x, line.p2.x))
            {
                return coll;
            }
        }
        else
        {
            if (coll.y >= Mathf.Min(line.p1.y, line.p2.y) && coll.y <= Mathf.Max(line.p1.y, line.p2.y))
            {
                return coll;
            }
        }

        return null;
    }

    public static Vector2[] GetCollistion(Line line, Vector2 circleCenter, float r, bool isGraphLine)
    {
        // TODO: Should be in the modifier minus
        Line absoluteLine = new Line(line.p1 - circleCenter, line.p2 - circleCenter);

        float minY = Mathf.Min(absoluteLine.p1.y, absoluteLine.p2.y);
        float maxY = minY == absoluteLine.p1.y ? absoluteLine.p2.y : absoluteLine.p1.y;

        float deltaY = absoluteLine.p1.y - absoluteLine.p2.y;
        float deltaX = absoluteLine.p1.x - absoluteLine.p2.x;

        List<Vector2> collisionPoints = new List<Vector2>();

        if (deltaX == 0)
        {
            float y1 = Mathf.Sqrt(r * r - absoluteLine.p1.x * absoluteLine.p1.x);

            if (float.IsNaN(y1))
            {
                return collisionPoints.ToArray();
            }

            float y2 = -y1;

            if (isGraphLine || (minY <= y1 && maxY >= y1))
            {
                collisionPoints.Add(new Vector2(absoluteLine.p1.x, y1) + circleCenter);
            }

            if (isGraphLine || (minY <= y2 && maxY >= y2))
            {
                collisionPoints.Add(new Vector2(absoluteLine.p1.x, y2) + circleCenter);
            }

            return collisionPoints.ToArray();
        }
        else
        {
            float a = deltaY / deltaX;
            float b = absoluteLine.p2.y - a * absoluteLine.p2.x;

            float[] results = collistion(a, b, r);

            for (int i = 0; i < results.Length; i++)
            {
                if (!float.IsNaN(results[i]))
                {
                    float collisionX = results[i];

                    Vector2 point = new Vector2(collisionX, a * collisionX + b);

                    if (isGraphLine)
                    {
                        collisionPoints.Add(point + circleCenter);
                    }
                    else
                    {
                        if (maxY == minY)
                        {
                            float minX = Mathf.Min(absoluteLine.p1.x, absoluteLine.p2.x);
                            float maxX = minX == absoluteLine.p1.x ? absoluteLine.p2.x : absoluteLine.p1.x;

                            if (point.x <= maxX && point.x >= minX)
                            {
                                collisionPoints.Add(point + circleCenter);
                            }
                        }
                        else if (point.y <= maxY && point.y >= minY)
                        {
                            collisionPoints.Add(point + circleCenter);
                        }
                    }
                }
            }
        }

        return collisionPoints.ToArray();
    }

    public static Vector2[] raycastGoesThroughPoly(LinearGraph raycast, Vector2[] polyPoints)
    {
        List<Vector2> result = new List<Vector2>();

        for (int ind = 0; ind < polyPoints.Length; ind++)
        {
            // TODO: Convert to function and use every where it is used insead
            Vector2 from = polyPoints[(ind - 1 + polyPoints.Length) % polyPoints.Length];
            Vector2 to = polyPoints[ind];

            Line e = new Line(from, to);

            Vector2? coll = SpaceCalculations.lineRayCollision(raycast, e);

            if (coll != null)
            {
                // TODO: Check error margin instead
                Vector2 roundedCollsion = new Vector2(Mathf.Round(coll.Value.x * 1000) * 0.0001f, Mathf.Round(coll.Value.y * 1000) * 0.0001f);
                Vector2 roundedStart = new Vector2(Mathf.Round(e.p1.x * 1000) * 0.0001f, Mathf.Round(e.p1.y * 1000) * 0.0001f);
                Vector2 roundedEnd = new Vector2(Mathf.Round(e.p2.x * 1000) * 0.0001f, Mathf.Round(e.p2.y * 1000) * 0.0001f);

                if (roundedStart == roundedCollsion)
                {
                    Vector2 prevPoint = polyPoints[(ind - 2 + polyPoints.Length) % polyPoints.Length];
                    Vector2 nextPoint = polyPoints[ind];

                    bool sideFlipPrev = raycast.subValue(prevPoint) > 0;
                    bool sideFlipNext = raycast.subValue(nextPoint) > 0;

                    if (sideFlipPrev != sideFlipNext)
                    {
                        result.Add(coll.Value);
                    }
                }
                else if (roundedEnd == roundedCollsion)
                {
                    Vector2 prevPoint = from;
                    Vector2 nextPoint = polyPoints[(ind + 1) % polyPoints.Length];

                    bool sideFlipPrev = raycast.subValue(prevPoint) > 0;
                    bool sideFlipNext = raycast.subValue(nextPoint) > 0;

                    if (sideFlipPrev != sideFlipNext)
                    {
                        result.Add(coll.Value);
                    }
                }
                else
                {
                    result.Add(coll.Value);
                }
            }
        }

        return result.ToArray();
    }

    public static float angleToLine(Line l, Vector2 point)
    {
        Vector2 dir = new Vector2(l.p2.y - l.p1.y, l.p2.x - l.p1.x);

        return 2;

        //Vector2 collisionPoint = ;
    }

    private static Arc arcCirclePoly(Vector2[] polyPoints, Vector2 arcCenter, float radius)
    {
        List<Vector2> points = new List<Vector2>();
        List<Vector2[]> ranges = new List<Vector2[]>();
        List<int> collisionIndexs = new List<int>();


        for (int ind = 0; ind < polyPoints.Length; ind++)
        {
            // graph[ind - 1] -> graph[i];
            Vector2 startPoint;

            if (ind == 0)
            {
                startPoint = polyPoints[polyPoints.Length - 1];
            }
            else
            {
                startPoint = polyPoints[ind - 1];
            }

            points.Add(startPoint);

            Line line = new Line(startPoint, polyPoints[ind]);

            Vector2[] collisionPoints = SpaceCalculations.GetCollistion(line, arcCenter, radius, false);

            // TODO: Order collisionPoints
            // Supposed to be in order already

            points.AddRange(collisionPoints);

            for (int counter = collisionPoints.Length; counter >= 1; counter--)
            {
                collisionIndexs.Add(points.Count - counter);
            }

            points.Add(polyPoints[ind]);
        }

        Arc arc = new Arc();

        if (collisionIndexs.Count == 1)
        {
            // TODO: Duplicate index
        }

        for (int ind = 0; ind < collisionIndexs.Count; ind++)
        {
            Vector2 beforePoint = points[collisionIndexs[ind] - 1];
            Vector2 afterPoint = points[collisionIndexs[ind] + 1];

            bool isGoingIn = Vector2.Distance(beforePoint, arcCenter) > Vector2.Distance(afterPoint, arcCenter);

            if (!isGoingIn)
            {
                Vector2 startPoint = points[collisionIndexs[ind]];
                Vector2 endPoint = points[collisionIndexs[(ind + 1) % collisionIndexs.Count]];

                startPoint -= arcCenter;
                endPoint -= arcCenter;

                float angle1 = Mathf.Atan2(startPoint.x, startPoint.y);
                float angle2 = Mathf.Atan2(endPoint.x, endPoint.y);

                arc.addRange(angle1, angle2);
            }
        }

        return arc;
    }

    //public static float[] GetViewRange(Platform platform, Transform player, float radius, float bulletRadius)
    //{
    //    // SpriteRenderer spriteRenderer = player.GetComponent<SpriteRenderer>();

    //    PlatformNetwork network = platform.GetComponent<PlatformNetwork>();

    //    Vector2[] graph = network.getJumpBackPoints();

    //    List<float> shootingPoints = new List<float>();

    //    //Arc arc = arcCirclePoly(graph, player.position, radius);

    //    //if (arc.ranges.Length == 0)
    //    //{
    //    //    return new float[0];
    //    //}

    //    float halfHeight = platform.GetComponent<SpriteRenderer>().bounds.size.y / 2;

    //    bool downShot;
    //    Platform handlePlatform;

    //    if (player.position.y <= platform.transform.position.y)
    //    {
    //        downShot = true;
    //        handlePlatform = platform;
    //    }
    //    else
    //    {
    //        downShot = false;
    //        LayerMask layerMask = LayerMask.GetMask("PlatformBase");

    //        RaycastHit2D[] platformsReach = Physics2D.RaycastAll(player.position, Vector2.down, radius, layerMask);

    //        Platform closest = null;

    //        foreach (RaycastHit2D hit in platformsReach)
    //        {
    //            Platform p = hit.collider.GetComponent<Platform>();

    //            if (closest == null || closest.transform.position.y < p.transform.position.y)
    //            {
    //                closest = p;
    //            }
    //        }

    //        handlePlatform = closest;

    //        if (closest == null)
    //        {
    //            float angle = Mathf.PI;

    //            shootingPoints.Add(arc.pointScan(angle, false));
    //            shootingPoints.Add(arc.pointScan(angle, true));

    //            return shootingPoints.ToArray();
    //        }
    //    }

    //    float[] platformEdges = handlePlatform.getPlatformEdges();
    //    float fallLeftEdgeX = platformEdges[0] - bulletRadius;
    //    float fallRightEdgeX = platformEdges[1] + bulletRadius;

    //    float yPos = handlePlatform.transform.position.y + halfHeight;

    //    Vector2 pointRight = new Vector2(fallRightEdgeX, yPos);
    //    Vector2 pointLeft = new Vector2(fallLeftEdgeX, yPos);

    //    Line lineFromRight = new Line(pointRight, player.position);
    //    Line lineFromLeft = new Line(pointLeft, player.position);

    //    Vector2[] rightPoints = SpaceCalculations.GetCollistion(lineFromRight, player.position, radius, true);
    //    Vector2[] leftPoints = SpaceCalculations.GetCollistion(lineFromLeft, player.position, radius, true);

    //    Vector2? getr(Vector2[] points)
    //    {
    //        Vector2? rightPoint = null;

    //        if (points.Length > 0)
    //        {
    //            rightPoint = points[0];
    //            for (int pIndex = 1; pIndex < points.Length; pIndex++)
    //            {
    //                if (downShot)
    //                {
    //                    if (((Vector2)rightPoint).y < points[pIndex].y)
    //                    {
    //                        rightPoint = points[pIndex];
    //                    }
    //                }
    //                else
    //                {
    //                    if (((Vector2)rightPoint).y > points[pIndex].y)
    //                    {
    //                        rightPoint = points[pIndex];
    //                    }
    //                }
    //            }
    //        }

    //        return rightPoint;
    //    }

    //    Vector2? rightPoint = getr(rightPoints);
    //    Vector2? leftPoint = getr(leftPoints);

    //    if (rightPoint != null)
    //    {
    //        Vector2 absolutePoint = (Vector2)rightPoint - (Vector2)player.position;

    //        float angle = Mathf.Atan2(absolutePoint.x, absolutePoint.y);

    //        float result = arc.pointScan(angle, false);

    //        shootingPoints.Add(result);
    //    }

    //    if (leftPoint != null)
    //    {
    //        Vector2 absolutePoint = (Vector2)leftPoint - (Vector2)player.position;

    //        float angle = Mathf.Atan2(absolutePoint.x, absolutePoint.y);

    //        float result = arc.pointScan(angle, true);

    //        shootingPoints.Add(result);
    //    }

    //    return shootingPoints.ToArray();
    //}

    //public static Vector2[] GetPlatformView(Vector2[] polyPoints, Transform player)
    //{
    //    Vector2 point = player.transform.position;


    //    List<int> indexes = new List<int>();
    //    List<Vector2> directions = new List<Vector2>();
    //    for (int i = 0; i < polyPoints.Length; i++)
    //    {
    //        Line line = new Line(point, polyPoints[i]);
    //        LinearGraph lg = new LinearGraph(line);
    //        bool isThrough = false;

    //        for (int ind = 0; ind < polyPoints.Length; ind++)
    //        {
    //            Vector2 from = polyPoints[(ind - 1 + polyPoints.Length) % polyPoints.Length];
    //            Vector2 to = polyPoints[ind];

    //            Line e = new Line(from, to);


    //            Vector2? coll = SpaceCalculations.lineRayCollision(lg, e);

    //            if (coll != null)
    //            {
    //                // TODO: Check error margin instead
    //                Vector2 roundedCollsion = new Vector2(Mathf.Round(coll.Value.x * 1000) * 0.0001f, Mathf.Round(coll.Value.y * 1000) * 0.0001f);
    //                Vector2 roundedStart = new Vector2(Mathf.Round(e.p1.x * 1000) * 0.0001f, Mathf.Round(e.p1.y * 1000) * 0.0001f);
    //                Vector2 roundedEnd = new Vector2(Mathf.Round(e.p2.x * 1000) * 0.0001f, Mathf.Round(e.p2.y * 1000) * 0.0001f);

    //                if (roundedStart == roundedCollsion)
    //                {
    //                    Vector2 prevPoint = polyPoints[(ind - 2 + polyPoints.Length) % polyPoints.Length];
    //                    Vector2 nextPoint = polyPoints[ind];

    //                    bool sideFlipPrev = lg.subValue(prevPoint) > 0;
    //                    bool sideFlipNext = lg.subValue(nextPoint) > 0;

    //                    if (sideFlipPrev != sideFlipNext)
    //                    {
    //                        isThrough = true;
    //                        break;
    //                    }
    //                }
    //                else if (roundedEnd == roundedCollsion)
    //                {
    //                    Vector2 prevPoint = from;
    //                    Vector2 nextPoint = polyPoints[(ind + 1) % polyPoints.Length];

    //                    bool sideFlipPrev = lg.subValue(prevPoint) > 0;
    //                    bool sideFlipNext = lg.subValue(nextPoint) > 0;

    //                    if (sideFlipPrev != sideFlipNext)
    //                    {
    //                        isThrough = true;
    //                        break;
    //                    }
    //                }
    //                else
    //                {
    //                    isThrough = true;
    //                    break;
    //                }
    //            }
    //        }

    //        if (!isThrough)
    //        {
    //            indexes.Add(i);

    //            directions.Add(polyPoints[i] - point);
    //        }
    //    }

    //    if (indexes.Count == 2)
    //    {
    //        Vector2 opposite = new Vector2(-directions[0].y, directions[0].x);

    //        Line l = new Line(Vector2.zero, directions[0]);
    //        LinearGraph lg = new LinearGraph(l);

    //        bool isFlipped = lg.subValue(opposite) > 0;

    //        if ((lg.subValue(directions[1]) > 0) != isFlipped)
    //        {
    //            Vector2 temp = directions[0];

    //            directions[0] = directions[1];
    //            directions[1] = temp;
    //        }

    //        return directions.ToArray();
    //    }

    //    return null;
    //}

    //public static Arc ViewRange(Platform platform, Transform player)
    //{
    //    Vector2 point = player.transform.position;

    //    BoxCollider2D collider = platform.GetComponent<BoxCollider2D>();

    //    Vector2 halfSize = collider.bounds.size / 2;

    //    Vector2[] polyPoints = new Vector2[]
    //    {
    //        new Vector2(-halfSize.x, halfSize.y) + (Vector2)platform.transform.position,
    //        halfSize + (Vector2)platform.transform.position,
    //        new Vector2(halfSize.x, -halfSize.y) + (Vector2)platform.transform.position,
    //        -halfSize + (Vector2)platform.transform.position
    //    };

    //    List<int> indexes = new List<int>();
    //    List<Vector2> directions = new List<Vector2>();
    //    for (int i = 0; i < polyPoints.Length; i++)
    //    {
    //        Line line = new Line(point, polyPoints[i]);
    //        LinearGraph lg = new LinearGraph(line);
    //        bool isThrough = false;

    //        for (int ind = 0; ind < polyPoints.Length; ind++)
    //        {
    //            Vector2 from = polyPoints[(ind - 1 + polyPoints.Length) % polyPoints.Length];
    //            Vector2 to = polyPoints[ind];

    //            Line e = new Line(from, to);


    //            Vector2? coll = SpaceCalculations.lineRayCollision(lg, e);

    //            if (coll != null)
    //            {
    //                Vector2 roundedCollsion = new Vector2(Mathf.Round(coll.Value.x * 1000) * 0.0001f, Mathf.Round(coll.Value.y * 1000) * 0.0001f);
    //                Vector2 roundedStart = new Vector2(Mathf.Round(e.p1.x * 1000) * 0.0001f, Mathf.Round(e.p1.y * 1000) * 0.0001f);
    //                Vector2 roundedEnd = new Vector2(Mathf.Round(e.p2.x * 1000) * 0.0001f, Mathf.Round(e.p2.y * 1000) * 0.0001f);

    //                if (roundedStart == roundedCollsion)
    //                {
    //                    Vector2 prevPoint = polyPoints[(ind - 2 + polyPoints.Length) % polyPoints.Length];
    //                    Vector2 nextPoint = polyPoints[ind];

    //                    bool sideFlipPrev = lg.subValue(prevPoint) > 0;
    //                    bool sideFlipNext = lg.subValue(nextPoint) > 0;

    //                    if (sideFlipPrev != sideFlipNext)
    //                    {
    //                        isThrough = true;
    //                        break;
    //                    }
    //                }
    //                else if (roundedEnd == roundedCollsion)
    //                {
    //                    Vector2 prevPoint = from;
    //                    Vector2 nextPoint = polyPoints[(ind + 1) % polyPoints.Length];

    //                    bool sideFlipPrev = lg.subValue(prevPoint) > 0;
    //                    bool sideFlipNext = lg.subValue(nextPoint) > 0;

    //                    if (sideFlipPrev != sideFlipNext)
    //                    {
    //                        isThrough = true;
    //                        break;
    //                    }
    //                }
    //                else
    //                {
    //                    isThrough = true;
    //                    break;
    //                }
    //            }
    //        }

    //        if (!isThrough)
    //        {
    //            indexes.Add(i);

    //            Vector2 dir = (polyPoints[i] - point).normalized;

    //            directions.Add(dir);
    //        }
    //    }

    //    if (indexes.Count == 2)
    //    {
    //        Vector2 opposite = new Vector2(-directions[0].y, directions[0].x);

    //        Line l = new Line(Vector2.zero, directions[0]);
    //        LinearGraph lg = new LinearGraph(l);

    //        bool isFlipped = lg.subValue(opposite) > 0;

    //        if ((lg.subValue(directions[1]) > 0) != isFlipped)
    //        {
    //            Vector2 temp = directions[0];

    //            directions[0] = directions[1];
    //            directions[1] = temp;
    //        }

    //        float angle1 = Mathf.Atan2(directions[1].x, directions[1].y);
    //        float angle2 = Mathf.Atan2(directions[0].x, directions[0].y);

    //        Arc arc = new Arc();

    //        arc.addRange(angle1, angle2);

    //        return arc;
    //    }

    //    return null;
    //}

    //public static float[] GetPlayerPlatformViewRange(Platdform platform, Transform player, float radius, float bulletRadius)
    //{
    //    // SpriteRenderer spriteRenderer = player.GetComponent<SpriteRenderer>();

    //    PlatformNetwork network = platform.GetComponent<PlatformNetwork>();

    //    Vector2[] graph = network.getJumpBackPoints();

    //    List<float> shootingPoints = new List<float>();

    //    Arc arc = arcCirclePoly(graph, player.position, radius);

    //    if (arc.ranges.Length == 0)
    //    {
    //        return new float[0];
    //    }

    //    float halfHeight = platform.GetComponent<SpriteRenderer>().bounds.size.y / 2;

    //    bool downShot;
    //    Platform handlePlatform;

    //    if (player.position.y <= platform.transform.position.y)
    //    {
    //        downShot = true;
    //        handlePlatform = platform;
    //    }
    //    else
    //    {
    //        downShot = false;
    //        LayerMask layerMask = LayerMask.GetMask("PlatformBase");

    //        RaycastHit2D[] platformsReach = Physics2D.RaycastAll(player.position, Vector2.down, radius, layerMask);

    //        //if (platformsReach.Length == 0)
    //        //{
    //        //    return shootingPoints.ToArray();
    //        //}

    //        Platform closest = null;

    //        foreach (RaycastHit2D hit in platformsReach)
    //        {
    //            Platform p = hit.collider.GetComponent<Platform>();

    //            if (closest == null || closest.transform.position.y < p.transform.position.y)
    //            {
    //                closest = p;
    //            }
    //        }

    //        handlePlatform = closest;

    //        if (closest == null)
    //        {
    //            float angle = Mathf.PI;

    //            shootingPoints.Add(arc.pointScan(angle, false));
    //            shootingPoints.Add(arc.pointScan(angle, true));

    //            return shootingPoints.ToArray();
                
    //            // TODO: Handle. Should get lowest point as shooting point and not continue to the below section
    //            //if (platform.transform.position.x > player.position.x)
    //            //{
    //            //    arc.pointScan(, false);
    //            //}  
    //            //else
    //            //{
    //            //    arc.pointScan(, true);
    //            //}
    //        }
    //    }

    //    float[] platformEdges = handlePlatform.getPlatformEdges();
    //    float fallLeftEdgeX = platformEdges[0] - bulletRadius;
    //    float fallRightEdgeX = platformEdges[1] + bulletRadius;

    //    float yPos = handlePlatform.transform.position.y + halfHeight;

    //    Vector2 pointRight = new Vector2(fallRightEdgeX, yPos);
    //    Vector2 pointLeft = new Vector2(fallLeftEdgeX, yPos);
        
    //    Line lineFromRight = new Line(pointRight, player.position);
    //    Line lineFromLeft = new Line(pointLeft, player.position);

    //    Vector2[] rightPoints = SpaceCalculations.GetCollistion(lineFromRight, player.position, radius, true);
    //    Vector2[] leftPoints = SpaceCalculations.GetCollistion(lineFromLeft, player.position, radius, true);

    //    Vector2? getr(Vector2[] points)
    //    {
    //        Vector2? rightPoint = null;

    //        if (points.Length > 0)
    //        {
    //            rightPoint = points[0];
    //            for (int pIndex = 1; pIndex < points.Length; pIndex++)
    //            {
    //                if (downShot)
    //                {
    //                    if (((Vector2)rightPoint).y < points[pIndex].y)
    //                    {
    //                        rightPoint = points[pIndex];
    //                    }
    //                }
    //                else
    //                {
    //                    if (((Vector2)rightPoint).y > points[pIndex].y)
    //                    {
    //                        rightPoint = points[pIndex];
    //                    }
    //                }
    //            }
    //        }

    //        return rightPoint;
    //    }

    //    Vector2? rightPoint = getr(rightPoints);
    //    Vector2? leftPoint  = getr(leftPoints);

    //    if (rightPoint != null)
    //    {
    //        Vector2 absolutePoint = (Vector2)rightPoint - (Vector2)player.position;

    //        float angle = Mathf.Atan2(absolutePoint.x, absolutePoint.y);

    //        float result = arc.pointScan(angle, false);

    //        shootingPoints.Add(result);
    //    }

    //    if (leftPoint != null)
    //    {
    //        Vector2 absolutePoint = (Vector2)leftPoint - (Vector2)player.position;

    //        float angle = Mathf.Atan2(absolutePoint.x, absolutePoint.y);

    //        float result = arc.pointScan(angle, true);

    //        shootingPoints.Add(result);
    //    }

    //    return shootingPoints.ToArray();
    //}
}
