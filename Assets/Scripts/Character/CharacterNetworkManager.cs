using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterNetworkManager : NetworkBehaviour
{
    CharacterManager character;

    [Header("Position")]
    public NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<Quaternion> networkRotation = new NetworkVariable<Quaternion>(Quaternion.identity, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public Vector3 networkPositionVelocity;
    public float networkPositionSmoothTime = 0.1f;
    public float networkRotationSmoothTime = 0.1f;

    [Header("Animator")]
    public NetworkVariable<float> horizontalMovement = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> verticalMovement = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> moveAmount = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    // Server RPC is called from the client to the server
    [ServerRpc]
    public void NotifyTheServerOfActionAnimationServerRpc(ulong clientId, string animationId, bool applyRootMotion)
    {
        // If charcter is host/server, activate client RPC 
        if (IsServer)
        {
            PLayerActionAnimationForAllClientsClientRpc(clientId, animationId, applyRootMotion);
        }
    }

    // Client RPC is sent to all clients from the server
    [ClientRpc]
    public void PLayerActionAnimationForAllClientsClientRpc(ulong clientId, string animationId, bool applyRootMotion)
    {
        // Ensures function is not run on client who sent the animation
        if (clientId != NetworkManager.Singleton.LocalClientId)
        {
            PerformActionAnimationFromServer(animationId, applyRootMotion);
        }
    }

    private void PerformActionAnimationFromServer(string animationId, bool applyRootMotion)
    {
        character.applyRootMotion = applyRootMotion;
        // same crossfade time from the CharacterAnimatorManager class
        character.animator.CrossFade(animationId, 0.2f);
    }
}
