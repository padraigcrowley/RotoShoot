
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Inteface defining messages sent by the player
/// </summary>
public interface IPlayerEvents : IEventSystemHandler
{
    void OnPlayerHitpointsChanged (int oldHealth, int newHealth);

    void OnPlayerReachedExit (GameObject exit);
}