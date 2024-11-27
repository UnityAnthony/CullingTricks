using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// In charge of holding the CullingGroup which holds each CullingSphere
/// </summary>
public class CullingManager : MonoBehaviour
{
    #region Public Vars
    public CullingGroup group;
    public BoundingSphere[] spheres;
    public int MAX_CULLSPHERES = 500;
    public Dictionary<int, CullSphere> cSpheres = new Dictionary<int, CullSphere>();
    public int currentMaxSpheres = 0;
    public float[] distances = new float[2] { 25.0f, 50.0f };
    #endregion
    /// <summary>
    /// Call SetupCullSpheres
    /// </summary>
    void Awake()
    {
        SetupCullSpheres();
    }
    /// <summary>
    /// Clear all the group and sphere data
    /// </summary>
    private void OnDestroy()
    {
        group.Dispose();
        group = null;
        spheres = null;
    }
    #region CullSphere
    /// <summary>
    /// Make a single CullingGroup that has MAX_CULLSPHERES spheres. It is better to have unused spheres than to continuously grow the spheres array.
    /// Get all the CullSpheres in the scene, these are the objects saved in the scene. For each CullSphere add to the CullingGroup
    /// </summary>
    private void SetupCullSpheres()
    {

        group = new CullingGroup();
        group.targetCamera = Camera.main;
        spheres = new BoundingSphere[MAX_CULLSPHERES];

        CullSphere[] cullers = FindObjectsOfType<CullSphere>();
        cSpheres.Clear();

        if (cullers.Length > MAX_CULLSPHERES)
        {
            Debug.LogWarning("Warning more cullers than room in group");
        }
        for (int i = 0; i < cullers.Length; i++)
        {
            cSpheres.Add(i, cullers[i]);
            cullers[i].SetIndex(i);
            spheres[i] = cullers[i].GetSphere();
            cullers[i].cullMan = this;
            cullers[i].CanUpdate(true);
        }
        currentMaxSpheres = cullers.Length;
        group.SetBoundingSpheres(spheres);
        group.SetBoundingSphereCount(currentMaxSpheres);
        //Distance
        group.SetBoundingDistances(distances);
        group.SetDistanceReferencePoint(Camera.main.transform);

    }
    /// <summary>
    /// Remove this sphere from group and index from the dictionary
    /// </summary>
    /// <param name="cS"></param>
    public void RemoveSphere(CullSphere cS)
    {
        int i = cS.GetIndex();
        RemoveSphere(i);
    }
    void RemoveAndCompact(int i)
    { 
        if (!cSpheres.ContainsKey(i))
        {
            Debug.Log("Culler not in dictionary");
            return;
        }

        int lastIndex = currentMaxSpheres - 1;
        // Remove from hashmap
        CullSphere cS = cSpheres[i];
        CullSphere lastCS = cSpheres[lastIndex];
        cS.CanUpdate(false);
        cSpheres.Remove(i);
        
        if (i < lastIndex)
        {
            // Get the last object
            BoundingSphere lastObject = spheres[lastIndex];

            // Move it to the removed position
            spheres[i] = lastObject;

            // Update its index in the map
            cSpheres.Remove(lastIndex);
            lastCS.SetIndex(i);
            cSpheres.Add(i, lastCS);
        }

        currentMaxSpheres--;

        group.SetBoundingSpheres(spheres);
        group.SetBoundingSphereCount(currentMaxSpheres);
    }


    /// <summary>
    /// Remove Sphere from group and disctionary
    /// </summary>
    /// <param name="i"></param>
    public void RemoveSphere(int i)
    {
         RemoveAndCompact(i);
        //if (cSpheres.ContainsKey(i))
        //{
        //    CullSphere cS = cSpheres[i];
        //    cS.CanUpdate(false);
        //    cSpheres.Remove(i);
        //    currentMaxSpheres--;
        //    group.SetBoundingSphereCount(currentMaxSpheres);
        //}
        //else
        //{
        //    Debug.Log("RemoveSphere does not contain key for " + i);
        //}
    }

    /// <summary>
    /// Add a dynamic sphere to the group and dictionary, if it is not already there.
    /// </summary>
    /// <param name="cS"></param>
    public void AddSphere(CullSphere cS)
    {
        if (cSpheres.ContainsKey(cS.GetIndex()))
        {
            //Debug.Log(cS.name + " CullingSphere already added");
            return;
        }
        int i = 0;
        while (i < MAX_CULLSPHERES)
        {
            if (!cSpheres.ContainsKey(i))
            {
                cS.SetIndex(i);
                cSpheres.Add(i, cS);
                spheres[i] = cS.GetSphere();
                cS.CanUpdate(true);
                currentMaxSpheres++;
                group.SetBoundingSpheres(spheres);
                group.SetBoundingSphereCount(currentMaxSpheres);
                return;
            }
            i++;
        }
    }
    #endregion
}
