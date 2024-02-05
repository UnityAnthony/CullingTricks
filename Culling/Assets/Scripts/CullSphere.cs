using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CullSphere : MonoBehaviour
{
    #region Public Variables
    public float radius = 1;
    public bool DEBUG = false;
    public int cullingIndex = -1;
    public bool canUpdate = false;
    public CullingManager cullMan = null;
    #endregion
    #region Private Variables
    float lastRadus;
    Vector3 lastPos;
    #endregion
    /// <summary>
    /// Find the manager
    /// </summary>
    private void Awake()
    {
        cullMan = FindObjectOfType<CullingManager>();
    }
    /// <summary>
    /// Clear the current sphere
    /// </summary>
    void OnDestroy()
    {
        if (cullMan)
        {
            cullMan.RemoveSphere(cullingIndex);
        }
    }
    /// <summary>
    /// Render Gizmo for the Sphere for debugging
    /// </summary>
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

    /// <summary>
    /// Store position and radius
    /// </summary>
    private void OnEnable()
    {
        Vector3 curPos = transform.position;
        lastRadus = radius;
        lastPos = curPos;

    }

    /// <summary>
    /// Creates a sphere based on position of the GameObject
    /// </summary>
    /// <returns></returns>
    public BoundingSphere GetSphere()
    {
        Vector3 curPos = transform.position;
        return new BoundingSphere(curPos, radius);

    }

    /// <summary>
    /// If the Object moves or radius changes update the sphere in the manager, using lateupdate incase some other component might move the parent object first
    /// </summary>
    void LateUpdate()
    {
        if (!canUpdate)
        {
            return;
        }

        if (cullMan != null && cullMan.spheres != null)
        {
            Vector3 curPos = transform.position;
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
    /// <summary>
    /// Set the canUpdate bool to prevent spam in LateUpdate before the sphere is ready to use
    /// </summary>
    /// <param name="b"></param>
    public void CanUpdate(bool b) { canUpdate = b; }
    /// <summary>
    /// Get Index for Sphere
    /// </summary>
    /// <returns></returns>
    public int GetIndex() { return cullingIndex; }
    /// <summary>
    /// Set Index for Sphere
    /// </summary>
    /// <param name="i"></param>
    public void SetIndex(int i) { cullingIndex = i; }
}