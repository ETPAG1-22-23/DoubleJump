using UnityEngine;

public class ClimbController : MonoBehaviour
{
    public float climbSpeed = 5f; // Vitesse de montée
    public float maxClimbHeight = 5f; // Hauteur maximale que le joueur peut escalader
    public float climbTime = 2f; // Temps maximum pour escalader

    private float climbTimer = 0f; // Temps écoulé pour escalader
    private bool isClimbing = false; // Indique si le joueur est en train d'escalader
    private bool canClimb = false; // Indique si le joueur est en contact avec une surface escalable

    private Rigidbody2D rb; // Référence au Rigidbody2D du joueur

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); // Récupère le Rigidbody2D du joueur
    }

    private void FixedUpdate()
    {
        // Vérifie si le joueur peut escalader et si une touche est appuyée pour commencer l'escalade
        if (canClimb && Input.GetKeyDown(KeyCode.Space))
        {
            StartClimbing(); // Commence l'escalade
        }

        // Si le joueur est en train d'escalader, met à jour sa position en fonction de la vitesse de montée
        if (isClimbing)
        {
            float verticalInput = Input.GetAxisRaw("Vertical"); // Récupère l'input vertical
            float climbAmount = climbSpeed * Time.deltaTime * verticalInput; // Calcule la distance de montée

            // Si le joueur a atteint la hauteur maximale ou si le temps d'escalade est écoulé, arrête l'escalade
            if (transform.position.y >= maxClimbHeight || climbTimer >= climbTime)
            {
                StopClimbing();
            }
            else
            {
                // Met à jour la position du joueur
                rb.MovePosition(rb.position + new Vector2(0f, climbAmount));
                climbTimer += Time.deltaTime; // Incrémente le temps écoulé
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
        climbTimer = 0f; // Réinitialise le temps écoulé
        rb.gravityScale = 0f; // Désactive la gravité du joueur
    }

    private void StopClimbing()
    {
        isClimbing = false; // Indique que le joueur a arrêté d'escalader
        rb.gravityScale = 1f; // Réactive la gravité du joueur
    }
}
