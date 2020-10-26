using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class FPController : MonoBehaviour
{
    public GameObject cam;
    public Animator anim;
    float speed = 0.1f;
    float sensitivity = 2;
    float minimumX = -90;
    float maximumX = 90;
    Rigidbody rb;
    CapsuleCollider capsule;
    bool cursorIsLocked = true;
    bool lockCursor = true;
    float x;
    float z;
    Quaternion cameraRot;
    Quaternion characterRot;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        capsule = this.GetComponent<CapsuleCollider>();
        cameraRot = cam.transform.localRotation;
        characterRot = cam.transform.localRotation;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            anim.SetBool("arm", !anim.GetBool("arm"));

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            anim.SetTrigger("fire");

        }

        if (Input.GetKeyDown(KeyCode.R))
            anim.SetTrigger("reload");

        if (Mathf.Abs(x) > 0 || Mathf.Abs(z) > 0)

            if(!anim.GetBool("walking"))

            anim.SetBool("walking", true);

        else if(anim.GetBool("walking"))

            anim.SetBool("walking", false);
    }

    void FixedUpdate()
    {
        float yRot = Input.GetAxis("Mouse X");
        float xRot = Input.GetAxis("Mouse Y");

        cameraRot *= Quaternion.Euler(-xRot * sensitivity, 0, 0);
        characterRot *= Quaternion.Euler(0, yRot * sensitivity, 0);

        cameraRot = ClampRotationAroundXAxis(cameraRot);

        this.transform.localRotation = characterRot;
        cam.transform.localRotation = cameraRot;

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
        rb.AddForce(0, 300, 0);

        }

        x = Input.GetAxis("Horizontal") * speed;
        z = Input.GetAxis("Vertical") * speed;

        //transform.position += new Vector3(x * speed, 0, z * speed);
        transform.position += cam.transform.forward * z + cam.transform.right * x;
        UpdateCursorLock();
    }

    bool IsGrounded()
    {
        RaycastHit hitInfo;
        if(Physics.SphereCast(transform.position, capsule.radius, Vector3.down, out hitInfo, (capsule.height /2) - capsule.radius + 0.1f))
        {
            return true;
        }
        return false;
    }

    Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);
        angleX = Mathf.Clamp(angleX, minimumX, maximumX);

        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;

    }

    public void SetCursorLock(bool value)
    {
        lockCursor = value;
        if (!lockCursor)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void UpdateCursorLock()
    {
        if (lockCursor)
        {
            InternalLockUpdate();
        }
    }

    public void InternalLockUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            cursorIsLocked = false;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            cursorIsLocked = true;
        }

        if (cursorIsLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if(!cursorIsLocked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
