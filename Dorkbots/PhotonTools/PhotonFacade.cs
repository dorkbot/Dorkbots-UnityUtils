using System;
using System.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;

namespace Dorkbots.PhotonTools
{
    public class PhotonFacade : MonoBehaviourPunCallbacks
    {
        private Action<Player> _onMasterClientSwitchedAction;

        //PUN CALLS
        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            base.OnMasterClientSwitched(newMasterClient);
            _onMasterClientSwitchedAction?.Invoke(newMasterClient);
        }
        
        public void IsMasterClient(Action<bool> callback)
        {
            if (PhotonNetwork.MasterClient != null)
            {
                callback(PhotonNetwork.IsMasterClient);
            }
            else
            {
                _onMasterClientSwitchedAction += OnMasterClientSwitchedHandler;

                void OnMasterClientSwitchedHandler(Player newMasterClient)
                {
                    _onMasterClientSwitchedAction -= OnMasterClientSwitchedHandler;
                    callback(PhotonNetwork.IsMasterClient);
                }
            }
        }
        
        public async Task<bool> IsMasterClient()
        {
            if (PhotonNetwork.MasterClient != null)
            {
                return PhotonNetwork.IsMasterClient;
            }
            else
            {
                TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
                _onMasterClientSwitchedAction += OnMasterClientSwitchedHandler;
                await taskCompletionSource.Task;
                return taskCompletionSource.Task.Result;
                
                void OnMasterClientSwitchedHandler(Player newMasterClient)
                {
                    _onMasterClientSwitchedAction -= OnMasterClientSwitchedHandler;
                    taskCompletionSource.SetResult(PhotonNetwork.IsMasterClient);
                }
            }
        }
    }
}