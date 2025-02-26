using Unity.Multiplayer;
using UnityEngine;
using UnityEngine.XR.Management;

public class XRManager : MonoBehaviour
{
    void Awake()
    {
        var role = MultiplayerRolesManager.ActiveMultiplayerRoleMask;

        if (role == MultiplayerRoleFlags.Server)
        {
            DisableXR();
        }
    }

    void DisableXR()
    {
        var xrSettings = XRGeneralSettings.Instance;
        if (xrSettings != null)
        {
            Debug.Log("Disabling XR for Dedicated Server...");
            xrSettings.Manager.DeinitializeLoader();
        }
    }
}
