using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cleaning : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("Start");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionStay(Collision collision) {
        Debug.Log(collision.gameObject.name);
    }
}
