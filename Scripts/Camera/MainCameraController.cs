using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraController : MonoBehaviour
{
    float timer = 0f;
    void Start()
    {
        transform.LookAt(Vector3.zero);
    }
    void Update()
    {
        var scroll = Input.mouseScrollDelta.y;
        if (scroll != 0f)
        {
            var n = transform.position.normalized;
            transform.position -= n * scroll * Constants.cameraScrollSpeed;
            if (transform.position.magnitude < 10)
            {
                transform.position = transform.position.normalized * 10;
            }
            if (transform.position.magnitude > 40)
            {
                transform.position = transform.position.normalized * 40;
            }
        }

        if (Input.GetMouseButton(2))
        {
            var dx = Input.GetAxis("Mouse X") * Constants.cameraHorizontalRotateSpeed * Time.deltaTime * transform.position.magnitude;
            var dy = Input.GetAxis("Mouse Y") * Constants.cameraVerticalRotateSpeed * Time.deltaTime * transform.position.magnitude;
            if (dx != 0 || dy != 0)
            {
                timer = 0f;
                if (dx != 0)
                {
                    var r = new Vector2(transform.position.x, transform.position.z).magnitude;
                    var oriAngle = transform.position.z >= 0 ? Mathf.Acos(transform.position.x / r) : -Mathf.Acos(transform.position.x / r);
                    var deltaAngle = dx / r;
                    var targetAngle = oriAngle - deltaAngle;
                    var newX = Mathf.Cos(targetAngle) * r;
                    var newZ = Mathf.Sin(targetAngle) * r;
                    transform.position = new Vector3(newX, transform.position.y, newZ).normalized * transform.position.magnitude;
                    transform.LookAt(Vector3.zero);
                }
                if (dy != 0)
                {
                    var min = 10f * Mathf.Deg2Rad;
                    var max = 80f * Mathf.Deg2Rad;
                    var r = new Vector2(transform.position.x, transform.position.z);
                    var R = transform.position.magnitude;
                    var oriAngle = Mathf.Acos(transform.position.y / R);
                    var deltaAngle = dy / R;
                    var targetAngle = Mathf.Clamp(oriAngle + deltaAngle, min, max);
                    var newY = Mathf.Cos(targetAngle) * R;
                    var newR = Mathf.Sin(targetAngle) * R;
                    r = r.normalized * newR;
                    transform.position = new Vector3(r.x, newY, r.y).normalized * R;
                    transform.LookAt(Vector3.zero);
                }
            }
            else
            {
                timer += Time.deltaTime;
                if (timer > 1)
                {
                    timer = 0f;
                    ResetCamera();
                }
            }
        }
        else
        {
            timer = 0f;
        }
    }

    public void ResetCamera()
    {
        transform.position = GameManager.playerIsWhite ? Constants.whiteCameraPos : Constants.blackCameraPos;
        transform.LookAt(Vector3.zero);
    }
}
