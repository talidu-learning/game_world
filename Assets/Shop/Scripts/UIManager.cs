using System.Collections;
using ServerConnection;
using UnityEngine;

namespace Shop
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField]private Animation LoadingAnimation;
    
        void Start()
        {
            SaveGame.LoadedPlayerData.AddListener(()=> StartCoroutine(OnLoadedGame()));
        }

        private IEnumerator OnLoadedGame()
        {
            LoadingAnimation.Play();

            yield return null;

            yield return new WaitWhile(() => LoadingAnimation.isPlaying);
        
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
        
            LoadingAnimation.gameObject.SetActive(false);
        }
    }
}
