using UnityEngine;
using static IPowerup;

public class CoinPowerUp : BasePowerup
{
    public AudioSource coinAudioSource;
    public AudioClip coinSpawnSound;

    protected override void Start()
    {
        base.Start();
        this.type = PowerupType.Coin;
    }

    // called by an animation event
    public override void SpawnPowerup()
    {
        spawned = true;
        coinAudioSource.PlayOneShot(coinSpawnSound);
        // score is added by the IncreaseScore animation event
    }

    // call from an animation event on the last frame
    public void HideCoin()
    {
        gameObject.SetActive(false);
    }

    public override void ApplyPowerup(MonoBehaviour i) { }
}