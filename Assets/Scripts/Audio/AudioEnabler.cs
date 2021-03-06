﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioEnabler : MonoBehaviour
{

    private GameObject[] buttons;

    void Start()
    {
        FindObjectOfType<AudioManager>().Play("Ambient");
        FindObjectOfType<AudioManager>().Play("Music");
    }

    public void FindButtons()
    {
        StartCoroutine(DelayedFind());
    }

    private IEnumerator DelayedFind()
    {
        yield return new WaitForSeconds(0.3f);
        buttons = GameObject.FindGameObjectsWithTag("Button");

        foreach (GameObject b in buttons)
        {
            b.GetComponent<Button>().onClick.AddListener(PlayButtonSound);
        }
    }

    void PlayButtonSound()
    {
        FindObjectOfType<AudioManager>().Play("Normal Button");
    }
}
