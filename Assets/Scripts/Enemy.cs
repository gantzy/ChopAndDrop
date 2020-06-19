using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class Enemy : MonoBehaviourPun
{
    [Header("Info")]
    public string enemyName;
    public float moveSpeed;
    public int curHP;
    public int maxHP;

    public float chaseRange;
    public float attackRange;

    private PlayerController targetPlayer;

    public float playerDetectRate = 0.2f;
    private float lastPlayerDetectTime;

    public string objectToSpawnOnDeath;

    [Header("Attack")]
    public int damage;
    public float attackRate;
    private float lastAttackTime;

    [Header("Components")]
    public HeaderInfo healthBar;
    public SpriteRenderer sr;
    public Rigidbody2D rig;

    private void Start()
    {
        healthBar.Initialize(enemyName, maxHP);
    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (targetPlayer != null)
        {
            // calculate distance
            float dist = Vector2.Distance(transform.position, targetPlayer.transform.position);
            if (dist < attackRange && Time.time - lastAttackTime >= attackRate)
                Attack();
            else if (dist > attackRange)
            {
                Vector3 dir = targetPlayer.transform.position - transform.position;
                rig.velocity = dir.normalized * moveSpeed;
            }
            else
            {
                rig.velocity = Vector2.zero;
            }
        }
        else
        {
            rig.velocity = Vector2.zero;
        }

        DetectPlayer();
    }

    void Attack()
    {
        lastAttackTime = Time.time;
        targetPlayer.photonView.RPC("TakeDamage", targetPlayer.photonPlayer, damage);
    }

    void DetectPlayer()
    {
        if(Time.time - lastPlayerDetectTime > playerDetectRate)
        {
            lastPlayerDetectTime = Time.time;

            // loop through players
            foreach(PlayerController player in GameManager.instance.players)
            {

                // calc distance between enemy and player
                float dist = Vector2.Distance(transform.position, player.transform.position);

                if (player == targetPlayer)
                {
                    if (dist > chaseRange)
                        targetPlayer = null;
                }
                else if (dist < chaseRange)
                {
                    if (targetPlayer == null)
                        targetPlayer = player;
                }
            }
        }
    }

    [PunRPC]
    public void TakeDamage(int damage)
    {
        curHP -= damage;
        healthBar.photonView.RPC("UpdateHealthBar", RpcTarget.All, curHP);

        if (curHP <= 0)
            Die();
        else
            photonView.RPC("FlashDamage", RpcTarget.All);
    }

    [PunRPC]
    public void FlashDamage()
    {
        StartCoroutine(DamageFlash());

        IEnumerator DamageFlash()
        {
            sr.color = Color.red;
            yield return new WaitForSeconds(0.05f);
            sr.color = Color.white;
        }
    }

    void Die()
    {
        if (objectToSpawnOnDeath != string.Empty)
            PhotonNetwork.Instantiate(objectToSpawnOnDeath, transform.position, Quaternion.identity);

        PhotonNetwork.Destroy(gameObject);
    }

}
