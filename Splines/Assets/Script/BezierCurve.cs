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
                new Vector3(3f, 0f, 0f)   
            };
        }


    }
}
