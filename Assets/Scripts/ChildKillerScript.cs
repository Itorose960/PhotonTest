using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildKillerScript : MonoBehaviour
{
    private void FixedUpdate()
    {
        if(transform.childCount > 0)
        {
            Destroy(transform.GetChild(0).gameObject);
        }
    }
}
