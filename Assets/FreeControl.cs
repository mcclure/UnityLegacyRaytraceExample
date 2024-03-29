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

    Quaternion qLeft = Quaternion.Euler(0.0f, -90.0f, 0.0f);
    Quaternion qDown = Quaternion.Euler(90.0f, 0.0f, 0.0f);

    void MoveFor(UnityEngine.KeyCode minus, UnityEngine.KeyCode plus, Quaternion q) {
        int move = 0;
        var rotate = transform.rotation*q;
        var forward = rotate*Vector3.forward;
        if (Input.GetKey(minus))
            move = 1;
        else if (Input.GetKey(plus))
            move = -1;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            move *= 10;
        if (move != 0) {
            transform.position += move * forward * moveSpeed * Time.deltaTime;
        }
    }

    // Update is called once per frame
    void Update()
    {
        rotation.y += Input.GetAxis ("Mouse X");
		rotation.x += -Input.GetAxis ("Mouse Y");
		transform.eulerAngles = (Vector2)rotation * rotSpeed;
        MoveFor(KeyCode.W, KeyCode.S, Quaternion.identity);
        MoveFor(KeyCode.A, KeyCode.D, qLeft); // Strafe
        MoveFor(KeyCode.C, KeyCode.E, qDown); // Elevator
    }
}
