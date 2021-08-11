using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class Launcher : MonoBehaviourPunCallbacks
{

	public static Launcher Instance;

	[SerializeField] TMP_InputField roomNameInputField;
	[SerializeField] TMP_Text errorText;
	[SerializeField] TMP_Text roomNameText;
	[SerializeField] Transform roomListContent;
	[SerializeField] Transform playerListContent;
	[SerializeField] GameObject roomListItemPrefab;
	[SerializeField] GameObject playerListItemPrefab;
	[SerializeField] GameObject startGameButton;

	private Dictionary<string, GameObject> roomButtons = new Dictionary<string, GameObject>();

	void Awake()
    {
		Instance = this;
    }

	void Start()
	{
		Debug.Log("Connecting to Master");
		PhotonNetwork.ConnectUsingSettings();
	}

	public override void OnConnectedToMaster()
	{
		Debug.Log("Connected to Master");
		PhotonNetwork.JoinLobby();
		PhotonNetwork.AutomaticallySyncScene = true;
	}

	public override void OnJoinedLobby()
	{
		MenuManager.Instance.OpenMenu("title");
		Debug.Log("Joined Lobby");
		PhotonNetwork.NickName = "Player" + Random.Range(0, 1000).ToString("0000");
	}

	public void CreateRoom()
	{
		if (string.IsNullOrEmpty(roomNameInputField.text))
		{
			return;
		}
		PhotonNetwork.CreateRoom(roomNameInputField.text);
		MenuManager.Instance.OpenMenu("loading");
	}

	public override void OnJoinedRoom()
    {
		MenuManager.Instance.OpenMenu("room");
		roomNameText.text = PhotonNetwork.CurrentRoom.Name;

		Player[] players = PhotonNetwork.PlayerList;

		foreach(Transform child in playerListContent)
        {
			Destroy(child.gameObject);
        }

		for(int i = 0; i < players.Length; i++)
		{
			Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
		}

		startGameButton.SetActive(PhotonNetwork.IsMasterClient);
	}

	public override void OnMasterClientSwitched(Player newMasterClient)
    {
		startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

	public override void OnCreateRoomFailed(short returnCode, string message)
    {
		errorText.text = "Room Creation Failed: " + message;
		MenuManager.Instance.OpenMenu("error");
    }

	public void StartGame()
    {
		PhotonNetwork.LoadLevel(1);
    }

	public void LeaveRoom()
    {
		PhotonNetwork.LeaveRoom();
		MenuManager.Instance.OpenMenu("loading");
    }

	public void JoinRoom(RoomInfo info)
    {
		PhotonNetwork.JoinRoom(info.Name);
		MenuManager.Instance.OpenMenu("loading");
	}

	public override void OnLeftRoom()
    {
		MenuManager.Instance.OpenMenu("title");
    }

	public override void OnRoomListUpdate(List<RoomInfo> roomList)
	{	
		for (int i = 0; i < roomList.Count; i++)
        {
			if (roomList[i].RemovedFromList)
			{
				// remove the right room button from roomListContent for this roomList[i] item
				if(roomButtons.ContainsKey(roomList[i].Name))
				{
					Destroy(roomButtons[roomList[i].Name]);
					roomButtons.Remove(roomList[i].Name);
				}
			}
			else
			{
				if(roomButtons.ContainsKey(roomList[i].Name) == false)
				{
					var gameObject = Instantiate(roomListItemPrefab, roomListContent);
					gameObject.GetComponent<RoomListItem>().SetUp(roomList[i]);
					roomButtons.Add(roomList[i].Name, gameObject);
				}
			}
		}
	}

	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
	}
}