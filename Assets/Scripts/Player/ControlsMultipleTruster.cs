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
        if (Input.GetButton("TrusterNorth")){
            gameObject.GetComponent<Rigidbody>().AddForceAtPosition(-northTruster.forward, northTruster.position);
        }
        if (Input.GetButton("TrusterSouth")){
            gameObject.GetComponent<Rigidbody>().AddForceAtPosition(-southTruster.forward, southTruster.position);
        }
        if (Input.GetButton("TrusterEast")){
            gameObject.GetComponent<Rigidbody>().AddForceAtPosition(-eastTruster.forward, eastTruster.position);
        }
        if (Input.GetButton("TrusterWest")){
            gameObject.GetComponent<Rigidbody>().AddForceAtPosition(-westTruster.forward, westTruster.position);
        }
        if (Input.GetButton("TrusterFront")){
            gameObject.GetComponent<Rigidbody>().AddForceAtPosition(-frontTruster.forward, frontTruster.position);
        }
        if (Input.GetButton("TrusterBack")){
            gameObject.GetComponent<Rigidbody>().AddForceAtPosition(-backTruster.forward, backTruster.position);
        }
    }
}