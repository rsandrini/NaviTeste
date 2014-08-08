using UnityEngine;
using System.Collections;
using System;

public class MovimentScript : MonoBehaviour {

    public float forcePropusionTop;
    public float maxForcePropusionTop;
    public float maxAceleration;
    public float fixedForce;
    public float recordedAltitude;
    public bool stabilize;

    public float diffY;

    public ParticleSystem particle;

	// Use this for initialization
	void Start () {
        forcePropusionTop = 0;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.Space))
        {
            addForce(0.05f);
            fixedForce = 0f; // reset fixed force
        }

        if (Input.GetKeyDown(KeyCode.Alpha0)){
            fixedForce = forcePropusionTop;
        }

        if (Input.GetKeyDown(KeyCode.E)){
            stabilize = !stabilize;
            recordedAltitude = this.transform.position.y;
        }

        updateMotorForces();
        this.transform.rotation = this.transform.rotation;

	}


    void updateMotorForces()
    {
        if (forcePropusionTop > 0)
            forcePropusionTop -= 0.01f;
        else
            forcePropusionTop = 0;

        if (fixedForce > forcePropusionTop)
            forcePropusionTop = fixedForce;

        if (stabilize)
        {
            stabilizeAltitude();
            /*
            if (forcePropusionTop < 8f)
                forcePropusionTop = 8f;
            else
            {
                if ((recordedAltitude - transform.position.y) < 3f && (recordedAltitude - transform.position.y) > 0f)
                {
                    forcePropusionTop = 8.15f;
                }
            }
            */
        }
        this.rigidbody.AddForceAtPosition(new Vector3(0, forcePropusionTop, 0), this.transform.position);
        particle.emissionRate = forcePropusionTop;
    }

    void addForce(float value)
    {
        if (value > maxAceleration)
        {
            Debug.Log("limited aceleration");
            value = maxAceleration;
        }

        if (value + forcePropusionTop <= maxForcePropusionTop)
        {
            Debug.Log("adding force: " + value);
            forcePropusionTop += value;
        }
    }

    void stabilizeAltitude()
    {
        if (recordedAltitude > 0.5f)
        {
            float diffYTest = recordedAltitude - this.transform.position.y;
            if (recordedAltitude > this.transform.position.y)
            {
                if (diffYTest < diffY)
                {
                    if (forcePropusionTop > 8.2f)
                    {
                        forcePropusionTop = 8.2f;
                    }
                    else
                    {
                        addForce(0.02f);
                    }
                }
                else
                    addForce(0.05f);
            }
            else
                forcePropusionTop = 8.0f;

            diffY = recordedAltitude - this.transform.position.y;
        }
        else
        {
            stabilize = false;
        }
    }
}
