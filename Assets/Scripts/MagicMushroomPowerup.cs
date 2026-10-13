
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IPowerup;

public class MagicMushroomPowerup : BasePowerup
{
    // setup this object's type
    // instantiate variables

    // Audio
    public AudioSource mushroomAudioSource;
    public AudioClip mushroomSpawnSound;
    public AudioClip mushroomEatSound;

    protected override void Start()
    {
        base.Start(); // call base class Start()
        this.type = PowerupType.MagicMushroom;
    }



    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player") && spawned)
        {
            // TODO: do something when colliding with Player

            // hide the mushroom and play sound, then destroy after sound is done
            mushroomAudioSource.PlayOneShot(mushroomEatSound);
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;
            Destroy(gameObject, mushroomEatSound.length);

        }
        else if (col.gameObject.layer == 10) // else if hitting Pipe, flip travel direction
        {
            if (spawned)
            {
                goRight = !goRight;
                rigidBody.AddForce(Vector2.right * 3 * (goRight ? 1 : -1), ForceMode2D.Impulse);

            }
        }
    }

    // interface implementation
    public override void SpawnPowerup()
    {
        Debug.Log("Magic Mushroom spawned!");
        spawned = true;

        mushroomAudioSource.PlayOneShot(mushroomSpawnSound);

        GetComponent<Animator>().enabled = false;   // animation is done, hand over to physics
        GetComponent<Collider2D>().enabled = true;
        rigidBody.linearVelocity = new Vector2(5f, 0f); // mushroom starts moving after popping out
    
    }


    // interface implementation
    public override void ApplyPowerup(MonoBehaviour i)
    {
        // TODO: do something with the object

    }
}