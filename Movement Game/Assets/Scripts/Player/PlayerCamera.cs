using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;

public class PlayerCamera : MonoBehaviour
{
    [Header("Settings")]
    public bool lockCursor;

    [Header("Sensitivity")]
    public float sensX;
    public float sensY;

    [Header("References")]
    public Transform orientation;
    public Transform holder;
    public Camera mainCamera;
    public CinemachineVirtualCamera vCam;

    float xRot;
    float yRot;

    // Start is called before the first frame update
    void Start()
    {
        vCam.m_Lens.FieldOfView = mainCamera.fieldOfView;
    }

    private void Update()
    {
        Cursor.visible = !lockCursor;
        if (lockCursor)        
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.Confined;

    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!lockCursor) return;

        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRot += mouseX;

        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRot, yRot, 0);
        holder.rotation = Quaternion.Euler(xRot, yRot, 0);
        orientation.rotation = Quaternion.Euler(0, yRot, 0);
    }

    public void DoFov(float targetFOV)
    {
        mainCamera.DOFieldOfView(targetFOV, 0.25f).SetEase(Ease.InOutSine).OnUpdate(() => vCam.m_Lens.FieldOfView = mainCamera.fieldOfView); // Sync FOV during animation
    }

    public void ZoomOut()
    {
        float originalFOV = vCam.m_Lens.FieldOfView;
        mainCamera.DOFieldOfView(originalFOV, 0.25f).SetEase(Ease.InOutSine).OnUpdate(() => vCam.m_Lens.FieldOfView = mainCamera.fieldOfView); // Sync FOV during animation
    }

    public void DoTilt(float endValue)
    {
        StopCoroutine(nameof(LerpTilt));
        StartCoroutine(LerpTilt(endValue));
    }

    IEnumerator LerpTilt(float endValue)
    {
        float t = 0;
        float d = Mathf.Abs(vCam.m_Lens.Dutch - endValue);
        float s = vCam.m_Lens.Dutch;

        while (t < d)
        {
            vCam.m_Lens.Dutch = Mathf.Lerp(s, endValue, t / d);
            t += Time.deltaTime * 40f;
            yield return null;
        }

        vCam.m_Lens.Dutch = endValue;
    }
}
