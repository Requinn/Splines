using System.Collections;
using System.Collections.Generic;
using JLProject.Spline;
using UnityEngine;

public class SplineWalker : MonoBehaviour{
    public enum WalkerMode{
        Once, 
        Loop,
        PingPong
    }

    public WalkerMode mode;
    private bool goingForward = true;

    public bool lookForward;
    public BezierSpline spline;
    public float duration;
    private float progress;

    private void Update(){
        if (goingForward){
            progress += Time.deltaTime / duration;
            if (progress > 1f){
                if (mode == WalkerMode.Once){
                    progress = 1f;
                }
                else if (mode == WalkerMode.Loop){
                    progress -= 1f;
                }
                else{
                    progress = 2f - progress;
                    goingForward = false;
                }
            }
        }
        else{
            progress -= Time.deltaTime / duration;
            if (progress < 0){
                progress = -progress;
                goingForward = true;
            }
        }

        Vector3 position = spline.GetPoint(progress);
        transform.localPosition = position;
        if (lookForward) {
            transform.LookAt(position + spline.GetDirection(progress));
        }
    }
}
