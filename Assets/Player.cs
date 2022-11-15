using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float mouseSensitive = 3.0f;
    float cameraAngle = 0;
    GameObject camera;
    Transform tr;
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        tr = gameObject.GetComponent<Transform>();
        camera = tr.Find("Main Camera").gameObject;
    }

    // Update is called once per frame
    void Update()
    {

        float xinput = Input.GetAxisRaw("Horizontal");
        float zinput = Input.GetAxisRaw("Vertical");
        Vector3 moveHorizontal = transform.right * xinput;
        Vector3 moveVertical = transform.forward * zinput;
        Vector3 velocity = (moveHorizontal + moveVertical).normalized * moveSpeed;
        tr.position = (transform.position + velocity * Time.deltaTime);

        float yMouseInput = Input.GetAxisRaw("Mouse Y");
        cameraAngle -= yMouseInput * mouseSensitive;
        cameraAngle = Mathf.Clamp(cameraAngle, -60, 60);
        camera.transform.localEulerAngles = new Vector3(cameraAngle, 0, 0);

        float xMouseInput = Input.GetAxisRaw("Mouse X");
        Vector3 rot = new Vector3(0f, xMouseInput, 0f) * mouseSensitive;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rot));

        if(yMouseInput == 0 && xMouseInput == 0)
        {
            rb.angularVelocity = Vector3.zero;
        }
    }
}
