using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour {

    Rigidbody2D rb2D;
    CircleCollider2D mCollider;
    public Vector3 target = new Vector3(0,0,0);
    public float speed = 250f;

    // Use this for initialization
    void Start ()
    {
        rb2D = GetComponent<Rigidbody2D>();
        mCollider = GetComponent<CircleCollider2D>();
        Vector3 dir = target - transform.position;
        dir.Normalize();
        rb2D.AddForce(dir * speed);
        GameMgr.inst.mMissiles.Add(this);
    }
	
    private void OnTriggerEnter2D(Collider2D collision)
    {
        enabled = false;
    }

    public bool IsWarn(Vector2 _pos)
    {
        if (gameObject.layer == 10) //in case of player's missile
            return false;

        Utils.Line line = Utils.Line.Create(rb2D.velocity, rb2D.position);
        return line.DistanceToPoint(_pos) < (mCollider.radius * 2);
    }
    public float CalcWeight(Vector2 _pos)
    {
        float dist = (_pos - rb2D.position).magnitude;
        float weight = 0;
        if (dist < 1f)
            weight = 0f;
        else if (dist < 10f)
            weight = dist / 10f;
        else
            weight = 1.0f;

        return weight;
    }
}
