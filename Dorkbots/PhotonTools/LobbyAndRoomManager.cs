using System;
using Photon.Pun;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Photon.Realtime;
using Random = UnityEngine.Random;

namespace Dorkbots.PhotonTools
{
    public interface ILobbyAndRoomManager
    {
        Task<LobbyAndRoomManager.JoinLobbyResponses> ConnectAndJoinLobby(string lobbyName, string gameVersion);
    }
    
    public class LobbyAndRoomManager : MonoBehaviourPunCallbacks, ILobbyAndRoomManager
    {
        private const string RoomCharacters = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789";
        //private const string RoomCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        
        private Dictionary<string, RoomInfo> _cachedRoomList = new Dictionary<string, RoomInfo>();
        private string _lobbyName;
        private string _gameVersion;

        private Action _connectedToMasterAction;

        public enum JoinRoomResponses
        {
            RoomNameIsNull,
            RoomDoesNotExist,
            RoomIsFull,
            FailedToJoinRoom,
            RoomIsJoinable,
            RoomJoined,
            RoomCreated,
            FailedToCreateRoom,
            FailedCreateRoomAlreadyExists,
            FailedToJoinRandomRoom
        }
        private Action<JoinRoomResponses> _joinRoomResultAction;
        private Action<JoinRoomResponses> _createRoomResultAction;
        private Action<JoinRoomResponses> _joinRandomRoomResultAction;
        
        public enum JoinLobbyResponses
        {
            LobbyJoined
        }
        private Action<JoinLobbyResponses> _joinLobbyResultAction;

        //PUN CALLBACKS
        public override void OnConnectedToMaster()
        {
            //Debug.Log("Connected to Master Server. \n");
            _connectedToMasterAction?.Invoke();
        }
        
        public override void OnJoinedLobby()
        {
            _cachedRoomList.Clear();
            _joinLobbyResultAction?.Invoke(JoinLobbyResponses.LobbyJoined);
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            UpdateCachedRoomList(roomList);
        }

        public override void OnLeftLobby()
        {
            _cachedRoomList.Clear();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            _cachedRoomList.Clear();
        }
        
        public override void OnCreatedRoom()
        {
            //Debug.Log("ROOM CREATED!!!!!!!");
            _createRoomResultAction?.Invoke(JoinRoomResponses.RoomCreated);
        }
        
        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            //Debug.LogWarningFormat("Room creation failed with error code {0} and error message {1}", returnCode, message);
            if (returnCode == 32766)
            {
                //room already exists with given name, this should only happen when creating a private room
                _createRoomResultAction?.Invoke(JoinRoomResponses.FailedCreateRoomAlreadyExists);
            }
            else
            {
                _createRoomResultAction?.Invoke(JoinRoomResponses.FailedToCreateRoom);
            }
        }

        public override void OnJoinedRoom()
        {
            _joinRoomResultAction?.Invoke(JoinRoomResponses.RoomJoined);
            _joinRandomRoomResultAction?.Invoke(JoinRoomResponses.RoomJoined);
        }
        
        public override void OnJoinRoomFailed(short returnCode, string message) 
        {
            if (returnCode == 32765)//game is full
            {
                _joinRoomResultAction?.Invoke(JoinRoomResponses.RoomIsFull);
            }
            else if (returnCode == 32758)//room doesn't exist
            {
                _joinRoomResultAction?.Invoke(JoinRoomResponses.RoomDoesNotExist);
            }
            else
            {
                _joinRoomResultAction?.Invoke(JoinRoomResponses.FailedToJoinRoom);
            }
        }
        
        public override void OnJoinRandomFailed(short returnCode, string message) 
        {
            //Debug.Log("OnJoinRandomFailed Failed -> returnCode = " + returnCode + " || message = " + message);
            _joinRandomRoomResultAction?.Invoke(JoinRoomResponses.FailedToJoinRandomRoom);
        }

        //LOBBY
        public async Task<JoinLobbyResponses> ConnectAndJoinLobby(string lobbyName, string gameVersion)
        {
            _lobbyName = lobbyName;
            _gameVersion = gameVersion;
            
            TaskCompletionSource<JoinLobbyResponses> taskCompletionSource = new TaskCompletionSource<JoinLobbyResponses>();
            
            if (PhotonNetwork.IsConnected)
            {
                await LeaveRoom();
                return await JoinLobby();
            }
            // Otherwise establish a new connection. We can then connect via OnConnectedToMaster
            else
            {
                _connectedToMasterAction += ConnectedToMasterHandler;
                PhotonNetwork.GameVersion = gameVersion;
                PhotonNetwork.ConnectUsingSettings();

                await taskCompletionSource.Task;
                return taskCompletionSource.Task.Result;
            }

            async void ConnectedToMasterHandler()
            {
                _connectedToMasterAction -= ConnectedToMasterHandler;
                JoinLobbyResponses response = await JoinLobby();
                taskCompletionSource.SetResult(response);
            }
        }

        private Task<JoinLobbyResponses> JoinLobby()
        {
            TaskCompletionSource<JoinLobbyResponses> taskCompletionSource = new TaskCompletionSource<JoinLobbyResponses>();
            _joinLobbyResultAction += JoinLobbyResultHandler;
            PhotonNetwork.JoinLobby(new TypedLobby(_lobbyName, LobbyType.Default));
            
            return taskCompletionSource.Task;
            
            void JoinLobbyResultHandler(JoinLobbyResponses response)
            {
                _joinLobbyResultAction -= JoinLobbyResultHandler;
                taskCompletionSource.SetResult(response);
            }
        }
        
        private void UpdateCachedRoomList(List<RoomInfo> roomList)
        {
            for (int i = 0; i < roomList.Count; i++)
            {
                RoomInfo info = roomList[i];
                Debug.Log("<LobbyAndRoomManager> UpdateCachedRoomList -> info.Name = " + info.Name);
                if (info.RemovedFromList)
                {
                    _cachedRoomList.Remove(info.Name);
                }
                else
                {
                    _cachedRoomList[info.Name] = info;
                }
            }
        }
        
        public string GetRandomRoomName(int minCharAmount, int maxCharAmount)
        {
            string roomName = "";
            bool foundNewName = false;
            int charAmount;

            int attempts = 0;
            while (!foundNewName && attempts < 100)
            {
                charAmount = Random.Range(minCharAmount, maxCharAmount);
                for (int i = 0; i < charAmount; i++)
                {
                    roomName += RoomCharacters[Random.Range(0, RoomCharacters.Length)];
                }

                foundNewName = !_cachedRoomList.ContainsKey(roomName);

                attempts++;
                if (!foundNewName && attempts >= 100)
                {
                    foundNewName = true;
                    Debug.LogWarning("Tried " + attempts + " times to find a unique room name and failed!!!!!");
                    break;
                }
            }

            return roomName;
        }

        public JoinRoomResponses LookForRoom(string roomName)
        {
            if (String.IsNullOrEmpty(roomName))
            {
                return JoinRoomResponses.RoomNameIsNull;
            }

            RoomInfo roomInfo;
            if (_cachedRoomList.TryGetValue(roomName, out roomInfo))
            {
                if (roomInfo.MaxPlayers <= roomInfo.PlayerCount)
                {
                    return JoinRoomResponses.RoomIsFull;
                }
                else
                {
                    return JoinRoomResponses.RoomIsJoinable;
                }
            }

            return JoinRoomResponses.RoomDoesNotExist;
        }

        public async Task<JoinRoomResponses> JoinRoom(string roomName)
        {
            JoinRoomResponses joinRoomResponse = LookForRoom(roomName);
            TaskCompletionSource<JoinRoomResponses> taskCompletionSource = new TaskCompletionSource<JoinRoomResponses>();
            if (joinRoomResponse != JoinRoomResponses.RoomIsJoinable)
            {
                taskCompletionSource.SetResult(joinRoomResponse);
            }
            else
            {
                await LeaveRoom();
                _joinRoomResultAction += JoinRoomResultHandler;
                PhotonNetwork.JoinRoom(roomName);
            }

            await taskCompletionSource.Task;
            return taskCompletionSource.Task.Result;

            void JoinRoomResultHandler(JoinRoomResponses response)
            {
                _joinRoomResultAction -= JoinRoomResultHandler;
                taskCompletionSource.SetResult(response);
            }
        }

        public async Task<bool> LeaveRoom()
        {
            TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
            if (PhotonNetwork.Server == ServerConnection.GameServer)
            {
                if (PhotonNetwork.IsMasterClient && PhotonNetwork.PlayerListOthers.Length > 0)
                {
                    PhotonNetwork.SetMasterClient(PhotonNetwork.PlayerListOthers[0]);
                }

                _connectedToMasterAction += ConnectedToMasterHandler;
                PhotonNetwork.LeaveRoom(false);//results in OnConnectedToMaster getting called
            }
            else
            {
                taskCompletionSource.SetResult(true);
            }
            
            await taskCompletionSource.Task;
            return taskCompletionSource.Task.Result;
            
            void ConnectedToMasterHandler()
            {
                _connectedToMasterAction -= ConnectedToMasterHandler;
                taskCompletionSource.SetResult(true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roomName">Passing null will result in a fail</param>
        /// <param name="roomOptions"></param>
        /// <returns></returns>
        public async Task<JoinRoomResponses> CreateNamedRoom(string roomName, RoomOptions roomOptions)
        {
            JoinRoomResponses joinRoomResponse = LookForRoom(roomName);
            TaskCompletionSource<JoinRoomResponses> taskCompletionSource = new TaskCompletionSource<JoinRoomResponses>();
            if (joinRoomResponse != JoinRoomResponses.RoomDoesNotExist)
            {
                taskCompletionSource.SetResult(joinRoomResponse);
            }
            else
            {
                return await CreateRoom(roomName, roomOptions);
            }
            
            await taskCompletionSource.Task;
            return taskCompletionSource.Task.Result;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="roomName">Pass null if joining random room</param>
        /// <param name="roomOptions"></param>
        /// <returns></returns>
        public async Task<JoinRoomResponses> CreateRoom(string roomName, RoomOptions roomOptions)
        {
            TaskCompletionSource<JoinRoomResponses> taskCompletionSource = new TaskCompletionSource<JoinRoomResponses>();
            
            await LeaveRoom();
            
            _createRoomResultAction += CreateRoomResultHandler;
            PhotonNetwork.CreateRoom( roomName, roomOptions, TypedLobby.Default);

            await taskCompletionSource.Task;
            return taskCompletionSource.Task.Result;
            
            void CreateRoomResultHandler(JoinRoomResponses response)
            {
                _createRoomResultAction -= CreateRoomResultHandler;
                taskCompletionSource.SetResult(response);
            }
        }

        public async Task<JoinRoomResponses> JoinRandomRoom(RoomOptions roomOptions)
        {
            await LeaveRoom();

            TaskCompletionSource<JoinRoomResponses> taskCompletionSource = new TaskCompletionSource<JoinRoomResponses>();
            _joinRandomRoomResultAction += JoinRandomRoomResultHandler;
            PhotonNetwork.JoinRandomRoom(roomOptions.CustomRoomProperties, Convert.ToByte(roomOptions.MaxPlayers));
            
            JoinRoomResponses response = await taskCompletionSource.Task;
            if (response == JoinRoomResponses.FailedToJoinRandomRoom)
            {
                return await CreateRoom(null, roomOptions);
            }
            else
            {
                return taskCompletionSource.Task.Result;
            }

            void JoinRandomRoomResultHandler(JoinRoomResponses joinResponse)
            {
                _joinRandomRoomResultAction -= JoinRandomRoomResultHandler;
                taskCompletionSource.SetResult(joinResponse);
            }
        }
    }
}