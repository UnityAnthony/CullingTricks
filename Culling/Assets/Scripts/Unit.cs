using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public int cullIndex = -1;
    public CullSphere culler = null;
    public Renderer[] rends = new Renderer[0];
    public CullingManager cullMan = null;
    // Start is called before the first frame update
    void Start()
    {
        if(!cullMan)
            cullMan = FindObjectOfType<CullingManager>();
        if (culler && cullMan)
        {
            cullMan.AddSphere(culler);
            cullIndex = culler.GetIndex();
            cullMan.group.onStateChanged += OnLeaveCameraCuller;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnDestroy()
    {
        if (culler && cullMan)
        {
            cullMan.group.onStateChanged -= OnLeaveCameraCuller;
        }
    }

    /// <summary>
    /// ToDO for optimizations
    /// </summary>
    /// <param name="evt"></param>
    public void OnLeaveCameraCuller(CullingGroupEvent evt)
    {
        //  Debug.Log("OnLeaveCameraCuller"  + evt.index);
        if (cullIndex != evt.index)
        {
            return;
        }
        Debug.Log("OnLeaveCameraCuller distance " + evt.currentDistance);

        if (evt.hasBecomeVisible)
        {
            //Debug.LogFormat(name + " has become visible!", evt.index);
            for (int i = 0; i < rends.Length; i++)
                rends[i].enabled = true;
            //enable other things like animators
        }
        if (evt.hasBecomeInvisible)
        {
            // Debug.LogFormat(name + "has become invisible!", evt.index);
            for (int i = 0; i < rends.Length; i++)
                rends[i].enabled = false;
            
            //disable other things like animators
        }
    }
}
