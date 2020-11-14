using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagementReferencing : MonoBehaviour
{
    public ComputeShader CS_Gravity;
    private void Start()
    {
        GameManager.GetSingleton()._ref = this;
    }
}
