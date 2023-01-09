using Enumerations;
using Interactables;
using UnityEngine;
using UnityEngine.UI;

namespace BuildingSystem
{
    public class DeleteItemButton : MonoBehaviour
    {
        void Awake()
        {
            GetComponent<Button>().onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            GameAudio.PlaySoundEvent.Invoke(SoundType.Delete);
            SelectionManager.DELETE_OBJECT_EVENT.Invoke();
        }
    }
}
