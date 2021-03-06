﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UserControl : MonoBehaviour
{
    public static UserControl _instance;

    public float rotationSpeed = 30;
    public float minY, maxY;
    public bool lockMouse = false;

    [HideInInspector]
    public bool runCamera = true;

    private Vector3 curEuler;
    private Vector3 inputRot;
    private bool lockMouseLastFrame = false;

    private bool runApplyRotation = false;
    private Vector3 moveDir;

    private void Awake()
    {
        _instance = this;
    }

    public void Look(InputAction.CallbackContext context)
    {
        moveDir.x = context.ReadValue<Vector2>().x;
        moveDir.y = context.ReadValue<Vector2>().y;
    }

    public void Grab(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed || lockMouse)
            runApplyRotation = true;
        else
            runApplyRotation = false;
    }

    public void LockMouse(bool lockedState = true)
    {
        lockMouse = lockedState;
        Cursor.lockState = (lockMouse) ? CursorLockMode.Confined : CursorLockMode.None;
        Cursor.visible = !lockMouse;
    }

    private void Update()
    {
        if (!runCamera)
            return;

        if (runApplyRotation)
            ApplyRotation();

        if (lockMouse != lockMouseLastFrame)
            LockMouse(lockMouse);

        lockMouseLastFrame = lockMouse;
        Cursor.visible = !runApplyRotation;
    }

    private void ApplyRotation()
    {
        inputRot = (moveDir * rotationSpeed) * Time.deltaTime;

        curEuler.y = (lockMouse) ? curEuler.y + inputRot.x : curEuler.y - inputRot.x;
        curEuler.x = (lockMouse) ? Mathf.Clamp(curEuler.x - inputRot.y, minY, maxY) : Mathf.Clamp(curEuler.x + inputRot.y, minY, maxY);

        transform.eulerAngles = curEuler;
    }

    public void TryUseAction(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("Try Use");
            WorldManager._instance.RunActionOnActiveNavPoint();
        }
    }
}
