﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dog : MonoBehaviour {

    public float speed = 2;
    public float rotation = 3;

    private Rigidbody2D body;
    private bool stunned = false;
    private Animator anim;
    private GameObject costume;
    private PlayerObject player;

    //Power Ups
    private bool hasWool = false;

	// Use this for initialization
	void Start () {
        body = GetComponent<Rigidbody2D>();
        body.gravityScale = 0;  
        body.drag = 0;
        body.freezeRotation = true;

        Transform animBody = transform.GetChild(0);
        anim = animBody.GetComponent<Animator>();
        animBody.GetChild(0).GetComponent<SpriteRenderer>().sprite = player.images[0];
        animBody.GetChild(1).GetComponent<SpriteRenderer>().sprite = player.images[1];
        animBody.GetChild(2).GetComponent<SpriteRenderer>().sprite = player.images[2];
        animBody.GetChild(3).GetChild(0).GetComponent<SpriteRenderer>().sprite = player.images[3];
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (stunned)
            return;
        
        body.velocity = transform.right * speed;
        if (Input.GetKey(player.left))
            body.rotation += rotation * Time.deltaTime * 50;
        if (Input.GetKey(player.right))
            body.rotation -= rotation * Time.deltaTime * 50;
	}

    public void SetOwner(PlayerObject owner)
    {
        player = owner;
        name = player.playerName.ToString();
    }

    public PlayerObject GetOwner()
    {
        return player;
    }

    //Stun
    public void stun(float stunTime)
    {
        body.velocity = Vector2.zero;
        stunned = true;
        anim.SetBool("stunned", true);
        StartCoroutine(removeStun(stunTime));
    }
    IEnumerator removeStun(float seconds)
    {
        yield return new WaitForSeconds(seconds / 4);
        GetComponent<RandomWuffGenerator>().playRandomSound();
        yield return new WaitForSeconds(3 * seconds/2);
        stunned = false;
        anim.SetBool("stunned", false);
    }

    //Wool PowerUp
    public void AddWool(float duration, GameObject costume)
    {
        hasWool = true;
        costume.transform.parent = transform;
        costume.transform.localPosition = Vector3.zero;
        costume.transform.rotation = new Quaternion();
        costume.GetComponent<SpriteRenderer>().enabled = true;
        this.costume = costume;
        StartCoroutine(RemoveWool(duration));
    }
    IEnumerator RemoveWool(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        hasWool = false;
        Destroy(costume);
    }
    public bool IsSheep()
    {
        return hasWool;
    }

    //SpeedUp
    public void SpeedUp(float duration, float add)
    {
        speed += add;
        StartCoroutine(RemoveSpeed(duration, add));
    }
    IEnumerator RemoveSpeed(float seconds, float sub)
    {
        yield return new WaitForSeconds(seconds);
        speed -= sub;
    }
}