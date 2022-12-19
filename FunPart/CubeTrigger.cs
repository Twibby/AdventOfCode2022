using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.LogError("Another cube entered in my cube!!");
    }
}
