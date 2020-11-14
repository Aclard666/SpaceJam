using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsMultipleTruster : MonoBehaviour
{
    public string northKey;
    public string southKey;
    public string eastKey;
    public string westKey;
    public string frontKey;
    public string backKey;
    public Transform northTruster;
    public Transform southTruster;
    public Transform eastTruster;
    public Transform westTruster;
    public Transform frontTruster;
    public Transform backTruster;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetInput(frontKey))
        if (Input.GetKey(frontKey))
        {
           gameObject.GetComponent<Rigidbody>().AddForceAtPosition(-frontTruster.forward, frontTruster.position);
        }
        if (Input.GetKey(backKey))
        {
           gameObject.GetComponent<Rigidbody>().AddForceAtPosition(-backTruster.forward, backTruster.position);
        }
        if (Input.GetKey(northKey))
        {
           gameObject.GetComponent<Rigidbody>().AddForceAtPosition(-northTruster.forward, northTruster.position);
        }
        if (Input.GetKey(southKey))
        {
           gameObject.GetComponent<Rigidbody>().AddForceAtPosition(-southTruster.forward, southTruster.position);
        }
        if (Input.GetKey(eastKey))
        {
           gameObject.GetComponent<Rigidbody>().AddForceAtPosition(-eastTruster.forward, eastTruster.position);
        }
        if (Input.GetKey(westKey))
        {
           gameObject.GetComponent<Rigidbody>().AddForceAtPosition(-westTruster.forward, westTruster.position);
        }
    }
}