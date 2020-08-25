using FGClient;
using UnityEngine;
namespace FallGuysMods
{
    public class FreezeMe : ModBase
    {
        Vector3 FreezePos = Vector3.zero;
        GameObject cube;
        public override void OnEnable()
        {
            var clientGameManager = GlobalGameStateClient.Instance.GameStateView.GetLiveClientGameManager();
            var players = clientGameManager._localPlayers;
            foreach (var player in players) FreezePos = player.Value.transform.position;
            cube = GameObject.CreatePrimitive(PrimitiveType.Plane);
            cube.transform.position = FreezePos;
            cube.transform.position -= 2 * Vector3.up;
        }
        public override void Update()
        {
            var clientGameManager = GlobalGameStateClient.Instance.GameStateView.GetLiveClientGameManager();
            var players = clientGameManager._localPlayers;
            foreach (var player in players) player.Value.transform.position = FreezePos;
        }
    }
}
