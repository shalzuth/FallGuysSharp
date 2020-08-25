using FGClient;
using UnityEngine;
namespace FallGuysMods
{
    public class LevelHelper : ModBase
    {
        public override void OnEnable()
        {
            var clientGameManager = GlobalGameStateClient.Instance.GameStateView.GetLiveClientGameManager();
            var tiptoes2 = GameObject.Find("Spinner");
            var tiptoes = Object.FindObjectsOfType<TipToe_Platform>();
            foreach(var tiptoe in tiptoes)
            {
                if (tiptoe.IsFakePlatform) GameObject.Destroy(tiptoe.gameObject);
            }
            var realDoors = Object.FindObjectsOfType<RealDoorController>();
            foreach(var door in realDoors)
            {
                //door.gameObject.transform.localScale = new Vector3(2, 2, 2);
                GameObject.Destroy(door.gameObject);
            }
            var players = clientGameManager._players;
            foreach(var player in players)
            {
                if (player.Value._tailTagAccessory._isAccessoryEnabled) player.Value.transform.localScale = new Vector3(3, 3, 3);
                else player.Value.transform.localScale = new Vector3(1, 1, 1);
            }
            var matchFallManager = clientGameManager.GetGameSystem<MatchFallManager>();
            if (matchFallManager != null)
            {
                foreach (var tile in matchFallManager.Tiles)
                {
                    if (tile._tileSurfaceOnObject.gameObject.activeInHierarchy) tile.gameObject.SetActive(true);
                    if (tile._tileSurfaceOffObject.gameObject.activeInHierarchy) tile.gameObject.SetActive(false);
                }
            }
        }
    }
}
