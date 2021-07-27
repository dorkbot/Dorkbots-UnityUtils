using System;
using System.Collections;
using Dorkbots.MonoBehaviorUtils;
using GameSparks.Api.Messages;
using GameSparks.RT;
using UnityEngine;
using UnityEngine.UI;

namespace Dorkbots.GameSparksTools.GameSparksChat
{
    public class ChatManager : MonoBehaviour
    {
        [SerializeField] private InputField messageInput;
        [SerializeField] private Dropdown recipientOption;
        [SerializeField] private Button sendMessageBttn;

        [SerializeField] private Text chatLogOutput;
        [SerializeField] private int elementsInChatLog = 7;

        [SerializeField] private GameObject chatWindow;
        [SerializeField] private Scrollbar chatScrollBar;
        [SerializeField] private Button chatToogleBttn;
        private bool isChatWindowOpen;
        private Coroutine setChatScrollBarCoroutine;

        // Start is called before the first frame update
        void Start()
        {

        }

        private void OnEnable()
        {
            //chatLogOutput.text = string.Empty; // we want to clear the chat log at the start of the game in case there is any debug text in there
            //chatWindow.SetActive(false); // we don't want the chat window to show at the start of the level, so we disable it here
            chatToogleBttn.gameObject.SetActive(false);
            chatToogleBttn.onClick.AddListener(ToogleChatWindow); // assign the toggle-button to the chat-toggle method which will enable and disable the chat window

            UpdatePlayerList();

            GameSparksMatchChatManager.instance.onPacketReceivedSignal.Add(OnPacketReceivedHandler);
            GameSparksMatchChatManager.instance.onMatchUpdatedMessageSignal.Add(OnMatchUpdatedMessageHandler);
            //GameSparksMatchChatManager.instance.onPacketReceivedSignal.Add(OnPacketReceivedHandler);

            sendMessageBttn.onClick.AddListener(SendMessage); // we add a listener to the send message button so we can choose what happens when this button is clicked

            UpdateChatLog();
        }

        private void OnDestroy()
        {
            if (GameSparksMatchChatManager.instance != null)
            {
                GameSparksMatchChatManager.instance.onPacketReceivedSignal.Remove(OnPacketReceivedHandler);
                GameSparksMatchChatManager.instance.onMatchUpdatedMessageSignal.Remove(OnMatchUpdatedMessageHandler);
                //GameSparksMatchChatManager.instance.onPacketReceivedSignal.Remove(OnPacketReceivedHandler);
            }
        }

        private string UpdatePlayerList()
        {
            //recipientOption.ClearOptions();
            //recipientOption.options.Add(new Dropdown.OptionData() { text = "To All" });
            bool addOption = true;
            bool removeOption = true;
            Dropdown.OptionData[] options;
            string displayName = "";

            // options will have "To All" and Player List will have current user, therefore if they equal each other then don't update.
            if (recipientOption.options.Count > GameSparksMatchChatManager.instance.GetSessionInfo().GetPlayerList().Count)
            {
                // remove
                options = recipientOption.options.ToArray();
                for (int i = 0; i < options.Length; i++)
                {
                    removeOption = true;
                    foreach (RTPlayer player in GameSparksMatchChatManager.instance.GetSessionInfo().GetPlayerList())
                    {
                        if (options[i].text == player.displayName)
                        {
                            removeOption = false;
                        }
                    }

                    if (removeOption)
                    {
                        recipientOption.options.Remove(options[i]);
                        displayName = options[i].text + " LEFT!!";
                    }
                }

                return displayName;
            }
            else if (recipientOption.options.Count < GameSparksMatchChatManager.instance.GetSessionInfo().GetPlayerList().Count)
            {
                // add
                options = recipientOption.options.ToArray();
                foreach (RTPlayer player in GameSparksMatchChatManager.instance.GetSessionInfo().GetPlayerList())
                {
                    // we don't want to add the option to send a message to ourselves, so we'll use our own peerId to exclude this option; we'll only be able to send messages to others //
                    if (player.peerId != GameSparksMatchChatManager.instance.GetRTSession().PeerId)
                    {
                        addOption = true;
                        for (int i = 0; i < options.Length; i++)
                        {
                            if (options[i].text == player.displayName)
                            {
                                addOption = false;
                            }
                        }

                        if (addOption)
                        {
                            recipientOption.options.Add(new Dropdown.OptionData() { text = player.displayName });
                            displayName = player.displayName + " JOINED!!";
                        }
                    }
                }

                return displayName;
            }

            return displayName;
        }

        /// <summary>
        /// This method will check the player the message is being sent to, and then construct an
        /// RTData packet and send the packet with the current player's message to the chosen player
        /// </summary>
        private void SendMessage()
        {
            if (messageInput.text != string.Empty)
            { // first check to see if there is any message to send
              // for all RT-data we are sending, we use an instance of the RTData object //
              // this is a disposable object, so we wrap it in this using statement to make sure it is returned to the pool //
                using (RTData data = RTData.Get())
                {
                    data.SetString(1, messageInput.text); // we add the message data to the RTPacket at key '1', so we know how to key it when the packet is receieved
                    data.SetString(2, DateTime.Now.ToString()); // we are also going to send the time at which the user sent this message

                    GameSparksMatchChatManager.instance.UpdateChatLog("Me", messageInput.text, DateTime.Now.ToString()); // we will update the chat-log for the current user to display the message they just sent
                    UpdateChatLog();
                    messageInput.text = string.Empty; // and we clear the message window

                    if (recipientOption.options[recipientOption.value].text == "To All")
                    { // we check to see if the packet is sent to all players
                        Debug.Log("Sending Message to All Players... \n" + messageInput.text);
                        // for this example we are sending RTData, but there are other methods for sending data we will look at later //
                        // the first parameter we use is the op-code. This is used to index the type of data being send, and so we can identify to ourselves which packet this is when it is received //
                        // the second parameter is the delivery intent. The intent we are using here is 'reliable', which means it will be send via TCP. This is because we aren't concerned about //
                        // speed when it comes to these chat messages, but we very much want to make sure the whole packet is received //
                        // the final parameter is the RTData object itself //
                        GameSparksMatchChatManager.instance.GetRTSession().SendData(1, GameSparks.RT.GameSparksRT.DeliveryIntent.RELIABLE, data);
                    }
                    else
                    {
                        // if we are not sending the message to all players, then we need to search through the players we wish to send it to, so we can get their peerId //
                        foreach (RTPlayer player in GameSparksMatchChatManager.instance.GetSessionInfo().GetPlayerList())
                        {
                            if (recipientOption.options[recipientOption.value].text == player.displayName)
                            { // check the display name matching the player
                                Debug.Log("Sending Message to " + player.displayName + " ... \n" + messageInput.text);
                                // all methods for sending packets have the option to send to a specific player //
                                // if this option is left out, it will send to all players //
                                // in order to send to a specific player(s) you will need to create an array of ints corresponding to the player's peerId (what we called playerNo in the last tutorial)
                                GameSparksMatchChatManager.instance.GetRTSession().SendData(1, GameSparks.RT.GameSparksRT.DeliveryIntent.RELIABLE, data, new int[] { player.peerId });
                            }
                        }
                    }
                }
            }
            else
            {
                Debug.Log("Not Chat Message To Send...");
            }
        }

        private IEnumerator SetChatScrollBarEnumerator()
        {
            yield return new WaitForSeconds(.5f);

            chatScrollBar.value = 0;
        }

        // HANDLERS
        private void OnMatchUpdatedMessageHandler(MatchUpdatedMessage message)
        {
            GameSparksMatchChatManager.instance.chatLog.Enqueue("<b>" + UpdatePlayerList() + "</b>");

            chatLogOutput.text = string.Empty; // we need to clear the log, otherwise we always get the same messages repeating
            foreach (string logEntry in GameSparksMatchChatManager.instance.chatLog.ToArray())
            { // go through each chat item and add the entry to the output log
                chatLogOutput.text += logEntry + "\n";
            }
            chatScrollBar.value = 0;
            StartStopCoroutine.StartCoroutine(ref setChatScrollBarCoroutine, SetChatScrollBarEnumerator(), this);
        }

        /// <summary>
        /// This is called from the GameSparksManager class.
        /// It send any packets with op-code '1' to the chat manager so the chat manager can parse the necessary details out for display in the chat log window
        /// </summary>
        /// <param name="_packet">Data.</param>
        private void OnPacketReceivedHandler(RTPacket _packet)
        {
            //Debug.Log("Message Received...\n" + _packet.Data.GetString(1)); // the RTData we sent the message with used an index '1' so we can parse the data back using the same index
            //                                                                // we need the display name of the sender. We get this by using the packet sender id and comparing that to the peerId of the player //
            //foreach (RTPlayer player in GameSparksMatchChatManager.instance.GetSessionInfo().GetPlayerList())
            //{
                //if (player.peerId == _packet.Sender)
                //{
                    // we want to get the message and time and print those to the local users chat-log //
                    UpdateChatLog();
                //}
            //}
        }

        /// <summary>
        /// This method will update the current user's chat log and print the new log to the screen.
        /// </summary>
        private void UpdateChatLog()
        {
            // In this example, the message we want to display is formatted so that we can distinguish each part of the message when //
            // it is displayed, all the information is clearly visible //
            //GameSparksMatchChatManager.instance.chatLog.Enqueue("<b>" + _sender + "</b>\n<color=black>" + _message + "</color>" + "\n<i>" + _date + "</i>");
            //if (chatLog.Count > elementsInChatLog)
            //{ // if we have exceeded the amount of messages in the log, then remove the top message
            //    chatLog.Dequeue();
            //}
            chatLogOutput.text = string.Empty; // we need to clear the log, otherwise we always get the same messages repeating
            foreach (string logEntry in GameSparksMatchChatManager.instance.chatLog.ToArray())
            { // go through each chat item and add the entry to the output log
                chatLogOutput.text += logEntry + "\n";
            }
            chatScrollBar.value = 0;
            StartStopCoroutine.StartCoroutine(ref setChatScrollBarCoroutine, SetChatScrollBarEnumerator(), this);
        }

        /// <summary>
        /// This is called by the chatToggleBttn and it will enable/disable the chat window
        /// </summary>
        private void ToogleChatWindow()
        {
            isChatWindowOpen = !isChatWindowOpen;
            if (isChatWindowOpen)
            {
                chatWindow.SetActive(true); // toggle the gameobject on and off
                chatToogleBttn.transform.GetComponentInChildren<Text>().text = "End Chat"; // set the text on the button to show the chat window is on or off
            }
            else
            {
                chatWindow.SetActive(false);
                chatToogleBttn.transform.GetComponentInChildren<Text>().text = "Start Chat";
            }
        }

        // BUTTON HANDLERS
        public void BtnClose()
        {
            gameObject.SetActive(false);
        }
    }
}