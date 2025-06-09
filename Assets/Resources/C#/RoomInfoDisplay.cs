using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class RoomInfoDisplay : MonoBehaviourPunCallbacks
{
    public List<TMP_Text> roomTexts; // UIに配置された10個のText（room1～room10の「n/2人」）

    private Dictionary<string, RoomInfo> roomDict = new Dictionary<string, RoomInfo>();

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        roomDict.Clear();

        foreach (RoomInfo room in roomList)
        {
            if (room.RemovedFromList) continue;
            roomDict[room.Name] = room;
        }

        UpdateRoomText();
    }

    void UpdateRoomText()
    {
        for (int i = 0; i < roomTexts.Count; i++)
        {
            string roomName = "Room" + (i + 1); // Room1～Room10

            if (roomDict.TryGetValue(roomName, out RoomInfo info))
            {
                roomTexts[i].text = $"{info.PlayerCount}人/2人";
            }
            else
            {
                roomTexts[i].text = "0人/2人";
            }
        }
    }
}
