using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FreeControl : MonoBehaviour
{
    Vector2 rotation = Vector2.zero;
	public float rotSpeed = 3;
    public float moveSpeed = 2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rotation.y += Input.GetAxis ("Mouse X");
		rotation.x += -Input.GetAxis ("Mouse Y");
		transform.eulerAngles = (Vector2)rotation * rotSpeed;
        int move = 0;
        if (Input.GetKey(KeyCode.W))
            move = 1;
        else if (Input.GetKey(KeyCode.S))
            move = -1;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            move *= 10;
        if (move != 0) {
            transform.position += move * transform.forward * moveSpeed * Time.deltaTime;
        }
    }
}
