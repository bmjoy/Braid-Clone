using UnityEngine;

public abstract class Character : MonoBehaviour
{
    protected SpriteRenderer _sprite;
    protected Animator _anim;

    protected bool _facingRight;

    protected virtual void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _anim = GetComponent<Animator>();

        _facingRight = !_sprite.flipX;
    }

    protected virtual void Update()
    {
        _facingRight = !_sprite.flipX;
    }

    public abstract void Death();

    protected void Flip(bool facingRight)
    {
        _sprite.flipX = !facingRight;
    }

}
