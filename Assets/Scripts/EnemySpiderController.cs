using System;
using UnityEngine;
using Random = UnityEngine.Random;

//An enumerated type for all the states of this enemy (included here for simplicity, usually would be in separate file)
//note that the first one starts at 1 and the rest will get consecutively numbered.
//this is done so we can use a random integer to choose one (in EnemySpiderController)
enum SpiderState
{
    Waiting = 1,
    Walking,
    Jumping
}

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(HealthSystem))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class EnemySpiderController : MonoBehaviour
{
    #region serialized fields
    
    [SerializeField] private int _playerDamageAmt = 3;
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float jumpForce = 22f;
    [SerializeField] private float _stateDelayMin = 0.1f;
    [SerializeField] private float _stateDelayMax = 1.5f;
    [SerializeField] private GameObject spiderFoodPrefab;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.0625f;
    [SerializeField] private AudioClip _soundJump;
    [SerializeField] private AudioClip _soundDefeat;
    [SerializeField] private AudioClip _soundWalk;
    

    #endregion
    
    private SpiderState _state = SpiderState.Waiting;
    private float _timeLeftBeforeStateChange;
  
    #region component reference fields

    private Rigidbody2D _rb;
    private Animator _animator;
    private HealthSystem _healthSystem;
    private BoxCollider2D _collider;

    #endregion
    
    #region properties
    private bool IsFacingRight
    {
        get
        {
            //the IsFacingRight property is tied directly to the x scale
            return transform.localScale.x > 0;
        }
        set
        {
            //setting IsFacingRight flips scale of our object in x, which also flips image
            Vector3 scale = transform.localScale; //copy unchangeable scale
            scale.x = value ? 1 : -1; //set x in copy based on IsFacingRight now set to true or false
            transform.localScale = scale; //replace scale with changed copy
        }
    }
    
    #endregion
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _healthSystem = GetComponent<HealthSystem>();
        _animator = GetComponent<Animator>();
        _collider = GetComponent<BoxCollider2D>();
    }
    
    // Start is called before the first frame update
    private void Start()
    {
        _state = SpiderState.Walking;
        _timeLeftBeforeStateChange = .01f;
    }

    public void FlipFacingDirection()
    {
        //IsFacingRight property is tied to x scale, so it flips it. (See IsFacingRight property definition above)
        IsFacingRight = !IsFacingRight; //if it's true set it to false, if it's false set it to true
    }

    private void Update()
    {
        //make no updates unless we are in the in the playing state
        if (GameManager.Instance.State != GameState.Playing) 
            return;

        //subtract time passed since last update from time left before we should change
        _timeLeftBeforeStateChange -= Time.deltaTime;
        
        //if there is still time left, we are done here
        if (_timeLeftBeforeStateChange > 0) 
            return;
        
        //reset time left
        _timeLeftBeforeStateChange = Random.Range(_stateDelayMin, _stateDelayMax);
        //interrupt whatever we were doing to change to a new state
        ChangeToRandomState(); 
    }

    private void ChangeToRandomState()
    {
        //change state to a random state (notice the int is typecast into the SpiderState enum)
        _state = (SpiderState) Random.Range(1, 4); //[1..3]

        //preform some initial actions when first switching to the new state
        switch (_state)
        {
            case SpiderState.Waiting:
                _animator.Play("spider_idle");
                break;
            case SpiderState.Walking:
                AudioSystem.Instance.PlaySoundRandomPitch(_soundWalk, transform.position);
                _animator.Play("spider_walk");
                break;
            case SpiderState.Jumping:
                AudioSystem.Instance.PlaySoundRandomPitch(_soundJump, transform.position, 0.3f);
                _animator.Play("spider_idle");
                break;
        }
    }

    private void FixedUpdate()
    {
        switch (_state)
        {
            case SpiderState.Waiting:
                //if on the ground, stop x velocity
                if (IsOnGround()) _rb.velocity = new Vector2(0f, _rb.velocity.y);
                break;

            case SpiderState.Jumping:
                //jump at the first opportunity, then immediately switch to walking state
                if (IsOnGround())
                {
                    _rb.velocity = new Vector2(_rb.velocity.x, jumpForce);
                    _state = SpiderState.Walking; //once we jump, switch to walking state (walk in the air)
                }
                break;
            
            case SpiderState.Walking:
                //move forward, "bouncing" off walls
                if (isAgainstWall()) 
                    FlipFacingDirection();
                _rb.velocity = new Vector2(walkSpeed * (IsFacingRight ? 1 : -1), _rb.velocity.y);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private bool isAgainstWall()
    {
        var bounds = _collider.bounds;
        float forwardX = IsFacingRight ? bounds.max.x : bounds.min.x;
        var forwardBottomCorner = new Vector2(forwardX, bounds.min.y + 0.1f); //lifted one-tenth a unit off bottom

        RaycastHit2D hitForward = Physics2D.Raycast(forwardBottomCorner, 
            new Vector2((IsFacingRight ? 1 : -1), 0),
            groundCheckDistance, 
            groundLayer);

        return hitForward.collider; // hitBottom.collider || hitTop.collider;        
    }
    
    private bool IsOnGround()
    {
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

    public void TakeDamage()
    {
        _animator.Play("shared_damage");
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.CompareTag("Player") == false) return; //if it's not the player we don't care, bail out
        
        other.transform.GetComponent<HealthSystem>()?.Damage(_playerDamageAmt);
        Vector2 awayDirection = other.transform.position - transform.position;
        other.transform.GetComponent<PlayerController>().KickBack(awayDirection * 3f);
    }

    public void AcceptDefeat()
    {
        GameEventDispatcher.TriggerEnemyDefeated();
        Destroy(gameObject);
        
        AudioSystem.Instance.PlaySound(_soundDefeat, transform.position);

        //instantiate spider food in place of spider
        if (!spiderFoodPrefab) return;
        var food = Instantiate(spiderFoodPrefab, transform.position, Quaternion.identity);
        if (!food) return;

        //transfer spider's physics to food so it continues momentum
        var foodRb = food.GetComponent<Rigidbody2D>();
        if (!foodRb) return;

        foodRb.velocity = _rb.velocity;
        foodRb.gravityScale = _rb.gravityScale;
    }
    
    private void OnDrawGizmos()
    {
        var bounds = GetComponent<BoxCollider2D>().bounds;
        
        //Ground Check raycast lines
        var bottomCornerRight = new Vector2(bounds.max.x, bounds.min.y);
        var bottomCornerLeft = new Vector2(bounds.min.x, bounds.min.y);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(bottomCornerRight,
            new Vector3(bottomCornerRight.x, bottomCornerRight.y - groundCheckDistance, 0f));
        Gizmos.DrawLine(bottomCornerLeft,
            new Vector3(bottomCornerLeft.x, bottomCornerLeft.y - groundCheckDistance, 0f));
        
        //Wall check raycast line
        var forwardX = IsFacingRight ? bounds.max.x : bounds.min.x;
        var forwardBottomCorner = new Vector2(forwardX, bounds.min.y + 0.1f);
        Gizmos.DrawLine(forwardBottomCorner, new Vector3(forwardX + (IsFacingRight ? 1 : -1) * groundCheckDistance, forwardBottomCorner.y));
    }
}