using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    public float MoveSpeed = 1;
    public float MaxMoveSpeed = 2.7f;
    public float DamageForce = 2;
    public float HitInputDisableLength;
    public float m_arrowSpeed = 30;
    public float m_arrowLifeSpan = 3;
    public float m_attackCooldown = 1;
    [HideInInspector] public float Health = 100;
    [SerializeField] Light2D Torch;
    [SerializeField] Transform PlayerSpawn;
    [SerializeField] GameObject Arrow;

    bool m_holdingMoveKey;
    bool m_holdingAttackKey;
    bool m_HUDDirty;
    float m_lastAttackTime;
    HUD HUD;
    PlayerControls controls;
    Rigidbody2D m_Rigid;
    Vector2 m_MoveValue;

    public void ReceiveDamage(int damage)
    {
        Debug.Log("Player Received Damage");
        Health -= damage;
        m_HUDDirty = true;
    }

    public void PushAway(Vector2 collisionPoint)
    {
        StartCoroutine(DisablePlayerInput());
        Vector2 dir = (Vector2)transform.position - collisionPoint;
        m_Rigid.velocity = Vector2.zero;
        m_Rigid.AddForceAtPosition(dir * DamageForce, collisionPoint, ForceMode2D.Impulse);
    }

    private void Awake()
    {
        instance = this;
        controls = new PlayerControls();
        HUD = GameObject.FindGameObjectWithTag("HUD").GetComponent<HUD>();
        // Movement
        controls.Player.Movement.started += Movement_started;
        controls.Player.Movement.performed += Movement_performed;
        controls.Player.Movement.canceled += Movement_canceled;
        // Torch
        controls.Player.TorchToggle.performed += TorchToggle_performed;
        // Attack
        controls.Player.Attack.started += Attack_started;
        controls.Player.Attack.canceled += Attack_canceled;

        transform.position = PlayerSpawn.position;
    }

    private void Attack_started(InputAction.CallbackContext obj)
    {
        m_holdingAttackKey = true;
    }

    private void Attack_canceled(InputAction.CallbackContext obj)
    {
        m_holdingAttackKey = false;
    }

    private void TorchToggle_performed(InputAction.CallbackContext obj)
    {
        Torch.enabled = !Torch.enabled;
    }

    private void Start()
    {
        m_Rigid = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (m_HUDDirty)
        {
            RefreshHUD();
        }
    }

    private void FixedUpdate()
    {
        AddMovement();
        Attack();
    }

    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    private void ShootArrow()
    {
        Vector2 mousePoint = (Vector2)Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 direction = mousePoint - (Vector2)transform.position;
        GameObject arrow = Instantiate(Arrow, transform.position, Quaternion.identity);
        float angle = Vector3.SignedAngle(Vector3.up, direction, Vector3.forward);
        arrow.transform.RotateAround(arrow.transform.position, Vector3.forward, angle);
        arrow.SetActive(true);
        arrow.GetComponent<Rigidbody2D>().AddForce(direction.normalized * m_arrowSpeed);
        Destroy(arrow, m_arrowLifeSpan);
    }

    private void AddMovement()
    {
        if (m_holdingMoveKey)
        {
            var velocity = m_Rigid.velocity + m_MoveValue * MoveSpeed;
             m_Rigid.velocity = Vector3.ClampMagnitude(velocity, MaxMoveSpeed);
            //print("Move= "+MoveSpeed * m_MoveValue);
            //print("Velocity= " + m_Rigid.velocity);
        }
    }

    private void Attack()
    {
        if (m_holdingAttackKey)
        {
            if (m_lastAttackTime + m_attackCooldown < Time.time)
            {
                m_lastAttackTime = Time.time;
                ShootArrow();
            }
        }
    }

    private void RefreshHUD()
    {
        m_HUDDirty = false;
        HUD.Refresh();
    }

    IEnumerator DisablePlayerInput()
    {
        controls.Player.Disable();
        yield return new WaitForSeconds(HitInputDisableLength);
        controls.Player.Enable();
    }

    private void Movement_started(InputAction.CallbackContext obj)
    {
        m_holdingMoveKey = true;
    }

    private void Movement_performed(InputAction.CallbackContext obj)
    {
        m_MoveValue = obj.ReadValue<Vector2>();
        m_Rigid.AddForce(m_MoveValue * MoveSpeed);
    }

    private void Movement_canceled(InputAction.CallbackContext obj)
    {
        m_holdingMoveKey = false;
        m_Rigid.velocity = Vector2.zero;
    }
}
