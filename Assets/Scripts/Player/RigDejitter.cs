using UnityEngine;
using UnityEngine.Animations.Rigging;

//this thing is to force the rig to update after the camera
//camera controls ik target positions 
//rig updates after camera meaning the rig is out of date by one frame = jitter
[DefaultExecutionOrder(120)] //after camera
public class RigDejitter : MonoBehaviour
{
    private RigBuilder _rigBuilder;

    private void Awake()
    {
     _rigBuilder = GetComponent<RigBuilder>();   
    }

    private void LateUpdate()
    {
        if (_rigBuilder != null)
        {
            _rigBuilder.Build();
            //_rigBuilder.graph.Evaluate(0f);
        }
    }
}
