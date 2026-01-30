using UnityEngine;
using UnityEngine.InputSystem;

public class GrapplingGun : MonoBehaviour
{
    /// <summary>
    /// Gestion du grappin sans le rendu physique et implémentation de la mécanique de tirage de corde pour remonter.
    /// </summary>
    [SerializeField] private AudioClip cassetteClip;
    [SerializeField] private AudioClip extensionClip;
    [SerializeField] private AudioClip balancierClip;
    [SerializeField] private Transform feet;
    [SerializeField] private LayerMask groundLayer;

    private bool isGrounded;
    private float balancierSoundTimer = 0f;
    private float climbSoundTimer = 0f;

    private Vector3 grapplePoint;
    [SerializeField] private Transform gunTip;
    [SerializeField] private GrappleSelector selector;
    private InputAction grappleAction;  // nouvelle action
    private InputAction pullRopeAction; // nouvelle action
    private InputAction elongateRopeAction; // nouvelle action
    private bool isGrappling = false;
    private bool grappleAllowed = false;

    // Variables pour tirer sur la corde
    private bool isClimbing;
    private bool isElongating;
    [SerializeField] private Rope rope;
    [SerializeField] private Rigidbody playerRigidbody;

    // Variables pour la pause quand on tire sur la corde
    private int nodesRemovedSincePause = 0; // Compteur de nœuds supprimés depuis la dernière pause
    private int nodesAddedSincePause = 0; // Compteur de nœuds ajoutés depuis la dernière pause
    private bool isPaused = false; // État de pause
    [SerializeField] private float pauseDuration = 1f; // Durée de la pause en secondes
    private float pauseTimer = 0f; // Timer pour la pause

    [SerializeField] private GameObject playerObject;

    private void Awake()
    {
        isGrappling = false;
        grapplePoint = gunTip.position;
        playerRigidbody = playerObject.GetComponent<Rigidbody>();

        var playerInput = playerObject.GetComponent<PlayerInput>();
        grappleAction = playerInput.actions["Grapple"];
        pullRopeAction = playerInput.actions["PullRope"];
        elongateRopeAction = playerInput.actions["ElongateRope"];
    }

    private void OnEnable()
    {
        grappleAction.performed += GrappleManager; // Sans lambda
        pullRopeAction.started += _ => isClimbing = true;
        pullRopeAction.canceled += _ => isClimbing = false;
        pullRopeAction.canceled += _ => playerRigidbody.useGravity = true;

        pullRopeAction.Enable();
        grappleAction.Enable();

        elongateRopeAction.started += _ => isElongating = true;
        elongateRopeAction.canceled += _ => isElongating = false;
        elongateRopeAction.canceled += _ => playerRigidbody.useGravity = true;
        elongateRopeAction.Enable();
    }

    private void OnDisable()
    {
        grappleAction.performed -= GrappleManager;
        pullRopeAction.started -= _ => isClimbing = true;
        pullRopeAction.canceled -= _ => isClimbing = false;
        pullRopeAction.canceled -= _ => playerRigidbody.useGravity = true;

        grappleAction.Disable();
        pullRopeAction.Disable();
    }

    private void Update()
    {
        if (balancierSoundTimer > 0)
            balancierSoundTimer -= Time.deltaTime;
        if (isGrappling)
        {
            isGrounded = Physics.CheckSphere(feet.position, 0.2f, groundLayer); // radius of 0.2f
            if (!isGrounded && balancierSoundTimer <= 0)
            {
                SoundFXManager.instance.PlaySoundFXClip(balancierClip, gunTip.transform, 1f);
                balancierSoundTimer = balancierClip.length * 3; // joue le son une fois sur 3
            }
        }
        if (climbSoundTimer > 0)
            climbSoundTimer -= Time.deltaTime;
        if (isGrappling && isClimbing && !isPaused)
            PullRope();
        if (isGrappling && isElongating && !isPaused)
            ElongateRope();
        // Gestion de la pause
        if (isPaused)
        {
            pauseTimer -= Time.deltaTime;
            if (pauseTimer <= 0f)
            {
                isPaused = false;
                pauseTimer = pauseDuration;
            }
        }
    }

    private void GrappleManager(InputAction.CallbackContext context)
    {
        if (!grappleAllowed)
            return;
        isGrappling = !isGrappling;
        if (isGrappling)
            StartGrapple();
        else
            StopGrapple();
    }

    private void StartGrapple()
    {
        if (selector == null)
        {
            Debug.LogError("Missing GrappleSelector reference");
            return;
        }
        var best = selector.GetSelectedTarget();
        if (best == null)
        {
            Debug.Log("No selected grapple target");
            return;
        }
        grapplePoint = best.position;
        rope.InitRope(gunTip, grapplePoint);
        SoundFXManager.instance.PlaySoundFXClip(cassetteClip, transform, 1f);
        rope.enabled = true;
    }

    private void PullRope()
    {
        if (rope.GetTotalNodes() < 2)
            return;
        rope.RemoveFirstNode();
        nodesRemovedSincePause++;
        if (nodesRemovedSincePause >= 10)
        {
            isPaused = true;
            pauseTimer = pauseDuration;
            nodesRemovedSincePause = 0;
        }
    }

    private void ElongateRope()
    {
        if (rope.IsMaxLength())
            return;
        rope.AddNode();
        nodesAddedSincePause++;
        if (nodesAddedSincePause >= 10)
        {
            isPaused = true;
            pauseTimer = pauseDuration;
            nodesAddedSincePause = 0;
        }
    }

    private void StopGrapple()
    {
        grapplePoint = gunTip.position;
        rope.enabled = false;
    }

    public void SetGrappleAllowed(bool value)
    {
        grappleAllowed = value;
        if (!value)
            ForceStopGrapple();
    }

    private void ForceStopGrapple()
    {
        isGrappling = false;
        isClimbing = false;
        isPaused = false;
        nodesRemovedSincePause = 0;
        if (rope != null)
            rope.enabled = false;
    }
}
