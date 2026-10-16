using UnityEngine;


public class RealtimeBody : AbstractBody
{
    private Vector2 _size;


    private Rigidbody2D rigidbody;


    protected override Vector2 Size
    {
        get { return _size; }
        set { _size = value; }
    }
    public override Vector2 Velocity
    {
        get { return rigidbody.linearVelocity; }
        set { rigidbody.linearVelocity = value; }
    }
    public override Vector2 Position
    {
        get { return rigidbody.position; }
        set { rigidbody.position = value; }
    }

    public override float Drag 
    {
        get { return rigidbody.linearDamping; }
        set { rigidbody.linearDamping = value; }
    }
    public override float Gravity
    {
        get { return rigidbody.gravityScale * Mathf.Abs(Physics2D.gravity.y); }
        set { rigidbody.gravityScale = value / Mathf.Abs(Physics2D.gravity.y); }
    }

    public RealtimeBody(Rigidbody2D rigidbody)
    {
        this.rigidbody = rigidbody;

        _size = rigidbody.GetComponent<Collider2D>().bounds.size;
    }

    public override float GetDeltaTime()
    {
        return Time.fixedDeltaTime;
    }

    public override void addForce(Vector2 force, ForceMode2D forceMode = ForceMode2D.Force)
    {
        rigidbody.AddForce(force, forceMode);
    }
}
