using System;
using System.Collections;
using System.Collections.Generic;
using GameStatusSystem.PlayerStatus;
using UnityEngine;

[RequireComponent(typeof(Shootable))]
public class Gun : Weapon
{
    public Shootable shooter;
    

#region Weapon Override Methods
    protected override void UpdatePosition(Vector3 mousePosition)
    {
        this.UpdateFirePointWithMousePosition(mousePosition);
    }

    protected override void Attack(Vector3 targetPosition)
    {
        this.shooter.ShootBullet(targetPosition);
    }
#endregion


    private void UpdateFirePointWithMousePosition(Vector3 mousePosition)
    {
        Vector3 lookDirection = mousePosition - transform.position;

        float magnitue = lookDirection.magnitude;

        if (magnitue >= 1.5)
        {
            float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
            var rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            this.transform.rotation = rotation;
        }


        if (lookDirection.x > 0)
        {
            this.transform.eulerAngles = new Vector3(
                this.transform.eulerAngles.x,
                this.transform.eulerAngles.y,
                this.transform.eulerAngles.z
            );
        }
        else
        {
            this.transform.eulerAngles = new Vector3(
                this.transform.eulerAngles.x - 180,
                this.transform.eulerAngles.y,
                - this.transform.eulerAngles.z
            );
        }

    }

    
}