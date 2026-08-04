using UnityEngine;


public abstract class AbstractBody
{
    protected virtual Vector2 Size
    {
        get {  return Vector2.zero; }
        set { }
    }


    public virtual Vector2 Velocity
    {
        get { return Vector2.zero; }
        set { }
    }

    public virtual Vector2 Position
    {
        get { return Vector2.zero; }
        set { }
    }


    public abstract float Drag
    {
        get;
        set;
    }

    public abstract float Gravity
    {
        get;
        set;
    }

    //public abstract float 

    public abstract float GetDeltaTime();
    public abstract void addForce(Vector2 velocity, ForceMode2D forceMode = ForceMode2D.Force);

    public virtual float GetWidth()
    {
        return Size.x;
    }
    public virtual float GetHeight()
    {
        return Size.y;
    }
}
