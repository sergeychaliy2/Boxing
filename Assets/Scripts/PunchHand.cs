using System.Collections;
using UnityEngine;
using UnityEngine.XR;

public class PunchHand : MonoBehaviour
{
    private Rigidbody rBody;
    [SerializeField] private XRNode handNode = XRNode.RightHand;
    [SerializeField] private AudioClip hitSound;
    private AudioSource audioSource;
    private InputDevice device;

    private IDamageable cachedTarget;
    private GameObject lastTargetObject;

    private void Start()
    {
        rBody = GetComponent<Rigidbody>();
        device = InputDevices.GetDeviceAtXRNode(handNode);
        audioSource = GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        rBody.MovePosition(transform.position);
        rBody.MoveRotation(transform.rotation);
    }

    private void OnCollisionEnter(Collision other)
    {
        var avgPoint = Vector3.zero;
        foreach (var p in other.contacts)
            avgPoint += p.point;
        avgPoint /= other.contactCount;

        var otherR = other.rigidbody;
        if (otherR != null)
        {
            var dir = (avgPoint - transform.position).normalized;
            var force = 10f * rBody.velocity.magnitude;
            otherR.AddForceAtPosition(dir * force, avgPoint);
        }

        if (BloodPoolManager.Instance != null)
            BloodPoolManager.Instance.SpawnBlood(avgPoint, Quaternion.LookRotation(-transform.forward));

        if (other.gameObject != lastTargetObject)
        {
            lastTargetObject = other.gameObject;
            cachedTarget = other.gameObject.GetComponentInParent<IDamageable>();
        }

        if (cachedTarget != null)
        {
            var hitStrength = other.relativeVelocity.magnitude * 10f;
            cachedTarget.ApplyDamage(hitStrength);
        }

        if (audioSource != null && hitSound != null)
            audioSource.PlayOneShot(hitSound);

        StartCoroutine(HapticPulse(0.1f, 0.5f));
    }

    private IEnumerator HapticPulse(float duration, float amplitude)
    {
        if (!device.isValid)
            device = InputDevices.GetDeviceAtXRNode(handNode);

        if (device.TryGetHapticCapabilities(out var capabilities) && capabilities.supportsImpulse)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                device.SendHapticImpulse(0, amplitude, 0.05f);
                elapsed += 0.05f;
                yield return new WaitForSeconds(0.05f);
            }
        }
    }
}
