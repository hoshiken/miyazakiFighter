using Fusion;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class NetworkRigidbody2D : NetworkBehaviour
{
    [Networked] private Vector2 NetworkedPosition { get; set; }
    [Networked] private float NetworkedRotation { get; set; }
    [Networked] private Vector2 NetworkedVelocity { get; set; }
    [Networked] private float NetworkedAngularVelocity { get; set; }

    private Rigidbody2D _rb;
    private float _interpolationSpeed = 10f;

    public Rigidbody2D Rigidbody => _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        if (_rb == null)
        {
            Debug.LogError("Missing Rigidbody2D on NetworkRigidbody2D!");
        }
    }

    public override void Spawned()
    {
        if (_rb == null)
            _rb = GetComponent<Rigidbody2D>();

        if (HasStateAuthority)
        {
            NetworkedPosition = _rb.position;
            NetworkedRotation = _rb.rotation;
            NetworkedVelocity = _rb.velocity;
            NetworkedAngularVelocity = _rb.angularVelocity;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (_rb == null) return;

        if (HasStateAuthority)
        {
            NetworkedPosition = _rb.position;
            NetworkedRotation = _rb.rotation;
            NetworkedVelocity = _rb.velocity;
            NetworkedAngularVelocity = _rb.angularVelocity;
        }
        else
        {
            _rb.position = Vector2.Lerp(_rb.position, NetworkedPosition, Runner.DeltaTime * _interpolationSpeed);
            _rb.rotation = Mathf.Lerp(_rb.rotation, NetworkedRotation, Runner.DeltaTime * _interpolationSpeed);
            _rb.velocity = NetworkedVelocity;
            _rb.angularVelocity = NetworkedAngularVelocity;
        }
    }
}
