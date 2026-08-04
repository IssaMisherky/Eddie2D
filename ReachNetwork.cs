using UnityEngine;
using System.Collections.Generic;


public class ReachNetwork
{
    private Transform _origin;

    private GameObject _collidersGameObject;

    private PolygonCollider2D[] _levelColliders;


    public ReachNetwork(Transform origin, Vector2[][] reachLevels)
    {
        _origin = origin;

        _collidersGameObject = new GameObject();

        _collidersGameObject.transform.parent = origin;
        _collidersGameObject.layer = LayerMask.NameToLayer("ReachArea");
        _collidersGameObject.transform.localPosition = Vector3.zero;
        _collidersGameObject.transform.localScale = Vector3.one;


        _levelColliders = new PolygonCollider2D[reachLevels.Length];


        for (int i = 0; i < reachLevels.Length; i++)
        {
            PolygonCollider2D collider = _collidersGameObject.AddComponent<PolygonCollider2D>();

            collider.enabled = false;
            collider.isTrigger = true;

            collider.points = reachLevels[i];

            _levelColliders[i] = collider;
        }
    }


    public StepPoint[] InReach(ContactFilter2D? filter = null)
    {
        List<Collider2D> result = new List<Collider2D>();

        Collider2D collider = _levelColliders[_levelColliders.Length - 1];

        try
        {
            collider.enabled = true;

            if (filter != null)
            {
                Physics2D.OverlapCollider(collider, (ContactFilter2D)filter, result);
            }
            else
            {
                Physics2D.OverlapCollider(collider, result);
            }
        }
        finally
        {
            collider.enabled = false;
        }

        List<StepPoint> reachablesList = new List<StepPoint>();

        foreach (Collider2D hit in result)
        {
            if (hit.transform.parent != _origin)
            {
                reachablesList.Add(hit.transform.parent.GetComponent<StepPoint>());
            }
        }

        return reachablesList.ToArray();
    }

    public int JumpType(StepPoint target)
    {
        Collider2D targetedCollider = target.GetSurface().GetComponent<Collider2D>();


        // =====
        ContactFilter2D filter = new ContactFilter2D();

        filter.layerMask = LayerMask.GetMask("StepPoint");
        filter.useLayerMask = true;
        filter.useTriggers = true;
        // =====


        for (int i = 0; i < _levelColliders.Length; i++)
        {
            Collider2D collider = _levelColliders[i];

            List<Collider2D> results = new List<Collider2D>();

            collider.enabled = true;

            try
            {
                Physics2D.OverlapCollider(collider, filter, results);
            }
            finally
            {
                collider.enabled = false;
            }

            if (results.Contains(targetedCollider))
            {
                return i;
            }
        }

        return -1;
    }

    public Collider2D[] ReachByType(int type)
    {
        Collider2D collider = _levelColliders[type];

        ContactFilter2D filter = new ContactFilter2D();

        filter.layerMask = LayerMask.GetMask("StepPoint");
        filter.useLayerMask = true;
        filter.useTriggers = true;

        List<Collider2D> collisions = new List<Collider2D>();

        collider.enabled = true;

        try
        {
            Physics2D.OverlapCollider(collider, filter, collisions);
        }
        finally
        {
            collider.enabled = false;
        }

        return collisions.ToArray();
    }
}