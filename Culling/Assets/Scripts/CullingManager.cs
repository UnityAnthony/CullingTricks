using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CullingManager : MonoBehaviour
{

    public CullingGroup group;
    public BoundingSphere[] spheres;
    public int MAX_CULLSPHERES = 500;
    public Dictionary<int, CullSphere> cSpheres = new Dictionary<int, CullSphere>();
    public List<int> availableSphereIndex = new List<int>();
    public int currentMaxSpheres = 0;
    public float[] distances = new float[2] { 25.0f, 50.0f };
    // Start is called before the first frame update
    void Awake()
    {
        SetupCullSpheres();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnDestroy()
    {
        group.Dispose();
        group = null;
        spheres = null;
    }
    #region CullSphere
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
    public void RemoveSphere(CullSphere cS)
    {
        int i = cS.GetIndex();
        if (cSpheres.ContainsKey(i))
        {
            // CullSphere cS = cSpheres[i];
            cS.CanUpdate(false);
            cSpheres.Remove(i);
            currentMaxSpheres--;
            group.SetBoundingSphereCount(currentMaxSpheres);
        }
        else
        {
            Debug.Log("RemoveSphere does not contain key for " + i);
        }
    }
    public void RemoveSphere(int i)
    {
        if (cSpheres.ContainsKey(i))
        {
            CullSphere cS = cSpheres[i];
            cS.CanUpdate(false);
            cSpheres.Remove(i);
            currentMaxSpheres--;
            group.SetBoundingSphereCount(currentMaxSpheres);
        }
        else
        {
            Debug.Log("RemoveSphere does not contain key for " + i);
        }
    }

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
                group.SetBoundingSphereCount(currentMaxSpheres);
                return;
            }
            i++;
        }
    }
    #endregion
}
