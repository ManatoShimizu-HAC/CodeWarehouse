using System.Diagnostics;
using UnityEngine;

/// <summary>
/// �v���C���[��ǂ������铮��������R���|�[�l���g
/// </summary>
public class FollowPlayer : MonoBehaviour
{
    [Header("�ݒ�")]
    [Tooltip("�ǂ������鑬�x")]
    public float speed = 5f;

    [Tooltip("�W�����v�����𔻒肷��I�u�W�F�N�g")]
    public GameObject jumpCheckObject;

    [SerializeField, Tooltip("�ǐՑΏۂ�Y���W���Œ肷��l")]
    private float positionY = 0;

    // �����Q��
    private Transform playerTransform;
    private JumpCheck jumpCheck;

    void Start()
    {
        InitializePlayer();
        InitializeJumpCheck();
    }

    /// <summary>
    /// �v���C���[�I�u�W�F�N�g��T���ĎQ�Ƃ��Z�b�g����
    /// </summary>
    private void InitializePlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Player�^�O�̂����I�u�W�F�N�g��������܂���I");
        }
    }

    /// <summary>
    /// JumpCheck�R���|�[�l���g���擾����
    /// </summary>
    private void InitializeJumpCheck()
    {
        if (jumpCheckObject != null)
        {
            jumpCheck = jumpCheckObject.GetComponent<JumpCheck>();
            if (jumpCheck == null)
            {
                Debug.LogError("jumpCheckObject��JumpCheck�R���|�[�l���g��������܂���I");
            }
        }
        else
        {
            Debug.LogError("�W�����v�`�F�b�N�I�u�W�F�N�g���w�肳��Ă��܂���I");
        }
    }

    void Update()
    {
        FixYPosition();

        if (playerTransform != null && (jumpCheck == null || !jumpCheck.isJumping))
        {
            MoveTowardsPlayer();
        }
    }

    /// <summary>
    /// ���g��Y���W����ɌŒ肷��
    /// </summary>
    private void FixYPosition()
    {
        Vector3 position = transform.position;
        position.y = positionY;
        transform.position = position;
    }

    /// <summary>
    /// �v���C���[�Ɍ������Ĉړ�����
    /// </summary>
    private void MoveTowardsPlayer()
    {
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        // �ړ���ɍēxY���W���Œ�
        FixYPosition();
    }
}
