using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] prefabs = new GameObject[0];
    public int maxObjects;
    public Grid grid;
    public int width = 100;
    public List<GameObject> objs = new List<GameObject>();
    public CullingManager cullMan;
    // Start is called before the first frame update
    void Start()
    {
        cullMan = FindObjectOfType<CullingManager>();
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



    private void UpdateCuller(AsyncOperation operation)
    {
        AsyncInstantiateOperation op = operation as AsyncInstantiateOperation;
        if (op != null)
        {
            Unit u = op.Result[0].GetComponent<Unit>();
            if (u)
            {
                u.cullMan = cullMan;  
            }

            CullSphere cS = op.Result[0].GetComponent<CullSphere>();
            if (cS)
            {
                cS.cullMan = cullMan;
                objs.Add(cS.gameObject);
            }
               
        }

    }



}
