using Unity.VisualScripting;
using UnityEngine;
using Unity.Netcode;

public class NetworkPlayer : NetworkBehaviour
{
    public Transform root;
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;

    private void Update()
    {
        if (IsOwner)
        {
            root.position = VRRigReference.Singleton.root.position;
            root.rotation = VRRigReference.Singleton.root.rotation;
            
            head.position = VRRigReference.Singleton.head.position;
            head.rotation = VRRigReference.Singleton.head.rotation;
            
            leftHand.position = VRRigReference.Singleton.leftHand.position;
            leftHand.rotation = VRRigReference.Singleton.leftHand.rotation;
            
            rightHand.position = VRRigReference.Singleton.rightHand.position;
            rightHand.rotation = VRRigReference.Singleton.rightHand.rotation;
        }
        
    }
}
