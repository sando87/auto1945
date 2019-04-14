using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils {

    public class Line
    {
        private Vector3 dir;
        private Vector3 pos;
        private Line(Vector3 _dir, Vector3 _pos) { dir = _dir; pos = _pos; dir.Normalize(); }
        static public Line Create(Vector3 _dir, Vector3 _pos)
        {
            Line line = new Line(_dir, _pos);
            return line;
        }
        public float DistanceToPoint(Vector3 _point)
        {
            float a = dir.x * dir.x + dir.y * dir.y + dir.z * dir.z;
            float b = (pos.x - _point.x) * dir.x + (pos.y - _point.y) * dir.y + (pos.z - _point.z) * dir.z;
            float k = -b / a;

            Vector3 p = new Vector3(dir.x * k + pos.x, dir.y * k + pos.y, dir.z * k + pos.z);
            return (p - _point).magnitude;
        }
    }
	
}
