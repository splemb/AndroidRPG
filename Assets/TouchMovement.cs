using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchMovement : MonoBehaviour
{
    Rigidbody rb;
    Vector3 touchPos;
    [SerializeField] Transform model;
    Camera inputCamera;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        inputCamera = Camera.main.GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }
    void Movement()
    {
        if (Input.touchCount > 0)
        {

            rb.drag = 2f;
            Touch touch = Input.GetTouch(0);
            touchPos = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, Camera.main.transform.position.y));

            touchPos = new Vector3(touchPos.x, transform.position.y, touchPos.z);
            Debug.Log(touch.position + " || " + touchPos);

            Vector3 playerPos = transform.position;
            Vector3 toPoint = (touchPos - playerPos);


            if (toPoint.magnitude > 0.01f)
            {
                rb.AddForce(toPoint.normalized * Mathf.Clamp(toPoint.magnitude, 0, 2f) * Time.deltaTime * 60f * 5f);
                Vector3 lookDir = touchPos - transform.position;
                lookDir.y = 0; // keep only the horizontal direction
                model.rotation = Quaternion.LookRotation(lookDir);
                //model.rotation = Quaternion.FromToRotation(Vector3.forward, toPoint);
            }
        }

        else { rb.drag = 5f; }
    }
}
