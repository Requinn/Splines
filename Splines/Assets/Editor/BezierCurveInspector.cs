using System.Runtime.InteropServices;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEngine;

namespace JLProject.Spline{
    /// <summary>
    /// used to display a bezier curve in the editor
    /// </summary>
    [CustomEditor(typeof(BezierCurve))]
    public class BezierCurveInspector : Editor{
        private BezierCurve curve;
        private Transform handleTransform;
        private Quaternion handleRotation;

        private const int lineSteps = 10; //bezier curves are parametric, given a value you get a point on the line
        private const float directionScale = 0.5f; //so our direction vectors don't clutter the screen
        private void OnSceneGUI(){
            curve = target as BezierCurve;
            handleTransform = curve.transform;
            handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;

            //draw and get the points in the curve
            Vector3 p0 = ShowPoint(0);
            Vector3 p1 = ShowPoint(1);
            Vector3 p2 = ShowPoint(2);
            Vector3 p3 = ShowPoint(3);

            //draw the curve with straight lines
            Handles.color = Color.gray;
            Handles.DrawLine(p0, p1);
            Handles.DrawLine(p2, p3);

            ShowDirections();
            Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2f);
        }

        /// <summary>
        /// Draws the direction vectors of the bezier
        /// </summary>
        private void ShowDirections(){
            Handles.color = Color.green;
            Vector3 point = curve.GetPoint(0f);
            Handles.DrawLine(point, point + curve.GetDirection(0f) * directionScale);

            for (int i = 1; i <= lineSteps; i++){
                point = curve.GetPoint(i / (float) lineSteps);
                Handles.DrawLine(point, point + curve.GetDirection(i / (float)lineSteps * directionScale));
            }
        }
        /// <summary>
        /// show a point at the index in the editor
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private Vector3 ShowPoint(int index){
            Vector3 point = handleTransform.TransformPoint(curve.points[index]); //get the point at index
            EditorGUI.BeginChangeCheck();
            point = Handles.DoPositionHandle(point, handleRotation);
            if (EditorGUI.EndChangeCheck()){
                Undo.RecordObject(curve, "Move Point");
                EditorUtility.SetDirty(curve);
                curve.points[index] = handleTransform.InverseTransformPoint(point);
            }
            return point;
        }
    }
}
