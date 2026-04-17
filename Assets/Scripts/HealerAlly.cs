using System.Collections.Generic;
using UnityEngine;

public class HealerAlly : Ally // the clone
{

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

    }

    protected override void ActivateAbility1(Vector3 mouse_position)
    {
        HealerAbility1(mouse_position);
    }


    protected override void ActivateAbility2(Vector3 mouse_position)
    {

    }

    protected void HealerAbility1(Vector3 mouse_position)
    {
        return;
    }



    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
    }


}
