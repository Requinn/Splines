using System.Configuration.Assemblies;
using UnityEngine;

namespace JLProject.Spline{
    public static class Bezier{
        /// <summary>
        /// Utility function to get a point on a quadratic bezier curve at value t
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t){
            //lerp between p0 and p1, then lerp between p1 and p2, then lerp between the results of those two lerps
            t = Mathf.Clamp01(t);
            float oneMinusT = 1f - t;
            //(B(t) = (1 - t)^2 * p0 + 2(1-t) * t * p1 + t^2 * p2)
            return
                oneMinusT * oneMinusT * p0 + 
                2f * oneMinusT * t * p1 + 
                t * t * p2;
        }

        /// <summary>
        /// get the derivative (velocity) at a point on the curve through tangents
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, float t){
            return 
                2f * (1f - t) * (p1 - p0) + 
                2f * t * (p2 - p1);
        }

        /// <summary>
        /// Utility function to get a point on a cubic bezier curve at value t
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t){
            t = Mathf.Clamp01(t);
            float oneMinusT = 1f - t;
            //B(t) = (1 - t)^3 P0 + 3 (1 - t)^2 t P1 + 3 (1 - t) t^2 P2 + t^3 P3
            return
                oneMinusT * oneMinusT * oneMinusT * p0 +
                3f * oneMinusT * oneMinusT * t * p1 +
                3f * oneMinusT * t * t * p2 +
                t * t * t * p3;
        }

        /// <summary>
        /// get the derivative at a point on the cubic curve 
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t){
            t = Mathf.Clamp01(t);
            float oneMinusT = 1f - t;
            return
                3f * oneMinusT * oneMinusT * (p1 - p0) +
                6f * oneMinusT * t * (p2 - p1) +
                3f * t * t * (p3 - p2);
        }


    }
}