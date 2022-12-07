using Enumerations;
using Interactables;
using UnityEngine;
using UnityEngine.UI;

namespace BuildingSystem
{
    public class WithdrawButton : MonoBehaviour
    {
        void Awake()
        {
            GetComponent<Button>().onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            GameAudio.PlaySoundEvent.Invoke(SoundType.Withdraw);
            SelectionManager.WITHDRAW_OBJECT_EVENT.Invoke();
        }
    }
}
