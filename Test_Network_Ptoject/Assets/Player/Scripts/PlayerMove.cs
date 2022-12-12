using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : NetworkBehaviour
{
    [SerializeField] private float speedMove = 1f;
    private Rigidbody _playerRb;
    private Vector3 _directionMove;

    private void Start()
    {
        _playerRb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (isLocalPlayer == false) return;
        Move();
    }

    private void Move()
    {
        _directionMove = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        _playerRb.AddForce(_directionMove * speedMove, ForceMode.Force);
    }
}
