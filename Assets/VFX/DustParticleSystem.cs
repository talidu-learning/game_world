using UnityEngine;
using UnityEngine.Events;

public class Vector3UnityEvent : UnityEvent<Vector3>{}

public class DustParticleSystem : MonoBehaviour
{
    public static Vector3UnityEvent PlayDustPartclesEvent = new Vector3UnityEvent();
    void Start()
    {
        PlayDustPartclesEvent.AddListener(OnPlayParticles);
    }

    private void OnPlayParticles(Vector3 dustPosition)
    {
        transform.position = dustPosition;
        GetComponent<ParticleSystem>().Play();
    }
}
