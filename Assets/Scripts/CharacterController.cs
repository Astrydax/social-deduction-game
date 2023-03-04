using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System;
using Newtonsoft.Json.Bson;
using Unity.Netcode;
using Unity.Collections;
using static CharacterController;

public class CharacterController : NetworkBehaviour
{
    [SerializeField] bool hasControl;
        public static CharacterController localPlayer;

    [SerializeField] Camera playerCamera;

    //components CHECK THESE IF SOMETHING BREAKS ITS SUPPOSED TO BE 3D
    Rigidbody2D myRB;
    Transform myAvatar;
    Animator myAnim;
    [SerializeField] InputAction WASD;
    Vector2 movementInput;
    [SerializeField] float movementSpeed;

    
    [SerializeField] TMPro.TMP_Text myNameplate;

    private NetworkVariable<PlayerData> playerData = new NetworkVariable<PlayerData>(new PlayerData
    {
        _color = new Color(1f,1f,1f,1f),
        _name = "Choose a Name",
    }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public struct PlayerData : INetworkSerializable
    {
        public FixedString32Bytes _name;
        public Color _color;

        public PlayerData(FixedString32Bytes name, Color color)
        {
            _name = name;
            _color = color;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _name);
            serializer.SerializeValue(ref _color);
        }
    }

    

    private void OnEnable()
    {
        WASD.Enable();

    }

    private void OnDisable()
    {
        WASD.Disable();
    }

    public override void OnNetworkSpawn()
    {

        
        playerData.OnValueChanged += (PlayerData previousValue, PlayerData newValue) =>
        {
            myNameplate.color = newValue._color;
            myNameplate.text = newValue._name.ToString();
        };

        if(!IsOwner)
        {
            playerCamera.gameObject.SetActive(false);
        }
        myNameplate.color = playerData.Value._color;
        myNameplate.text = playerData.Value._name.ToString();
    }

    private void Start()
    {        
        myRB = GetComponent<Rigidbody2D>();
        myAvatar = transform.GetChild(0);
        myAnim = GetComponent<Animator>(); 
    }

    private void Update()
    {
        if (!IsOwner) return; 
        movementInput = WASD.ReadValue<Vector2>();
        if(movementInput.x != 0)
        {
            myAvatar.localScale = new Vector2(Mathf.Sign(movementInput.x), 1);
        }
        myAnim.SetFloat("Speed", movementInput.magnitude);

    }

    private void FixedUpdate()
    {
        myRB.velocity = movementInput * movementSpeed;
    }

    public void SetColor(Color newColor)
    {
        playerData.Value = new PlayerData(playerData.Value._name, newColor);
        /*if(myNameplate!= null)
        {
            myNameplate.color = playerData.Value._color;
        }*/
    }

    public void SetName(string name)
    {
        playerData.Value = new PlayerData(name, playerData.Value._color);
        /*if(myNameplate != null)
        {
            myNameplate.text = playerData.Value._name.ToString();
            
        }*/
    }   

    public string GetName()
    {
        return playerData.Value._name.ToString();
    }
}
