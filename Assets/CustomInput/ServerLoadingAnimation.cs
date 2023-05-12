using UnityEngine;
using UnityEngine.Events;

namespace CustomInput
{
    public class ServerLoadingAnimation : MonoBehaviour
    {
        public static readonly UnityEvent ENABLE_ANIMATION = new UnityEvent();
        public static readonly UnityEvent DISABLE_ANIMATION = new UnityEvent();
        [SerializeField] private GameObject AnimationObject;
        void Start()
        {
            ENABLE_ANIMATION.AddListener(OnInvoke);
            DISABLE_ANIMATION.AddListener(OnInvokeEnd);
        }

        private void OnInvokeEnd()
        {
            AnimationObject.SetActive(false);
        }

        private void OnInvoke()
        {
            AnimationObject.SetActive(true);
        }
    }
}
