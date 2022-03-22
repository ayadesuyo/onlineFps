using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomInfoItem : MonoBehaviour
{
    [SerializeField] Text roomName;

    public void Set(string _roomName)
    {
        roomName.text = _roomName;
    }

    /// <summary>
    /// ƒNƒŠƒbƒN‚·‚é‚ÆŒÄ‚Î‚ê‚é
    /// </summary>
    public void JoinRoom()
    {
        Launcher.JoinRoom(roomName.text);
    }
}
