using UnityEngine;

public class ClimbController : MonoBehaviour
{
    public float climbSpeed = 5f; // Vitesse de mont�e
    public float maxClimbHeight = 5f; // Hauteur maximale que le joueur peut escalader
    public float climbTime = 2f; // Temps maximum pour escalader

    private float climbTimer = 0f; // Temps �coul� pour escalader
    private bool isClimbing = false; // Indique si le joueur est en train d'escalader
    private bool canClimb = false; // Indique si le joueur est en contact avec une surface escalable

    private Rigidbody2D rb; // R�f�rence au Rigidbody2D du joueur

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); // R�cup�re le Rigidbody2D du joueur
    }

    private void FixedUpdate()
    {
        // V�rifie si le joueur peut escalader et si une touche est appuy�e pour commencer l'escalade
        if (canClimb && Input.GetKeyDown(KeyCode.Space))
        {
            StartClimbing(); // Commence l'escalade
        }

        // Si le joueur est en train d'escalader, met � jour sa position en fonction de la vitesse de mont�e
        if (isClimbing)
        {
            float verticalInput = Input.GetAxisRaw("Vertical"); // R�cup�re l'input vertical
            float climbAmount = climbSpeed * Time.deltaTime * verticalInput; // Calcule la distance de mont�e

            // Si le joueur a atteint la hauteur maximale ou si le temps d'escalade est �coul�, arr�te l'escalade
            if (transform.position.y >= maxClimbHeight || climbTimer >= climbTime)
            {
                StopClimbing();
            }
            else
            {
                // Met � jour la position du joueur
                rb.MovePosition(rb.position + new Vector2(0f, climbAmount));
                climbTimer += Time.deltaTime; // Incr�mente le temps �coul�
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si le joueur entre en contact avec une surface escalable, il peut escalader
        if (collision.CompareTag("Climbable"))
        {
            canClimb = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Si le joueur quitte la surface escalable, il ne peut plus escalader
        if (collision.CompareTag("Climbable"))
        {
            canClimb = false;
            StopClimbing();
        }
    }

    private void StartClimbing()
    {
        isClimbing = true; // Indique que le joueur est en train d'escalader
        climbTimer = 0f; // R�initialise le temps �coul�
        rb.gravityScale = 0f; // D�sactive la gravit� du joueur
    }

    private void StopClimbing()
    {
        isClimbing = false; // Indique que le joueur a arr�t� d'escalader
        rb.gravityScale = 1f; // R�active la gravit� du joueur
    }
}
