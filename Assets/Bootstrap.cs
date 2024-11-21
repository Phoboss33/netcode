using System;
using System.Collections;
using TMPro;
using Unity.Multiplayer;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Bootstrap : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public Button connectButton;
    public Button refreshButton;

    private Server[] serverList;

    void Start()
    {
        connectButton.onClick.AddListener(OnConnectButtonClicked);
        refreshButton.onClick.AddListener(OnRefreshButtonClicked);

        var role = MultiplayerRolesManager.ActiveMultiplayerRoleMask;

        if (role == MultiplayerRoleFlags.Client)
        {
            StartCoroutine(GetServers());
        }
        else if (role == MultiplayerRoleFlags.Server)
        {
            ushort serverPort = GetPortFromCommandLineArgs();
            Debug.Log($"Server port: {serverPort}");

            StartCoroutine(SendServerData(serverPort));
            NetworkManager.Singleton.StartServer();
        }
    }

    IEnumerator SendServerData(ushort port)
    {
        string url = "http://192.168.0.21:3000/addServer";
        string jsonData = JsonUtility.ToJson(new Server { port = port });

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("POST Error: " + request.error);
        }
        else
        {
            Debug.Log("POST Success: " + request.downloadHandler.text);
            StartCoroutine(GetServers());
        }
    }

    IEnumerator GetServers()
    {
        UnityWebRequest www = UnityWebRequest.Get("http://192.168.0.21:3000/servers");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            string json = www.downloadHandler.text;
            serverList = JsonUtility.FromJson<ServerList>("{\"servers\":" + json + "}").servers;

            dropdown.options.Clear();
            foreach (Server server in serverList)
            {
                string servers = $"IP: {server.ip}, Port: {server.port}";
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(servers);
                dropdown.options.Add(option);
            }
            dropdown.RefreshShownValue();
        }
    }

    void OnConnectButtonClicked()
    {
        int selectedIndex = dropdown.value;

        if (serverList != null && selectedIndex >= 0 && selectedIndex < serverList.Length)
        {
            Server selectedServer = serverList[selectedIndex];
            var transport = NetworkManager.Singleton.NetworkConfig.NetworkTransport as UnityTransport;
            if (transport != null)
            {
                transport.ConnectionData.Address = selectedServer.ip;
                transport.ConnectionData.Port = selectedServer.port;

                Debug.Log($"Connecting to {transport.ConnectionData.Address}:{transport.ConnectionData.Port}");
                NetworkManager.Singleton.StartClient();
            }
        }
        else
        {
            Debug.LogError("Invalid server selected.");
        }
    }

    void OnRefreshButtonClicked()
    {
        dropdown.ClearOptions();
        StartCoroutine(GetServers());
    }

    ushort GetPortFromCommandLineArgs()
    {
        string[] args = Environment.GetCommandLineArgs();

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-port" && i + 1 < args.Length && ushort.TryParse(args[i + 1], out ushort port))
            {
                return port;
            }
        }

        Debug.LogWarning("No port specified, using default: 7777");
        return 7777;
    }

    [Serializable]
    public class Server
    {
        public string ip;
        public ushort port;
    }

    [Serializable]
    public class ServerList
    {
        public Server[] servers;
    }
}
