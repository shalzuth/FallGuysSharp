using FGClient;
using UnityEngine;
namespace FallGuysMods
{
    public class AvoidDataCheck : ModBase
    {
        public override void Update()
        {
            var clientGameManager = GlobalGameStateClient.Instance.GameStateView.GetLiveClientGameManager();
            clientGameManager._characterDataMonitor._timeToRunNextCharacterControllerDataCheck = 3;
        }
    }
}
