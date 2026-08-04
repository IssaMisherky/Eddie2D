using UnityEngine;

public class LinearGraph
{
    public bool isVertical;
    public float a, b;

    private void init(Line line)
    {
        float deltaX = line.p2.x - line.p1.x;

        if (deltaX == 0)
        {
            isVertical = true;
            a = line.p1.x;
            b = 0;

            return;
        }

        isVertical = false;

        float deltaY = line.p2.y - line.p1.y;

        a = deltaY / deltaX;
        b = line.p1.y - a * line.p1.x;
    }

    public LinearGraph(Line line)
    {
        init(line);
    }

    public LinearGraph(Vector2 p1, Vector2 p2)
    {
        Line l = new Line(p1, p2);

        init(l);
    }

    public float subValue(Vector2 point)
    {
        return a * point.x - point.y + b;
    }
}