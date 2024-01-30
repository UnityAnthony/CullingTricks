using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class Spawner : MonoBehaviour
{
    #region Public Vars
    public GameObject[] prefabs = new GameObject[0];
    public int maxObjects;
    public Grid grid;
    public int width = 100;
    public CullingManager cullMan;
    #endregion


    /// <summary>
    /// Note this uses InstantiateAsync only used available in 2023, Once InstantiateAsync is called we subscribe to the .completed callback
    /// Added a check to make sure we don't add more objects than there are spheres.
    /// </summary>
    void Start()
    {
       
        cullMan = FindObjectOfType<CullingManager>();

        if (cullMan)
        { 
            if(maxObjects > cullMan.MAX_CULLSPHERES)
            {
                maxObjects = cullMan.MAX_CULLSPHERES - cullMan.currentMaxSpheres;// reduce the amount by any sphere already in the scene
            }
        }

        for (int i = 0; i < maxObjects; i++) 
        {
            int index = Random.Range(0, prefabs.Length);
            GameObject gameObject = prefabs[index];
            int x = i % width;
            int z = i / width;

            Vector3 position = grid.GetCellCenterWorld( new Vector3Int(x, 0, z) );
            AsyncInstantiateOperation<GameObject> result = InstantiateAsync(gameObject, position, Quaternion.identity);
            result.completed += UpdateCuller;

        }
    }



    /// <summary>
    /// We must cast the AsyncOperation to AsyncInstantiateOperation in order to gain access to the .Result[] Array, inside will be the GameObject which was Instatiated. 
    /// </summary>
    /// <param name="operation"></param>
    private void UpdateCuller(AsyncOperation operation)
    {
        AsyncInstantiateOperation op = operation as AsyncInstantiateOperation;
        if (op != null)
        {
            //Unit u = op.Result[0].GetComponent<Unit>();
            //if (u)
            //{
            //    u.cullMan = cullMan;  
            //}

            //CullSphere cS = op.Result[0].GetComponent<CullSphere>();
            //if (cS)
            //{
            //    cS.cullMan = cullMan;
            //}
        }

    }
}
