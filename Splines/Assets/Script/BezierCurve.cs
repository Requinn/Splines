using UnityEngine;

namespace JLProject.Spline{
    /// <summary>
    /// A class to describe a bezier curve
    /// </summary>
    public class BezierCurve : MonoBehaviour{
        public Vector3[] points;

        /// <summary>
        /// Unity method used to reset or initialize a new curve
        /// </summary>
        public void Reset(){
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
            return transform.TransformPoint(Bezier.GetPoint(points[0], points[1], points[2], points[3], t));
        }

        /// <summary>
        /// get the velocity vector on the line at value t in world space
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public Vector3 GetVelocity(float t){
            return transform.TransformPoint(Bezier.GetFirstDerivative(points[0], points[1], points[2], points[3], t)) -
                   transform.position; //we don't a location, subtract position
        }

        /// <summary>
        /// get the direction of the curve
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public Vector3 GetDirection(float t){
            return GetVelocity(t).normalized;
        }
    }
}
