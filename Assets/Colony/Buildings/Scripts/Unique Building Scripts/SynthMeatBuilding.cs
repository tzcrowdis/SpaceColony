using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynthMeatBuilding : Building
{
    [Header("Meat Metrics")]
    public GameObject meat;
    public float[] bigRange;
    public float[] smallRange;
    public float speed;
    float smallSize;
    float bigSize;
    Vector3 meatDelta;
    bool grow;
    
    protected override void Start()
    {
        base.Start();

        grow = true;
        smallSize = Random.Range(smallRange[0], smallRange[1]);
        bigSize = Random.Range(bigRange[0], bigRange[1]);
        meatDelta = new Vector3(0, bigSize, 0);
    }

    protected override void Update()
    {
        base.Update();

        if (state == State.Operating && BuildingEfficiency() > 0)
            PulseMeat();
    }

    void PulseMeat()
    {
        // pulse
        meat.transform.localScale += speed * meatDelta * Time.deltaTime;

        // end condition
        if (grow)
        {
            if (meat.transform.localScale.y >= bigSize)
            {
                smallSize = Random.Range(smallRange[0], smallRange[1]);
                meatDelta = new Vector3(0, smallSize, 0);
                speed *= -1;
                grow = false;
            }
        }
        else
        {
            if (meat.transform.localScale.y <= smallSize)
            {
                bigSize = Random.Range(bigRange[0], bigRange[1]);
                meatDelta = new Vector3(0, bigSize, 0);
                speed *= -1;
                grow = true;
            }
        }
    }
}
