using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Example class of how to use Culling on a Player, Item, or anything.
/// </summary>
public class Unit : MonoBehaviour
{
    public int cullIndex = -1;
    public CullSphere culler = null;
    public Renderer[] rends = new Renderer[0];
    public CullingManager cullMan = null;
    // Start is called before the first frame update
    /// <summary>
    /// If you have a CullSphere and there is a CullManager, this will add the Unit's Sphere to the CullManager. This also subscribes to the CullingGroup's OnCullerStateChanged Callback.
    /// This is called in Start because CullManager:Awake sets up the CullingGroup which needs to exist before we add to the CullingGroup. 
    /// </summary>
    void Start()
    {
        if (!cullMan)
        {
            cullMan = FindObjectOfType<CullingManager>();
        }
        if (culler && cullMan)
        {
            cullMan.AddSphere(culler);
            cullIndex = culler.GetIndex();
            cullMan.group.onStateChanged += OnCullerStateChanged;
            ShowRenderers(cullMan.group.IsVisible(cullIndex));   
        }
    }

    /// <summary>
    /// Un subscribe if the object is destroyed
    /// </summary>
    private void OnDestroy()
    {
        if (culler && cullMan)
        {
            cullMan.group.onStateChanged -= OnCullerStateChanged;
        }
    }

    /// <summary>
    /// This is called when an object is visible OR not visible OR is located in a different distance band 
    /// </summary>
    /// <param name="evt"></param>
    public void OnCullerStateChanged(CullingGroupEvent evt)
    {
        //  Debug.Log("OnLeaveCameraCuller"  + evt.index);
        if (cullIndex != evt.index)
        {
            return;
        }
       // Debug.Log("OnLeaveCameraCuller distance " + evt.currentDistance);// evt.currentDistance will be 0 if Distances are not set.

        if (evt.hasBecomeVisible)
        {
            //Debug.LogFormat(name + " has become visible!", evt.index);
            ShowRenderers(true);
            //enable other things like animators
        }
        if (evt.hasBecomeInvisible)
        {
            // Debug.LogFormat(name + "has become invisible!", evt.index);
            ShowRenderers(false);
            //disable other things like animators
        }
    }
    /// <summary>
    /// Show/Hide all Renderers for this object
    /// </summary>
    /// <param name="b"></param>
    public void ShowRenderers(bool b)
    {
        for (int i = 0; i < rends.Length; i++)
        {
            rends[i].enabled = b;
        }
    }
}
