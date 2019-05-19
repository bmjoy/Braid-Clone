using System;
using UnityEngine;

[RequireComponent(typeof(Raycaster))]
public class Player : Character
{
    #region Components
    private Rigidbody2D _rb;
    private Raycaster _raycaster;
    #endregion

    #region Structs
    private GhostJump _ghostJump;
    public static Timer IdleTimer;
    #endregion

    #region Primitives
    private bool isGrounded, isLeaping, isNearLadder, canClimbDown;
    private float _vertical, _horizontal, _xforce, _yforce, _centerOfLadder;
    #endregion

    /// <summary>
    ///  Determines if the player is actually moving up or down the ladder, or just staying still on the ladder. 
    ///  Keep in mind that if the player has reached the very top of the ladder, then 'nearLadder' will be set to false.
    /// </summary>
    /// <returns></returns>
    public bool IsClimbingLadder => (_vertical == 0f || !isNearLadder);

    public static event Action<Transform> OnDeath;

    #region Unity callbacks
    protected override void Start()
    {
        base.Start();
        _rb = GetComponent<Rigidbody2D>();
        _raycaster = GetComponent<Raycaster>();

        IdleTimer = new Timer(20f);
    }

    private void FixedUpdate()
    {
        _horizontal = Input.GetAxisRaw("Horizontal");
        _vertical = Input.GetAxisRaw("Vertical");

        SetDeadZone(ref _horizontal, .8f);
        SetDeadZone(ref _vertical, .8f);
        
        var speed = isGrounded ? 5f : 2.8f; //If the player is airbourne, we lower his speed so that he can't jump too far across the screen. These values can also be lerped for a smoother transition.
        var climbSpeed = 3f;

        SetGhostJump();
        CheckAltitude();
        HorizontalMovement();
        VerticalMovement();
        CheckLookingDirection();

        if (CurrentState == State.CLIMB)
        {
            if (isNearLadder)
                _rb.velocity = new Vector2(_horizontal * speed, _vertical * climbSpeed);
            else
            {
                _rb.velocity = new Vector2(_horizontal * speed, (_vertical > 0f) ? 0f : _vertical * climbSpeed);
            }
        }
        else if (CurrentState == State.MOVE_TOWARDS_LADDER)
        {
            var ladder = new Vector2(_centerOfLadder, transform.position.y);
            var position = Vector2.MoveTowards(transform.position, ladder, speed * Time.deltaTime);
            _rb.MovePosition(position);
        }
        else if (!isLeaping)
        {
            _rb.velocity = new Vector2(_horizontal * speed, _rb.velocity.y);
        }
    }

    private void Update()
    {
        //Prevents the player from jumping when the user hits the spacebar to resume the game.
        if (GameManager.IsGamePaused)
            return;

        //Prevents the player from awkwardly landing on the platforms while climbing.
        isGrounded = (CurrentState == State.CLIMB && canClimbDown) ? false : _raycaster.IsGrounded();

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
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
                isNearLadder = true;
                _centerOfLadder = other.bounds.center.x;
                break;
            case "ClimbDownTrue":
                canClimbDown = true;
                break;
            case "ClimbDownFalse":
                canClimbDown = false;
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
            isNearLadder = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            _xforce = 0f;
            _yforce = 4.6f;
            isLeaping = false;
        }

        if (other.gameObject.CompareTag("Enemy"))
        {
            var enemy = other.gameObject.GetComponent<Enemy>();

            if (other.contacts[0].normal.y > 0f && CurrentState != State.CLIMB)
            {
                enemy.Death();
                _rb.velocity = Vector2.zero;
                ApplyForce();
                AudioManager.Instance.Play(Sound.PLAYER_BOUNCE);
            }
            else
            {
                Death();
            }
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            CheckForEdgeCollision(_raycaster.NumOfVerticalRayCollisions);
        }
    }
    #endregion

    private void HorizontalMovement()
    {
        bool blocked = _raycaster.IsBlocked(_horizontal);

        if (_horizontal != 0f)
        {
            Flip(_horizontal > 0f);

            if (!blocked)
            {
                if (isGrounded || CurrentState == State.CLIMB)
                    CurrentState = State.WALK;
            }
            else if (isGrounded)
                CurrentState = State.STUCK;
        }
        else if (isGrounded && _vertical == 0f)
        {
            //If player is idle for a certain amount of time, switch to an impatient state.
            IdleTimer.Tick();
            CurrentState = IdleTimer.IsFinished ? State.IMPATIENT : State.IDLE;
        }

    }

    /// <summary>
    /// Handles climbing behavior
    /// </summary>
    private void VerticalMovement()
    {
        if (_vertical > 0f || _vertical < 0f && canClimbDown)
        {
            if (isNearLadder)
            {
                if (CurrentState != State.CLIMB)
                    MoveTowardsLadder();

                var offset = .1f;

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
        CurrentState = State.MOVE_TOWARDS_LADDER;
        Flip(_centerOfLadder - transform.position.x >= 0f);
    }

    private void ClimbLadder()
    {
        CurrentState = State.CLIMB;
        transform.position = new Vector2(_centerOfLadder, transform.position.y);
    }

    private void Jump()
    {
        if (isGrounded || _ghostJump.enabled)
        {
            _ghostJump.time = 0;
            ApplyForce();
            AudioManager.Instance.Play(Sound.PLAYER_JUMP);
        }

        if (CurrentState == State.CLIMB && _vertical == 0f)
            CurrentState = State.FALL;
    }

    private void ApplyForce()
    {
        _rb.AddForce(new Vector2(_xforce, _yforce), ForceMode2D.Impulse);

        if (_xforce > 0f)
        {
            //Don't confuse leaping for jumping! Leaping is when the player is bouncing off of enemy(3) in order to reach platform(3).
            isLeaping = true;
        }
    }

    /// <summary>
    /// If the player is airbourne, checks to see if he's jumping or landing.
    /// </summary>
    private void CheckAltitude()
    {
        if (!isGrounded && CurrentState != State.CLIMB)
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

    /// <summary>
    /// Checks to see if the player has pressed the 'up' key to look up, or the 'down' key to look down.
    /// </summary>
    private void CheckLookingDirection()
    {
        if (_horizontal == 0f && isGrounded)
        {
            if (_vertical > 0f)
            {
                if (!isNearLadder && CurrentState != State.CLIMB)
                {
                    CurrentState = State.LOOKING_UP;
                }
            }
            else if (_vertical < 0f)
            {
                if (!isNearLadder || !canClimbDown)
                {
                    CurrentState = State.LOOKING_DOWN;
                }
            }
        }
    }

    /// <summary>
    /// Ghost jumping is a feature that is common in most platformer games. If the player tries to jump off of the very edge of a platform but presses the 'jump' button a little too late,
    /// they still have a little bit of time to make the jump.
    /// </summary>
    private void SetGhostJump()
    {
        _ghostJump.time += isGrounded ? 1 : -1;
        _ghostJump.time = Mathf.Clamp(_ghostJump.time, GhostJump.MIN_TIME, GhostJump.MAX_TIME);
        _ghostJump.enabled = _ghostJump.time > 0 && CurrentState != State.CLIMB;
    }

    /// <summary>
    /// If the player is standing too far at the very edge of a platform, they'll be pushed off. This is so that it doesn't look like they're standing in the air.
    /// </summary>
    /// <param name="rayCollisions"></param>
    private void CheckForEdgeCollision(int rayCollisions)
    {
        var force = 3f;

        if (rayCollisions == 1)
        {
            _rb.AddForce(force * (isFacingRight ? Vector2.right : Vector2.left), ForceMode2D.Impulse);
        }
    }

    /// <summary>
    /// I set the dead zone manually in code, rather than in the editor. This is so that the player's speed does NOT vary depending on how hard or fast the analog stick is being pushed.
    /// Rather, it will remain constant, regardless of how much pressure is being applied to the analog stick.
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="dead"></param>
    /// <returns></returns>
    private void SetDeadZone(ref float dir, float dead)
    {
        if (dir < dead && dir > -dead)
        {
            dir = 0f;
        }
    }

    /// <summary>
    /// Wrapper function for the Reset method inside of the Timer struct. This is so the editor can call this function as an animation event at the end of the 'impatient' animation.
    /// </summary>
    public void ResetTimer()
    {
        IdleTimer.Reset();
    }

    public override void Death()
    {
        Destroy(gameObject);
        AudioManager.Instance.Play(Sound.PLAYER_HURT);
        OnDeath?.Invoke(transform);
    }

    private State CurrentState
    {
        get => (State)_anim.GetInteger("State");
        set => _anim.SetInteger("State", (int)value);
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
        MOVE_TOWARDS_LADDER = 6,
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
