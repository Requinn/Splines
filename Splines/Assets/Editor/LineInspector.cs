using UnityEditor;
using UnityEngine;

namespace JLProject.Spline{
    /// <summary>
    /// used to display a line in the editor
    /// </summary>
    [CustomEditor(typeof(Line), true)]
    public class LineInspector : Editor{
        private void OnSceneGUI(){
            Line line = target as Line;

            //alows the transform of the base object to move the line as well
            Transform handleTransform = line.transform;
            //allows the rotation to move the points instead of rotating them in place
            Quaternion handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;

            //convert the local points into world space
            Vector3 p0 = handleTransform.TransformPoint(line.p0);
            Vector3 p1 = handleTransform.TransformPoint(line.p1);

            //draw a line in worldspace
            Handles.color = Color.white;
            Handles.DrawLine(p0, p1);    
            
            //apply any changes made to the handles back to the line itself
            EditorGUI.BeginChangeCheck();
            p0 = Handles.DoPositionHandle(p0, handleRotation);
            if (EditorGUI.EndChangeCheck()){
                Undo.RecordObject(line, "Move Point"); //allows the object to be undone
                EditorUtility.SetDirty(line); //marks the object as changed when altered
                line.p0 = handleTransform.InverseTransformPoint(p0); //converts from worldspace to local space to handle the transform changes
            }
            EditorGUI.BeginChangeCheck();
            p1 = Handles.DoPositionHandle(p1, handleRotation);
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(line, "Move Point");
                EditorUtility.SetDirty(line);
                line.p1 = handleTransform.InverseTransformPoint(p1);
            }
        }
    }
}