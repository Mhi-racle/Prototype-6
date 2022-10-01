using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public Rigidbody rb;
    public float speed = 15f;


    public bool isMoving;
    private Vector3 travelDirection;
    private Vector3 nextCollisionPosition;

    public int minSwipeRecognition = 500;
    private Vector2 swipePositionLastFrame;
    private Vector2 swipePositionCurrentFrame;
    private Vector2 currentSwipe;

    public Color solveColor;



    void Start()
    {
        solveColor = Random.ColorHSV(0.5f, 1);
        GetComponent<MeshRenderer>().material.color = solveColor;
    }


    private void FixedUpdate()
    {
        if (isMoving)
        {
            rb.velocity = speed * travelDirection;
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position - (Vector3.up / 2), 0.3f); //creating a small ball underneath the ball and store what it hits in the hitColliders
        int i = 0;
        while (i < hitColliders.Length)
        {
            GroundPiece ground = hitColliders[i].transform.GetComponent<GroundPiece>(); //checks if it hit a ground piece and store it
            // checks if it actually hit a ground and if it isnt colored
            if (ground && !ground.isColored)
            {
                ground.ChangeColor(solveColor);
            }
            i++;
        }

        if (nextCollisionPosition != Vector3.zero) //this checks if we are about to hit something using the rayCast in SetDestination()
        {
            if (Vector3.Distance(transform.position, nextCollisionPosition) < 1)
            {
                isMoving = false;
                travelDirection = Vector3.zero;
                nextCollisionPosition = Vector3.zero;
            }
        }

        if (isMoving)
        {
            return;
        }

        if (Input.GetMouseButton(0))
        {
            swipePositionCurrentFrame = new Vector2(Input.mousePosition.x, Input.mousePosition.y); //gets the x and y position of the touch

            if (swipePositionLastFrame != Vector2.zero)
            {
                currentSwipe = swipePositionCurrentFrame - swipePositionLastFrame; //checks where your finger is currently in comparison to where it was in the previous frame
                if (currentSwipe.sqrMagnitude < minSwipeRecognition) //creates a square root of the current swipe and then compares it to the min swipe..... if it is below it, we assume it was not a swipe but a mistake
                {
                    return;
                }

                currentSwipe.Normalize(); //just gives the direction not the distance

                //Up / Down
                if (currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
                {
                    //Go up/Down
                    SetDestination(currentSwipe.y > 0 ? Vector3.forward : Vector3.back); //if currentSwipe.y is greater than 0, it goes up hence Vector3.up, if it is less than 0, it goes down
                }

                // Left/Right
                if (currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
                {
                    //Go Left/right
                    SetDestination(currentSwipe.x > 0 ? Vector3.right : Vector3.left); //if currentSwipe.x is greater than 0, it goes right hence Vector3.right, if it is less than 0, it goes to the left
                }


            }

            swipePositionLastFrame = swipePositionCurrentFrame; //in the next frame, the lastPos would be == to the current pose in the current frame
        }

        if (Input.GetMouseButtonUp(0))
        {
            swipePositionLastFrame = Vector2.zero;
            currentSwipe = Vector2.zero;
        } // when we release the screen
    }


    private void SetDestination(Vector3 direction)
    {
        travelDirection = direction;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, 100f))
        {
            nextCollisionPosition = hit.point; //makes the nextCollision position the poisiton of an obstacle ahead
        }

        isMoving = true;
    }

}
