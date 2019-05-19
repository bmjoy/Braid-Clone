using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Raycaster : MonoBehaviour
{
    private BoxCollider2D _col;
    private Bounds _bounds;
    private RaycastOrigins _raycastOrigins;

    [SerializeField]
    private int _rayCount = 3;

    private float _verticalRaySpacing, _horizontalRaySpacing;

    public int NumOfVerticalRayCollisions { get; private set; }

    private void Start()
    {
        _col = GetComponent<BoxCollider2D>();

        UpdateRaycastOrigins();
        CalculateRaySpacing();
    }

    private void FixedUpdate()
    {
        UpdateRaycastOrigins();
    }

    public bool IsGrounded()
    {
        float rayLength = .15f;
        float offset = .02f;
        NumOfVerticalRayCollisions = 0;

        Vector2 rayOrigin = _raycastOrigins.bottomLeft + Vector2.up * offset;

        for (int i = 0; i < _rayCount; i++)
        {
            Vector2 ray = rayOrigin + (Vector2.right * (_verticalRaySpacing * i));
            Debug.DrawRay(ray, Vector2.down * rayLength, Color.red);
            bool hit = Physics2D.Raycast(ray, Vector2.down, rayLength, 1 << LayerMask.NameToLayer("Ground"));

            if (hit)
            {
                NumOfVerticalRayCollisions++;
            }
        }
        return NumOfVerticalRayCollisions > 0;
    }

    /// <summary>
    /// Checks to see if the player is blocked by a wall by casting horizontal rays on either side of him. These rays are very small compared to the vertical ones that check
    /// if he's grounded.
    /// </summary>
    /// <param name="horizontal"></param>
    /// <returns></returns>
    public bool IsBlocked(float horizontal)
    {
        float directionX = Mathf.Sign(horizontal);
        float rayLength = .04f, offset = .015f;
        int rayCollisions = 0;

        Vector2 rayOrigin = (directionX == -1) ? _raycastOrigins.bottomLeft + Vector2.right * offset : _raycastOrigins.bottomRight + Vector2.left * offset;
        if (horizontal != 0)
        {
            for (int i = 0; i < _rayCount; i++)
            {
                Vector2 ray = rayOrigin + (Vector2.up * (_horizontalRaySpacing * i));
                Debug.DrawRay(ray, Vector2.right * (directionX * rayLength), Color.red);
                bool hit = Physics2D.Raycast(ray, Vector2.right * directionX, rayLength, 1 << LayerMask.NameToLayer("Wall"));

                if (hit)
                {
                    rayCollisions++;
                }
            }
        }
        return rayCollisions > 0;
    }

    private void UpdateRaycastOrigins()
    {
        _bounds = _col.bounds;

        _raycastOrigins.bottomLeft = new Vector2(_bounds.min.x, _bounds.min.y);
        _raycastOrigins.bottomRight = new Vector2(_bounds.max.x, _bounds.min.y);
    }

    private struct RaycastOrigins
    {
        public Vector2 bottomLeft, bottomRight;
    }

    private void CalculateRaySpacing()
    {
        _horizontalRaySpacing = _bounds.size.y / (_rayCount - 1);
        _verticalRaySpacing = _bounds.size.x / (_rayCount - 1);
    }

}
