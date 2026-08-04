using UnityEngine;


public abstract class StepPoint : MonoBehaviour
{
    private MobilitySurface surface;
    protected ReachNetwork network;


    protected abstract MobilitySurface InitMobilitySurface();


    protected virtual void Start()
    {
        surface = InitMobilitySurface();
    }


    public ReachNetwork GetNetwork()
    {
        return network;
    }

    public Transform GetSurface()
    {
        return surface.transform;
    }    
}