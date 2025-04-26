using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

/// <summary>
/// �{�X�u�S�[�����v�̍s���Ǘ��N���X
/// </summary>
public class Boss_Golem : MonoBehaviour
{
    [Header("�Q�[���N���A����")]
    public ToGameClerScene gameClerScene;

    [Header("�U���G�t�F�N�g�֘A")]
    public GameObject attackEffectPrefab1, attackEffectPrefab2, attackEffectPrefab3;
    private ParticleSystem AttackEffect1, AttackEffect2, AttackEffect3;

    [Header("�U���R���C�_�[")]
    [SerializeField] protected Collider Attack1, Attack2, Attack3;

    [Header("�U������")]
    [SerializeField] private AudioClip AttackSound1, AttackSound2, AttackSound3;
    private AudioSource audioSource;

    [Header("�̗͊Ǘ�")]
    public HealthManager m_Health;
    [SerializeField] protected int damageAmount = 1;

    [Header("�U���^�[�Q�b�g�ʒu�Ȃ�")]
    public Transform Hand, Mouth;
    public Transform jumpTarget;
    public Transform laser_spot;

    [Header("�W�����v�E�`���[�W�ݒ�")]
    public float height = 2f, timeToTarget = 1.5f;
    [SerializeField] private float chargeRange;
    [SerializeField] private bool isJumping = false;
    [SerializeField] private bool isCharging = false;
    [SerializeField] private bool isChasing = false;
    [SerializeField] private bool isWaiting = false;
    private Vector3 initialPosition;
    private float startTime;

    [Header("�W�����v�`�F�b�N")]
    public GameObject jumpCheckObject;
    private JumpCheck jumpCheck;

    [Header("�e���ːݒ�")]
    public int bulletCount = 5;
    public float fireInterval = 0.5f, bulletSpeed = 10f;

    [Header("�ǐՐݒ�")]
    public float moveSpeed = 2.0f, rotationSpeed = 2.0f;
    [SerializeField] private float minDistanceToPlayerForAttack1 = 1.5f;

    private Animator m_Animator;

    void Start()
    {
        m_Animator = GetComponent<Animator>();
        InitializeJumpCheck();
        InitializeAudioSource();
        InitializeAttackEffects();
    }

    void Update()
    {
        if (isWaiting) return;

        ChooseAction();

        if (isJumping) PerformJump();
        if (isCharging) PerformCharge();
        if (isChasing) PerformChase();
    }

    #region ���������\�b�h

    void InitializeJumpCheck()
    {
        if (jumpCheckObject != null)
        {
            jumpCheck = jumpCheckObject.GetComponent<JumpCheck>();
            if (jumpCheck == null)
                Debug.LogError("jumpCheckObject��JumpCheck�R���|�[�l���g��������܂���I");
        }
        else
        {
            Debug.LogError("�W�����v�`�F�b�N�I�u�W�F�N�g���w�肳��Ă��܂���I");
        }
    }

    void InitializeAudioSource()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void InitializeAttackEffects()
    {
        AttackEffect1 = InstantiateEffect(attackEffectPrefab1);
        AttackEffect2 = InstantiateEffect(attackEffectPrefab2);
        AttackEffect3 = InstantiateEffect(attackEffectPrefab3);

        AttackEffect1.gameObject.SetActive(false);
        AttackEffect2.gameObject.SetActive(false);
        AttackEffect3.gameObject.SetActive(false);
    }

    ParticleSystem InstantiateEffect(GameObject prefab)
    {
        GameObject instance = Instantiate(prefab, transform.position, Quaternion.identity);
        return instance.GetComponent<ParticleSystem>();
    }

    #endregion

    #region ���C���s���Ǘ�

    void ChooseAction()
    {
        isWaiting = true;

        float rand = Random.value;
        if (rand < 0.5f) StartChase();
        else if (rand < 0.75f) StartJump();
        else StartCharge();
    }

    void StartJump()
    {
        initialPosition = transform.position;
        startTime = Time.time;
        isJumping = true;
        m_Animator.SetBool("IsJumping", true);
        if (jumpCheck != null) jumpCheck.isJumping = true;
    }

    void PerformJump()
    {
        float timeSinceStart = Time.time - startTime;
        if (timeSinceStart <= timeToTarget)
        {
            float t = timeSinceStart / timeToTarget;
            float yOffset = height * 4 * (t - t * t);
            Vector3 currentPosition = Vector3.Lerp(initialPosition, jumpTarget.position, t);
            currentPosition.y += yOffset;
            transform.position = currentPosition;
        }
        else
        {
            transform.position = jumpTarget.position;
            isJumping = false;
            m_Animator.SetBool("IsJumping", false);
            m_Animator.SetInteger("Attack", 2);
            if (jumpCheck != null) jumpCheck.isJumping = false;
        }
    }

    void StartCharge()
    {
        if (Vector3.Distance(transform.position, laser_spot.position) <= chargeRange)
        {
            m_Animator.SetInteger("Attack", 3);
        }
        else
        {
            initialPosition = transform.position;
            startTime = Time.time;
            isCharging = true;
            m_Animator.SetBool("IsJumping", true);
            if (jumpCheck != null) jumpCheck.isJumping = true;
        }
    }

    void PerformCharge()
    {
        float timeSinceStart = Time.time - startTime;
        if (timeSinceStart <= timeToTarget)
        {
            float t = timeSinceStart / timeToTarget;
            float yOffset = height * 4 * (t - t * t);
            Vector3 currentPosition = Vector3.Lerp(initialPosition, laser_spot.position, t);
            currentPosition.y += yOffset;
            transform.position = currentPosition;
        }
        else
        {
            transform.position = laser_spot.position;
            isCharging = false;
            m_Animator.SetBool("IsJumping", false);
            RotateTowards(jumpTarget.position);
            m_Animator.SetInteger("Attack", 3);
            if (jumpCheck != null) jumpCheck.isJumping = false;
        }
    }

    void StartChase()
    {
        if (jumpTarget == null)
        {
            Debug.LogError("�W�����v�^�[�Q�b�g�����ݒ�I");
            return;
        }

        isChasing = true;
        m_Animator.SetBool("IsChasing", true);
    }

    void PerformChase()
    {
        Vector3 direction = (jumpTarget.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        if (Vector3.Distance(transform.position, jumpTarget.position) < minDistanceToPlayerForAttack1)
        {
            isChasing = false;
            m_Animator.SetBool("IsChasing", false);
            m_Animator.SetInteger("Attack", 1);
        }
    }

    #endregion

    #region �U�����\�b�h

    public void Attack1_Start() => EnableCollider(Attack1);

    public void Attack1_Effect() => PlayAttackEffect(AttackEffect1, Hand.position, Hand.rotation, AttackSound1);

    public void Attack2_Start()
    {
        EnableCollider(Attack2);
        PlayAttackEffect(AttackEffect2, transform.position, transform.rotation, AttackSound2);
    }

    public void Attack3_Start()
    {
        isCharging = false;
        if (Attack3 != null)
            StartCoroutine(FireBullets());
        else
            Debug.LogError("Attack3�̃R���C�_�[���ݒ肳��Ă��܂���I");
    }

    IEnumerator FireBullets()
    {
        for (int i = 0; i < bulletCount; i++)
        {
            yield return new WaitForSeconds(fireInterval);
            GameObject bullet = Instantiate(hitBox, Mouth.position, Mouth.rotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();

            if (rb != null)
            {
                Vector3 direction = (jumpTarget.position - Mouth.position).normalized;
                direction.y = 0;
                rb.velocity = direction * bulletSpeed;
            }
        }
    }

    public void Attack3_Effect() => PlayAttackEffect(AttackEffect3, Mouth.position, Mouth.rotation, AttackSound3);

    void EnableCollider(Collider collider)
    {
        if (collider != null)
            collider.enabled = true;
    }

    void PlayAttackEffect(ParticleSystem effect, Vector3 position, Quaternion rotation, AudioClip sound)
    {
        effect.transform.position = position;
        effect.transform.rotation = rotation;
        effect.gameObject.SetActive(true);
        effect.Play();
        audioSource.PlayOneShot(sound);
    }

    #endregion

    #region �_���[�W�E���S����

    public void TakeDamage()
    {
        if (m_Health == null)
        {
            Debug.LogError("HealthManager�����ݒ�I");
            return;
        }

        m_Health.TakeDamage(damageAmount);

        if (m_Health.currentHealth > 0)
        {
            Debug.Log("�_���[�W���󂯂��I");
        }
        else
        {
            m_Animator.SetBool("Down", true);
            Debug.Log("�S�[�������|�ꂽ�I");
        }
    }

    public void Die()
    {
        if (gameClerScene != null)
            gameClerScene.TriggerGameClear();
        else
            Debug.LogError("�Q�[���N���A�V�[���ւ̎Q�Ƃ�����܂���I");

        Destroy(this);
    }

    public void DiePoint()
    {
        m_Animator.SetBool("Living", false);
    }

    #endregion

    #region ���̑�

    public void Attack_End()
    {
        m_Animator.SetInteger("Attack", 0);

        DisableAttack(Attack1, AttackEffect1);
        DisableAttack(Attack2, AttackEffect2);
        DisableAttack(Attack3, AttackEffect3);

        isJumping = false;
        isCharging = false;
        isChasing = false;

        RotateTowards(jumpTarget.position);
        isWaiting = false;
    }

    void DisableAttack(Collider collider, ParticleSystem effect)
    {
        if (collider != null) collider.enabled = false;
        if (effect != null) effect.gameObject.SetActive(false);
    }

    public void WaitingFalse()
    {
        isWaiting = false;
    }

    void RotateTowards(Vector3 targetPosition)
    {
        Vector3 lookDirection = new Vector3(targetPosition.x, transform.position.y, targetPosition.z) - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    #endregion
}
