using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;

public class PoseManager : MonoBehaviour
{

    public InputActionReference joystickXY;
    public InputActionReference joystickZR;
    public InputActionReference alt;
    public InputActionReference action;
    public UnityEvent actions;
    public GameObject sphere;
    public float speed = 1.0f;
    public bool handSelectable = false;
    public Transform root;
    public Transform _root;
    private Transform _mainCamera;

    void Start()
    {
        if (root == null)
        {
            root = transform;
        }
        // need to ensure this happens after tf init....
        _mainCamera = Camera.main.transform;

        if (actions != null && action == null)
        {
            actions = null;
            Debug.LogWarning("PoseManager: actions set but action not set");
        }

    }

    void Update()
    {
        while (root.parent != null)
        {
            root = root.parent;
            _root = root;
            Debug.Log("root frame: " + root);

        }

        if (action.action.IsPressed())
        {
            if (alt.action.IsPressed())
            {
                ResetScale();
            }
            else if (actions != null)
            {
                actions.Invoke();
            }
        }


        if (joystickXY.action.IsPressed() || joystickZR.action.IsPressed())
        {
            if (alt.action.IsPressed())
                Scale(joystickXY.action.ReadValue<Vector2>());
            else
                Move(joystickXY.action.ReadValue<Vector2>());

            OffsetRotate(joystickZR.action.ReadValue<Vector2>());

            sphere.SetActive(true);
        }
        else
        {
            sphere.SetActive(false);
        }
    }

    void ResetScale()
    {
        _root.localScale = Vector3.one;
    }

    void Move(Vector2 input)
    {
        Vector3 move = new Vector3(input.x, 0, input.y);
        // move = root.TransformDirection(move);

        // get relative to the player's view point
        move = _mainCamera.TransformDirection(move);
        // take into account the gameobject's orientation
        // zero out the vertical component
        move.y = 0;
        // move the gameobject relative to the player regardless of gameobject orientation
        _root.Translate(move * speed * Time.deltaTime, Space.World);
    }

    void Scale(Vector2 input)
    {
        Vector3 scale = new Vector3(input.x, input.x, input.x);
        scale = Vector3.Scale(_root.localScale, scale);
        _root.localScale += scale * speed * Time.deltaTime;
    }

    void OffsetRotate(Vector2 input)
    {
        // offset on the y axis based on forwards/back on second joystick
        Vector3 move = new Vector3(0, input.y, 0);

        // rotate on the x axis based on left/right on second joystick
        _root.Translate(move * speed * Time.deltaTime / 10);
        _root.Rotate(0, input.x * speed * Time.deltaTime * 20, 0);
    }

    public void ClickCb(SelectEnterEventArgs args)
    {
        if (_root == null || !handSelectable) return;

        Vector3 position;
        XRRayInteractor rayInteractor = (XRRayInteractor)args.interactor;
        rayInteractor.TryGetHitInfo(out position, out _, out _, out _);
        _root.position = position;

    }
}
