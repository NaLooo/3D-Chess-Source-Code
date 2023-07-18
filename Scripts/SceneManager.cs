using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SceneManager : MonoBehaviour
{
    public GameObject forest;
    public GameObject meadow;
    public GameObject swamp;
    public GameObject tropical;

    GameObject currentScene;

    void Start()
    {
        currentScene = forest;
    }

    public void ToForest()
    {
        SwitchScene(forest);
    }

    public void ToMeadow()
    {
        SwitchScene(meadow);
    }

    public void ToSwamp()
    {
        SwitchScene(swamp);
    }

    public void ToTropical()
    {
        SwitchScene(tropical);
    }

    void SwitchScene(GameObject next)
    {
        currentScene.SetActive(false);
        currentScene = next;
        currentScene.SetActive(true);
    }
}
