using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public GameObject mDetectPoint;

    public float planeSpeed = 3;
    public Vector3 targetDirect = new Vector3(0, 1, 0);
    private Rigidbody2D mRigidbody;
    private CircleCollider2D mCollider;
    private float time = 0;
    public GameObject mPlayerMissile;
    private Vector3 mTargetPosition = new Vector3(0, 0, 0);

    private const int mDetectSize = 9;
    private const float mDetectStep = 0.5f;
    private Vector3[,] mDetectArray = new Vector3[mDetectSize, mDetectSize]; //Vector값의 z값을 가중치로 활용
    private GameObject[,] mDetectDebug = new GameObject[mDetectSize, mDetectSize]; //Debugging

    // Use this for initialization
    void Start () {
        mRigidbody = GetComponent<Rigidbody2D>();
        mCollider = GetComponent<CircleCollider2D>();
        UpdateDetectPosition();
        InitDebugPoint();
    }

    // Update is called once per frame
    void Update ()
    {
        if ((time += Time.deltaTime) > 1)
        {
            time = 0;
            Vector3 nor = Vector3.Normalize(transform.up) * 0.5f;
            GameObject obj = Instantiate(mPlayerMissile, transform.position + nor, Quaternion.identity);
            obj.GetComponent<Missile>().target = transform.position + transform.up;
            obj.layer = 10;//playerMissile Layer
        }

        //float vert = Input.GetKey(KeyCode.W) ? 1f : (Input.GetKey(KeyCode.S) ? -1f : 0);
        //float hori = Input.GetKey(KeyCode.D) ? 1f : (Input.GetKey(KeyCode.A) ? -1f : 0);
        //Vector3 towardPos = transform.position + new Vector3(hori, vert, 0);
        //Vector3 newPos = Vector3.MoveTowards(transform.position, towardPos, planeSpeed * Time.deltaTime);
        //mRigidbody.MovePosition(newPos);

        Vector3 newPos = Vector3.MoveTowards(transform.position, mTargetPosition, planeSpeed * Time.deltaTime);
        mRigidbody.MovePosition(newPos);

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetDirect = worldPos - transform.position;
            targetDirect.z = 0;
            targetDirect.Normalize();
            StopAllCoroutines();
            StartCoroutine( RotatePlane() );
        }

        UpdateDetectPosition();
        UpdateDetectWeight();
        UpdateTargetPosistion();

        UpdateDebugObject();
    }

    private IEnumerator RotatePlane()
    {
        float currentAngle = mRigidbody.rotation;
        while (Vector3.Dot(transform.up, targetDirect) < 0.999)
        {
            float turnSpeed = Vector3.Cross(transform.up, targetDirect).z < 0 ? -180f : 180f;
            currentAngle += turnSpeed * Time.deltaTime;
            mRigidbody.MoveRotation(currentAngle);
            yield return null;
        }
    }

    private float CalcAngle(Vector3 target)
    {
        float deg = Vector3.Angle(transform.up, target);
        if (Vector3.Cross(transform.up, target).z < 0)
            deg *= -1;
        return deg;
    }

    private float ModAngle_0to360(float degree)
    {
        float step = (degree < 0) ? 360f : -360f;
        while (degree < 0 || 360 < degree)
            degree += step;
        return degree;
    }

    //+y축 기준으로 반시계방향의 dgree각도값으로 변환
    float ToDegree(Vector3 direction)
    {
        float radian = Mathf.Atan2(direction.y, direction.x);
        float degree = radian * Mathf.Rad2Deg;
        return degree - 90f;
    }

    private void InitDebugPoint()
    {
        for (int y = 0; y < mDetectSize; ++y)
        {
            for (int x = 0; x < mDetectSize; ++x)
            {
                mDetectDebug[y, x] = Instantiate(mDetectPoint, mDetectArray[y, x], Quaternion.identity) as GameObject;
            }
        }
    }
    private void UpdateDebugObject()
    {
        for (int y = 0; y < mDetectSize; ++y)
        {
            for (int x = 0; x < mDetectSize; ++x)
            {
                Vector2 newPos = mDetectArray[y, x];
                mDetectDebug[y, x].transform.position = newPos;
                SpriteRenderer comp = mDetectDebug[y, x].GetComponent<SpriteRenderer>();
                Color color = new Color(0, 1f, 0, 1f);
                color.r = 1f - mDetectArray[y, x].z;
                color.g = mDetectArray[y, x].z;
                comp.color = color;
            }
        }
    }

    private void UpdateDetectPosition()
    {
        float centerOffset = ((mDetectSize - 1) * mDetectStep) * 0.5f;
        for (int y = 0; y < mDetectSize; ++y)
        {
            for (int x = 0; x < mDetectSize; ++x)
            {
                mDetectArray[y, x].x = x * mDetectStep - centerOffset + transform.position.x;
                mDetectArray[y, x].y = y * mDetectStep - centerOffset + transform.position.y;
                mDetectArray[y, x].z = 1f;
            }
        }
    }

    private void UpdateDetectWeight()
    {
        for (int y = 0; y < mDetectSize; ++y)
        {
            for (int x = 0; x < mDetectSize; ++x)
            {
                UpdateDetectPoint(ref mDetectArray[y, x]);
            }
        }
    }
    private void UpdateDetectPoint(ref Vector3 _point)
    {
        List<Missile> missiles = GameMgr.inst.mMissiles;
        int cnt = missiles.Count;
        for (int i = 0; i < cnt; ++i)
        {
            if (missiles[i].IsWarn(_point))
            {
                float weight = missiles[i].CalcWeight(_point);
                _point.z = weight < _point.z ? weight : _point.z;
            }
                
        }
    }
    private void UpdateTargetPosistion()
    {
        float weight = 0;
        for (int y = 0; y < mDetectSize; ++y)
        {
            for (int x = 0; x < mDetectSize; ++x)
            {
                float dist = 50f - mDetectArray[y, x].magnitude;
                float tmpWeight = mDetectArray[y, x].z * 100f + dist;
                if (weight < tmpWeight)
                {
                    weight = tmpWeight;
                    mTargetPosition.x = mDetectArray[y, x].x;
                    mTargetPosition.y = mDetectArray[y, x].y;
                }
            }
        }
    }

}
