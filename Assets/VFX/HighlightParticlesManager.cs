using System.Collections;
using UnityEngine;

namespace VFX
{
    public class HighlightParticlesManager : MonoBehaviour
    {
        [SerializeField] private GameObject ParticlesgameObject;
        private ParticleSystem highlightParticleSystemComp;

       // private Coroutine playParticles;

        private void Awake()
        {
            highlightParticleSystemComp = ParticlesgameObject.GetComponent<ParticleSystem>();
        }

        public void MoveParticleSystem(GameObject item)
        {
            highlightParticleSystemComp.transform.SetParent(item.transform, false);
            //   playParticles = StartCoroutine(PlayParticleSystem());
            PlayParticleSystem();
        }

        public void StopParticleSystem()
        {
            highlightParticleSystemComp.Stop();
            highlightParticleSystemComp.Clear();
            ParticlesgameObject.transform.SetParent(transform, false);
        }

     /*   private IEnumerator PlayParticleSystem()
        {
            highlightParticleSystemComp.Play();
            yield return new WaitForSeconds(2);
            StopParticleSystem();
        }
     */

        private void PlayParticleSystem()
        {
            highlightParticleSystemComp.Play();
        }
    }
}
