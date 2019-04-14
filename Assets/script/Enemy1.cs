using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1 : MonoBehaviour {

    public GameObject mMissile;
    private GameObject player;
    private Rigidbody2D rb2D;
    private bool getOut = false;
    private float time = 0;

	// Use this for initialization
	void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb2D = GetComponent<Rigidbody2D>();
        rb2D.AddForce(new Vector2(0, -50));
        GameMgr.inst.mEnemies.Add(this);
        Invoke("GetOut", 5f);
    }

    // Update is called once per frame
    void Update()
    {
        if ((time += Time.deltaTime) > 2)
        {
            time = 0;
            Vector3 nor = Vector3.Normalize(transform.up) * 0.5f;
            GameObject obj = Instantiate(mMissile, transform.position + nor, Quaternion.identity);
            obj.GetComponent<Missile>().target = player.transform.position;
            obj.layer = 11;//EnemyMissile Layer
        }

        LookPlayerAlways();
        if (getOut)
        {
            rb2D.AddForce(new Vector2(40, 0));
            getOut = false;
        }
    }

    private void GetOut()
    {
        getOut = true;
        //rb2D.AddForce(new Vector2(100, -100));
        //StartCoroutine(SmoothMovement());
    }

    private void LookPlayerAlways()
    {
        Vector3 dir = player.transform.position - transform.position;
        float deg = Vector3.Angle(new Vector3(0, 1, 0), dir);
        if (dir.x > 0)
            deg *= -1;
        rb2D.rotation = deg;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        enabled = false;
    }

    //private IEnumerator SmoothMovement()
    //{
    //    Vector3 end = new Vector3(10, 0, 0);
    //    float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
    //
    //    while (sqrRemainingDistance > 0.001)
    //    {
    //        Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, 1 * Time.deltaTime);
    //        rb2D.MovePosition(newPosition);
    //        sqrRemainingDistance = (transform.position - end).sqrMagnitude;
    //        yield return null;
    //    }
    //}

}
