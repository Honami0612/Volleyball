using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour {

    public float startSpeed;
    public float hitSpeedMultiplier;
    [HideInInspector]
    public float speed;
    [HideInInspector]
    public Vector3 direction;
    [HideInInspector]
    public int lastHitter = -1;
    [HideInInspector]
    public Vector3 position;

    //more behavoir types of the ball can be added here
    public enum BallState
    {
        SERVE,
        NORMAL,
        HITSTUN,
        STANDBY
    }
    [HideInInspector]
    public BallState ballState = BallState.SERVE;
    private Vector3 startPosition;

    public static BallScript ball;

    void Start () {
        ball = this;
        speed = startSpeed;
        position = startPosition = transform.position;
	}

    //updating the state of the ball
    void Update() {

        //NORMAL is the state where the ball moves
        if (ballState == BallState.NORMAL)
        {
            position += direction * speed;
        }

        //if the ball is not dead, we have to make sure it stays in the stage
        if (ballState != BallState.STANDBY)
        {
            if (position.x < -9)
            {
                HitWall(true);
                position.x = -9;
            }
            if (position.x > 9)
            {
                HitWall(true);
                position.x = 9;
            }
            if (position.y < -5)
            {
                HitWall(false);
                position.y = -5;
            }
            if (position.y > 5)
            {
                HitWall(false);
                position.y = 5;
            }
        }

        transform.position = position;
    }

    //the ball gets hit
    public void GetHit(PlayerScript byPlayer, float hitPause)
    {
        //you can set the starting speed in the editor
        //we can play around with this of course
        //try speed *= 1.1f instead
        //speed = 0.3f;
        speed *= hitSpeedMultiplier;
        lastHitter = byPlayer.playerNum;

        //default direction set to which player it is
        if (byPlayer.playerNum == 0) direction = Vector2.right;
        else direction = Vector2.left;
        StartCoroutine(GetHitCoroutine(hitPause));
    }

    public void HitWall(bool vertical)
    {
        if (vertical) direction.x *= -1;
        else direction.y *= -1;
        lastHitter = -1;
    }

    public IEnumerator GetHitCoroutine(float hitPause)
    {
        ballState = BallState.HITSTUN;

        yield return new WaitForSeconds(hitPause);

        Camera.main.GetComponent<LethalCamera>().Shake();

        ballState = BallState.NORMAL;
    }

    public IEnumerator HitPlayerCoroutine(PlayerScript player)
    {
        Camera.main.GetComponent<LethalCamera>().Shake();

        ballState = BallState.STANDBY;
        position = 1000 * Vector3.down;
        speed = startSpeed;

        yield return new WaitForSeconds(2.4f);

        lastHitter = -1;
        ballState = BallState.SERVE;
        position = player.position + Vector3.right*(player.playerNum == 0 ? 1.5f : -1.5f);
    }
}
