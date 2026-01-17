
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;   // nécessaire
using UnityEngine.Rendering;

public class GrapplingGun : MonoBehaviour
{   
    /// <summary>
    /// Gestion du grappin sans le rendu physique et implémentation de la mécanique de tirage de corde pour remonter.
    /// </summary>
    private Vector3 grapplePoint;
    public Transform gunTip, player;
    public GrappleSelector selector;
    private InputAction grappleAction;   // nouvelle action
    private InputAction pullRopeAction;   // nouvelle action
    private bool isGrappling = false;

    //Variables pour tirer sur la corde
    private bool isClimbing;
    public Rope rope;
    public float pullSpeed = 0.00001f; // Vitesse d'augmentation (unité/seconde)
    public Rigidbody playerRigidbody;

    //Variables pour la pause quand on tire sur la corde
    private int nodesRemovedSincePause = 0; // Compteur de nœuds supprimés depuis la dernière pause
    private bool isPaused = false; // État de pause
    public float pauseDuration = 1f; // Durée de la pause en secondes
    private float pauseTimer = 0f; // Timer pour la pause

    void Awake()
    {
        isGrappling = false;
        grapplePoint = gunTip.position;
        playerRigidbody = player.GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        grappleAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/x");
        grappleAction.performed += GrappleManager; // Sans lambda
        grappleAction.Enable();
        
        pullRopeAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/c");
        pullRopeAction.started += _ => isClimbing = true;
        pullRopeAction.canceled += _ => isClimbing = false;
        pullRopeAction.canceled += _ => playerRigidbody.useGravity=true;
        pullRopeAction.Enable();

    }
    void Update()
    {
        if (isGrappling && isClimbing && !isPaused)
        {
            PullRope();
        }

        // Gestion de la pause
        if (isPaused)
        {
            pauseTimer -= Time.deltaTime;
            if (pauseTimer <= 0f)
            {
                isPaused = false;
                pauseTimer=pauseDuration;
            }
        }
    }

    private void OnDisable()
    {
        grappleAction.performed -= GrappleManager;
        pullRopeAction.started -= _ => isClimbing = true;
        pullRopeAction.canceled -= _ => isClimbing = false;
        pullRopeAction.canceled -= _ => playerRigidbody.useGravity=true;

        grappleAction.Disable();
        pullRopeAction.Disable();
    }

    void GrappleManager(InputAction.CallbackContext context)
    {
        isGrappling = !isGrappling;
        if (isGrappling)
        {
            StartGrapple();
        }
        else
        {
            StopGrapple();
        }

    }
    void StartGrapple()
    {
        if (selector == null)
        {
            Debug.LogError("Missing GrappleSelector reference");
            return;
        }

        Transform best = selector.GetSelectedTarget();
        if (best == null)
        {
            Debug.Log("No selected grapple target");
            return;
        }

        grapplePoint = best.position;
        rope.initRope(gunTip, grapplePoint);
        rope.enabled = true;


    }
    void PullRope()
    {
        if (isPaused)
        {
            return; // Bloque toute action pendant la pause
        }

        if(!isGrappling || !isClimbing || rope.getTotalNodes() <2)
        {
            return;
        }
 

        if (!isPaused)
        {
            rope.RemoveFirstNode();
            nodesRemovedSincePause++;

            if (nodesRemovedSincePause >= 5)
            {
                isPaused = true;
                pauseTimer = pauseDuration;
                nodesRemovedSincePause = 0;

            }
        }
        
    }

    void StopGrapple()
    {

        grapplePoint = gunTip.position;
        rope.enabled = false;
    }

}
