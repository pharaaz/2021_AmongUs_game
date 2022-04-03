using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class AU_PlayerController : MonoBehaviour, IPunObservable
{
    [SerializeField] public Text playerTextField;

    [SerializeField] bool hasControl;
    public static AU_PlayerController localPlayer;

    public int tasksCompleted = 0;

    //Components
    Rigidbody myRB;
    Animator myAnim;
    Transform myAvatar;

    //Player movement
    [SerializeField] InputAction WASD;
    Vector2 movementInput;
    [SerializeField] float movementSpeed;

    float direction = 1;
    //Player Color
    static Color myColor;
    Vector3 colorVector;
    Color syncColor;
    SpriteRenderer myAvatarSprite;

    //Role
    [SerializeField] bool isAngel;
    [SerializeField] InputAction REVIVE;
    float reviveInput;
    [SerializeField] bool isImposter;
    static int imposterNumber;
    static bool imposterAssigned;
    [SerializeField] bool isInspector;
    [SerializeField] InputAction INSPECTT;
    float inspecttInput;
    bool isInspectAni;
    [SerializeField] InputAction KILL;
    float killInput;
    List<AU_PlayerController> targets;
    [SerializeField] Collider myCollider;
    bool isDead;
    [SerializeField] GameObject bodyPrefab;
    public static List<Transform> allBodies;
    List<Transform> bodiesFound;
    [SerializeField] InputAction REPORT;
    [SerializeField] LayerMask ignoreForBody;

    //Interaction
    [SerializeField] InputAction MOUSE;
    Vector2 mousePositionInput;
    Camera myCamera;
    [SerializeField] InputAction INTERACTION;
    [SerializeField] LayerMask interactLayer;

    //Networking
    public PhotonView myPV;
    [SerializeField] GameObject lightMask;

    public static List<AU_PlayerController> playersInGame;
    public static List<string> playerNames;

    private void Awake()
    {
        KILL.performed += KillTarget;
        INTERACTION.performed += Interact;
        REVIVE.performed += ReviveTarget;
        INSPECTT.performed += InspecttTarget;
    }
    private void OnEnable()
    {
        WASD.Enable();
        KILL.Enable();
        REPORT.Enable();
        MOUSE.Enable();
        INTERACTION.Enable();
        REVIVE.Enable();
        INSPECTT.Enable();
    }
    private void OnDisable()
    {
        WASD.Disable();
        KILL.Disable();
        REPORT.Disable();
        MOUSE.Disable();
        INTERACTION.Disable();
        REVIVE.Disable();
        INSPECTT.Disable();
    }
    // Start is called before the first frame update
    void Start()
    {
        myPV = GetComponent<PhotonView>();

        if (myPV.IsMine)
        {
            localPlayer = this;
            Debug.Log("Player localised");
            playerTextField.text = PhotonNetwork.NickName;
        }
        else
        {
            playerTextField.text = myPV.Owner.NickName;
        }
        myCamera = transform.GetChild(1).GetComponent<Camera>();
        targets = new List<AU_PlayerController>();
        myRB = GetComponent<Rigidbody>();
        myAnim = GetComponent<Animator>();
        myAvatar = transform.GetChild(0);
        myAvatarSprite = myAvatar.GetComponent<SpriteRenderer>();
        if (!myPV.IsMine)
        {
            myCamera.gameObject.SetActive(false);
            lightMask.SetActive(false);
            return;
        }
        if (myColor == Color.clear)
            myColor = Color.white;
        myAvatarSprite.color = myColor;
        if (allBodies == null)
        {
            allBodies = new List<Transform>();
        }
        bodiesFound = new List<Transform>();

        playersInGame = new List<AU_PlayerController>();
        playerNames = new List<string>();

    }
    // Update is called once per frame
    void Update()
    {
        if ((playersInGame != null && this != null) && !playersInGame.Contains(this))
        {
            playersInGame.Add(this);
        }

        Debug.Log("Tasks done = " + tasksCompleted);

        BecomeImposter(imposterNumber);

        if (checkCrewmateVictory())
        {
            Debug.Log("Crewmates Win!");
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.LoadLevel(3);
        }

        if (checkMafiaVictory())
        {
            Debug.Log("Mafia Wins!");
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.LoadLevel(4);
        }

        myAvatar.localScale = new Vector2(direction, 1);

        if (!myPV.IsMine)
        {
            myAvatarSprite.color = syncColor;
            return;
        }

        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            myCamera.enabled = false;
        }

        if (SceneManager.GetActiveScene().buildIndex == 2)
        {

            if (myPV.IsMine)
            {
                this.myCamera.enabled = true;
            }


        }

        movementInput = WASD.ReadValue<Vector2>();
        myAnim.SetFloat("Speed", movementInput.magnitude);

        if (movementInput.x != 0)
        {
            direction = Mathf.Sign(movementInput.x);
            isInspectAni = false;
            myAnim.SetBool("isInspectAni", isInspectAni);
        }
        if (movementInput.y != 0)
        {
            isInspectAni = false;
            myAnim.SetBool("isInspectAni", isInspectAni);
        }
        if (allBodies.Count > 0)
        {
            BodySearch();
        }

        if (REPORT.triggered)
        {
            if (bodiesFound.Count == 0)
                return;
            Transform tempBody = bodiesFound[bodiesFound.Count - 1];
            allBodies.Remove(tempBody);
            bodiesFound.Remove(tempBody);
            tempBody.GetComponent<AU_Body>().Report();
        }
        mousePositionInput = MOUSE.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        if (!myPV.IsMine)
            return;
        myRB.velocity = movementInput * movementSpeed;
    }

    public void SetColor(Color newColor)
    {
        myColor = newColor;
        if (myAvatarSprite != null)
        {
            myAvatarSprite.color = myColor;
        }
    }


    public void SetRole(bool newRole)
    {
        isImposter = newRole;
        isAngel = newRole;
        isInspector = newRole;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            AU_PlayerController tempTarget = other.GetComponent<AU_PlayerController>();
            if (isImposter)
            {
                if (tempTarget.isImposter)
                    return;
                else
                {
                    if (targets != null)
                    {
                        targets.Add(tempTarget);
                    }


                }
            }
            if (isAngel)
            {

                // if (tempTarget.isDead != false)
                //     return;
                // else
                // {
                targets.Add(tempTarget);

                //}
            }
            if (isInspector)
            {
                if (tempTarget.isImposter)
                    targets.Add(tempTarget);
                else
                {
                    return;

                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            AU_PlayerController tempTarget = other.GetComponent<AU_PlayerController>();
            if (targets.Contains(tempTarget))
            {
                targets.Remove(tempTarget);
            }
        }
    }

    private void InspecttTarget(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            //Debug.Log(targets.Count);
            if (targets.Count == 0)
                return;
            else
            {

                if (targets[targets.Count - 1].isImposter!=false)
                {
                    return;
                }
               // transform.position = targets[targets.Count - 1].transform.position;

                //Thread worker1 = new Thread(targets[targets.Count - 1].Inspectyy());
                // Thread worker2 = new Thread(targets[targets.Count - 1].changeInpect());

                // worker1.Start();
                // worker2.Start();

                targets[targets.Count - 1].myPV.RPC("RPC_Inspec", RpcTarget.All);
                // targets[targets.Count - 1].Inspectyy();
                // targets[targets.Count - 1].changeInpect();
                // targets.RemoveAt(targets.Count - 1);
            }
        }
    }

    public void Inspectyy()
    {
        isInspectAni = true;
        myAnim.SetBool("isInspectAni", isInspectAni);
        gameObject.layer = 9;
        // myCollider.enabled = true;
    }

    private void ReviveTarget(InputAction.CallbackContext context)
    {

        if (!myPV.IsMine)
        {
            return;
        }
        if (!isAngel)
        {
            return;
        }

        if (context.phase == InputActionPhase.Performed)
        {
            if (targets.Count == 0)
                return;
            else
            {
                if (targets[targets.Count - 1].isDead)
                    return;
                transform.position = targets[targets.Count - 1].transform.position;
                // targets[targets.Count - 1].Die();

                targets[targets.Count - 1].myPV.RPC("RPC_Revive", RpcTarget.All);

                targets.RemoveAt(targets.Count - 1);
            }
        }
    }

    void KillTarget(InputAction.CallbackContext context)
    {

        if (!myPV.IsMine)
        {
            return;
        }
        if (!isImposter)
        {
            return;
        }

        if (context.phase == InputActionPhase.Performed)
        {
            if (targets.Count == 0)
                return;
            else
            {
                if (targets[targets.Count - 1].isDead)
                    return;
                transform.position = targets[targets.Count - 1].transform.position;
                // targets[targets.Count - 1].Die();
                targets[targets.Count - 1].myPV.RPC("RPC_Kill", RpcTarget.All);
                targets.RemoveAt(targets.Count - 1);
            }
        }
    }

    [PunRPC]
    void RPC_Inspec()
    {
        Inspectyy();
    }

    [PunRPC]
    void RPC_Revive()
    {
        Revivve();
    }

    [PunRPC]
    void RPC_Kill()
    {
        Die();
    }

    public void Revivve()
    {

        if (!myPV.IsMine)
        {
            return;
        }
        //AU_Body tempBody = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "AU_Body"), transform.position, transform.rotation).GetComponent<AU_Body>();
        // tempBody.SetColor(myAvatarSprite.color);
        isDead = false;
        myAnim.SetBool("IsDead", isDead);
        gameObject.layer = 9;
        myCollider.enabled = false;

    }

    public void Die()
    {
        if (!myPV.IsMine)
        {
            return;
        }
        AU_Body tempBody = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "AU_Body"), transform.position, transform.rotation).GetComponent<AU_Body>();
        tempBody.SetColor(myAvatarSprite.color);
        isDead = true;
        myAnim.SetBool("IsDead", isDead);
        gameObject.layer = 9;
        myCollider.enabled = false;
    }
    void BodySearch()
    {
        foreach (Transform body in allBodies)
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position, body.position - transform.position);
            Debug.DrawRay(transform.position, body.position - transform.position, Color.cyan);
            if (Physics.Raycast(ray, out hit, 1000f, ~ignoreForBody))
            {

                if (hit.transform == body)
                {
                    Debug.Log(hit.transform.name);
                    Debug.Log(bodiesFound.Count);
                    if (bodiesFound.Contains(body.transform))
                        return;
                    bodiesFound.Add(body.transform);
                }
                else
                {

                    bodiesFound.Remove(body.transform);
                }
            }
        }
    }

    void Interact(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            //Debug.Log("Here");
            RaycastHit hit;
            Ray ray = myCamera.ScreenPointToRay(mousePositionInput);
            if (Physics.Raycast(ray, out hit, interactLayer))
            {
                //If the Object has the tag interactable for task objects and if the player is a villiage
                if ((hit.transform.tag == "Interactable"))
                {
                    Debug.Log("THe interactable tag works");
                    AU_Interactable temp = hit.transform.GetComponent<AU_Interactable>();
                    temp.PlayMiniGame();
                }

                //If the Object has the tag Vent and if the player is an imposter
                if ((hit.transform.tag == "Vent") && (isImposter == true))
                {
                    Debug.Log("THe vent tag works");
                    AU_Interactable temp = hit.transform.GetComponent<AU_Interactable>();
                    temp.PlayMiniGame();
                }

                //If the object has the tag Emergency
                if ((hit.transform.tag == "Emergency"))
                {
                    Debug.Log("The emergency tag works");
                    AU_Interactable temp = hit.transform.GetComponent<AU_Interactable>();
                    temp.PlayMiniGame();
                }
            }

        }

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(direction);
            stream.SendNext(isImposter);
            colorVector = new Vector3(myColor.r, myColor.g, myColor.b);
            stream.SendNext(colorVector);
        }
        else
        {
            this.direction = (float)stream.ReceiveNext();
            this.isImposter = (bool)stream.ReceiveNext();
            this.colorVector = (Vector3)stream.ReceiveNext();
            syncColor = new Color(colorVector.x, colorVector.y, colorVector.z, 1.0f);
        }
    }

    public void BecomeImposter(int ImposterNumber)
    {
        imposterAssigned = false;
        if (playersInGame != null)
        {
            foreach (AU_PlayerController p in playersInGame)
            {
                if (p.isImposter)
                {
                    imposterAssigned = true;
                }
            }
        }

        if (!imposterAssigned && (PhotonNetwork.LocalPlayer == PhotonNetwork.PlayerList[ImposterNumber]))
        {

            this.isImposter = true;

        }
    }

    public void setImposterNumber(int ImposterNumber)
    {
        imposterNumber = ImposterNumber;
    }

    bool checkCrewmateVictory()
    {
        bool nonImposterExists = false;
        foreach (AU_PlayerController p in playersInGame)
        {
            if (!p.isImposter)
            {
                nonImposterExists = true;
                if (p.tasksCompleted != 2)
                {
                    return false;
                }
            }
        }
        if (nonImposterExists)
        {
            return true;
        }
        return false;
    }

    bool checkMafiaVictory()
    {
        bool nonImposterExists = false;
        foreach (AU_PlayerController p in playersInGame)
        {
            if (!p.isImposter)
            {
                nonImposterExists = true;
                if (!p.isDead)
                {
                    return false;
                }
            }
        }
        if (nonImposterExists)
        {
            return true;
        }
        return false;
    }

}