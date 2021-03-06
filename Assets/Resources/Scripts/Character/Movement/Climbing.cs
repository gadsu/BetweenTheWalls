using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climbing : CharacterStates {
    private readonly CharacterMovement player;

    private enum ClimbingDir { up = 1, down = -1, idle = 0, jump = 2 };
    private ClimbingDir climbing;

    public Climbing( CharacterMovement cMovement ) {
        player = cMovement;
    }

    public void Update( ) {
        // Get integer value for direction character is moving
        //player.directions.GetDirection( );
        player.GetComponent<Animator>().SetBool("Climbing", true);

        // Get input for climbing up or down the wall
        if ( player.controller.Up ) {
            climbing = ClimbingDir.up;
            GetConstraints( );
        } else if ( player.controller.Down ) {
            climbing = ClimbingDir.down;
            GetConstraints( );
        } else {
            GetConstraints( );
            climbing = ClimbingDir.idle;
        }

        if ( player.controller.Jump ) {
            climbing = ClimbingDir.jump;
            GetConstraints( );
            player.gameObject.GetComponent<Rigidbody>( ).useGravity = true;
            // can't get player to jump or move away from wall
            PositionStates.Direction dir;
            if ( player.coll.RightCollided( ) )
                dir = PositionStates.Direction.left;
            else
                dir = PositionStates.Direction.right;
            player.SetHorizontalMovement( dir );
            SwitchToPlayerMovement( );
        }
    }

    public void FixedUpdate( ) {
        // Ascending
        if ( climbing == ClimbingDir.up ) {
            player.GetComponent<Rigidbody>( ).velocity = new Vector3( 0, player.climbSpeed, 0 );
        }
        // Descending
        else if ( climbing == ClimbingDir.down ) {
            player.GetComponent<Rigidbody>( ).velocity = new Vector3( 0, -player.climbSpeed, 0 );
        }

        // Horizontal movement
        //player.SetHorizontalMovement( player.directions.currDirection );
    }

    public void OnTriggerExit( Collider other ) {
        SwitchToPlayerMovement( );
    }

    private void GetConstraints( ) {
        player.GetConstraints( );
        if ( climbing == ClimbingDir.idle ) {
            player.GetComponent<Rigidbody>( ).constraints |= RigidbodyConstraints.FreezePositionY;

        }
    }

    public void SwitchToPlayerMovement( ) {
        player.GetComponent<Animator>().SetBool("Climbing", false);
        if ( climbing == ClimbingDir.jump && !player.grav.IsGrounded( player.groundCheck ) ) {
            player.GetComponent<Rigidbody>( ).AddForce
                 ( new Vector3( 0f, player.jumpSpeed, 0f ), ForceMode.VelocityChange );
        }
        PositionStates.GetConstraints( player.gameObject, player.currentRotation );
        player.gameObject.GetComponent<Rigidbody>( ).useGravity = true;
        player.currentState = player.playerInput;
    }
    
    public void OnTriggerEnter( Collider other ) { }
    public void SwitchToPlayerClimb( ) { }
    public void OnTriggerStay( Collider other ) { }
}
