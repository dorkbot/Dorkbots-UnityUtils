using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dorkbots.UpdateDataQueue
{
    /// <summary>
    /// This helps to queue updates to data before sending and after getting
    /// </summary>
    public interface IUpdateDataQueueService
    {
        void Dispose();
        void AddGetStartAction(string data, Action<Action<string>> startAction);
        void RemoveGetStartAction(string data);
        bool DoesGetStartActionExist(string data);
        void AddUpdateAfterGetAction(string data, Action downloadComplete);
        void RemoveUpdateAfterGetAction(string data, Action getComplete);
        void AddPutStartAction(string data, Action<Action<string>> startAction);
        void RemovePutStartAction(string data);
        bool DoesPutStartActionExist(string data);
        void AddUpdateBeforePutAction(string data, Action updateBeforePutAction);
        void RemovePutBeforePutAction(string data, Action updateBeforePutAction);
        void RemoveAllPutBeforePutActions(string data);
    }
    /// <summary>
    /// This helps to queue updates to data before sending and after getting
    /// </summary>
    public class UpdateDataQueueService : IUpdateDataQueueService
    {
        //This action is called BEFORE calling update actions for the same data. The intent is for the action to save the data to disk (ex: serialize JSON, etc.) before the update actions do their thing with the data.
        private Dictionary<string, Action<Action<string>>> _getStartActions = new Dictionary<string, Action<Action<string>>>();
        //These actions are called in order and after getting data
        private Dictionary<string, List<Action>> _updateAfterGetActions = new Dictionary<string, List<Action>>();
        //These actions are called in order and before putting Data
        private Dictionary<string, List<Action>> _updateBeforePutActions = new Dictionary<string, List<Action>>();
        //These actions are called after the other actions have made updates and before putting the data. (ex: serialize a class to JSON and send to server)
        private Dictionary<string, Action<Action<string>>> _putStartActions = new Dictionary<string, Action<Action<string>>>();//check and then call this action after calling actions in _updateAfterGetActions
        
        private string _currentDataPutting = String.Empty;
        private string _currentDataSending = String.Empty;
        
        public UpdateDataQueueService()
        {
        }

        public void Dispose()
        {
            _getStartActions = null;
            _updateAfterGetActions = null;
            _updateBeforePutActions = null;
            _putStartActions = null;
        }
        
        private void StartNextData()
        {
            if (_updateAfterGetActions.Keys.Count > 0)
            {
                StartGet(_updateAfterGetActions.First().Key);
            }
            else if (_updateBeforePutActions.Keys.Count > 0)
            {
                StartPut(_updateBeforePutActions.First().Key);
            }
        }

        /*
         * Get data
         */
        
        public void AddGetStartAction(string data, Action<Action<string>> startAction)
        {
            if (_getStartActions.ContainsKey(data))
            {
                Debug.LogError("Start get action already added, remove before adding new one!");
            }
            else
            {
                _getStartActions.Add(data, startAction);
            }
        }
        
        public void RemoveGetStartAction(string data)
        {
            _getStartActions.Remove(data);
        }
        
        public bool DoesGetStartActionExist(string data)
        {
            return _getStartActions.ContainsKey(data);
        }
        
        public void AddUpdateAfterGetAction(string data, Action getComplete)
        {
            if (_updateAfterGetActions.TryGetValue(data, out var actions))
            {
                actions.Add(getComplete);
            }
            else
            {
                _updateAfterGetActions.Add(data, new List<Action> {getComplete});
            }

            if (String.IsNullOrEmpty(_currentDataPutting)  && String.IsNullOrEmpty(_currentDataSending))
            {
                StartGet(data);
            }
        }

        public void RemoveUpdateAfterGetAction(string data, Action getComplete)
        {
            if (_updateAfterGetActions.TryGetValue(data, out var actions))
            {
                actions.Remove(getComplete);
                if (actions.Count == 0)
                {
                    _updateAfterGetActions.Remove(data);
                }
            }
        }
        
        public void RemoveAllUpdateAfterDownloadActions(string data)
        {
            _updateAfterGetActions.Remove(data);
        }

        private void StartGet(string data)
        {
            if (!_getStartActions.ContainsKey(data))
            {
                Debug.LogError("There is no start get action for the data " + data);
                return;
            }
            
            if (!String.IsNullOrEmpty(_currentDataPutting) || !String.IsNullOrEmpty(_currentDataSending))
            {
                return;//a data is getting so don't start yet
            }
            
            _currentDataPutting = data;
            _getStartActions[data].Invoke(GetCompleteHandler);//the intent is that another member handles getting and then invokes the action to let us know the get is complete
        }
        
        private void GetCompleteHandler(string data)//called by the member that gets the data
        {
            if (_updateAfterGetActions.TryGetValue(data, out var actions))
            {
                for (int i = 0; i < actions.Count; i++)//Invoke all actions, getting data completed
                {
                    actions[i].Invoke();
                }
                _updateAfterGetActions.Remove(data);
            }
            
            _currentDataPutting = String.Empty;

            StartNextData();
        }
        
        /*
         * Put data
         */
        
        public void AddPutStartAction(string data, Action<Action<string>> startAction)
        {
            if (_putStartActions.ContainsKey(data))
            {
                Debug.LogError("Start put action already added, remove before adding new one!");
            }
            else
            {
                _putStartActions.Add(data, startAction);
            }
        }
        
        public void RemovePutStartAction(string data)
        {
            _putStartActions.Remove(data);
        }

        public bool DoesPutStartActionExist(string data)
        {
            return _putStartActions.ContainsKey(data);
        }
        
        public void AddUpdateBeforePutAction(string data, Action updateBeforePutAction)
        {
            if (_updateBeforePutActions.TryGetValue(data, out var actions))
            {
                actions.Add(updateBeforePutAction);
            }
            else
            {
                _updateBeforePutActions.Add(data, new List<Action> {updateBeforePutAction});
            }
            
            if (String.IsNullOrEmpty(_currentDataPutting) && String.IsNullOrEmpty(_currentDataSending))
            {
                StartPut(data);
            }
        }
        
        public void RemovePutBeforePutAction(string data, Action updateBeforePutAction)
        {
            if (_updateBeforePutActions.TryGetValue(data, out var actions))
            {
                actions.Remove(updateBeforePutAction);
                if (actions.Count == 0)
                {
                    _updateBeforePutActions.Remove(data);
                }
            }
        }

        public void RemoveAllPutBeforePutActions(string data)
        {
            _updateBeforePutActions.Remove(data);
        }

        private void StartPut(string data)
        {
            if (!_putStartActions.ContainsKey(data))
            {
                Debug.LogError("There is no start put action for the data " + data);
            }
            
            if (!String.IsNullOrEmpty(_currentDataPutting) || !String.IsNullOrEmpty(_currentDataSending))
            {
                return;//a data is downloading so don't start yet
            }

            _currentDataSending = data;
            
            for (int i = 0; i < _updateBeforePutActions[data].Count; i++)
            {
                _updateBeforePutActions[data][i].Invoke();
            }
            
            _updateBeforePutActions.Remove(data);
            
            _putStartActions[data].Invoke(PutCompleteHandler);//start put
        }

        private void PutCompleteHandler(string file)
        {
            _currentDataSending = String.Empty;
            
            StartNextData();
        }
    }
}