using System.Diagnostics;
using UnityEngine;

/// <summary>
/// プレイヤーを追いかける動きをするコンポーネント
/// </summary>
public class FollowPlayer : MonoBehaviour
{
    [Header("設定")]
    [Tooltip("追いかける速度")]
    public float speed = 5f;

    [Tooltip("ジャンプ中かを判定するオブジェクト")]
    public GameObject jumpCheckObject;

    [SerializeField, Tooltip("追跡対象のY座標を固定する値")]
    private float positionY = 0;

    // 内部参照
    private Transform playerTransform;
    private JumpCheck jumpCheck;

    void Start()
    {
        InitializePlayer();
        InitializeJumpCheck();
    }

    /// <summary>
    /// プレイヤーオブジェクトを探して参照をセットする
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
            Debug.LogError("Playerタグのついたオブジェクトが見つかりません！");
        }
    }

    /// <summary>
    /// JumpCheckコンポーネントを取得する
    /// </summary>
    private void InitializeJumpCheck()
    {
        if (jumpCheckObject != null)
        {
            jumpCheck = jumpCheckObject.GetComponent<JumpCheck>();
            if (jumpCheck == null)
            {
                Debug.LogError("jumpCheckObjectにJumpCheckコンポーネントが見つかりません！");
            }
        }
        else
        {
            Debug.LogError("ジャンプチェックオブジェクトが指定されていません！");
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
    /// 自身のY座標を常に固定する
    /// </summary>
    private void FixYPosition()
    {
        Vector3 position = transform.position;
        position.y = positionY;
        transform.position = position;
    }

    /// <summary>
    /// プレイヤーに向かって移動する
    /// </summary>
    private void MoveTowardsPlayer()
    {
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        // 移動後に再度Y座標を固定
        FixYPosition();
    }
}
