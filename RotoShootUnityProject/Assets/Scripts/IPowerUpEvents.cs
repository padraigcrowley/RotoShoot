using UnityEngine.EventSystems;

/// <summary>
/// Interface defining messages sent by the pickups
/// </summary>
public interface IPowerUpEvents : IEventSystemHandler
{
  void OnPowerUpCollected(PowerUp powerup, PlayerShip player);

  void OnPowerUpExpired(PowerUp powerup, PlayerShip player);
}