using ExitGames.Client.Photon;
using Photon;
using System;
using System.Collections.Generic;
using UnityEngine;
using SpaceFox;

namespace SpaceFox
{
    public static class Text
    {
        public static string Bold(this string s) => $"<b>{s}</b>";
        public static string Size(this string s, int r = 10) => $"<size={r}>{s}</size>";
        /// <summary>
        /// Converts Text to Colored Text
        /// </summary>
        /// <param name="s">A string containing the message to be colored</param>
        /// <param name="HexColor">a hex color value with a #prefix (#0077ff)</param>
        /// <returns></returns>
        public static string Color(this string s, string HexColorP = "#0077ff" /*yeah thats my favourite color*/ ) => $"<color={HexColorP}>{s}</color>";

        public static string msc(this string s, int n)
        {
            string srx = string.Empty;
            int m = s.Length;
            if (s.Length > n) m = n; 
            for (var i = 0; i < m; i++) srx += s[i];
            srx += "...";
            return srx;
        }

        public static string ColorRGB(this string s, byte r, byte g, byte b) => $"<color=#{r.ToString("X2")}{g.ToString("X2")}{b.ToString("X2")}>{s}</color>";
    }
}

public class InRoomChat : Photon.MonoBehaviour
{
    private bool AlignBottom = true;
    const string DefaultColor = "#FFCC00";
    public static readonly string ChatRPC = "Chat";
    public static Rect GuiRect = new Rect(0f, 100f, 300f, 470f);
    public static Rect GuiRect2 = new Rect(30f, 575f, 300f, 25f);
    private string inputLine = string.Empty;
    public bool IsVisible = true;
    public static List<string> messages = new List<string>();
    private Vector2 scrollPos = Vector2.zero;
    InRoomChat instance;

    public void addLINE(string newLine)
    {
        messages.Add(newLine);
    }

    public void AddLine(string newLine)
    {
        messages.Add(newLine);
    }

    public void OnGUI()
    {
        int num4;
        if (!this.IsVisible || (PhotonNetwork.connectionStatesDetailed != PeerStates.Joined))
        {
            return;
        }
        if (Event.current.type == EventType.KeyDown)
        {
            if ((((Event.current.keyCode != KeyCode.Tab) && (Event.current.character != '\t')) || IN_GAME_MAIN_CAMERA.isPausing) || (FengGameManagerMKII.inputRC.humanKeys[InputCodeRC.chat] == KeyCode.Tab))
            {
                goto Label_00E1;
            }
            Event.current.Use();
            goto Label_013D;
        }
        if ((Event.current.type == EventType.KeyUp) && (((Event.current.keyCode != KeyCode.None) && (Event.current.keyCode == FengGameManagerMKII.inputRC.humanKeys[InputCodeRC.chat])) && (GUI.GetNameOfFocusedControl() != "ChatInput")))
        {
            this.inputLine = string.Empty;
            GUI.FocusControl("ChatInput");
            goto Label_013D;
        }
    Label_00E1:
        if ((Event.current.type == EventType.KeyDown) && ((Event.current.keyCode == KeyCode.KeypadEnter) || (Event.current.keyCode == KeyCode.Return)))
        {
            if (!string.IsNullOrEmpty(this.inputLine))
            {
                string str2;
                if (this.inputLine == "\t")
                {
                    this.inputLine = string.Empty;
                    GUI.FocusControl(string.Empty);
                    return;
                }
                if (FengGameManagerMKII.RCEvents.ContainsKey("OnChatInput"))
                {
                    string key = (string) FengGameManagerMKII.RCVariableNames["OnChatInput"];
                    if (FengGameManagerMKII.stringVariables.ContainsKey(key))
                    {
                        FengGameManagerMKII.stringVariables[key] = this.inputLine;
                    }
                    else
                    {
                        FengGameManagerMKII.stringVariables.Add(key, this.inputLine);
                    }
                    ((RCEvent) FengGameManagerMKII.RCEvents["OnChatInput"]).checkEvent();
                }
                if (!this.inputLine.StartsWith("/"))
                {
                    str2 = RCextensions.returnStringFromObject(PhotonNetwork.player.customProperties[PhotonPlayerProperty.name]).hexColor();
                    if (str2 == string.Empty)
                    {
                        str2 = RCextensions.returnStringFromObject(PhotonNetwork.player.customProperties[PhotonPlayerProperty.name]);
                        if (PhotonNetwork.player.customProperties[PhotonPlayerProperty.RCteam] != null)
                        {
                            if (RCextensions.returnIntFromObject(PhotonNetwork.player.customProperties[PhotonPlayerProperty.RCteam]) == 1)
                            {
                                str2 = $"<color=#00FFFF>{str2}</color>";
                            }
                            else if (RCextensions.returnIntFromObject(PhotonNetwork.player.customProperties[PhotonPlayerProperty.RCteam]) == 2)
                            {
                                str2 = $"<color=#FF00FF>{str2}</color>";
                            }
                        }
                    }
                    object[] parameters = new object[] { this.inputLine, str2 };
                    FengGameManagerMKII.instance.photonView.RPC("Chat", PhotonTargets.All, parameters);
                }
                string[] cmdLineArgs = inputLine.Split();
                //Does the switch even work? i havent tested a single command til now
                //but in theory it should work just perfectly, just gotta add a few more FUCKS
                switch (cmdLineArgs[0])
                { //Why switch? CUZ IT SLAPS NEXT QUESTION.
                    case "/cloth":
                        AddLine(ClothFactory.GetDebugInfo());
                        break;
                    case "/aso":
                        if(cmdLineArgs.Length > 1)
                        {
                            if (PhotonNetwork.isMasterClient)
                            {
                                switch (cmdLineArgs[1])
                                {
                                    case "kdr":
                                        if(RCSettings.asoPreservekdr == 0)
                                        {
                                            RCSettings.asoPreservekdr = 1;
                                            addLINE("KDR's will be preserved from disconnects.".Color("#FFCC00"));
                                        }
                                        else
                                        {
                                            RCSettings.asoPreservekdr = 0;
                                            addLINE("KDR's will not be preserved from disconnects".Color("#FFCC00"));
                                        }
                                        break;
                                    case "racing":
                                        if(RCSettings.racingStatic == 0)
                                        {
                                            RCSettings.racingStatic = 1;
                                            addLINE("Racing will not end on finish.".Color("#FFCC00"));
                                        }
                                        else
                                        {
                                            RCSettings.racingStatic = 0;
                                            addLINE("Racing will end on finish.".Color("#FFCC00"));
                                        }
                                        break;
                                    default:
                                        // ignore
                                        break;
                                }
                            }
                        }
                        break;
                    case "/pause":
                        if (PhotonNetwork.isMasterClient)
                        {
                            FengGameManagerMKII.instance.photonView.RPC("pauseRPC", PhotonTargets.All, new object[] { true });
                            FengGameManagerMKII.instance.photonView.RPC(ChatRPC, PhotonTargets.All, new object[] { "MasterClient has paused the Game.".Color(DefaultColor), string.Empty });
                        }
                        else
                            addLINE("Error: Not MasterClient".Color(DefaultColor));
                        break;
                    case "/unpause":
                        if (PhotonNetwork.isMasterClient)
                        {
                            FengGameManagerMKII.instance.photonView.RPC("pauseRPC", PhotonTargets.All, new object[] { false });
                            FengGameManagerMKII.instance.photonView.RPC(ChatRPC, PhotonTargets.All, new object[] { "MasterClient has unpaused the Game.".Color(DefaultColor), string.Empty });
                        }
                        else
                            addLINE("Error: Not MasterClient".Color(DefaultColor));
                        break;
                    case "/checklevel":
                        foreach (PhotonPlayer p in PhotonNetwork.playerList)
                            addLINE(RCextensions.returnStringFromObject(p.customProperties[PhotonPlayerProperty.currentLevel]));
                        break;
                    case "/isrc":
                        //WHAT THE FUCK IS THIS EVEN FOR? WHO USES IT?!
                        //right.nobody. yeet it away. WHY IS THAT A THING EEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE
                        addLINE(FengGameManagerMKII.masterRC ? "Is RC" : "Not RC");
                        break;
                    case "/ignorelist":
                        foreach (int n in FengGameManagerMKII.ignoreList)
                            addLINE(n.ToString());
                        break;
                    case "/room":
                        if (PhotonNetwork.isMasterClient)
                        {
                            if (cmdLineArgs.Length > 2)
                            {
                                if (cmdLineArgs[1] == "max")
                                {
                                    int n = Convert.ToInt32(cmdLineArgs[2]);
                                    FengGameManagerMKII.instance.maxPlayers = n;
                                    PhotonNetwork.room.maxPlayers = n;
                                    FengGameManagerMKII.instance.photonView.RPC(ChatRPC, PhotonTargets.All, new object[] {
                                        $"Max Players changed to {n}!".Color(DefaultColor),
                                        string.Empty
                                    });
                                }
                                else if (cmdLineArgs[1] == "time")
                                {
                                    float n = Convert.ToSingle(cmdLineArgs[2]);
                                    FengGameManagerMKII.instance.addTime(n);
                                    FengGameManagerMKII.instance.photonView.RPC(ChatRPC, PhotonTargets.All, new object[]
                                    {
                                        $"{n} seconds added to the clock".Color(DefaultColor),
                                        string.Empty
                                    });
                                }
                                else addLINE("Invalid argument");
                            }
                        }
                        else addLINE("Not MasterClient.");
                        break;
                    case "/resetkd":
                        Hashtable resetkd;
                        resetkd = new Hashtable
                        {
                            { PhotonPlayerProperty.kills, 0 },
                            { PhotonPlayerProperty.deaths, 0 },
                            { PhotonPlayerProperty.max_dmg, 0 },
                            { PhotonPlayerProperty.total_dmg, 0 }
                        };
                        PhotonNetwork.player.SetCustomProperties(resetkd);
                        addLINE("Your stats have been reset");
                        break;
                    case "/resetkdall":
                        if (PhotonNetwork.isMasterClient)
                        {
                            Hashtable resetkdall = new Hashtable() {
                                {PhotonPlayerProperty.kills, 0 },
                                {PhotonPlayerProperty.deaths, 0 },
                                {PhotonPlayerProperty.max_dmg, 0 },
                                {PhotonPlayerProperty.total_dmg, 0 }
                            };
                            foreach (var p in PhotonNetwork.playerList)
                            {
                                p.SetCustomProperties(resetkdall);
                            }
                            FengGameManagerMKII.instance.photonView.RPC(
                                ChatRPC, 
                                PhotonTargets.All, 
                                new object[] { "All stats have been reset".Color(DefaultColor), string.Empty });
                        }
                        else addLINE("Not MasterClient");
                        break;
                    case "/pm":
                        int ID = Convert.ToInt32(cmdLineArgs[1]);
                        var Player = PhotonPlayer.Find(ID);
                        string Content = string.Empty;
                        for (int i = 2; i < cmdLineArgs.Length; i++) Content += $"{cmdLineArgs[i]} ";
                        FengGameManagerMKII.instance.photonView.RPC(
                            "ChatPM", 
                            Player, 
                            new object[] {
                                Content,
                                RCextensions.returnStringFromObject(PhotonNetwork.player.customProperties[PhotonPlayerProperty.name]).hexColor()
                            });
                        addLINE($"Send Private Message to {Player.name} with Content ({Content.msc(16)})".Color("#0077ff").Bold());
                        break;
                    default:
                        // ignore
                        break;
                }
                #region Comment
                /*
                 if (this.inputLine.StartsWith("/pm"))
                {
                    string[] strArray = this.inputLine.Split(new char[] { ' ' });
                    PhotonPlayer targetPlayer = PhotonPlayer.Find(Convert.ToInt32(strArray[1]));
                    str2 = RCextensions.returnStringFromObject(PhotonNetwork.player.customProperties[PhotonPlayerProperty.name]).hexColor();
                    if (str2 == string.Empty)
                    {
                        str2 = RCextensions.returnStringFromObject(PhotonNetwork.player.customProperties[PhotonPlayerProperty.name]);
                        if (PhotonNetwork.player.customProperties[PhotonPlayerProperty.RCteam] != null)
                        {
                            if (RCextensions.returnIntFromObject(PhotonNetwork.player.customProperties[PhotonPlayerProperty.RCteam]) == 1)
                            {
                                str2 = "<color=#00FFFF>" + str2 + "</color>";
                            }
                            else if (RCextensions.returnIntFromObject(PhotonNetwork.player.customProperties[PhotonPlayerProperty.RCteam]) == 2)
                            {
                                str2 = "<color=#FF00FF>" + str2 + "</color>";
                            }
                        }
                    }
                    string str3 = RCextensions.returnStringFromObject(targetPlayer.customProperties[PhotonPlayerProperty.name]).hexColor();
                    if (str3 == string.Empty)
                    {
                        str3 = RCextensions.returnStringFromObject(targetPlayer.customProperties[PhotonPlayerProperty.name]);
                        if (targetPlayer.customProperties[PhotonPlayerProperty.RCteam] != null)
                        {
                            if (RCextensions.returnIntFromObject(targetPlayer.customProperties[PhotonPlayerProperty.RCteam]) == 1)
                            {
                                str3 = "<color=#00FFFF>" + str3 + "</color>";
                            }
                            else if (RCextensions.returnIntFromObject(targetPlayer.customProperties[PhotonPlayerProperty.RCteam]) == 2)
                            {
                                str3 = "<color=#FF00FF>" + str3 + "</color>";
                            }
                        }
                    }
                    string str4 = string.Empty;
                    for (num4 = 2; num4 < strArray.Length; num4++)
                    {
                        str4 += strArray[num4] + " ";
                    }
                    FengGameManagerMKII.instance.photonView.RPC("ChatPM", targetPlayer, new object[] { str2, str4 });
                    this.addLINE("<color=#FFC000>TO [" + targetPlayer.ID.ToString() + "]</color> " + str3 + ":" + str4);
                }
                 */
                #endregion
                if (this.inputLine.StartsWith("/team"))
                {
                    if (RCSettings.teamMode == 1)
                    {
                        if ((this.inputLine.Substring(6) == "1") || (this.inputLine.Substring(6) == "cyan"))
                        {
                            FengGameManagerMKII.instance.photonView.RPC("setTeamRPC", PhotonNetwork.player, new object[] { 1 });
                            this.addLINE("<color=#00FFFF>You have joined team cyan.</color>");
                            foreach (GameObject obj2 in GameObject.FindGameObjectsWithTag("Player"))
                            {
                                if (obj2.GetPhotonView().isMine)
                                {
                                    obj2.GetComponent<HERO>().markDie();
                                    obj2.GetComponent<HERO>().photonView.RPC("netDie2", PhotonTargets.All, new object[] { -1, "Team Switch" });
                                }
                            }
                        }
                        else if ((this.inputLine.Substring(6) == "2") || (this.inputLine.Substring(6) == "magenta"))
                        {
                            FengGameManagerMKII.instance.photonView.RPC("setTeamRPC", PhotonNetwork.player, new object[] { 2 });
                            this.addLINE("<color=#FF00FF>You have joined team magenta.</color>");
                            foreach (GameObject obj3 in GameObject.FindGameObjectsWithTag("Player"))
                            {
                                if (obj3.GetPhotonView().isMine)
                                {
                                    obj3.GetComponent<HERO>().markDie();
                                    obj3.GetComponent<HERO>().photonView.RPC("netDie2", PhotonTargets.All, new object[] { -1, "Team Switch" });
                                }
                            }
                        }
                        else if ((this.inputLine.Substring(6) == "0") || (this.inputLine.Substring(6) == "individual"))
                        {
                            FengGameManagerMKII.instance.photonView.RPC("setTeamRPC", PhotonNetwork.player, new object[] { 0 });
                            this.addLINE("<color=#00FF00>You have joined individuals.</color>");
                            foreach (GameObject obj4 in GameObject.FindGameObjectsWithTag("Player"))
                            {
                                if (obj4.GetPhotonView().isMine)
                                {
                                    obj4.GetComponent<HERO>().markDie();
                                    obj4.GetComponent<HERO>().photonView.RPC("netDie2", PhotonTargets.All, new object[] { -1, "Team Switch" });
                                }
                            }
                        }
                        else
                        {
                            this.addLINE("<color=#FFCC00>error: invalid team code. Accepted values are 0,1, and 2.</color>");
                        }
                    }
                    else
                    {
                        this.addLINE("<color=#FFCC00>error: teams are locked or disabled. </color>");
                    }
                }
                else if (this.inputLine == "/restart")
                {
                    if (PhotonNetwork.isMasterClient)
                    {
                        objArray3 = new object[] { "<color=#FFCC00>MasterClient has restarted the game!</color>", "" };
                        FengGameManagerMKII.instance.photonView.RPC("Chat", PhotonTargets.All, objArray3);
                        FengGameManagerMKII.instance.restartRC();
                    }
                    else
                    {
                        this.addLINE("<color=#FFCC00>error: not master client</color>");
                    }
                }
                else if (this.inputLine.StartsWith("/specmode"))
                {
                    if (((int) FengGameManagerMKII.settings[0xf5]) == 0)
                    {
                        FengGameManagerMKII.settings[0xf5] = 1;
                        FengGameManagerMKII.instance.EnterSpecMode(true);
                        this.addLINE("<color=#FFCC00>You have entered spectator mode.</color>");
                    }
                    else
                    {
                        FengGameManagerMKII.settings[0xf5] = 0;
                        FengGameManagerMKII.instance.EnterSpecMode(false);
                        this.addLINE("<color=#FFCC00>You have exited spectator mode.</color>");
                    }
                }
                else if (this.inputLine.StartsWith("/fov"))
                {
                    int num6 = Convert.ToInt32(this.inputLine.Substring(5));
                    Camera.main.fieldOfView = num6;
                    this.addLINE("<color=#FFCC00>Field of vision set to " + num6.ToString() + ".</color>");
                }
                else if (this.inputLine == "/colliders")
                {
                    int num7 = 0;
                    foreach (TITAN titan in FengGameManagerMKII.instance.getTitans())
                    {
                        if (titan.myTitanTrigger.isCollide)
                        {
                            num7++;
                        }
                    }
                    FengGameManagerMKII.instance.chatRoom.addLINE(num7.ToString());
                }
                else
                {
                    int num8;
                    if (this.inputLine.StartsWith("/spectate"))
                    {
                        num8 = Convert.ToInt32(this.inputLine.Substring(10));
                        foreach (GameObject obj5 in GameObject.FindGameObjectsWithTag("Player"))
                        {
                            if (obj5.GetPhotonView().owner.ID == num8)
                            {
                                Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(obj5, true, false);
                                Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().setSpectorMode(false);
                            }
                        }
                    }
                    else if (!this.inputLine.StartsWith("/kill"))
                    {
                        object[] objArray5;
                        if (this.inputLine.StartsWith("/revive"))
                        {
                            if (PhotonNetwork.isMasterClient)
                            {
                                if (this.inputLine == "/reviveall")
                                {
                                    objArray5 = new object[] { "<color=#FFCC00>All players have been revived.</color>", string.Empty };
                                    FengGameManagerMKII.instance.photonView.RPC("Chat", PhotonTargets.All, objArray5);
                                    foreach (PhotonPlayer player in PhotonNetwork.playerList)
                                    {
                                        if (((player.customProperties[PhotonPlayerProperty.dead] != null) && RCextensions.returnBoolFromObject(player.customProperties[PhotonPlayerProperty.dead])) && (RCextensions.returnIntFromObject(player.customProperties[PhotonPlayerProperty.isTitan]) != 2))
                                        {
                                            FengGameManagerMKII.instance.photonView.RPC("respawnHeroInNewRound", player, new object[0]);
                                        }
                                    }
                                }
                                else
                                {
                                    num8 = Convert.ToInt32(this.inputLine.Substring(8));
                                    foreach (PhotonPlayer player in PhotonNetwork.playerList)
                                    {
                                        if (player.ID == num8)
                                        {
                                            this.addLINE("<color=#FFCC00>Player " + num8.ToString() + " has been revived.</color>");
                                            if (((player.customProperties[PhotonPlayerProperty.dead] != null) && RCextensions.returnBoolFromObject(player.customProperties[PhotonPlayerProperty.dead])) && (RCextensions.returnIntFromObject(player.customProperties[PhotonPlayerProperty.isTitan]) != 2))
                                            {
                                                objArray5 = new object[] { "<color=#FFCC00>You have been revived by the master client.</color>", string.Empty };
                                                FengGameManagerMKII.instance.photonView.RPC("Chat", player, objArray5);
                                                FengGameManagerMKII.instance.photonView.RPC("respawnHeroInNewRound", player, new object[0]);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                this.addLINE("<color=#FFCC00>error: not master client</color>");
                            }
                        }
                        else if (this.inputLine.StartsWith("/unban"))
                        {
                            if (FengGameManagerMKII.OnPrivateServer)
                            {
                                FengGameManagerMKII.ServerRequestUnban(this.inputLine.Substring(7));
                            }
                            else if (PhotonNetwork.isMasterClient)
                            {
                                int num9 = Convert.ToInt32(this.inputLine.Substring(7));
                                if (FengGameManagerMKII.banHash.ContainsKey(num9))
                                {
                                    objArray5 = new object[] { "<color=#FFCC00>" + ((string) FengGameManagerMKII.banHash[num9]) + " has been unbanned from the server. </color>", string.Empty };
                                    FengGameManagerMKII.instance.photonView.RPC("Chat", PhotonTargets.All, objArray5);
                                    FengGameManagerMKII.banHash.Remove(num9);
                                }
                                else
                                {
                                    this.addLINE("error: no such player");
                                }
                            }
                            else
                            {
                                this.addLINE("<color=#FFCC00>error: not master client</color>");
                            }
                        }
                        else if (this.inputLine.StartsWith("/rules"))
                        {
                            this.addLINE("<color=#FFCC00>Currently activated gamemodes:</color>");
                            if (RCSettings.bombMode > 0)
                            {
                                this.addLINE("<color=#FFCC00>Bomb mode is on.</color>");
                            }
                            if (RCSettings.teamMode > 0)
                            {
                                if (RCSettings.teamMode == 1)
                                {
                                    this.addLINE("<color=#FFCC00>Team mode is on (no sort).</color>");
                                }
                                else if (RCSettings.teamMode == 2)
                                {
                                    this.addLINE("<color=#FFCC00>Team mode is on (sort by size).</color>");
                                }
                                else if (RCSettings.teamMode == 3)
                                {
                                    this.addLINE("<color=#FFCC00>Team mode is on (sort by skill).</color>");
                                }
                            }
                            if (RCSettings.pointMode > 0)
                            {
                                this.addLINE("<color=#FFCC00>Point mode is on (" + Convert.ToString(RCSettings.pointMode) + ").</color>");
                            }
                            if (RCSettings.disableRock > 0)
                            {
                                this.addLINE("<color=#FFCC00>Punk Rock-Throwing is disabled.</color>");
                            }
                            if (RCSettings.spawnMode > 0)
                            {
                                this.addLINE("<color=#FFCC00>Custom spawn rate is on (" + RCSettings.nRate.ToString("F2") + "% Normal, " + RCSettings.aRate.ToString("F2") + "% Abnormal, " + RCSettings.jRate.ToString("F2") + "% Jumper, " + RCSettings.cRate.ToString("F2") + "% Crawler, " + RCSettings.pRate.ToString("F2") + "% Punk </color>");
                            }
                            if (RCSettings.explodeMode > 0)
                            {
                                this.addLINE("<color=#FFCC00>Titan explode mode is on (" + Convert.ToString(RCSettings.explodeMode) + ").</color>");
                            }
                            if (RCSettings.healthMode > 0)
                            {
                                this.addLINE("<color=#FFCC00>Titan health mode is on (" + Convert.ToString(RCSettings.healthLower) + "-" + Convert.ToString(RCSettings.healthUpper) + ").</color>");
                            }
                            if (RCSettings.infectionMode > 0)
                            {
                                this.addLINE("<color=#FFCC00>Infection mode is on (" + Convert.ToString(RCSettings.infectionMode) + ").</color>");
                            }
                            if (RCSettings.damageMode > 0)
                            {
                                this.addLINE("<color=#FFCC00>Minimum nape damage is on (" + Convert.ToString(RCSettings.damageMode) + ").</color>");
                            }
                            if (RCSettings.moreTitans > 0)
                            {
                                this.addLINE("<color=#FFCC00>Custom titan # is on (" + Convert.ToString(RCSettings.moreTitans) + ").</color>");
                            }
                            if (RCSettings.sizeMode > 0)
                            {
                                this.addLINE("<color=#FFCC00>Custom titan size is on (" + RCSettings.sizeLower.ToString("F2") + "," + RCSettings.sizeUpper.ToString("F2") + ").</color>");
                            }
                            if (RCSettings.banEren > 0)
                            {
                                this.addLINE("<color=#FFCC00>Anti-Eren is on. Using Titan eren will get you kicked.</color>");
                            }
                            if (RCSettings.waveModeOn == 1)
                            {
                                this.addLINE("<color=#FFCC00>Custom wave mode is on (" + Convert.ToString(RCSettings.waveModeNum) + ").</color>");
                            }
                            if (RCSettings.friendlyMode > 0)
                            {
                                this.addLINE("<color=#FFCC00>Friendly-Fire disabled. PVP is prohibited.</color>");
                            }
                            if (RCSettings.pvpMode > 0)
                            {
                                if (RCSettings.pvpMode == 1)
                                {
                                    this.addLINE("<color=#FFCC00>AHSS/Blade PVP is on (team-based).</color>");
                                }
                                else if (RCSettings.pvpMode == 2)
                                {
                                    this.addLINE("<color=#FFCC00>AHSS/Blade PVP is on (FFA).</color>");
                                }
                            }
                            if (RCSettings.maxWave > 0)
                            {
                                this.addLINE("<color=#FFCC00>Max Wave set to " + RCSettings.maxWave.ToString() + "</color>");
                            }
                            if (RCSettings.horseMode > 0)
                            {
                                this.addLINE("<color=#FFCC00>Horses are enabled.</color>");
                            }
                            if (RCSettings.ahssReload > 0)
                            {
                                this.addLINE("<color=#FFCC00>AHSS Air-Reload disabled.</color>");
                            }
                            if (RCSettings.punkWaves > 0)
                            {
                                this.addLINE("<color=#FFCC00>Punk override every 5 waves enabled.</color>");
                            }
                            if (RCSettings.endlessMode > 0)
                            {
                                this.addLINE("<color=#FFCC00>Endless Respawn is enabled (" + RCSettings.endlessMode.ToString() + " seconds).</color>");
                            }
                            if (RCSettings.globalDisableMinimap > 0)
                            {
                                this.addLINE("<color=#FFCC00>Minimaps are disabled.</color>");
                            }
                            if (RCSettings.motd != string.Empty)
                            {
                                this.addLINE("<color=#FFCC00>MOTD:" + RCSettings.motd + "</color>");
                            }
                            if (RCSettings.deadlyCannons > 0)
                            {
                                this.addLINE("<color=#FFCC00>Cannons will kill humans.</color>");
                            }
                        }
                        else
                        {
                            object[] objArray6;
                            bool flag2;
                            object[] objArray7;
                            if (this.inputLine.StartsWith("/kick"))
                            {
                                num8 = Convert.ToInt32(this.inputLine.Substring(6));
                                if (num8 == PhotonNetwork.player.ID)
                                {
                                    this.addLINE("error:can't kick yourself.");
                                }
                                else if (!(FengGameManagerMKII.OnPrivateServer || PhotonNetwork.isMasterClient))
                                {
                                    objArray6 = new object[] { "/kick #" + Convert.ToString(num8), LoginFengKAI.player.name };
                                    FengGameManagerMKII.instance.photonView.RPC("Chat", PhotonTargets.All, objArray6);
                                }
                                else
                                {
                                    flag2 = false;
                                    foreach (PhotonPlayer player3 in PhotonNetwork.playerList)
                                    {
                                        if (num8 == player3.ID)
                                        {
                                            flag2 = true;
                                            if (FengGameManagerMKII.OnPrivateServer)
                                            {
                                                FengGameManagerMKII.instance.kickPlayerRC(player3, false, "");
                                            }
                                            else if (PhotonNetwork.isMasterClient)
                                            {
                                                FengGameManagerMKII.instance.kickPlayerRC(player3, false, "");
                                                objArray7 = new object[] { "<color=#FFCC00>" + RCextensions.returnStringFromObject(player3.customProperties[PhotonPlayerProperty.name]) + " has been kicked from the server!</color>", string.Empty };
                                                FengGameManagerMKII.instance.photonView.RPC("Chat", PhotonTargets.All, objArray7);
                                            }
                                        }
                                    }
                                    if (!flag2)
                                    {
                                        this.addLINE("error:no such player.");
                                    }
                                }
                            }
                            else if (this.inputLine.StartsWith("/ban"))
                            {
                                if (this.inputLine == "/banlist")
                                {
                                    this.addLINE("<color=#FFCC00>List of banned players:</color>");
                                    foreach (int num10 in FengGameManagerMKII.banHash.Keys)
                                    {
                                        this.addLINE("<color=#FFCC00>" + Convert.ToString(num10) + ":" + ((string) FengGameManagerMKII.banHash[num10]) + "</color>");
                                    }
                                }
                                else
                                {
                                    num8 = Convert.ToInt32(this.inputLine.Substring(5));
                                    if (num8 == PhotonNetwork.player.ID)
                                    {
                                        this.addLINE("error:can't kick yourself.");
                                    }
                                    else if (!(FengGameManagerMKII.OnPrivateServer || PhotonNetwork.isMasterClient))
                                    {
                                        objArray6 = new object[] { "/kick #" + Convert.ToString(num8), LoginFengKAI.player.name };
                                        FengGameManagerMKII.instance.photonView.RPC("Chat", PhotonTargets.All, objArray6);
                                    }
                                    else
                                    {
                                        flag2 = false;
                                        foreach (PhotonPlayer player3 in PhotonNetwork.playerList)
                                        {
                                            if (num8 == player3.ID)
                                            {
                                                flag2 = true;
                                                if (FengGameManagerMKII.OnPrivateServer)
                                                {
                                                    FengGameManagerMKII.instance.kickPlayerRC(player3, true, "");
                                                }
                                                else if (PhotonNetwork.isMasterClient)
                                                {
                                                    FengGameManagerMKII.instance.kickPlayerRC(player3, true, "");
                                                    objArray7 = new object[] { "<color=#FFCC00>" + RCextensions.returnStringFromObject(player3.customProperties[PhotonPlayerProperty.name]) + " has been banned from the server!</color>", string.Empty };
                                                    FengGameManagerMKII.instance.photonView.RPC("Chat", PhotonTargets.All, objArray7);
                                                }
                                            }
                                        }
                                        if (!flag2)
                                        {
                                            this.addLINE("error:no such player.");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                this.inputLine = string.Empty;
                GUI.FocusControl(string.Empty);
                return;
            }
            this.inputLine = "\t";
            GUI.FocusControl("ChatInput");
        }
    Label_013D:
        GUI.SetNextControlName(string.Empty);
        GUILayout.BeginArea(GuiRect);
        GUILayout.FlexibleSpace();
        string text = string.Empty;
        if (messages.Count < 15)
        {
            for (num4 = 0; num4 < messages.Count; num4++)
            {
                text = text + messages[num4] + "\n";
            }
        }
        else
        {
            for (int i = messages.Count - 15; i < messages.Count; i++)
            {
                text = text + messages[i] + "\n";
            }
        }
        GUILayout.Label(text, new GUILayoutOption[0]);
        GUILayout.EndArea();
        GUILayout.BeginArea(GuiRect2);
        GUILayout.BeginHorizontal(new GUILayoutOption[0]);
        GUI.SetNextControlName("ChatInput");
        this.inputLine = GUILayout.TextField(this.inputLine, new GUILayoutOption[0]);
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    public void setPosition()
    {
        if (this.AlignBottom)
        {
            GuiRect = new Rect(0f, (float) (Screen.height - 500), 300f, 470f);
            GuiRect2 = new Rect(30f, (float) ((Screen.height - 300) + 0x113), 300f, 25f);
        }
    }

    public void Start()
    {
        this.setPosition();
        instance = this;
    }
}

