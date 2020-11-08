using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour
{

    public string northKey;
    public string southKey;
    public string eastKey;
    public string westKey;
    public string firekey;
    public Transform truster;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(firekey))
        {
           gameObject.GetComponent<Rigidbody>().AddForceAtPosition(truster.forward, truster.position);
        }
        if (Input.GetKey(northKey))
        {
            truster.Rotate(Vector3.right * Time.deltaTime);
        }
        if (Input.GetKey(southKey))
        {
            truster.Rotate(Vector3.right * -Time.deltaTime);
        }
        if (Input.GetKey(eastKey))
        {
            truster.Rotate(Vector3.up * Time.deltaTime);
        }
        if (Input.GetKey(westKey))
        {
            truster.Rotate(Vector3.up * -Time.deltaTime);
        }
    }
}