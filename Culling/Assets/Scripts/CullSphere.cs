using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CullSphere : MonoBehaviour
{
    public CullingGroup group;
    public float radius = 1;
   // public Vector3 height = Vector3.zero;

    float lastRadus;
    public bool DEBUG = false;
    Vector3 lastPos;
    public int cullingIndex = -1;
    public bool canUpdate = false;
    public CullingManager cullMan = null;
    private void Awake()
    {
        cullMan = FindObjectOfType<CullingManager>();
    }
    // Start is called before the first frame update
    void Start()
    {

    }
    void OnDestroy()
    {
        if (cullMan)
        {
            cullMan.RemoveSphere(cullingIndex);
        }
    }


    private void OnDrawGizmosSelected()
    {
        if (!DEBUG)
        {
            return;
        }
        if (cullMan && cullMan.spheres != null)
        {
            Gizmos.DrawWireSphere(cullMan.spheres[cullingIndex].position, cullMan.spheres[cullingIndex].radius);
        }
        else
        {
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }



    private void OnEnable()
    {
        Vector3 curPos = transform.position ;
        lastRadus = radius;
        lastPos = curPos;

    }

    public BoundingSphere GetSphere()
    {
        Vector3 curPos = transform.position;
        return new BoundingSphere(curPos, radius);

    }


    // Update is called once per frame
    void Update()
    {
        if (!canUpdate)
        {
            return;
        }

        if (cullMan != null && cullMan.spheres != null)
        {
            Vector3 curPos = transform.position ;
            if (lastPos != curPos)
            {
                cullMan.spheres[cullingIndex].position = curPos;
                lastPos = curPos;
            }

            if (radius != lastRadus)
            {
                cullMan.spheres[cullingIndex].radius = radius;
                lastRadus = radius;
            }
        }
    }
    public void CanUpdate(bool b) { canUpdate = b; }
    public int GetIndex() { return cullingIndex; }
    public void SetIndex(int i) { cullingIndex = i; }
}