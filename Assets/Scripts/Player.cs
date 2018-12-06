using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Player : Character
{

    #region Components
    private Rigidbody2D _rb;
    private Controller2D _controller;
    #endregion

    #region Structs
    private GhostJump _ghostJump;
    public static Timer IdleTimer;
    #endregion

    #region Primitives
    private bool _grounded, _leaping, _nearLadder, _canClimbDown;
    private float _vertical, _horizontal, _xforce, _yforce, _centerOfLadder;
    #endregion

    //Deferred physics queue for actions such as jumping and other applied forces. Basically, if I want certain rigidbody physics operation to happen in FixedUpdate,
    //it's sometimes necessary to add them to a queue and then dequeue them in FixedUpdate.
    private Queue<Action> _force = new Queue<Action>();

    public override void Start()
    {
        base.Start();
        _rb = GetComponent<Rigidbody2D>();
        _controller = GetComponent<Controller2D>();

        IdleTimer = new Timer(20f);
    }

    private void FixedUpdate()
    {
        _horizontal = Input.GetAxisRaw("Horizontal");
        _vertical = Input.GetAxisRaw("Vertical");

        _horizontal = SetDeadZone(_horizontal, .8f);
        _vertical = SetDeadZone(_vertical, .8f);

        //If the player is airboune, we lower his speed so that he can't jump too far across the screen. You can also lerp these values for a smoother transition.
        float speed = _grounded ? 5f : 2.8f;

        float climbSpeed = 3f;

        SetGhostJump();
        CheckAltitude();
        HorizontalMovement();
        VerticalMovement();
        CheckLookingDirection();

        if (CurrentState == State.CLIMB)
        {
            if (_nearLadder)
                _rb.velocity = new Vector2(_horizontal * speed, _vertical * climbSpeed);
            else
            {
                _rb.velocity = new Vector2(_horizontal * speed, (_vertical > 0f) ? 0f : _vertical * climbSpeed);
            }
        }
        else if (CurrentState == State.MoveTowardsLadder)
        {
            Vector2 ladder = new Vector2(_centerOfLadder, transform.position.y);
            transform.position = Vector2.MoveTowards(transform.position, ladder, speed * Time.fixedDeltaTime);
        }
        else if (!_leaping)
        {
            _rb.velocity = new Vector2(_horizontal * speed, _rb.velocity.y);
        }

        while (_force.Count > 0)
            _force.Dequeue().Invoke();
    }

    public override void Update()
    {
        base.Update();

        //Prevents the player from jumping when the user hits the spacebar to resume the game.
        if (GameManager.Instance.GameIsPaused)
            return;
        //Prevents the player from awkwardly landing on the platforms while climbing.
        _grounded = (CurrentState == State.CLIMB && _canClimbDown) ? false : _controller.Grounded();

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
    }

    private void HorizontalMovement()
    {
        bool blocked = _controller.Blocked(_horizontal);

        if (_horizontal != 0f)
        {
            Flip(_horizontal > 0f);

            if (!blocked)
            {
                if (_grounded || CurrentState == State.CLIMB)
                    CurrentState = State.WALK;
            }
            else if (_grounded)
                CurrentState = State.STUCK;
        }
        else if (_grounded && _vertical == 0f)
        {
            //If player is idle for a certain amount of time, switch to an impatient state.
            IdleTimer.Tick();
            CurrentState = IdleTimer.IsFinished ? State.IMPATIENT : State.IDLE;
        }

    }

    //This is for climbing.
    private void VerticalMovement()
    {
        if (_vertical > 0f || _vertical < 0f && _canClimbDown)
        {
            if (_nearLadder)
            {
                if (CurrentState != State.CLIMB)
                    MoveTowardsLadder();

                float offset = .1f;
                if (Mathf.Abs(transform.position.x - _centerOfLadder) <= offset)
                {
                    ClimbLadder();
                }
                _horizontal = 0f;
            }
        }
    }

    private void MoveTowardsLadder()
    {
        CurrentState = State.MoveTowardsLadder;
        Flip(_centerOfLadder - transform.position.x >= 0f);
    }

    private void ClimbLadder()
    {
        CurrentState = State.CLIMB;
        transform.position = new Vector2(_centerOfLadder, transform.position.y);
    }

    //This gets called by the ClimbBehavior script in order to determine if the player is actually moving up or down the ladder, or just staying still on the ladder. 
    //Also, keep in mind that if the player has reached the very top of the ladder, then 'nearLadder' will be set to false.
    public bool ClimbingLadder()
    {
        return (_vertical == 0f || !_nearLadder);
    }

    private void Jump()
    {
        if (_grounded || _ghostJump.enabled)
        {
            _ghostJump.time = 0;
            ApplyForce();
            AudioManager.Instance.Play("playerJump");
        }

        if (CurrentState == State.CLIMB && _vertical == 0f)
            CurrentState = State.FALL;
    }

    private void ApplyForce()
    {
        //I want this rigidbody operation to happen in FixedUpdate rather than update. To do this, I add it to a queue and then dequeue it in FixedUpdate.
        _force.Enqueue(() => _rb.AddForce(new Vector2(_xforce, _yforce), ForceMode2D.Impulse));

        if (_xforce > 0f)
        {
            //Don't confuse leaping for jumping! Leaping is when the player is bouncing off of enemy(3) in order to reach platform(3).
            _leaping = true;
        }
    }

    //If the player is Airbourne, we need to check if it's jumping or landing.
    private void CheckAltitude()
    {
        if (!_grounded && CurrentState != State.CLIMB)
        {
            if (_rb.velocity.y > .01f)
            {
                CurrentState = State.JUMP;
            }
            else if (_rb.velocity.y < -.01f)
            {
                CurrentState = State.FALL;
            }
        }
    }

    //Checks to see if the player has pressed the 'up' key to look up, or the 'down' key to look down.
    private void CheckLookingDirection()
    {
        if (_horizontal == 0f && _grounded)
        {
            if (_vertical > 0f)
            {
                if (!_nearLadder && CurrentState != State.CLIMB)
                {
                    CurrentState = State.LOOKING_UP;
                }
            }
            else if (_vertical < 0f)
            {
                if (!_nearLadder || !_canClimbDown)
                {
                    CurrentState = State.LOOKING_DOWN;
                }
            }
        }
    }

    //Ghost jumping is a feature that is common in most platformer games. If the player tries to jump off of the very edge of a platform but presses the 'jump' button a little too late,
    //they still have a little bit of time to make the jump.
    private void SetGhostJump()
    {
        _ghostJump.time += _grounded ? 1 : -1;
        _ghostJump.time = Mathf.Clamp(_ghostJump.time, GhostJump.MIN_TIME, GhostJump.MAX_TIME);

        _ghostJump.enabled = _ghostJump.time > 0 && CurrentState != State.CLIMB;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.tag)
        {
            case "Puzzle":
                Destroy(other.gameObject);
                LevelManager.Instance.CapturePuzzle();
                break;
            case "Ladder":
                _nearLadder = true;
                _centerOfLadder = other.bounds.center.x;
                break;
            case "ClimbDownTrue":
                _canClimbDown = true;
                break;
            case "ClimbDownFalse":
                _canClimbDown = false;
                break;
            case "AddedBounce":
                _xforce = 3.4f;
                _yforce = 6.6f;
                break;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ladder"))
        {
            _nearLadder = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            _xforce = 0f;
            _yforce = 4.6f;
            _leaping = false;
        }
        if (other.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();

            if (other.contacts[0].normal.y > 0f && CurrentState != State.CLIMB)
            {
                enemy.Death();
                _rb.velocity = Vector2.zero;
                ApplyForce();
                AudioManager.Instance.Play("playerBounce");
            }
            else
            {
                LevelManager.Instance.Kill(gameObject);
            }
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            CheckForEdgeCollision(_controller.NumOfVerticalRayCollisions);
        }
    }

    //If the player is standing too far at the very edge of a platform, they'll be pushed off. This is so that it doesn't look like they're standing in the air.
    private void CheckForEdgeCollision(int rayCollisions)
    {
        float force = 3.0f;

        if (rayCollisions == 1)
        {
            if (_facingRight)
            {
                _force.Enqueue(() => transform.Translate(Vector2.right * (force * Time.fixedDeltaTime)));
            }
            else
            {
                _force.Enqueue(() => transform.Translate(Vector2.left * (force * Time.fixedDeltaTime)));
            }
        }
    }

    //I set the dead zone manually in code, rather than in the editor. This is so that the player's speed does NOT vary depending on how hard or fast the analog stick is being pushed.
    //Rather, it will remain constant, regardless of how much pressure is being applied to the analog stick.
    private float SetDeadZone(float dir, float dead)
    {
        if (dir < dead && dir > -dead)
        {
            return 0f;
        }
        return dir;
    }

    //This is a wrapper function for the Reset method inside of the Timer struct. This is so the editor can call this function as an animation event at the end of the 'impatient' animation.
    public void ResetTimer()
    {
        IdleTimer.Reset();
    }

    private State CurrentState
    {
        get
        {
            return (State)_anim.GetInteger("State");
        }
        set
        {
            _anim.SetInteger("State", (int)value);
        }
    }

    #region Types
    private enum State
    {
        IDLE = 0,
        WALK = 1,
        JUMP = 2,
        FALL = 3,
        CLIMB = 4,
        LOOKING_DOWN = 5,
        MoveTowardsLadder = 6,
        IMPATIENT = 7,
        LOOKING_UP = 8,
        STUCK = 9,
    }

    private struct GhostJump
    {
        public int time;
        public const int MIN_TIME = 0, MAX_TIME = 8;
        public bool enabled;
    }

    public struct Timer
    {
        private float _time;
        public readonly float DURATION;

        public Timer(float duration)
        {
            _time = 0f;
            DURATION = duration;
        }

        public void Reset()
        {
            _time = 0f;
        }

        public void Tick()
        {
            _time += Time.deltaTime;
        }

        public bool IsFinished { get { return _time >= DURATION; } }
    }
    #endregion
}
