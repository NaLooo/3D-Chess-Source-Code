using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    public static Vector3 boardOrigin = new Vector3(-7, 0, -7);
    public static int blockWidth = 2;
    public static string startingBoard = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

    public static Vector3 whiteCameraPos = new Vector3(0, 20, -12);
    public static Vector3 blackCameraPos = new Vector3(0, 20, 12);
    public static Quaternion whiteCameraRot = Quaternion.Euler(60, 0, 0);
    public static Quaternion blackCameraRot = Quaternion.Euler(60, 180, 0);

    public static Quaternion front = Quaternion.identity;
    public static Quaternion back = new Quaternion(0, 180, 0, 0);
    public static Quaternion right = Quaternion.Euler(0, 90, 0);
    public static Quaternion left = Quaternion.Euler(0, -90, 0);
    public static Quaternion flip = Quaternion.Euler(0, 0, 180);

    public static Color transparent = new Color(0, 0, 0, 0);
    public static Color normalColor = new Color(255, 255, 255, 255);

    public static float moveSpeed = 3.5f;
    public static float cameraScrollSpeed = 1f;
    public static float cameraHorizontalRotateSpeed = 1f;
    public static float cameraVerticalRotateSpeed = 2f;
}