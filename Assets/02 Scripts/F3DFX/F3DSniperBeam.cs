using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class F3DSniperBeam : MonoBehaviour
{
    public LayerMask layerMask;

    public bool OneShot;            // Constant or single beam?
    
    public Texture[] BeamFrames;    // Animation frame sequence
    public float FrameStep;         // Animation time

    public float beamScale;         // Default beam scale to be kept over distance
    public float MaxBeamLength;     // Maximum beam length

    public bool AnimateUV;          // UV Animation
    public float UVTime;            // UV Animation speed
        
    public Transform rayImpact;     // Impact transform
    public Transform rayMuzzle;     // Muzzle flash transform

    LineRenderer lineRenderer;      // Line rendered component
    RaycastHit hitPoint;            // Raycast structure

    int frameNo;                    // Frame counter
    int FrameTimerID;               // Frame timer reference
    float beamLength;               // Current beam length
    float initialBeamOffset;        // Initial UV offset 

    public float applyForce = 4.0f;
    public float applyDamage = 4.0f;

    private object[] damageParam = new object[4];
    // param[0]:Vector3　発射地点
    // param[1]:Vector3　着弾地点
    // param[2]:Vector3　着弾地点の法線
    // param[3]:float　最大ダメージ量

    void Awake()
    {
        // Get line renderer component
        lineRenderer = GetComponent<LineRenderer>();

        // Assign first frame texture
        if (!AnimateUV && BeamFrames.Length > 0)
            lineRenderer.material.mainTexture = BeamFrames[0];

        // Randomize uv offset
        initialBeamOffset = Random.Range(0f, 5f);
    }
    
    // OnSpawned called by pool manager 
    void OnSpawned()
    {
        Raycast();

        // Start animation sequence if beam frames array has more than 2 elements
        if (BeamFrames.Length > 1)
            Animate();

        AppearForceField(applyForce);
        ApplyForce(applyForce);
        ApplyDamage(applyDamage);
    }

    // OnDespawned called by pool manager 
    void OnDespawned()
    {
        // Reset frame counter
        frameNo = 0;

        // Clear timer
        if (FrameTimerID != -1)
        {
            F3DTime.time.RemoveTimer(FrameTimerID);
            FrameTimerID = -1;
        }
    }

    // Hit point calculation
    void Raycast()
    {
        // Prepare structure and create ray
        hitPoint = new RaycastHit();
        Ray ray = new Ray(transform.position, transform.forward);

        // Calculate default beam proportion multiplier based on default scale and maximum length
        float propMult = MaxBeamLength * (beamScale / 10f);

        // Raycast
        if (Physics.Raycast(ray, out hitPoint, MaxBeamLength, layerMask))
        {
            // Get current beam length and update line renderer accordingly
            beamLength = Vector3.Distance(transform.position, hitPoint.point);
            lineRenderer.SetPosition(1, new Vector3(0f, 0f, beamLength));

            // Calculate default beam proportion multiplier based on default scale and current length
            propMult = beamLength * (beamScale / 10f);

            SniperImpact(hitPoint.point + hitPoint.normal * 0.2f);
            ApplyForce(applyForce);
        }
        // Nothing was his
        else
        {
            // Set beam to maximum length
            beamLength = MaxBeamLength;
            lineRenderer.SetPosition(1, new Vector3(0f, 0f, beamLength));
        }

        // Adjust muzzle position
        if(rayMuzzle)
            rayMuzzle.position = transform.position + transform.forward * 0.1f;

        // Set beam scaling according to its length
        lineRenderer.material.SetTextureScale("_MainTex", new Vector2(propMult, 1f));
    }   

    // Advance texture frame
    void OnFrameStep()
    {
        // Set current texture frame based on frame counter
        lineRenderer.material.mainTexture = BeamFrames[frameNo];
        frameNo++;

        // Reset frame counter
        if (frameNo == BeamFrames.Length)
            frameNo = 0;
    }

    // Initialize frame animation
    void Animate()
    {
        if (BeamFrames.Length > 1)
        {
            // Set current frame
            frameNo = 0;
            lineRenderer.material.mainTexture = BeamFrames[frameNo];

            // Add timer 
            FrameTimerID = F3DTime.time.AddTimer(FrameStep, BeamFrames.Length - 1, OnFrameStep);

            frameNo = 1;
        }
    }

    // Apply force to last hit object
    void ApplyForce(float force)
    {
        if (hitPoint.rigidbody != null)
            hitPoint.rigidbody.AddForceAtPosition(transform.forward * force, hitPoint.point, ForceMode.VelocityChange);
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

    void Update()
    {
        // Animate texture UV
        if (AnimateUV)        
            lineRenderer.material.SetTextureOffset("_MainTex", new Vector2(Time.time * UVTime + initialBeamOffset, 0f));
    }

    // Spawn sniper weapon impact
    void SniperImpact(Vector3 pos)
    {
        F3DPool.instance.Spawn(rayImpact, pos, Quaternion.identity, null);
        F3DAudioController.instance.SniperHit(pos);
    }
}
