using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerControler : MonoBehaviour
{
    [Header("Basic Information Player"), Space(5)]
    [SerializeField] private Rigidbody2D rb;

    public float PowerJump = 1.0f;
    public float PowerSpeed = 7.0f;
    public bool canJump = true;
    public bool isAlterne = false;

    [Header("Effect Particle"), Space(5)]
    public GameObject ParticleWalk;

    // Event pour déclencher un son de saut
    public static event Action OnJump;

    private void Start()
    {
        // Si Rigidbody2D vide récupération auto
        if (rb == null)
        {
            rb = this.gameObject.GetComponent<Rigidbody2D>();
        }
        
        // Si Particule Walk non assigné
        if (ParticleWalk == null)
        {
            ParticleWalk = GameObject.Find("WalkEffectPlayer");
        }
    }

    private void Update()
    {
        ControlPlayer();

        if (Input.GetKey(KeyCode.LeftShift))
        {
            PowerSpeed = 9.0f;
        }
        else
        {
            PowerSpeed = 7.0f;
        }

        // Si le joueur bouge
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetButton("Horizontal"))
        {
            ParticleWalkUpdate(true);
        }
        else
        {
            ParticleWalkUpdate(false);
        }

        // Détection si le joueur passe sur l'autre monde
        isAlterne = rb.gravityScale == 1 ? false : true;
    }

    private void FixedUpdate()
    {
        float dirX = Input.GetAxis("Horizontal");
        FlipImage(dirX);

        // Déplacement du joueur
        rb.velocity = new Vector2(dirX * PowerSpeed, rb.velocity.y);
    }


    public void ControlPlayer()
    {
        // Faire sauté le joueur
        if (canJump)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!isAlterne)
                {
                    rb.AddForce(transform.up * PowerJump, ForceMode2D.Impulse);
                }
                else
                {
                    rb.AddForce(-transform.up * -PowerJump, ForceMode2D.Impulse);
                }
                canJump = false;
                // Déclenchement de l'event pour le son
                OnJump?.Invoke();
            }
        }
    }

    // Fonction qui permet de faire tourner le sprite pour lui faire changer de direction
    private void FlipImage(float inputX)
    {
        if (inputX > 0)
        {
            if (!isAlterne)
            {
                Quaternion target = quaternion.Euler(0.0f, 0.0f, 0.0f);
                transform.rotation = Quaternion.Slerp(transform.rotation, target, 2.0f);
            }
            else
            {
                Quaternion target = quaternion.Euler(3.15f, 0.0f, 0.0f);
                transform.rotation = Quaternion.Slerp(transform.rotation, target, 2.0f);
            }
        }
        else if (inputX < 0)
        {
            if (!isAlterne)
            {
                Quaternion target = quaternion.Euler(0.0f, 3.15f, 0.0f);
                transform.rotation = Quaternion.Slerp(transform.rotation, target, 2.0f);
            }
            else
            {
                Quaternion target = quaternion.Euler(3.15f, 3.15f, 0.0f);
                transform.rotation = Quaternion.Slerp(transform.rotation, target, 2.0f);
            }
        }
    }

    // Détection si le joueur bouge la fonction est appelé
    private void ParticleWalkUpdate(bool i)
    {
        if (i && canJump)
        {
            ParticleWalk.SetActive(true);
        }
        else
        {
            ParticleWalk.SetActive(false);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector2 directionJump = new Vector2(0.0f, -2.0f);
        Gizmos.DrawRay(transform.position, directionJump);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!canJump)
        {
            if (!other.gameObject.CompareTag("Player"))
            {
                canJump = true;
            }
        }
    }
}
