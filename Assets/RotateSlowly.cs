using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSlowly : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    Quaternion perFrame = Quaternion.Euler(0.21f*4.0f, 0.2f*4.0f, 0.48126f * 4.0f); // Arbitrary

    // Update is called once per frame
    void Update()
    {
        transform.rotation *= (Quaternion.Slerp(Quaternion.identity, perFrame, Time.deltaTime));
    }
}
