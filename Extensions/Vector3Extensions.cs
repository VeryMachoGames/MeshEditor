using System;
using System.Collections.Generic;
using System.Linq;
using MeshEditor.PolygonMaker;
using UnityEngine;

namespace MeshEditor.Extensions
{
    public static class Vector3Extensions
    {
        public static CPoint2D ToCPoint2D(this Vector3 vector3)
        {
            return new CPoint2D(vector3.x, vector3.y);
        }

        public static char MovedAxis(this Vector3 vector3, Vector3 newPosition)
        {
            var distance = vector3 - newPosition;
            if (Mathf.Abs(distance.x) > Mathf.Abs(distance.y))
            {
                return 'x';
            }

            return 'y';
        }


        public static Vector2 ToVector2D(this Vector3 vector3)
        {
            return new Vector2(vector3.x, vector3.y);
        }

        public static List<Vector2> ToVector2List(this List<Vector3> vectorList)
        {
            var returnData = new List<Vector2>();

            foreach (Vector3 vector3 in vectorList)
            {
                returnData.Add(vector3.ToVector2D());
            }

            return returnData;
        }
    }
}