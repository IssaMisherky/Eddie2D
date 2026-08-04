using System.Linq;
using UnityEngine;


public class PolyGraph
{
    private Vector2 scale = Vector2.one;
    private Vector2 offset;
    private Vector2[] graph;


    public PolyGraph(Vector2[] graph)
    {
        this.graph = graph;
    }
    

    private Vector2[] offsetGraph(Vector2[] graph, Vector2 offset)
    {
        for (int i = 0; i < graph.Length; i++)
        {
            graph[i] = graph[i] + offset;
        }

        return graph;
    }

    private Vector2[] applyScale(Vector2[] graph, Vector2 scale)
    {
        for (int i = 0; i < graph.Length; i++)
        {
            graph[i] = graph[i] * scale;
        }

        return graph;
    }

    //private Vector2[] graphToLocalVerticies(Vector2[] graph,
    //bool rightSide = true,
    //bool reversed = false, bool flippedX = false, bool calculateScale = true)
    //{

    //    Vector2[] vertices = new Vector2[graph.Length];

    //    int multiplier = rightSide ? 1 : -1;
    //    int xFlipMultiplier = flippedX ? -1 : 1;


    //    for (int i = 0; i < graph.Length; i++)
    //    {
    //        int index = reversed ? (graph.Length - 1 - i) : i;

    //        vertices[i] = new Vector2(
    //            multiplier * (halfSize + (graph[index].x * xFlipMultiplier)) / scale.x,
    //            graph[index].y / scale.y);
    //    }

    //    return vertices;
    //}

    public void AddOffset(Vector2 offset)
    {
        this.offset += offset;
    }

    public void Scale(Vector2 scale)
    {
        this.scale *= scale;
    }

    public PolyGraph flipX()
    {
        scale.x *= -1;

        return this;
    }

    public Vector2[] bakeGraph()
    {
        Vector2[] baked = (Vector2[])graph.Clone();

        offsetGraph(baked, offset);
        applyScale(baked, scale);

        return baked;
    }


    public static Vector2[] reverse(Vector2[] poly)
    {
        return poly.Reverse().ToArray();
    }

    public static Vector2[] merge(params Vector2[][] polys)
    {
        Vector2[] main = polys[0];

        for (int i = 1; i < polys.Length; i++)
        {
            main = main.Concat(polys[i]).ToArray();
        }

        return main;
    }
}