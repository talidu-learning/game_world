using System.Collections;
using UnityEngine;

public class HighlightParticlesManager : MonoBehaviour
{
    [SerializeField] private GameObject ParticlesgameObject;
    private ParticleSystem particleSystem;

    private Coroutine playParticles;

    private void Awake()
    {
        particleSystem = ParticlesgameObject.GetComponent<ParticleSystem>();
    }

    public void MoveParticleSystem(GameObject item)
    {
        particleSystem.transform.SetParent(item.transform, false);
        playParticles = StartCoroutine(PlayParticleSystem());
    }

    public void StopParticleSystem()
    {
        particleSystem.Stop();
        particleSystem.Clear();
        ParticlesgameObject.transform.SetParent(transform, false);
    }

    private IEnumerator PlayParticleSystem()
    {
        particleSystem.Play();
        yield return new WaitForSeconds(2);
        StopParticleSystem();
    }
}
