using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using System.Threading.Tasks;
using TMPro;
using System.Text;

public class RelayManager : NetworkBehaviour
{
    public string joinCode;
    private static UnityTransport _transport;
    public TextMeshProUGUI updateText;

    public static RelayManager Instance { get; private set; }

    void Start()
    {

        _transport = Object.FindObjectOfType<UnityTransport>();

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else if (Instance != null)
        {
            Destroy(gameObject);
        }

        NetworkManager.Singleton.NetworkConfig.ConnectionApproval = true;


    }



    public async Task<string> CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);

            joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            updateText.text = "Creating Relay with Code: " + joinCode;


            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");

            _transport.SetRelayServerData(relayServerData);

            NetworkManager.Singleton.ConnectionApprovalCallback += ConnectionApprovalCallback;

            //NetworkManager.Singleton.NetworkConfig.ConnectionData = Encoding.ASCII.GetBytes(PlayerPrefs.GetString("name"));



            NetworkManager.Singleton.StartHost();

            updateText.text = "Id: " + NetworkManager.Singleton.LocalClientId;


            return joinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            return null;
        }
    }



    public async void JoinRelay(string joinCode)
    {
        try
        {
            updateText.text = "Joining Relay with Code: " + joinCode;
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");

            _transport.SetRelayServerData(relayServerData);

            //NetworkManager.Singleton.NetworkConfig.ConnectionData = Encoding.ASCII.GetBytes(PlayerPrefs.GetString("name"));

            NetworkManager.Singleton.StartClient();

            updateText.text = "Id: " + NetworkManager.Singleton.LocalClientId;

        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }
    private void ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        var clientId = request.ClientNetworkId;

        var connectionData = request.Payload;
        Debug.Log("Connecting");

        response.Approved = true;
        response.CreatePlayerObject = true;
        //response.Position;
        //response.Rotation;
        response.Pending = false;

    }



}
