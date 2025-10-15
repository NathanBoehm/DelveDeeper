using InfinityPBR;
using System.Collections.Generic;
using UnityEngine;

public class TalkingAnim : MonoBehaviour
{
    public string[] phonemeBlendShapeNames = { "Male_Phoneme_FV", "Male_Phoneme_PBM", "Male_Phoneme_ShCh", "Male_Phoneme_W", "Male_Phoneme_Wide" };
    private List<int> phonemeBlendShapes = new List<int> { 69, 70, 71, 72, 73 };

    public SkinnedMeshRenderer faceRenderer;
    public float _blendingSpeed = 10f;

    private int _currentPhoneme;
    private float phonemeTimer;

    private void Start()
    {
        foreach (var phoneme in phonemeBlendShapeNames)
        {
            //phonemeBlendShapes.Add(faceRenderer.sharedMesh.GetBlendShapeIndex(phoneme));
        }
    }

    // Update is called once per frame
    void Update()
    {

        phonemeTimer -= Time.deltaTime;
        if (phonemeTimer <= 0f)
        {
            phonemeTimer = Random.Range(0.15f, 0.3f);
            _currentPhoneme = phonemeBlendShapes.Random();
        }

        // Smoothly update blend shape weights
        foreach (var index in phonemeBlendShapes)
        {
            float target = (index == _currentPhoneme) ?  100f : 0f;
            float newWeight = Mathf.Lerp(faceRenderer.GetBlendShapeWeight(index), target, Time.deltaTime * _blendingSpeed);
            faceRenderer.SetBlendShapeWeight(index, newWeight);
        }
    }
}
