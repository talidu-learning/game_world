using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace CatBehavior
{
    public enum CatState
    {
        Default,
        Idle,
        Sit,
        Stretch,
        Walk
    }

    public class CatWalkingBehavior : MonoBehaviour
    {
        private Animator animator;
        private NavMeshAgent agent;

        private CatState currentState = CatState.Default;
        
        // Start is called before the first frame update
        void Start()
        {
            animator = GetComponent<Animator>();
        
            agent = GetComponent<NavMeshAgent>();
            agent.updateRotation = false;
            StartNextBehavior();
        }

        private void StartNextBehavior()
        {
            var value = Random.Range(0.0f, 1.0f);
            switch (value)
            {
                case < 0.3f:
                    SitDown();
                    break;
                case < 0.6f:
                    Move(new Vector3(Random.Range(-55, 55), transform.position.y, Random.Range(-55, 55)));
                    break;
                case < 0.9f:
                    StartCoroutine(Idle());
                    break;
                default:
                    Stretch();
                    break;
            }
        }

        // Idle
        private IEnumerator Idle()
        {
            currentState = CatState.Idle;
            yield return new WaitForSeconds(Random.Range(2, 8));
            StartNextBehavior();
        }

        //Sit Down
        private void SitDown()
        {
            currentState = CatState.Sit;
            animator.SetBool("SitDown", true);
            StartCoroutine(SitForAWhile());
        }

        private IEnumerator SitForAWhile()
        {
            yield return new WaitForSeconds(Random.Range(2, 8));
            animator.SetBool("SitDown", false);
            StartCoroutine(Idle());
        }

        // Stretch
        private void Stretch()
        {
            currentState = CatState.Stretch;
            animator.SetTrigger("Stretch");
            StartCoroutine(Idle());
        }
    
        // Walk
        private void Move(Vector3 destination)
        {
            animator.SetBool("IsWalking", true);
            if(destination.x > transform.position.x) transform.Rotate(Vector3.up, 180);
            else transform.rotation = new Quaternion(0,0,0,0);
            agent.SetDestination(destination);
            StartCoroutine(WaitForReachingDestination());
        }

        private IEnumerator WaitForReachingDestination()
        {
            yield return new WaitWhile(()=>agent.hasPath);
            animator.SetBool("IsWalking", false);
            yield return new WaitForSeconds(Random.Range(2, 8));
            StartCoroutine(Idle());
        }
    }
}