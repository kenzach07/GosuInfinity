using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boundaries : MonoBehaviour
{
   
    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -10f, 10f),
            Mathf.Clamp(transform.position.y, -5f, 5f), transform.position.z);
    }
}
