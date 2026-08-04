using UnityEngine;


public class PlatformStand : MobilitySurface
{
    BoxCollider2D standCollider;


    public PlatformStand(StepPoint stepPoint, Vector2 offset, Vector2 size) : base(stepPoint)
    {
        standCollider = transform.gameObject.AddComponent<BoxCollider2D>();

        standCollider.size = size;
        standCollider.offset = offset;
    }

    public float getStandPlatformEdge(float characterWidth, bool rightSide, float offset = 0.1f)
    {
        int edgeFlipper = rightSide ? -1 : 1;

        int edgeIndex = rightSide ? 1 : 0;

        float[] targetEdges = getPlatformEdges();

        float edge = targetEdges[edgeIndex];

        return edge + (characterWidth / 2 + offset) * edgeFlipper;
    }

    public float[] getPlatformEdges()
    {
        float[] edges = new float[2];

        float halfWidth = standCollider.bounds.size.x / 2;

        edges[0] = mainTransform.position.x - halfWidth;
        edges[1] = mainTransform.position.x + halfWidth;

        return edges;
    }

    public int getPlatformDirection(float characterWidth, float fromXPosition)
    {
        // Shoud take it from the platformstand object

        float standRight = getStandPlatformEdge(characterWidth, true);
        float standLeft = getStandPlatformEdge(characterWidth, false);

        if (fromXPosition > standRight)
        {
            return -1;
        }

        if (fromXPosition < standLeft)
        {
            return 1;
        }

        return 0;
    }

    public Collider2D GetCollider()
    {
        return standCollider;
    }
}