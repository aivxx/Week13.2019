using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

[Serializable]
public class DragEvent: UnityEvent<float> { }

public class DragInteractable : XRBaseInteractable
{
    public Transform startDragPos = null;
    public Transform endDragPos = null;

    [HideInInspector]
    public float dragPercent = 0.0f; //[0,1] 

   protected XRBaseInteractor m_interactor = null;

 
    

    public UnityEvent onDragStart = new UnityEvent();
    public UnityEvent onDragEnd = new UnityEvent();
    public DragEvent onDragUpdate = new DragEvent();

   Coroutine m_drag = null;
    void StartDrag()
    {
        if(m_drag !=null)
        {
            StopCoroutine(m_drag);
        }
        m_drag = StartCoroutine(CalculateDrag());
        onDragStart?.Invoke();
    }

    void EndDrag()
    {
        if (m_drag != null)
        {
            StopCoroutine(m_drag);
            m_drag = null;
            onDragEnd?.Invoke();
        }
    }

    public static float InverseLerp(Vector3 a, Vector3 b, Vector3 value)
    {
        Vector3 AB = b - a;
        Vector3 AV = value - a;
        //the dot of a value divided by the dot of the total range
        //gives the normalized 0-1 distance of value between a and b
        return Mathf.Clamp01(Vector3.Dot(AV, AB) / Vector3.Dot(AB, AB));
    }

    IEnumerator CalculateDrag() 
    {

        while(m_drag != null)
        {
            //get a line in local space
            Vector3 line = startDragPos.localPosition - endDragPos.localPosition;

            //convert our interactor pos to local space
            Vector3 interactorLocalPosition = startDragPos.parent.InverseTransformPoint(m_interactor.transform.position);

            //project interactor pos onto the line
            Vector3 projectedPoint = Vector3.Project(interactorLocalPosition, line.normalized);

            //reverse interpolate that pos on the line to get a percentage of how far drag has moved
            dragPercent = InverseLerp(startDragPos.localPosition, endDragPos.localPosition, projectedPoint);

            onDragUpdate?.Invoke(dragPercent);


           yield return null;
        }
       
    }

    [System.Obsolete]
    protected void SelectEntered(XRBaseInteractor interactor)
    {
        m_interactor = interactor;
        StartDrag();
        base.OnSelectEntered(interactor);

    }

    [Obsolete]
    protected void SelectExited(XRBaseInteractor interactor)
    {
        EndDrag();
        base.OnSelectExited(interactor);
    }
}
