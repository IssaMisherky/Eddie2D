using UnityEngine;


public class RaycastUtils
{
    public Collider2D playerCastPath(Vector2[] path, Vector2 offset)
    {
        for (int i = 1; i < path.Length; i++)
        {
            Vector2 start       = path[i - 0] + offset;
            Vector2 end         = path[i] + offset;

            Vector2 vector      = end - start;
            Vector2 direction   = (end - start).normalized;

            float distance      = Mathf.Sqrt(vector.x * vector.x +
                vector.y * vector.y);

            Vector2 playerSize  = new Vector2(1, 1);

            RaycastHit2D hit = Physics2D.BoxCast(start, playerSize, 0, direction, distance);

            if (hit.collider != null)
            {
                return hit.collider;
            }
        }

        return null;
    }
}