using System;
using UnityEngine;

public class Enemy : Character
{
    private Rigidbody2D _rb;
    private CircleCollider2D _collider;

    [SerializeField]
    private Transform _point1, _point2;

    [SerializeField]
    private float _speed;

    protected override void Start()
    {
        base.Start();

        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<CircleCollider2D>();

        if (!isFacingRight)
        {
            _speed *= -1;
        }
    }

    private void FixedUpdate()
    {
        Patrol();
    }

    private void Patrol()
    {
        _rb.velocity = new Vector2(_speed, _rb.velocity.y);
      
        if (transform.localPosition.x <= _point1.localPosition.x)
        {
            _speed = Math.Abs(_speed);
            Flip(_speed > 0f);
        }
        else if (transform.localPosition.x >= _point2.localPosition.x)
        {
            _speed = Math.Abs(_speed) * -1;
            Flip(_speed > 0f);
        }
    }

    public override void Death()
    {
        CurrentState = State.HURT;
        _speed = 0f;
        _collider.enabled = false;
        _rb.freezeRotation = false;
        LevelManager.Instance.EnemyDeathCount++;
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    private State CurrentState
    {
        get => (State)_anim.GetInteger("State");
        set => _anim.SetInteger("State", (int)value);
    }

    private enum State
    {
        PATROLLING = 0,
        HURT = 1,
    }

}