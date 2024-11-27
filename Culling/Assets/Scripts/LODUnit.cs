using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LODUnit : Unit
{
    
    [Serializable]
    public struct LODGroup
    {
        public List<Renderer> rends;
    }
    [Header("LODS")]
    public LODGroup[] LODs = new LODGroup[0];
    public int currentLOD = -1;
    protected void Start()
    {
        if (!cullMan)
        {
            cullMan = FindObjectOfType<CullingManager>();
        }
        if (culler && cullMan)
        {
            cullMan.AddSphere(culler);
            int cullIndex = culler.GetIndex();
            cullMan.group.onStateChanged += OnCullerStateChanged;

            currentLOD = cullMan.group.GetDistance(cullIndex);
            ShowOnlyRenderers(currentLOD);
        }
    }
    public override void OnCullerStateChanged(CullingGroupEvent evt)
    {
        //  Debug.Log("LODUnit OnCullerStateChanged " + evt.index);
        if (culler.GetIndex() != evt.index)
        {
            return;
        }
        // Debug.Log(name + " OnCullerStateChanged distance " + evt.currentDistance + " prev " + evt.previousDistance);// evt.currentDistance will be 0 if Distances are not set.
        //was visible and still is visible
        //Debug.Log("evt.wasVisible " + evt.wasVisible);
        //Debug.Log("evt.isVisible " + evt.isVisible);
        //Debug.Log("evt.isVisible " + evt.hasBecomeInvisible);
        //Debug.Log("evt.isVisible " + evt.hasBecomeVisible);
        //Debug.Log("evt.currentDistance " + evt.currentDistance);
        //Debug.Log("evt.previousDistance " + evt.previousDistance);
        if (evt.wasVisible)
        {
            
            if (evt.isVisible)
            {
                if (evt.previousDistance != evt.currentDistance)
                {
                   // Debug.Log("was visible and is visable disance change");
                    ShowRenderers(true, evt.currentDistance, evt.previousDistance);
                }
            }
            if(evt.hasBecomeInvisible) //was visible but not visible now
            {
                HideAllRenderers();
            }
        }else if (!evt.wasVisible)
        {
            if (evt.hasBecomeVisible)
            {
                ShowOnlyRenderers(evt.currentDistance);
            }
        }


        
    }
    public void ShowRenderers(bool b, int current, int previous)
    {

        if (current >= LODs.Length)
        {
            Debug.LogWarning("current " + current + " index is higher than LODs");
            return;
        }
        if (previous >= LODs.Length)
        {
            Debug.LogWarning("previous " + previous + " index is higher than LODs");
            return;

        }
        currentLOD = current;
        List<Renderer> currentRends;
        List<Renderer> previousRends;
        currentRends = LODs[current].rends;

        currentRends = LODs[current].rends;
        previousRends = LODs[previous].rends;

        for (int i = 0; i < currentRends.Count; i++)
        {
            currentRends[i].enabled = b;
            previousRends[i].enabled = !b;
        }
    }
    public void HideAllRenderers()
    {
        bool b = false;

        //speical times do this
        for (int j = 0; j < LODs.Length; j++)
        {
            List<Renderer> currentRends = LODs[j].rends;
            for (int i = 0; i < currentRends.Count; i++)
            {
                currentRends[i].enabled = b;
            }
        }
        currentLOD = -1;
    }
    public void ShowOnlyRenderers(int current)
    {

        if (current >= LODs.Length)
        {
            Debug.Log("current " + current + " index is higher than LODs");
            return;
        }
        

        bool b = true;
        List<Renderer> currentRends = LODs[current].rends;
        //speical times do this

        for (int j = 0; j < LODs.Length; j++)
        {
            currentRends = LODs[j].rends;
            for (int i = 0; i < currentRends.Count; i++)
            {
                if (j == current)
                {
                    currentRends[i].enabled = b;
                }
                else
                {
                    currentRends[i].enabled = !b;
                }
            }
        }
    }
}
