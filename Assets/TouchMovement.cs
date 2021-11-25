using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchMovement : MonoBehaviour
{
    Rigidbody rb;
    Vector3 touchPos;
    [SerializeField] Transform model;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
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
            Touch touch = Input.GetTouch(0);
            touchPos = Camera.main.ScreenToWorldPoint(touch.position);
            Debug.Log(touch.position + " || " + touchPos);
            touchPos = new Vector3(touchPos.x, 0, touchPos.y);

            Vector3 playerPos = transform.position;
            Vector3 toPoint = (touchPos - playerPos);

            

            if (toPoint.magnitude > 0.1f)
            {
                rb.AddForce(toPoint.normalized * Time.deltaTime * 60f * 50f);
                model.rotation = Quaternion.FromToRotation(Vector3.forward, toPoint);
            }
        }
    }
}
