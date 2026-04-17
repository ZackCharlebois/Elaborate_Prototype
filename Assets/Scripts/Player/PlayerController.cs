using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(ProjectileShooter))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(HealthSystemPlayer))]
public class PlayerController : MonoBehaviour
{
    #region serialized fields exposed in inspector

    [SerializeField] private float speed = 10f;
    [SerializeField] private float jumpForce = 20f;
    [SerializeField] private float gravityFall = 12f;
    [SerializeField] private float gravityFloat = 5f;
    [SerializeField] private float _coyoteTime = .05f;
    [SerializeField] private float _earlyJumpTime = .05f;
    
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.0625f;

    [SerializeField] private AudioClip _soundJump;
    [SerializeField] private AudioClip _soundFail;
    
    #endregion
    
    #region component reference private fields
    
    private Rigidbody2D _rb;
    private BoxCollider2D _collider;
    private Animator _animator;
    private ProjectileShooter _shooter;
    private HealthSystemPlayer _healthSystem;
    
    #endregion

    #region input private fields

    private PlayerInput _input;
    private float _horizontalInput = 0f;
    private bool _isJumpDown = false;
    private bool _isJumpHeldDown = false;

    #endregion

    #region state private fields
    
    private bool _isFacingRight = true;
    private bool _isInKickBack = false;
    private bool _isFainting = false;
    private bool _hasLeftGround = false;
    private float _coyoteTimeLeft = 0f;
    private float _earlyJumpTimeLeft = 0f;

    #endregion
    
    private void Awake()
    {
        _input = GetComponent<PlayerInput>();
        
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _collider = GetComponent<BoxCollider2D>();
        _shooter = GetComponent<ProjectileShooter>();
        _healthSystem = GetComponent<HealthSystemPlayer>();
        
        //set fall gravity
        _rb.gravityScale = gravityFall;
        
        //Find entrance location and move to it if there is one
        var starts = FindObjectsOfType<Entrance>();
        foreach (var entrance in starts)
        {
            if (entrance.GetNumber() == GameData.Instance.entranceNumber)
            {
                transform.position = entrance.transform.position;
            }
        }
    }

    private void Update()
    {
        //Check pause button to toggle pause state
        if (_input.actions["PauseToggle"].WasPressedThisFrame()) GameManager.Instance.TogglePause();
        
        //if we are not playing, cancel gameplay inputs!
        if (GameManager.Instance.State != GameState.Playing || _isFainting)  
            return;

        _coyoteTimeLeft -= Time.deltaTime; //pass coyote time
        _earlyJumpTimeLeft -= Time.deltaTime; //pass early jump time

        UpdateInputs();
        UpdateVisuals();
    }

    private void UpdateInputs()
    {
        //process buttons in Update
        //(changes involving physics rigidbodies go in FixedUpdate)
        if (_input.actions["Fire"].WasPressedThisFrame())
        {
            _shooter.Fire(new Vector2(_isFacingRight ? 1 : -1, 0));
            _healthSystem.SelfDamage(1);
        }

        if (_input.actions["CycleShot"].WasPressedThisFrame())
        {
            _shooter.ReadyNext();
        }

        if (_input.actions["Jump"].WasPressedThisFrame())
        {
            _isJumpDown = true;
            _earlyJumpTimeLeft = _earlyJumpTime; //reset to full amount of time
        }

        _isJumpHeldDown = _input.actions["Jump"].IsPressed();
        _horizontalInput = _input.actions["Move"].ReadValue<Vector2>().x;
    }
    
    private void UpdateVisuals()
    {
        //Change visuals and animation based on Inputs
        if (_horizontalInput > 0 && !_isFacingRight) FlipSprite();
        if (_horizontalInput < 0 && _isFacingRight) FlipSprite();

        //.Equals() is used because floating-point precision rarely hits 0.0 exactly
        if (_horizontalInput.Equals(0))
        {
            _animator.Play("player_idle");
        }
        else if (_horizontalInput > 0 || _horizontalInput < 0)
        {
            _animator.Play("player_walk");
        }    
    }

    private void FixedUpdate()
    {
        //FixedUpdate will not be updated when Time.TimeScale is zero, so "pausing" is automatically handled
        
        if (_isFainting || _isInKickBack) return;

        //Movement and Jumping
        _rb.velocity = new Vector2(_horizontalInput * speed, _rb.velocity.y);
        
        if (!_hasLeftGround && !IsOnGround())
        {
            //left ground without jumping, start extra coyote time to jump
            _coyoteTimeLeft = _coyoteTime; //reset to full amount of time   
            _hasLeftGround = true;
        }

        //if we land after walking off a cliff, reset hasLeftGround state and cancel coyote time
        if ((_coyoteTimeLeft > 0f || _hasLeftGround) && IsOnGround())
        {
            _coyoteTimeLeft = 0f;
            _hasLeftGround = false;
        }
        
        //Jump
        if (_isJumpDown || _earlyJumpTimeLeft > 0f)
        {
            _isJumpDown = false; //we have now used up this button press
            
            //either being on the ground or still in coyote time we can jump
            if (IsOnGround() || _coyoteTimeLeft > 0f)
            {
                _rb.velocity = new Vector2(_rb.velocity.x, jumpForce);
                _coyoteTimeLeft = 0f; //jumping cancels coyote time (or you will get air jumps)
                _earlyJumpTimeLeft = 0f; //can't reuse the early jump press.
                _hasLeftGround = true;
                AudioSystem.Instance.PlaySound(_soundJump, transform.position);
            }
        }
        
        // if jump is held down and we are on the way up
        // lower gravity to affect a lift
        // otherwise gravity goes back to normal (gravityFall)
        _rb.gravityScale = (_isJumpHeldDown && _rb.velocity.y > 0) ? gravityFloat : gravityFall;
    }
    
    private bool IsOnGround()
    {
        //check collision with ground below the left and right bottom corners of the box collider
        var bounds = _collider.bounds;
        var bottomCornerRight = new Vector2(bounds.max.x, bounds.min.y);
        var bottomCornerLeft = new Vector2(bounds.min.x, bounds.min.y);
        
        RaycastHit2D hitRight = Physics2D.Raycast(bottomCornerRight, 
                                                  Vector2.down, 
                                                  groundCheckDistance, 
                                                  groundLayer);
        RaycastHit2D hitLeft = Physics2D.Raycast(bottomCornerLeft, 
                                                 Vector2.down, 
                                                 groundCheckDistance, 
                                                 groundLayer);

        return hitRight.collider || hitLeft.collider;
    }

    private void OnDrawGizmos()
    {
        var bounds = GetComponent<BoxCollider2D>().bounds;
        
        var bottomCornerRight = new Vector2(bounds.max.x, bounds.min.y);
        var bottomCornerLeft = new Vector2(bounds.min.x, bounds.min.y);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(bottomCornerRight,
                        new Vector3(bottomCornerRight.x, bottomCornerRight.y - groundCheckDistance, 0f));
        Gizmos.DrawLine(bottomCornerLeft,
                        new Vector3(bottomCornerLeft.x, bottomCornerLeft.y - groundCheckDistance, 0f));
    }

    private void FlipSprite()
    {
        _isFacingRight = !_isFacingRight; //invert
        
        Vector3 transformScale = transform.localScale;
        transformScale.x *= -1;
        transform.localScale = transformScale;
    }


    public void KickBack(Vector2 directionVector)
    {
        _rb.AddForce(directionVector, ForceMode2D.Impulse);
        
        _isInKickBack = true; //take away control from player
        
        //give back control after .2 seconds
        Invoke(nameof(ReturnControl), .2f);
    }

    public void ReturnControl()
    {
        _isInKickBack = false;
    }

    public void TakeDamage()
    {
        if (_isFainting) return;
        
        _animator.Play("player_damage");
    }

    public void AcceptDefeat()
    {
        if (_isFainting) return;
        
        StartCoroutine(nameof(Faint));
    }

    public IEnumerator Faint()
    {
        _isFainting = true;
        _animator.Play("player_faint");
        AudioSystem.Instance.PlaySound(_soundFail, transform.position);
        yield return new WaitForSeconds(3);
        //reload this scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
