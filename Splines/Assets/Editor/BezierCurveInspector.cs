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

        private void OnSceneGUI(){
            curve = target as BezierCurve;
            handleTransform = curve.transform;
            handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;

            //draw and get the points in the curve
            Vector3 p0 = ShowPoint(0);
            Vector3 p1 = ShowPoint(1);
            Vector3 p2 = ShowPoint(2);

            //draw the curve
            Handles.color = Color.white;
            Handles.DrawLine(p0, p1);
            Handles.DrawLine(p1, p2);
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
