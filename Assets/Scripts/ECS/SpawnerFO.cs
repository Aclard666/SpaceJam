using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerFO : MonoBehaviour
{
    [Range(0,10000)]
    public int count;

    [Range(0,1000000)]
    public int rangeX;
    [Range(0, 1000000)]
    public int rangeY;
    [Range(0, 1000000)]
    public int rangeZ;


    public GameObject obj;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < count; i++)
        {
            Instantiate(obj, new Vector3(Random.Range(-rangeX / 2, rangeX / 2), Random.Range(-rangeY / 2, rangeY / 2), Random.Range(-rangeZ / 2, rangeZ / 2)),Quaternion.identity);

        }
        
    }

}
