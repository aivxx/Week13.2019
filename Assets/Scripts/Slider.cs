using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slider : MonoBehaviour
{
    public Transform startPos = null;
    public Transform endPos = null;

    MeshRenderer meshRenderer = null;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void OnSlideStart()
    {
        meshRenderer.material.SetColor("_Color", Color.red);    
            }

    public void OnSlideEnd()
    {
        meshRenderer.material.SetColor("_Color", Color.white);
    }

    public void UpdateSlider(float percent)
    {
        transform.position = Vector3.Lerp(startPos.position, endPos.position, percent);
    }
}
