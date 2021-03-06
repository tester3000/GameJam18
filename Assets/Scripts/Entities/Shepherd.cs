﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shepherd : MonoBehaviour {

    public float triggerRadius = 0.3f;
    public float hitTime = 0.5f;
    public float stunTime = 1.5f;
    public float readyTime = 0.5f;
    public SpriteRenderer hat;

    private PlayerObject player;
    private List<Dog> otherDogs = new List<Dog>();
    private float timer = 0;
    private bool ready = true;
    private Animator anim;
    private RandomHuhGenerator huh;

	// Use this for initialization
	void Start () {
        readyTime += stunTime;
        anim = GetComponentInChildren<Animator>();
        huh = GetComponent<RandomHuhGenerator>();
	}
	
	// Update is called once per frame
	void Update () {
        if (otherDogs.Count > 0)
        {
            Vector3 direction = transform.position - otherDogs[0].transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            //transform.LookAt(anotherDog.transform);
            timer -= Time.deltaTime;

            if(timer <= 0)
            {
                foreach (Dog doggo in otherDogs)
                {
                    doggo.stun(stunTime);
                }
                otherDogs.Clear();
                ready = false;
                anim.SetBool("attacking", true);
                huh.PlaySweep();
                StartCoroutine(MakeReady(readyTime));
            }
        }
	}

    public void SetOwner(PlayerObject owner)
    {
        hat.sprite = owner.hat;
        player = owner;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Dog tempDog = collision.GetComponent<Dog>();

        if (ready && tempDog && tempDog.GetOwner() != player && !otherDogs.Contains(tempDog))
        {
            if (otherDogs.Count==0)
            {
                timer = hitTime;
                huh.playAngrySound();
            }
            otherDogs.Add(tempDog);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Dog tempDog = collision.GetComponent<Dog>();
        if (tempDog)
        {
            otherDogs.Remove(tempDog);
        }
    }

    IEnumerator MakeReady(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        ready = true;
    }
}
