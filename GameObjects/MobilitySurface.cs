using UnityEngine;


public class MobilitySurface
{
    private Transform _transform;
    public Transform transform
    {
        get {  return _transform; }
        protected set { _transform = value; }
    }
    protected Transform mainTransform;


    public MobilitySurface(StepPoint stepPoint)
    {
        mainTransform = stepPoint.transform;

        GameObject mobilitySurface = new GameObject();

        mobilitySurface.layer = LayerMask.NameToLayer("StepPoint");
        mobilitySurface.transform.parent = stepPoint.transform;
        mobilitySurface.transform.localPosition = Vector3.zero;
        mobilitySurface.transform.localScale = Vector3.one;

        transform = mobilitySurface.transform;
    }
}