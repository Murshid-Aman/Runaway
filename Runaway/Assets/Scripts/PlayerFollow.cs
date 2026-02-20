using UnityEngine;

public class PlayerFollow : MonoBehaviour
{

    public Transform player;
    public Transform ground;
    public float camdist;

    // Update is called once per frame
    void Update()
    {
        // Ensure that both the player and the object to move are not null
        if (player != null && transform != null)
        {
            // Update the x position of the object to match the x position of the player
            Vector3 newPosition = transform.position;
            Vector3 groundPos = ground.position;
            newPosition.x = player.position.x + camdist;
            groundPos.x = player.position.x;
            transform.position = newPosition;
            ground.position= groundPos;
        }
    }
}
