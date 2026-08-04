using UnityEngine;


public class SimulationBody : AbstractBody
{
    private float mass = 1;


    private Vector2 position;
    public override Vector2 Position
    {
        get { return position; }
        set { position = value; }
    }

    private Vector2 _velocity;
    public override Vector2 Velocity
    {
        get { return _velocity; }
        set { _velocity = value; }
    }

    public override float Drag
    {
        get
        {
            return drag;
        }
        set
        {
            drag = value;
        }
    }
    public override float Gravity
    {
        get
        {
            return gravity;
        }
        set
        {
            gravity = value;
        }
    }

    private float drag;
    private float gravity;

    private bool realtimeFramerate;


    // public bool onGround = false;


    public SimulationBody(Vector2 position, Vector2 size, float drag, float gravity, bool realtimeFramerate = false)
    {
        this.drag = drag;
        this.gravity = gravity;

        this.position = position;

        this.realtimeFramerate = realtimeFramerate;
    }

    public override float GetDeltaTime()
    {
        return realtimeFramerate ? Time.deltaTime : Time.fixedDeltaTime;
    }

    public override void addForce(Vector2 force, ForceMode2D forceMode = ForceMode2D.Force)
    {
        switch (forceMode)
        {
            case ForceMode2D.Impulse:
                _velocity += force / mass;
                break;
            case ForceMode2D.Force:
                _velocity += force * GetDeltaTime() / mass;
                break;
        }
    }

    public void UpdateFrame(float simulationStepTime)
    {
        Vector2 dragForce = new Vector2(Velocity.x * Drag, Velocity.y * Drag);
        Vector2 netForce = new Vector2(0, -Gravity) - dragForce;
        Vector2 acceleration = netForce;

        Vector2 update = acceleration * simulationStepTime;

        //if (onGround() && Velocity.y < 0)
        //{
        //    update.y = 0;
        //}

        Velocity += update;

        Position += Velocity * simulationStepTime;
    }
}
