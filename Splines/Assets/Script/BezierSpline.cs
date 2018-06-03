using System;
using System.Xml.Schema;
using UnityEditor;
using UnityEngine;

namespace JLProject.Spline {
    /// <summary>
    /// A class to describe a bezier curve
    /// </summary>
    public class BezierSpline : MonoBehaviour {
        [SerializeField]
        private Vector3[] points;

        [SerializeField] private BezierPointMode.BezierControlPointMode[] modes;

        [SerializeField] private bool loop;
        public bool Loop{
            get{
                return loop;
            }
            set{
                loop = value;
                if (value){
                    modes[modes.Length - 1] = modes[0]; //make sure our modes are enforced by applying the mode of the first node to the last node
                    SetControlPoint(0, points[0]); //set the last node equal to the first node
                }
            }
        }

        /// <summary>
        /// gets number of controllable points in the spline
        /// </summary>
        public int ControlPointCount{
            get{ return points.Length; }
        }

        /// <summary>
        /// gets a point at index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Vector3 GetControlpoint(int index){
            return points[index];
        }

        /// <summary>
        /// assign a new point to the index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="point"></param>
        public void SetControlPoint(int index, Vector3 point){
            //if we select a middle point, make sure it adjusts the two other points connected to it as well
            //fixes the point left of middle staying fixed when adjusting middle, allowing to adjust the position of that curve specifically
            if (index % 3 == 0) {
                Vector3 delta = point - points[index]; //adjustments we make to a point are applied to all related/connected points
                //make sure we wrap our changes properly when adjusting edges in a loop
                if (loop){
                    if (index == 0){
                        points[1] += delta;
                        points[points.Length - 2] += delta;
                        points[points.Length - 1] = point;
                    }
                    if (index == points.Length - 1){
                        points[0] = point;
                        points[1] += delta;
                        points[index - 1] += delta;
                    }
                }
                else{
                    if (index > 0){
                        points[index - 1] += delta;
                    }
                    if (index + 1 < points.Length){
                        points[index + 1] += delta;
                    }
                }
            }
            points[index] = point;
            EnforceMode(index);
        }

        /// 
        /// <summary>
        /// Get the mode inbetween a curve, so we add one and divide by three
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public BezierPointMode.BezierControlPointMode GetControlPointMode(int index){
            return modes[(index + 1) / 3];
        }

        /// <summary>
        /// Set a mode for the point 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="mode"></param>
        public void SetControlPointmode(int index, BezierPointMode.BezierControlPointMode mode){
            int modeIndex = (index + 1) / 3;
            modes[modeIndex] = mode;
            //make sure the first and last modes are equal to each other in case we are looping
            if (loop){
                if (modeIndex == 0){
                    modes[modes.Length - 1] = mode;
                }
                else if (modeIndex == modes.Length - 1){
                    modes[0] = mode;
                }
            }
            EnforceMode(index);
        }

        /// <summary>
        /// Enforces the three types of point adjustements:
        /// Free: any point anywhere
        /// Aligned: Point opposite maintains distance while the moved points' distance is scaled.
        /// Mirrored: Changes made to adjusted point is reflected in the opposite point
        /// </summary>
        /// <param name="index"></param>
        private void EnforceMode(int index){
            int modeIndex = (index + 1) / 3;
            BezierPointMode.BezierControlPointMode mode = modes[modeIndex];
            //check if we should be doing anything
            if (mode == BezierPointMode.BezierControlPointMode.Free || !loop && (modeIndex == 0 || modeIndex == modeIndex - 1)){
                return;
            }

            //calculate where the points we are adjusting are at
            int middleIndex = modeIndex * 3;
            int fixedIndex, enforcedIndex;
            //if we have the middle mpoit selected, leave the previous alone and enforce onto the other side
            if (index <= middleIndex){
                fixedIndex = middleIndex - 1;
                if (fixedIndex < 0){
                    fixedIndex = points.Length - 2;
                }
                enforcedIndex = middleIndex + 1;
                if (enforcedIndex >= points.Length){
                    enforcedIndex = 1;
                }
            }
            //if we don't, keep the one we're at fixed and change the opposite side
            else{
                fixedIndex = middleIndex + 1;
                if (fixedIndex > points.Length){
                    fixedIndex = 1;
                }
                enforcedIndex = middleIndex - 1;
                if (enforcedIndex < 0){
                    enforcedIndex = points.Length - 2;
                }
            }
            //Mirrored.
            Vector3 middle = points[middleIndex];
            Vector3 enforcedTangent = middle - points[fixedIndex]; //get the vector from middle to fixed (fixed - middle), then invert it
            //if we are aligned, make sure the new tangent is the same length as the old one
            if (mode == BezierPointMode.BezierControlPointMode.Aligned){
                enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, points[enforcedIndex]);//normalize, then multiply by distance of middle and old enforced point
            }
            points[enforcedIndex] = middle + enforcedTangent; //then add it to the middle to get the point

        }
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
            //reset with only two, since we are getting control modes *between* the curves
            modes = new BezierPointMode.BezierControlPointMode[]{
                BezierPointMode.BezierControlPointMode.Free,
                BezierPointMode.BezierControlPointMode.Free
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

            //add a new mode to the created nodes
            Array.Resize(ref modes, modes.Length + 1);
            modes[modes.Length - 1] = modes[modes.Length - 2];

            //enforce constraints whenever we add a new curve
            EnforceMode(points.Length - 4);

            //special case if the spline loops
            if (loop){
                points[points.Length - 1] = points[0]; //set the last point we generate to the first point of the spline
                modes[modes.Length - 1] = modes[0];
                EnforceMode(0);
            }
        }
    }
}
