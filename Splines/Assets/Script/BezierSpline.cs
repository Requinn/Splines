using System;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

namespace JLProject.Spline {
    /// <summary>
    /// A class to describe a bezier curve
    /// </summary>
    public class BezierSpline : MonoBehaviour {
        public Vector3[] points;

        /// <summary>
        ///  Gets the number of curves in the spline
        /// </summary>
        public int CurveCount{
            get{ return (points.Length - 1) / 3; } //hi parenthesis are important
        }

        /// <summary>
        /// Unity method used to reset or initialize a new curve
        /// </summary>
        public void Reset() {
            points = new Vector3[]{
                new Vector3(1f, 0f, 0f),
                new Vector3(2f, 0f, 0f),
                new Vector3(3f, 0f, 0f),
                new Vector3(4f, 0f, 0f)
            };
        }

        /// <summary>
        /// get a point on the curve at value t in worldspace
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public Vector3 GetPoint(float t){
            //if our time is a value equal to or greater than 1, return the last curve
            int i;
            if (t >= 1f){
                t = 1f;
                i = points.Length - 4;
            }
            else{
                t = Mathf.Clamp01(t) * CurveCount; //scale our t in proportion to the number of curves we have
                i = (int)t;
                t -= i; //reduce t to get the fractional value
                i *= 3; //multiply by 3 (the number of points in a curve) to get the actua point
            }
            return transform.TransformPoint(Bezier.GetPoint(points[i], points[i + 1], points[i + 2], points[i + 3], t));
        }

        /// <summary>
        /// get the velocity vector on the line at value t in world space
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public Vector3 GetVelocity(float t) {
            //if our time is a value equal to or greater than 1, return the last curve
            int i;
            if (t >= 1f) {
                t = 1f;
                i = points.Length - 4;
            }
            else {
                t = Mathf.Clamp01(t) * CurveCount; //scale our t in proportion to the number of curves we have
                i = (int)t;
                t -= i; //reduce t to get the fractional value
                i *= 3; //multiply by 3 (the number of points in a curve) to get the actua point
            }
            return transform.TransformPoint(Bezier.GetFirstDerivative(points[i], points[i + 1], points[i + 2], points[i + 3], t)) -
                   transform.position; //we don't a location, subtract position
        }

        /// <summary>
        /// get the direction of the curve
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public Vector3 GetDirection(float t) {
            return GetVelocity(t).normalized;
        }

        /// <summary>
        /// Adds a new curve to the Bezier spline
        /// </summary>
        public void AddCurve(){
            //We want the spline to be continious, so our last point of the previous curve is going to the first point of the next curve
            Vector3 point = points[points.Length - 1];
            Array.Resize(ref points, points.Length + 3);
            point.x += 1f;
            points[points.Length - 3] = point;
            point.x += 1f;
            points[points.Length - 2] = point;
            point.x += 1f;
            points[points.Length - 1] = point;
        }
    }
}
