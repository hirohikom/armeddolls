using UnityEngine;
using System.Collections;

public class F3DSologunProjectile : MonoBehaviour
{
    public LayerMask layerMask;

    public float lifeTime = 2f;                 // Projectile life time
    public float despawnDelay = 0;                  // Delay despawn in ms
    public float velocity = 350f;               // Projectile velocity
    public float RaycastAdvance = 2f;           // Raycast advance multiplier
    public float applyForce = 15.0f;
    public float applyDamage = 15.0f;

    public Transform soloGunImpact;

    private object[] damageParam = new object[4];
    // param[0]:Vector3　発射地点
    // param[1]:Vector3　着弾地点
    // param[2]:Vector3　着弾地点の法線
    // param[3]:float　最大ダメージ量

    public bool DelayDespawn = false;           // Projectile despawn flag

    public ParticleSystem[] delayedParticles;   // Array of delayed particles
    ParticleSystem[] particles;                 // Array of projectile particles

    new Transform transform;                    // Cached transform
    
    RaycastHit hitPoint;                        // Raycast structure

    bool isHit = false;                         // Projectile hit flag
    bool isFXSpawned = false;                   // Hit FX prefab spawned flag

    float timer = 0f;                           // Projectile timer

    void Awake()
    {
        // Cache transform and get all particle systems attached
        transform = GetComponent<Transform>();
        particles = GetComponentsInChildren<ParticleSystem>();
    }

    // OnSpawned called by pool manager 
    public void OnSpawned()
    {
        // Reset flags and raycast structure
        isHit = false;
        isFXSpawned = false;
        timer = 0f;
        hitPoint = new RaycastHit();
    }

    // OnDespawned called by pool manager 
    public void OnDespawned()
    {          
    }

    // Stop attached particle systems emission and allow them to fade out before despawning
    void Delay()
    {       
        if(particles.Length > 0 && delayedParticles.Length > 0)
        {
            bool delayed;

            for (int i = 0; i < particles.Length; i++)
            {
                delayed = false;

                for (int y = 0; y < delayedParticles.Length; y++)                
                    if (particles[i] == delayedParticles[y])
                    {
                        delayed = true;
                        break;
                    }                

                particles[i].Stop(false);

                if (!delayed)
                    particles[i].Clear(false);                
            }
        }
    }

    // OnDespawned called by pool manager 
    void OnProjectileDestroy()
    {   
        F3DPool.instance.Despawn(transform);
    }
    
    // Apply hit force on impact
    void ApplyForce(float force)
    {
        if (hitPoint.rigidbody != null)
 //           hitPoint.rigidbody.AddForceAtPosition(transform.forward * force, hitPoint.point, ForceMode.VelocityChange);
            hitPoint.rigidbody.AddForceAtPosition(transform.forward * force, hitPoint.point, ForceMode.Impulse);
    }

    void ApplyDamage(float damage)
    {
        if (hitPoint.collider != null)
        {
            damageParam[0] = transform.position;
            damageParam[1] = hitPoint.transform.position;
            damageParam[2] = hitPoint.normal;
            damageParam[3] = damage;
            hitPoint.collider.gameObject.SendMessage("OnDamage", damageParam, SendMessageOptions.DontRequireReceiver);
        }
    }

    void AppearForceField(float force)
    {
        Forcefield forcefield = hitPoint.transform.gameObject.GetComponent<Forcefield>();
        if (forcefield != null)
        {
            float hitPower = force * 0.01f;// Random.Range(-7.0f, 1.0f);
            forcefield.OnHit(hitPoint.point, hitPower);
        }
    }

    // Spawn sologun weapon impact
    public void SoloGunImpact(Vector3 pos)
    {
        F3DPool.instance.Spawn(soloGunImpact, pos, Quaternion.identity, null);
        F3DAudioController.instance.SoloGunHit(pos);
    }

    void Update()
    {
        // If something was hit
        if (isHit)
        {
            // Execute once
            if (!isFXSpawned)
            {
                // Invoke corresponding method that spawns FX
                SoloGunImpact(hitPoint.point + hitPoint.normal * 0.2f);
                AppearForceField(applyForce);
                ApplyForce(applyForce);
                ApplyDamage(applyDamage);
                isFXSpawned = true;
            }
            
            // Despawn current projectile 
            if (!DelayDespawn || (DelayDespawn && (timer >= despawnDelay)))
                OnProjectileDestroy();
        }

        // No collision occurred yet
        else
        {
            // Projectile step per frame based on velocity and time
            Vector3 step = transform.forward * Time.deltaTime * velocity;

            // Raycast for targets with ray length based on frame step by ray cast advance multiplier
            if (Physics.Raycast(transform.position, transform.forward, out hitPoint, step.magnitude * RaycastAdvance, layerMask))
            {
                isHit = true;

                // Invoke delay routine if required
                if (DelayDespawn)
                {
                    // Reset projectile timer and let particles systems stop emitting and fade out correctly
                    timer = 0f;                    
                    Delay();
                }
            }
            // Nothing hit
            else
            {
                // Projectile despawn after run out of time
                if (timer >= lifeTime)
                    OnProjectileDestroy();
            }

            // Advances projectile forward
            transform.position += step;
        }

        // Updates projectile timer
        timer += Time.deltaTime;
    }
}
