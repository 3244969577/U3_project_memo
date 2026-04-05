using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : Damageable
{
    protected override void Update() 
    {
        if (this.healthBar.GetHealth() <= 0f)
        {
            this.OnDestroyed();
        } 
    }
    protected virtual void OnDestroyed() { }
    protected virtual void Destroy() { }

}
