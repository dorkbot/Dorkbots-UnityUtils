using System.Linq;
using System.Text;
using GameSparks.Api.Responses;
using GameSparks.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dorkbots.GameSparksTools.GameSparksChat
{
    public class MatchManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI userId; 
        [SerializeField] private TextMeshProUGUI connectionStatus;
        [SerializeField] private TMP_InputField userNameInput;
        [SerializeField] private TMP_InputField passwordInput;
        [SerializeField] private GameObject loginPanel;
        [SerializeField] private Button loginBttn;
        [SerializeField] private TextMeshProUGUI matchDetails;
        [SerializeField] private GameObject matchDetailsPanel;
        [SerializeField] private GameObject chatCanvas;

        private RTSessionInfo tempRTSessionInfo;
        private bool loggedIn = false;

        // Use this for initialization
        private void OnEnable()
        {
            if (GameSparksMatchChatManager.instance.GetSessionInfo() == null)
            {
                passwordInput.gameObject.SetActive(false);
                // we won't start with a user logged in so lets show this also
                userId.text = "No User Logged In...";

                // we won't immediately have connection, so at the start of the lobby we
                // will set the connection status to show this
                connectionStatus.text = "No Connection...";
                GS.GameSparksAvailable += (isAvailable) =>
                {
                    if (isAvailable)
                    {
                        connectionStatus.text = "GameSparks Connected...";
                    }
                    else
                    {
                        connectionStatus.text = "GameSparks Disconnected...";
                    }
                };
                // only the login panel and login button is needed at the start of the scene, so disable any other objects //
                matchDetailsPanel.SetActive(false);

                GameSparks.Api.Messages.MatchFoundMessage.Listener += OnMatchFound;

                // we add a custom listener to the on-click delegate of the login button so we don't need to create extra methods //
                loginBttn.onClick.AddListener(() =>
                {
                    Debug.Log("userNameInput.text = " + userNameInput.text);
                    Debug.Log("passwordInput.text = " + passwordInput.text);
                    GameSparksMatchChatManager.instance.AuthenticateUser(userNameInput.text, "password", OnRegistration, OnAuthentication);
                    //GameSparksMatchChatManager.instance.AuthenticateUser(userNameInput.text, passwordInput.text, OnRegistration, OnAuthentication);
                });

                // this listener will update the text in the player-list field if no match was found //
                GameSparks.Api.Messages.MatchNotFoundMessage.Listener = (message) =>
                {
                    matchDetails.text = "No Match Found...";
                };

                GameSparksMatchChatManager.instance.onRTReadySignal.Add(OnRTReadyHandler);
            }
            else
            {
                chatCanvas.SetActive(true);
                gameObject.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            if (GameSparksMatchChatManager.instance != null) GameSparksMatchChatManager.instance.onRTReadySignal.Remove(OnRTReadyHandler);
            GameSparks.Api.Messages.MatchFoundMessage.Listener -= OnMatchFound;
        }

        // HANDLERS
        private void OnRTReadyHandler()
        {
            chatCanvas.SetActive(true);
            gameObject.SetActive(false);
            //SceneManager.LoadScene(SceneNames.CHAT);
        }

        /// <summary>
        /// this is called when a player is registered
        /// </summary>
        /// <param name="_resp">Resp.</param>
        private void OnRegistration(RegistrationResponse _resp)
        {
            if (!loggedIn)
            {
                loggedIn = true;

                userId.text = "User ID: " + _resp.UserId;
                connectionStatus.text = "New User Registered...";
                loginPanel.SetActive(false);
                loginBttn.gameObject.SetActive(false);
                //matchmakingBttn.gameObject.SetActive(true);
                matchDetailsPanel.SetActive(true);

                GameSparksMatchChatManager.instance.FindPlayers();
            }
        }

        /// <summary>
        /// This is called when a player is authenticated
        /// </summary>
        /// <param name="_resp">Resp.</param>
        private void OnAuthentication(AuthenticationResponse _resp)
        {
            if (!loggedIn)
            {
                loggedIn = true;

                userId.text = "User ID: " + _resp.UserId;
                connectionStatus.text = "User Authenticated...";
                loginPanel.SetActive(false);
                loginBttn.gameObject.SetActive(false);
                matchDetailsPanel.SetActive(true);

                GameSparksMatchChatManager.instance.FindPlayers();
            }
        }

        private void OnMatchFound(GameSparks.Api.Messages.MatchFoundMessage _message)
        {
            Debug.Log("Match Found!...");
            StringBuilder sBuilder = new StringBuilder();
            sBuilder.AppendLine("Match Found...");
            sBuilder.AppendLine("Host URL:" + _message.Host);
            sBuilder.AppendLine("Port:" + _message.Port);
            sBuilder.AppendLine("Access Token:" + _message.AccessToken);
            sBuilder.AppendLine("MatchId:" + _message.MatchId);
            sBuilder.AppendLine("Opponents:" + _message.Participants.Count());
            sBuilder.AppendLine("_________________");
            sBuilder.AppendLine(); // we'll leave a space between the player-list and the match data
            foreach (GameSparks.Api.Messages.MatchFoundMessage._Participant player in _message.Participants)
            {
                sBuilder.AppendLine("Player:" + player.PeerId + " User Name:" + player.DisplayName); // add the player number and the display name to the list
            }
            matchDetails.text = sBuilder.ToString(); // set the string to be the player-list field

            tempRTSessionInfo = new RTSessionInfo(_message); // we'll store the match data until we need to create an RT session instance

            GameSparksMatchChatManager.instance.StartNewRTSession(tempRTSessionInfo);
        }
    }
}