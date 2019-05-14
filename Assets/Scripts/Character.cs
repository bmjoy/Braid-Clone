using UnityEngine;

public abstract class Character : MonoBehaviour
{
    protected SpriteRenderer _sprite;
    protected Animator _anim;

    protected bool isFacingRight;

    protected virtual void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _anim = GetComponent<Animator>();

        isFacingRight = !_sprite.flipX;
    }

    protected void Flip(bool facingRight)
    {
        _sprite.flipX = !facingRight;
        isFacingRight = facingRight;
    }

    public abstract void Death();

}
