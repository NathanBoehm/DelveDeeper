using EditorAttributes;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerCameraManager : MonoBehaviour, INetworkObjectInitializer
{
    [SerializeField, Required] private CinemachineCamera _mainCamera;
    [SerializeField, Required] private CinemachineCamera _leftSwingPrepCamera;
    [SerializeField, Required] private CinemachineCamera _leftSwingCamera;
    [SerializeField, Required] private CinemachineCamera _rightSwingPrepCamera;
    [SerializeField, Required] private CinemachineCamera _rightSwingCamera;
    [SerializeField, Required] private CinemachineCamera _leftDownSwingPrepCamera;
    [SerializeField, Required] private CinemachineCamera _leftDownSwingCamera;
    [SerializeField] CinemachineBasicMultiChannelPerlin _walkNoise;

    private List<CinemachineCamera> AllCameras;

    //[SerializeField] private CinemachineBrain _brain;

    public void Awake()
    {
        /*var fields = this.GetType().GetProperties();
        var cameras = new List<CinemachineCamera>();

        foreach (var field in fields)
            if (field.GetValue(this) is CinemachineCamera)
                cameras.Add(field.GetValue(this) as CinemachineCamera);*/

        AllCameras = new List<CinemachineCamera>()
        {
            _mainCamera,
            _leftSwingPrepCamera,
            _leftSwingCamera,
            _rightSwingCamera,
            _leftDownSwingPrepCamera,
            _leftDownSwingCamera,
        };
    }

    public void PlaySwingLeftPrepCamera()
    {
        AllCameras.ForEach(c => c.Priority = 0);
        _leftSwingPrepCamera.Priority = 1;
    }

    public void PlaySwingLeftCamera()
    {
        AllCameras.ForEach(c => c.Priority = 0);
        //_mainCamera.Priority = 0;
        //_rightSwingCamera.Priority = 0;
        _leftSwingCamera.Priority = 1;
    }

    public void PlayMainCamera()
    {
        AllCameras.ForEach(c => c.Priority = 0);
        //_leftSwingCamera.Priority = 0;
        //_rightSwingCamera.Priority = 0;
        _mainCamera.Priority = 1;
    }

    public void PlaySwingRightPrepCamera()
    {
        AllCameras.ForEach(c => c.Priority = 0);
        _rightSwingPrepCamera.Priority = 1;
    }

    public void PlaySwingRightCamera()
    {
        AllCameras.ForEach(c => c.Priority = 0);
        //_leftSwingCamera.Priority = 0;
        //_rightSwingCamera.Priority =
        _rightSwingCamera.Priority = 1;
    }

    public void PlaySwingDownLeftPrepCamera()
    {
        AllCameras.ForEach(c => c.Priority = 0);
        _leftDownSwingPrepCamera.Priority = 1;
    }

    public void PlaySwingDownLeftCamera()
    {
        AllCameras.ForEach(C => C.Priority = 0);
        _leftDownSwingCamera.Priority = 1;
    }

    public void PlayWalkNoise()
    {
        _walkNoise.FrequencyGain = 1;
        _walkNoise.enabled = true;
    }

    public void StopWalkNoise()
    {
        _walkNoise.enabled = false;
    }

    public void PlayRunNosie()
    {
        _walkNoise.FrequencyGain = 1.5f;
        _walkNoise.AmplitudeGain = 2f;
        _walkNoise.enabled = true;
    }

    public void Initialize()
    {
        this.enabled = false;
    }

    public void InitializeForOwner()
    {
        this.enabled = true;
    }
}
