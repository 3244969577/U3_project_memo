using UnityEngine;

public class BulletHitPartical : MonoBehaviour {
     public GameObject particleSystem;
     private Bullet bullet;

     private void Awake()
     {
        bullet = GetComponent<Bullet>();
        bullet.OnBulletHit += HandleBulletHit;
     }

     private void HandleBulletHit(GameObject target)
     {
        Debug.Log($"Bullet hit {target.name}");
        GameObject ps = Instantiate(particleSystem, target.transform.position, target.transform.rotation);
        Destroy(ps, 0.5f);
     }
}