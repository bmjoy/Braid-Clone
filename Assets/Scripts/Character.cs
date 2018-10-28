using UnityEngine;

public class Character : MonoBehaviour
{
    protected SpriteRenderer _sprite;
    protected Animator _anim;

    protected bool _facingRight;

    public virtual void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _anim = GetComponent<Animator>();

        _facingRight = !_sprite.flipX;
    }

    public virtual void Update()
    {
        _facingRight = !_sprite.flipX;
    }

    protected void Flip(bool facingRight)
    {
        _sprite.flipX = !facingRight;
    }

}
