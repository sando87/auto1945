using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : MonoBehaviour {

    public static GameMgr inst = null;

    public GameObject mPrefabEnemy;
    public List<Enemy1> mEnemies;
    public List<Missile> mMissiles;

    private System.Random rand = new System.Random();

    // Use this for initialization
    void Start () {
        if (inst == null)
            inst = this;
        else
            Destroy(this);

        DontDestroyOnLoad(gameObject);

    }
	
	// Update is called once per frame
	void Update () {
        mMissiles.RemoveAll(NeedRemove);
        mEnemies.RemoveAll(NeedRemove);

        int ranVal = rand.Next();
        if (ranVal % 116 == 0)
        {
            mEnemies.Add(CreateEnemy());
        }
    }

    public bool NeedRemove(MonoBehaviour item)
    {
        if (!item.enabled)
        {
            Destroy(item.gameObject);
            return true;
        }
        if(item.gameObject.transform.position.magnitude > 20)
        {
            Destroy(item.gameObject);
            return true;
        }

        return false;
    }

    private Enemy1 CreateEnemy()
    {
        GameObject obj = Instantiate(mPrefabEnemy, new Vector3(4.5f, 7f, 0), Quaternion.identity) as GameObject;
        return obj.GetComponent<Enemy1>() as Enemy1;
    }

}
