using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ViewerTrack : MonoBehaviour
{
    private NerfRender render;
    public float translationTolerance = 0.1f;
    public TMPro.TextMeshProUGUI text;

    private Vector3 lastPosition;
    private Quaternion lastRotation;
    private bool _enabled = false;
    private Animator _animator;

    void Start()
    {
        render = GetComponent<NerfRender>();
        lastPosition = transform.position;
        lastRotation = transform.rotation;
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_enabled || _animator.GetCurrentAnimatorStateInfo(0).IsName("Open"))
        {
            if (Vector3.Distance(transform.position, lastPosition) > translationTolerance 
                || Quaternion.Angle(transform.rotation, lastRotation) > translationTolerance * 10)
            {
                render.Render();
                lastPosition = transform.position;
                lastRotation = transform.rotation;
            }
        }
    }

    public void ChangeFOV(float fov)
    {
        render.fovFactor = 1f-fov+.001f;
        float val = fov * 100;
        text.SetText(val.ToString("0.00") + "%");
        render.Render();
    }
}
