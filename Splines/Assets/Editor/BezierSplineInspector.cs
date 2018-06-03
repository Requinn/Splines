﻿using System.Runtime.InteropServices;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEngine;

namespace JLProject.Spline {
    /// <summary>
    /// used to display a bezier curve in the editor
    /// </summary>
    [CustomEditor(typeof(BezierSpline))]
    public class BezierSplineInspector : Editor {
        private BezierSpline spline;
        private Transform handleTransform;
        private Quaternion handleRotation;

        private const int stepsPerCurve = 10; //bezier curves are parametric, given a value you get a point on the line
        private const float directionScale = 0.5f; //so our direction vectors don't clutter the screen

        //used for dummy points to avoid having the transform markers everywhere
        private const float handleSize = 0.04f;
        private const float pickSize = 0.0f;

        private int selectedIndex = -1;

        private void OnSceneGUI() {
            spline = target as BezierSpline;
            handleTransform = spline.transform;
            handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;

            //draw and get the points in the curve
            Vector3 p0 = ShowPoint(0);
            for (int i = 1; i < spline.points.Length; i += 3){
                Vector3 p1 = ShowPoint(i);
                Vector3 p2 = ShowPoint(i + 1);
                Vector3 p3 = ShowPoint(i + 2);

                //draw the curve with straight lines
                Handles.color = Color.gray;
                Handles.DrawLine(p0, p1);
                Handles.DrawLine(p2, p3);

                Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2f);
                //connect the splines
                p0 = p3;
            }
            //draw directions
            ShowDirections();
        }

        /// <summary>
        /// adds a button to add a new curve
        /// </summary>
        public override void OnInspectorGUI(){
            DrawDefaultInspector();
            spline = target as BezierSpline;
            if (GUILayout.Button("Add Curve")){
                Undo.RecordObject(spline, "Add Curve");
                spline.AddCurve();
                EditorUtility.SetDirty(spline);
            }
        }

        /// <summary>
        /// Draws the direction vectors of the bezier
        /// </summary>
        private void ShowDirections() {
            Handles.color = Color.green;
            Vector3 point = spline.GetPoint(0f);
            Handles.DrawLine(point, point + spline.GetDirection(0f) * directionScale);
            int steps = stepsPerCurve * spline.CurveCount;
            for (int i = 1; i <= steps; i++) {
                point = spline.GetPoint(i / (float)steps);
                Handles.DrawLine(point, point + spline.GetDirection(i / (float)steps * directionScale));
            }
        }
        /// <summary>
        /// show a point at the index in the editor
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private Vector3 ShowPoint(int index) {
            Vector3 point = handleTransform.TransformPoint(spline.points[index]); //get the point at index

            //use dummy points to reduce visual clutter of regular transform markers
            Handles.color = Color.white;
            if (Handles.Button(point, handleRotation, handleSize, pickSize, Handles.DotHandleCap)){
                selectedIndex = index;
            }
            if (selectedIndex == index){
                EditorGUI.BeginChangeCheck();
                point = Handles.DoPositionHandle(point, handleRotation);
                if (EditorGUI.EndChangeCheck()){
                    Undo.RecordObject(spline, "Move Point");
                    EditorUtility.SetDirty(spline);
                    spline.points[index] = handleTransform.InverseTransformPoint(point);
                }
            }
            return point;
        }
    }
}