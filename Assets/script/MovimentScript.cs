using UnityEngine;
using System.Collections;
using System;

public class MovimentScript : MonoBehaviour {

    public float forcePropusionTop; // Força principal da turbina
    public float forceBalance; // Força de equilibrio para manter parado
    public float maxForcePropusionTop; // Força máxima permitida pela turbina
    public float maxAceleration; // Aceleração máxima permitida
    //public float minPower; // Força mínima
    public float fixedForce; // Força fixada manualmente
    public float recordedAltitude; // Altitude registrada para estabilizar
    public bool stabilize; // Controle de estabilidade
    public float lastDiffY;
    public float diffY; // Diferença de altura entre a altura registrada e a atual

    public ParticleSystem particle;
    
    bool testing = true;

	// Use this for initialization
	void Start () {
        forcePropusionTop = 0;
        //rigidbody.freezeRotation = true;

        Debug.Log("Wait - Calibrating");
        StartCoroutine(testingMotor());
	}

    IEnumerator testingMotor()
    {
        float diff = this.transform.position.y;
        float thisAltitude = this.transform.position.y;
        forcePropusionTop = 9f;
        while(testing){

            thisAltitude = this.transform.position.y;
            if (thisAltitude < 0.5f)
            {
                addForce(0.1f);
            }
            else
            {
                Debug.Log(diff);
                if (diff < (thisAltitude - 0.6f) || thisAltitude > 0.6f)
                {
                    forcePropusionTop -= 0.05f;
                }
                else{
                    if (diff < 0.2f)
                    {
                        Debug.Log("Motor ready");
                        forceBalance = forcePropusionTop;
                        testing = false;
                        forcePropusionTop = 0;
                    }

                }
            }
            diff = thisAltitude - 0.6f;
            yield return new WaitForSeconds(0.5f);
        }
    }


	// Update is called once per frame
	void FixedUpdate() {
        if (!testing)
        {

            if (Input.GetKey(KeyCode.Space))
            {
                addForce(0.1f);
                fixedForce = 0f; // reset fixed force
            }

            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                fixedForce = forcePropusionTop;
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                stabilize = !stabilize;
                recordedAltitude = this.transform.position.y;
            }            
        }
        updateMotorForces();
        //this.transform.rotation = this.transform.rotation;
        lastDiffY = diffY;
	}

    void addForce(float value = 0)
    {
        if (value > 0.0f)
        {
            if (value > maxAceleration)
            {
                Debug.Log("limited aceleration");
                value = maxAceleration;
            }

            if (value + forcePropusionTop <= maxForcePropusionTop)
            {
                forcePropusionTop += value;
            }
        }
       
    }

    void updateMotorForces()
    {
        if (!testing && !stabilize)
        {
            if (forcePropusionTop > 0)
            {
                forcePropusionTop -= 0.05f;
                //Debug.Log("losing force: " + forcePropusionTop);
            }
            else
                forcePropusionTop = 0;
        }

        if (fixedForce > forcePropusionTop)
            forcePropusionTop = fixedForce;

        if (stabilize)
        {
            stabilizeAltitude();
        }

        this.rigidbody.AddForceAtPosition(new Vector3(0, forcePropusionTop, 0), this.transform.position);
        particle.emissionRate = forcePropusionTop;
    }


    void stabilizeAltitude()
    {
        if (recordedAltitude > 0.5f)
        {
            diffY = recordedAltitude - this.transform.position.y;
            //Debug.Log((lastDiffY - diffY));

            // Valores positivos == está caindo (acima da linha )
            // Valorse negativos == está subindo (abaixo da linha)


            if ((lastDiffY - diffY) > 0)
            { // caindo (controlando a força mínima) 
                Debug.Log("necessary DOWN");
                if ((lastDiffY - diffY) < -0.3)
                {
                    forcePropusionTop = forceBalance;
                    //  Debug.Log("Subindo normalmente " + forcePropusionTop);
                }
                else
                {
                    forcePropusionTop = forceBalance - (lastDiffY - diffY);
                    // Debug.Log("Subindo rápido demais " + forcePropusionTop);
                }
            }
            else    // Subindo (controlando força de propulsão)
            {
                Debug.Log("necessary UP");
                if ((lastDiffY - diffY) > 0.3)
                {
                    addForce(diffY * 0.1f);
                    // Debug.Log("Caindo e usando força calculada " + diffY * 0.1f);
                }
                else
                {
                    addForce((lastDiffY - diffY) * 0.1f);
                    // Debug.Log("Caindo Rápido demais " + (lastDiffY - diffY) * 0.1f);
                }
                
            }
            
        }
        else
        {
            stabilize = false;
            Debug.Log("Auto off stabilize - minimun altitude ");
        }
    }
}
