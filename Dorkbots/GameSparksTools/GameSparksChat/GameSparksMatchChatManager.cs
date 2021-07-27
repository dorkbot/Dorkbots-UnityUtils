using System.Collections.Generic;
using GameSparks.Api.Messages;
using GameSparks.Api.Responses;
using GameSparks.Core;
using GameSparks.RT;
using Signals;
using UnityEngine;

namespace Dorkbots.GameSparksTools.GameSparksChat
{
    public class GameSparksMatchChatManager : MonoBehaviour
    {
    // <summary>The GameSparks Manager singleton</summary>
        private static GameSparksMatchChatManager _instance = null;
        /// <summary>This method will return the current instance of this class </summary>
        public static GameSparksMatchChatManager instance 
        { 
            get 
            {
                if (_instance != null)
                {
                    return _instance; // return the singleton if the instance has been setup
                }
                //else
                //{ // otherwise return an error
                //    /Debug.LogError("GSM| GameSparksMatchChatManager Not Initialized...");
                //}
                return null;
             } 
        }

        public Signal<int> onPlayerConnectedToGameSignal { get; private set; }
        public Signal<int> onPlayerDisconnectedSignal { get; private set; }
        public Signal onRTReadySignal { get; private set; }
        public Signal<RTPacket> onPacketReceivedSignal { get; private set; }
        public Signal<MatchUpdatedMessage> onMatchUpdatedMessageSignal { get; private set; }
        public Queue<string> chatLog = new Queue<string>();

        private GameSparksRTUnity gameSparksRTUnity;
        private RTSessionInfo sessionInfo;

        void Awake()
        {
            if (_instance == null)
            {
                _instance = this; // if not, give it a reference to this class...
                DontDestroyOnLoad(this.gameObject); // and make this object persistent as we load new scenes

                onPlayerConnectedToGameSignal = new Signal<int>();
                onPlayerDisconnectedSignal = new Signal<int>();
                onRTReadySignal = new Signal();
                onPacketReceivedSignal = new Signal<RTPacket>();
                onMatchUpdatedMessageSignal = new Signal<MatchUpdatedMessage>();

                MatchUpdatedMessage.Listener += OnMatchMessage;
            }
            else
            {
                Destroy(this);
            }
        }

        private void OnDestroy()
        {
            if (this == _instance)
            {
                _instance = null;

                MatchUpdatedMessage.Listener -= OnMatchMessage;

                onPlayerConnectedToGameSignal.Dispose();
                onPlayerDisconnectedSignal.Dispose();
                onRTReadySignal.Dispose();
                onPacketReceivedSignal.Dispose();
                onMatchUpdatedMessageSignal.Dispose();
            }
        }

        public GameSparksRTUnity GetRTSession()
        {
            return gameSparksRTUnity;
        }

        public RTSessionInfo GetSessionInfo()
        {
            return sessionInfo;
        }

        public void StartNewRTSession(RTSessionInfo _info)
        {
            Debug.Log("GSM| Creating New RT Session Instance...");
            sessionInfo = _info;
            gameSparksRTUnity = this.gameObject.AddComponent<GameSparksRTUnity>(); // Adds the RT script to the game
                                                                                   // In order to create a new RT game we need a 'FindMatchResponse' //
                                                                                   // This would usually come from the server directly after a successful MatchmakingRequest //
                                                                                   // However, in our case, we want the game to be created only when the first player decides using a button //
                                                                                   // therefore, the details from the response is passed in from the gameInfo and a mock-up of a FindMatchResponse //
                                                                                   // is passed in. //
            GSRequestData mockedResponse = new GSRequestData()
                                                .AddNumber("port", (double)_info.GetPortID())
                                                .AddString("host", _info.GetHostURL())
                                                .AddString("accessToken", _info.GetAccessToken()); // construct a dataset from the game-details

            FindMatchResponse response = new FindMatchResponse(mockedResponse); // create a match-response from that data and pass it into the game-config
                                                                                // So in the game-config method we pass in the response which gives the instance its connection settings //
                                                                                // In this example, I use a lambda expression to pass in actions for 
                                                                                // OnPlayerConnect, OnPlayerDisconnect, OnReady and OnPacket actions //
                                                                                // These methods are self-explanatory, but the important one is the OnPacket Method //
                                                                                // this gets called when a packet is received //

            gameSparksRTUnity.Configure(response,
                (peerId) => { OnPlayerConnectedToGame(peerId); },
                (peerId) => { OnPlayerDisconnected(peerId); },
                (ready) => { OnRTReady(ready); },
                (packet) => { OnPacketReceived(packet); });

            gameSparksRTUnity.Connect(); // when the config is set, connect the game
        }

        private void OnPlayerConnectedToGame(int _peerId)
        {
            Debug.Log("GSM| Player Connected, " + _peerId);
            Debug.Log(gameSparksRTUnity.PeerId);
            Debug.Log(gameSparksRTUnity.ActivePeers.Count);
            onPlayerConnectedToGameSignal.Dispatch(_peerId);
        }

        private void OnPlayerDisconnected(int _peerId)
        {
            Debug.Log("GSM| Player Disconnected, " + _peerId);
            onPlayerDisconnectedSignal.Dispatch(_peerId);
        }

        private void OnRTReady(bool _isReady)
        {
            if (_isReady)
            {
                Debug.Log("GSM| RT Session Connected...");
                onRTReadySignal.Dispatch();
            }
        }

        private void OnPacketReceived(RTPacket _packet)
        {
            switch (_packet.OpCode)
            {
                // op-code 1 refers to any chat-messages being received by a player //
                // from here, we'll send them to the chat-manager //
                case 1:
                    OnPacketReceivedHandler(_packet);
                    onPacketReceivedSignal.Dispatch(_packet);
                    break;
            }
        }

        /// <summary>
        /// This is called from the GameSparksManager class.
        /// It send any packets with op-code '1' to the chat manager so the chat manager can parse the necessary details out for display in the chat log window
        /// </summary>
        /// <param name="_packet">Data.</param>
        private void OnPacketReceivedHandler(RTPacket _packet)
        {
            Debug.Log("Message Received...\n" + _packet.Data.GetString(1)); // the RTData we sent the message with used an index '1' so we can parse the data back using the same index
                                                                            // we need the display name of the sender. We get this by using the packet sender id and comparing that to the peerId of the player //
            foreach (RTPlayer player in GetSessionInfo().GetPlayerList())
            {
                if (player.peerId == _packet.Sender)
                {
                    // we want to get the message and time and print those to the local users chat-log //
                    UpdateChatLog(player.displayName, _packet.Data.GetString(1), _packet.Data.GetString(2));
                }
            }
        }

        /// <summary>
        /// This method will update the current user's chat log and print the new log to the screen.
        /// </summary>
        /// <param name="_sender">The name (display-name) of the sender</param>
        /// <param name="_message">the body of the message</param>
        /// <param name="_date">the date of the message</param>
        public void UpdateChatLog(string _sender, string _message, string _date)
        {
            // In this example, the message we want to display is formatted so that we can distinguish each part of the message when //
            // it is displayed, all the information is clearly visible //
            chatLog.Enqueue("<b>" + _sender + "</b>\n<color=black>" + _message + "</color>" + "\n<i>" + _date + "</i>");
            //if (chatLog.Count > elementsInChatLog)
            //{ // if we have exceeded the amount of messages in the log, then remove the top message
            //    chatLog.Dequeue();
            //}
            //chatLogOutput.text = string.Empty; // we need to clear the log, otherwise we always get the same messages repeating
            //foreach (string logEntry in chatLog.ToArray())
            //{ // go through each chat item and add the entry to the output log
            //    chatLogOutput.text += logEntry + "\n";
            //}
            //chatScrollBar.value = 0;
            //StartStopCoroutine.StartCoroutine(ref setChatScrollBarCoroutine, SetChatScrollBarEnumerator(), this);
        }

        private void OnMatchMessage(MatchUpdatedMessage message)
        {
            if (sessionInfo != null) sessionInfo.UpdateMatch(message);
            onMatchUpdatedMessageSignal.Dispatch(message);
        }


        #region Login & Registration
        public delegate void AuthCallback(AuthenticationResponse _authresp2);
        public delegate void RegCallback(RegistrationResponse _authResp);
        /// <summary>
        /// Sends an authentication request or registration request to GS.
        /// </summary>
        /// <param name="_authcallback">Auth-Response</param>
        /// <param name="_regcallback">Registration-Response</param>
        public void AuthenticateUser(string _userName, string _password, RegCallback _regcallback, AuthCallback _authcallback)
        {
            new GameSparks.Api.Requests.RegistrationRequest()
                      // this login method first attempts a registration //
                      // if the player is not new, we will be able to tell as the registrationResponse has a bool 'NewPlayer' which we can check
                      // for this example we use the user-name was the display name also //
                      .SetDisplayName(_userName)
                      .SetUserName(_userName)
                      .SetPassword(_password)
                      .Send((regResp) => {
                          if (!regResp.HasErrors)
                          { // if we get the response back with no errors then the registration was successful
                                Debug.Log("GSM| Registration Successful...");
                                _regcallback(regResp);
                          }
                          else
                          {
                              Debug.Log("regResp.Errors.JSON = " + regResp.Errors.JSON);
                                // if we receive errors in the response, then the first thing we check is if the player is new or not
                                if (!(bool)regResp.NewPlayer) // player already registered, lets authenticate instead
                                {
                                    Debug.LogWarning("GSM| Existing User, Switching to Authentication");
                                    new GameSparks.Api.Requests.AuthenticationRequest()
                                      .SetUserName(_userName)
                                      .SetPassword(_password)
                                      .Send((authResp) => {
                                          if (!authResp.HasErrors)
                                          {
                                              Debug.Log("Authentication Successful...");
                                              _authcallback(authResp);
                                          }
                                          else
                                          {
                                              Debug.LogWarning("GSM| Error Authenticating User \n" + authResp.Errors.JSON);
                                          }
                                      });
                              }
                              else
                              {
                                    // if there is another error, then the registration must have failed
                                    Debug.LogWarning("GSM| Error Authenticating User \n" + regResp.Errors.JSON);
                              }
                          }
                      });
        }
        #endregion

        #region Matchmaking Request
        /// <summary>
        /// This will request a match between as many players you have set in the match.
        /// When the max number of players is found each player will receive the MatchFound message
        /// </summary>
        public void FindPlayers()
        {
            Debug.Log("GSM| Attempting Matchmaking...");
            new GameSparks.Api.Requests.MatchmakingRequest()
                .SetMatchShortCode("AR_Checkpoint") // set the shortCode to be the same as the one we created in the first tutorial
                .SetSkill(0) // in this case we assume all players have skill level zero and we want anyone to be able to join so the skill level for the request is set to zero
                .Send((response) => {
                    if (response.HasErrors)
                    { // check for errors
                        Debug.LogError("GSM| MatchMaking Error \n" + response.Errors.JSON);
                    }
                });
        }
        #endregion
    }

    public class RTSessionInfo
    {
        private string hostURL;
        public string GetHostURL() { return this.hostURL; }
        private string acccessToken;
        public string GetAccessToken() { return this.acccessToken; }
        private int portID;
        public int GetPortID() { return this.portID; }
        private string matchID;
        public string GetMatchID() { return this.matchID; }

        private List<RTPlayer> playerList = new List<RTPlayer>();
        public List<RTPlayer> GetPlayerList()
        {
            return playerList;
        }

        /// <summary>
        /// Creates a new RTSession object which is held until a new RT session is created
        /// </summary>
        /// <param name="_message">Message.</param>
        public RTSessionInfo(MatchFoundMessage _message)
        {
            portID = (int)_message.Port;
            hostURL = _message.Host;
            acccessToken = _message.AccessToken;
            matchID = _message.MatchId;
            // we loop through each participant and get their peerId and display name //
            foreach (MatchFoundMessage._Participant p in _message.Participants)
            {
                playerList.Add(new RTPlayer(p.DisplayName, p.Id, (int)p.PeerId));
            }
        }

        public void UpdateMatch(MatchUpdatedMessage message)
        {
            playerList.Clear();
            foreach (MatchUpdatedMessage._Participant p in message.Participants)
            {
                playerList.Add(new RTPlayer(p.DisplayName, p.Id, (int)p.PeerId));
            }
        }
    }

    public class RTPlayer
    {
        public RTPlayer(string _displayName, string _id, int _peerId)
        {
            this.displayName = _displayName;
            this.id = _id;
            this.peerId = _peerId;
        }

        public string displayName;
        public string id;
        public int peerId;
        public bool isOnline;
    }
}