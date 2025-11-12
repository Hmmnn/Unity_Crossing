using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PlayerController : MonoBehaviour
{
#if ENABLE_INPUT_SYSTEM
    private PlayerInput _playerInput;
#endif
    private Animator _animator;
    private CharacterController _controller;
    private InputListener _input;
    private MapManager _mapManager;
    private EffectManager _effectManager;

    enum PlayerState { IDLE, MOVING, ATTACK, DEAD }
    private PlayerState _state = PlayerState.IDLE;

    private UIManager _UIManager;
    private int _life = 3;
    private bool _attackable = true;
    private int _curAtkPoint = 0;
    private int _maxAtkPoint = 5;

    // Sound
    [SerializeField]
    private string jumpSoundName = "JumpSound";
    [SerializeField]
    private string hitSoundName = "HittedSound";
    [SerializeField]
    private string blockSoundName = "BlockedSound";
    [SerializeField]
    private string breakSoundName = "BreakSound";

    // Particle
    [SerializeField]
    private GameObject jumpParticle;
    private ParticleSystem _jumpCloud;

    // Movement
    [SerializeField]
    private float _moveDist = 2f;
    [SerializeField]
    private float limitX = 12f;

    private Vector3 _moveVelocity = Vector3.zero;
    private Vector3 _targetPos = Vector3.zero;
    private float _moveDuration = 0.4f;
    private Vector3 _targetDir = Vector3.forward;
    private float _rotVelocity = 0f;
    private float _targetRotY = 0f;
    private Vector3 _prevPos = Vector3.zero;

    public float nextZ { get; private set; }
    private float _minZ;

    // animation
    private int _animIDJump;
    private int _animIDHit;
    private int _animIDDeath;
    private int _animIDAttack;

    #region Unity Method
    void Start()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponentInChildren<Animator>();
        _input = GetComponent<InputListener>();
#if ENABLE_INPUT_SYSTEM
        _playerInput = GetComponent<PlayerInput>();
#endif
        _mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        Debug.Assert(_mapManager, "MapManager Missing!");

        _UIManager = GameObject.Find("GameUI").GetComponent<UIManager>();
        Debug.Assert(_UIManager, "UIManager Missing");

        _effectManager = GameObject.Find("EffectManager").GetComponent<EffectManager>();
        Debug.Assert(_effectManager, "EffectManager Missing");

        GameObject effectobj = Instantiate(jumpParticle);
        _jumpCloud = effectobj.GetComponent<ParticleSystem>();

        AssignAnimationIDs();
    }

    void Update()
    {
        if(_state == PlayerState.DEAD)
        {
            return;
        }
            
        Move();
        Attack();
    }

    private void LateUpdate()
    {
        if (_life <= 0)
        {
            _state = PlayerState.DEAD;
            GameManager.instance.GameOver();
        }

        if(!_attackable) _input.ResetAttackValue();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_state == PlayerState.DEAD) return;

        if(other.CompareTag("Vehicle"))
        {
            SoundManager.instance.PlaySoundAtPosition(hitSoundName, transform.position);
            _life = 0;
        }
    }

    #endregion

    #region Utils
    private void Move()
    {
        if(_state == PlayerState.IDLE)
        {
            if(_input.forward != 0)
            {
                float factor = (_input.forward > 0) ? _moveDist : -_moveDist;
                _moveVelocity = Vector3.forward * factor;
                _targetDir = (factor > 0) ? Vector3.forward : Vector3.back;

                _input.ResetMoveValues();
            }
            else if(_input.right != 0)
            {
                float factor = (_input.right > 0) ? _moveDist : -_moveDist;
                _moveVelocity = Vector3.right * factor;
                _targetDir = (factor > 0) ? Vector3.right : Vector3.left;

                _input.ResetMoveValues();
            }
        }

        if (_moveVelocity != Vector3.zero)
        {
            // 이동 위치
            _targetPos = transform.position + _moveVelocity;
            _moveVelocity = Vector3.zero;
            _targetRotY = Mathf.Atan2(_targetDir.x, _targetDir.z) * Mathf.Rad2Deg;

            // 이동이 막혔을 경우 회전 코루틴 실행
            if (BlockCharacter(_targetPos))
            {
                _targetPos = transform.position;
                SoundManager.instance.PlaySoundAtPosition(blockSoundName, _targetPos);
                StartCoroutine(RotateCharacter());
            }
            else // 막히지 않은 경우 회전+이동 코루틴 실행
            {
                nextZ = _targetPos.z;
                _minZ = Mathf.Max(_minZ, nextZ - _moveDist * 2);
                _prevPos = transform.position;
                if (_prevPos.z < nextZ)
                {
                    _UIManager.UpdateScore();
                    if (_curAtkPoint < _maxAtkPoint) ++_curAtkPoint;
                }

                SoundManager.instance.PlaySoundAtPosition(jumpSoundName, _prevPos);
                _jumpCloud.transform.position = new Vector3(_prevPos.x, _prevPos.y + 0.2f, _prevPos.z);
                _jumpCloud.Play();
                StartCoroutine(MoveCharactor());
            }
        }
    }

    private void Attack()
    {
        if (_curAtkPoint >= _maxAtkPoint) 
        {
            _attackable = true;
            _UIManager.UpdateAttackable(_attackable);
        }

        if(_state == PlayerState.IDLE && _attackable)
        {
            if(_input.attack)
            {
                _state = PlayerState.ATTACK;
                _animator.SetBool(_animIDAttack, true);
                
                Vector3 attackPos = transform.position + transform.forward * _moveDist;
                bool attackResult = _mapManager.BreakBlock(attackPos);
                if(attackResult)
                {
                    _attackable = false;
                    _curAtkPoint = 0;
                    _UIManager.UpdateAttackable(_attackable);
                    SoundManager.instance.PlaySoundAtPosition(breakSoundName, attackPos);
                    _effectManager.PlayParticle("explosion", attackPos);
                }
            }
        }

        else if(_state == PlayerState.ATTACK)
        {
            // 공격 중에는 입력 X
            _input.ResetAttackValue();

            if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") == false)
                _animator.SetBool(_animIDAttack, false);

            _state = PlayerState.IDLE;
        }
    }

    private IEnumerator MoveCharactor()
    {
        _state = PlayerState.MOVING;
        _animator.SetBool(_animIDJump, true);
        _input.ResetAttackValue();

        Vector3 startPos = transform.position;

        float elapsedTime = 0f;
        while(elapsedTime < _moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float percent = elapsedTime / _moveDuration;

            Vector3 newPos = Vector3.Lerp(startPos, _targetPos, percent);
            newPos.y = 0f;
            transform.position = newPos;

            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotY, ref _rotVelocity, 0.1f);
            transform.rotation = Quaternion.Euler(0f, rotation, 0f);

            yield return null;
        }

        transform.position = _targetPos;
        transform.rotation = Quaternion.Euler(0f, _targetRotY, 0f);

        _state = PlayerState.IDLE;
        _animator.SetBool(_animIDJump, false);
    }

    private IEnumerator RotateCharacter()
    {
        _state = PlayerState.MOVING;
        _input.ResetAttackValue();

        float elapsedTime = 0f;
        while (elapsedTime < _moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float percent = elapsedTime / _moveDuration;

            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotY, ref _rotVelocity, 0.1f);
            transform.rotation = Quaternion.Euler(0f, rotation, 0f);

            yield return null;
        }

        transform.rotation = Quaternion.Euler(0f, _targetRotY, 0f);

        _state = PlayerState.IDLE;
    }

    private bool BlockCharacter(Vector3 nextPos)
    {
        if (nextPos.z < 0f) return true;

        // 좌우로 이동 가능 범위 한정
        if (nextPos.x < -limitX || nextPos.x > limitX)
            return true;

        // 뒤로 두 칸(moveDist) 이상 못 가도록 한정
        if (_minZ > nextPos.z)
            return true;

        // 장애물 검사
        if (_mapManager.CheckBlock(nextPos))
        {
            Debug.Log("Blocked");
            --_life;

            _UIManager.UpdateLife(_life);
            Vector3 particlePos = transform.position + transform.forward * 0.5f + new Vector3(0f, 0.5f, 0f);
            _effectManager.PlayParticle("hitTree", particlePos);
            return true;
        }

        return false;
    }

    public void ResetState()
    {
        _state = PlayerState.IDLE;
        _life = 3;
        _curAtkPoint = 0;
        _attackable = true;
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;

        _minZ = 0f;
        nextZ = 0f;

        _UIManager.UpdateLife(_life);
        _UIManager.UpdateAttackable(_attackable);
    }

    private void CameraMove()
    {
        // 플레이어를 고정했음
    }

    private void AssignAnimationIDs()
    {
        _animIDJump = Animator.StringToHash("Jump");
        _animIDHit = Animator.StringToHash("Hit");
        _animIDDeath = Animator.StringToHash("Death");
        _animIDAttack = Animator.StringToHash("Attack");
    }

    #endregion
}
