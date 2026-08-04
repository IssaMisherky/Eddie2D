using UnityEngine;


public class DogeUtils
{
    public static float[] ScoreSides(Vector2 bulletPoint, Vector2 bulletVelocity, Vector2 playerPosition)
    {
        Vector2 dartNormal = bulletVelocity.normalized;

        Vector2 perpendicularNormal1 = new Vector2(-dartNormal.y, dartNormal.x);
        Vector2 perpendicularNormal2 = new Vector2(dartNormal.y, -dartNormal.x);

        Vector2 p = playerPosition - bulletPoint;

        Vector2 targetNormal = Vector2.Dot(p, perpendicularNormal1) > 0 ? perpendicularNormal1 : perpendicularNormal2;

        return new float[] {
            Vector2.Dot(Vector2.up, targetNormal),
            Vector2.Dot(Vector2.right, targetNormal),
            Vector2.Dot(Vector2.down, targetNormal),
            Vector2.Dot(Vector2.left, targetNormal)
        };
    }

    public static float[] ScoreByMostPerpendicular(Vector2 bulletVelocity)
    {
        Vector2 dartNormal = bulletVelocity.normalized;

        return new float[] {
            1 - Mathf.Abs(Vector2.Dot(Vector2.up, dartNormal)),
            1 - Mathf.Abs(Vector2.Dot(Vector2.right, dartNormal)),
            1 - Mathf.Abs(Vector2.Dot(Vector2.down, dartNormal)),
            1 - Mathf.Abs(Vector2.Dot(Vector2.left, dartNormal))
        };
    }

    public static float[] ScoreByDistance(Vector2 playerPosition, Vector2 opponentPosition)
    {
        Vector2 normal = (playerPosition - opponentPosition).normalized;

        return new float[] {
            Vector2.Dot(Vector2.up, normal),
            Vector2.Dot(Vector2.right, normal),
            Vector2.Dot(Vector2.down, normal),
            Vector2.Dot(Vector2.left, normal)
        };
    }
}