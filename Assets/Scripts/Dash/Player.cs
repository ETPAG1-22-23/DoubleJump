using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D rb;
    SpriteRenderer sr;
    Animator animController;
    float horizontal_value;
    Vector2 ref_velocity = Vector2.zero;


    float jumpForce = 10f;

    [SerializeField] float moveSpeed_horizontal = 400.0f;
    [SerializeField] bool is_jumping = false;
    [SerializeField] bool can_jump = false;
    [Range(0, 1)] [SerializeField] float smooth_time = 1f;
    [SerializeField] private TrailRenderer tr;




    [SerializeField] bool IsGrounded = false;
    [SerializeField] int CountJump = 2;
    private int LastPressedJumpTime = 0;
    private int LastOnGroundTime = 0;
    [SerializeField] private Transform shootingPoint;
    [SerializeField] private Transform gun;
    

    // Vitesse de déplacement du joueur
    public float moveSpeed = 5f;
    // Distance maximale de déplacement lors du dash
    public float dashDistance = 5f;
    // Temps de recharge du dash en secondes
    public float dashCooldown = 1f;
    // Direction du dash
    private Vector2 dashDirection;
    // Temps restant avant de pouvoir utiliser le dash à nouveau
    private float dashCooldownTimer = 0f;




    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animController = GetComponent<Animator>();
        //Debug.Log(Mathf.Lerp(current, target, 0));
    }

    // Update is called once per frame
    void Update()
    {

        // Si le dash est en cours de recharge, mettre à jour le temps restant
        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
        }

        // Sinon, permettre au joueur d'utiliser le dash
        else
        {
            // Récupérer le vecteur de direction
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            Vector2 direction = new Vector2(horizontal, vertical).normalized;

            // Si le joueur appuie sur la touche de dash et que la direction est valide
            if (Input.GetKeyDown(KeyCode.LeftShift) && direction != Vector2.zero)
            {
                // Normaliser la direction pour éviter les déplacements excessifs en diagonale
                if (direction.magnitude > 1f)
                {
                    direction.Normalize();
                }

                // Stocker la direction pour le dash
                dashDirection = direction;

                // Mettre en place le temps de recharge du dash
                dashCooldownTimer = dashCooldown;
                tr.emitting = true;
            }
        }


        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            tr.emitting = false;
        }

        if (horizontal_value > 0)
        {
            sr.flipX = false;
            shootingPoint.localPosition = new Vector2(0.93f, -0.61f); // Comme le gun.localPosition mais avec des coord. differentes
            gun.localPosition = new Vector2(0.5f, -0.61f); // change sa position par rapport a ses parents en un nouveau vecteur qui as des coord. opposes a l'autre
        }
        else if (horizontal_value < 0)
        {
            sr.flipX = true;


            shootingPoint.localPosition = new Vector2(-0.93f, -0.61f); // Comme le gun.localPosition mais avec des coord. differentes

            gun.localPosition = new Vector2(-0.5f, -0.61f); // Change sa position par rapport a ses parents en un nouveau vecteur qui as des coord. opposes a l'autre
        }

        horizontal_value = Input.GetAxis("Horizontal");

        if (horizontal_value > 0) sr.flipX = false;
        else if (horizontal_value < 0) sr.flipX = true;

        animController.SetFloat("Speed", Mathf.Abs(horizontal_value));


        if (Input.GetKeyDown(KeyCode.Space) && CountJump > 0)
        {
            Jump();

        }

        
           

            if (Input.GetButtonDown("Jump") && can_jump)
            {
                is_jumping = true;
                animController.SetBool("Jumping", true);
            }
    }


    void Jump()
    {
        // Garantit que nous ne pouvons pas appeler Jump plusieurs fois à partir d'une seule pression
        LastPressedJumpTime = 0;
        LastOnGroundTime = 0;
        CountJump -= 1;

        // On augmente la force appliquée si on tombe
        // Cela signifie que nous aurons toujours l'impression de sauter le même montant
        float force = jumpForce;
        if (rb.velocity.y < 0)
            force -= rb.velocity.y;


        rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);

    }





    void FixedUpdate()
    {

        // Si le joueur est en train de dasher, le déplacer
        if (dashDirection != Vector2.zero)
        {
            //transform.position += dashDirection * dashDistance; //ca marche aussi avec ca mais ca te tp dans le sol

            //rb.velocity = dashDirection * dashDistance / Time.fixedDeltaTime; //envoie trop loin

            Vector2 targetPosition = rb.position + dashDirection * dashDistance; //CA MARCHE FINALEMENT

            rb.MovePosition(targetPosition);

            // Réinitialiser la direction du dash
            dashDirection = Vector2.zero;
        }

        else
        {
            Vector2 target_velocity = new Vector2(horizontal_value * moveSpeed_horizontal * Time.fixedDeltaTime, rb.velocity.y);
        rb.velocity = Vector2.SmoothDamp(rb.velocity, target_velocity, ref ref_velocity, 0.05f);
        
        }
        

    }
    private void OnTriggerStay2D(Collider2D collision)
    {

        animController.SetBool("Jumping", false);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        animController.SetBool("Jumping", false);
        IsGrounded = true;
        CountJump = 2; //reset double saut quand on touche le sol
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        animController.SetBool("Jumping", true);
        IsGrounded = false;
    }
    



}