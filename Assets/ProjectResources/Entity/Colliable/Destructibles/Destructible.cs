using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : Damageable
{
    public override void Update() 
    {
        if (this.healthBar.GetHealth() <= 0f)
        {
            this.OnDestroyed();
        } 
    }
    public virtual void OnDestroyed() { }
    public virtual void Destroy() { }

}
