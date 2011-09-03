using System;
using System.Drawing;
using System.Windows.Forms;
using GTA;

/*--Useful Statistics Numbers:
 * 257: Number of people killed
 * 262: People run down
 * 287: Shots fired
 * 288: Shots hit
 * 289: Kills by headshots
 * 290: Melee kills
 * 293: "Number of explosions"
 * 379: Kills with grenade
 * 392: Kills with RPG
 */
 

namespace GodmodePlus {
   //
    public class GodmodePlus : Script
    {
        const bool DEBUG = false;
        const int maxHealth = 199; // add 200 for the real health.

        text[] menuTexts = new text[16];
        Color textColour = Color.FromArgb(255, 75, 75, 200);

        GTA.Font screenFont;
        Keys activateKey = Keys.F1,
             secondaryKey = Keys.LControlKey,

             activateKeyTemp,
             secondaryKeyTemp;

        bool modEnabled = false,
             wsImmunity = false,
             bikeImmunity = false,
             fireImmunity = false,
             doImmunity = false,

             modEnabledTemp,
             wsImmunityTemp,
             bikeImmunityTemp,
             fireImmunityTemp,
             doImmunityTemp,
             tempBool,

             //activateKeyPressed = false,
             secondaryKeyPressed = false,
             menuEnabled = false;

        int currentMenu,
            menuMode = 1,
            loadTest = 0;

        /* menu modes:
         * 1: scrolling
         * 2: select activate key
         * 3: select secondary key
         * 4: quit
         * 5: save and quit
        */

        public GodmodePlus()
        {
            GeneralInfo = "Godmode Plus by thaCURSEDpie";
            // Bind method BasicKeyExample_KeyDown to the KeyDown event
            screenFont = new GTA.Font(0.03f, FontScaling.ScreenUnits);
            setUpTexts();
            loadSettings();
            BindConsoleCommand("god", new ConsoleCommandDelegate(modOnConsole), "- Enable Godmode.");
            BindConsoleCommand("godmenu", new ConsoleCommandDelegate(openMenuConsole), "- Open the Godmode Plus menu.");

            this.PerFrameDrawing += new GraphicsEventHandler(this.DrawingExample_PerFrameDrawing);
            this.Tick += new EventHandler(this.TickEvent);
            Interval = 80; // Interval is the time between two Ticks (in milliseconds)
            this.KeyDown += new GTA.KeyEventHandler(this.KeyDownEvent);
            this.KeyUp += new GTA.KeyEventHandler(this.KeyUpEvent);
        }

        private void TickEvent(object sender, EventArgs e) // each and every tick (interval is specified above)
        {
            tempBool = Player.Character.isOnScreen;
        }

        private int loadSettings()
        {
            Settings.Load();

            loadTest = Settings.GetValueInteger("LoadTest", "donottouch", 0);
            if (loadTest == 0)
            {
                Game.Console.Print("[Godmode Plus]: Settings file could not be read");
                return 1;
            }
            wsImmunity = Settings.GetValueBool("WindshieldImmunity", "variables", false);
            doImmunity = Settings.GetValueBool("DragOutImmunity", "variables", false);
            fireImmunity = Settings.GetValueBool("FireImmunity", "variables", false);
            bikeImmunity = Settings.GetValueBool("BikeFallImmunity", "variables", false);

            activateKey = Settings.GetValueKey("ActivateKey", "variables", Keys.F1);
            secondaryKey = Settings.GetValueKey("SecondaryKey", "variables", Keys.LControlKey);
            return 0;
        }

        private void saveSettings()
        {
            Settings.Save();
            Settings.SetValue("WindshieldImmunity", "variables", wsImmunity);
            Settings.SetValue("DragOutImmunity", "variables", doImmunity);
            Settings.SetValue("FireImmunity", "variables", fireImmunity);
            Settings.SetValue("BikeFallImmunity", "variables", bikeImmunity);
            Settings.SetValue("ActivateKey", "variables", activateKey);
            Settings.SetValue("SecondaryKey", "variables", secondaryKey);
            Settings.SetValue("LoadTest", "donottouch", 1);
        }


        private void updateMenuAttributes()
        {
            menuTexts[1].Attribute = modEnabledTemp.ToString();
            menuTexts[2].Attribute = wsImmunityTemp.ToString();
            menuTexts[3].Attribute = doImmunityTemp.ToString();
            menuTexts[4].Attribute = fireImmunityTemp.ToString();
            menuTexts[5].Attribute = bikeImmunityTemp.ToString();

            if (menuTexts[6].Selected) // the cursor is on the activate key
            {
                menuTexts[6].Attribute = "Press enter to choose a new key";
                menuTexts[7].Attribute = secondaryKeyTemp.ToString();
            }
            else if (menuTexts[7].Selected)
            {
                menuTexts[6].Attribute = activateKeyTemp.ToString();
                menuTexts[7].Attribute = "Press enter to choose a new key";
            }
            else if (menuMode == 1)
            {
                menuTexts[6].Attribute = activateKeyTemp.ToString();
                menuTexts[7].Attribute = secondaryKeyTemp.ToString();
            }
            
            if (menuMode == 2)
            {
                menuTexts[6].Attribute = "<Press any key>";
                menuTexts[7].Attribute = secondaryKeyTemp.ToString();
            }
            else if (menuMode == 3)
            {
                menuTexts[6].Attribute = activateKeyTemp.ToString();
                menuTexts[7].Attribute = "<Press any key>";
            }
        }

        private void loadTempVars()
        {
            modEnabledTemp = modEnabled;
            wsImmunityTemp = wsImmunity;
            doImmunityTemp = doImmunity;
            bikeImmunityTemp = bikeImmunity;
            fireImmunityTemp = fireImmunity;
            activateKeyTemp = activateKey;
            secondaryKeyTemp = secondaryKey;
        }

        private void saveTempVars()
        {
            modEnabled = modEnabledTemp;
            wsImmunity = wsImmunityTemp;
            doImmunity = doImmunityTemp;
            bikeImmunity = bikeImmunityTemp;
            fireImmunity = fireImmunityTemp;
            activateKey = activateKeyTemp;
            secondaryKey = secondaryKeyTemp;
        }

        private void setUpTexts()
        {
            // menu            
            menuTexts[0].Content = "Menu";
            menuTexts[0].textColour = Color.Gray;
            menuTexts[0].Xcoord = 0.45f;
            menuTexts[0].Ycoord = 0.095f;
            menuTexts[0].SelectAble = false;

            menuTexts[12].Content = "Godmode on/off";
            menuTexts[12].textColour = Color.Gray;
            menuTexts[12].Xcoord = 0.2f;
            menuTexts[12].Ycoord = 0.17f;
            menuTexts[12].SelectAble = false;

            menuTexts[1].Content = "Godmode:";
            menuTexts[1].textColour = Color.White;
            menuTexts[1].Xcoord = 0.2f;
            menuTexts[1].Ycoord = 0.20f;
            menuTexts[1].Attribute = modEnabled.ToString();
            menuTexts[1].SelectAble = true;

            menuTexts[10].Content = "Godmode Modifiers";
            menuTexts[10].textColour = Color.Gray;
            menuTexts[10].Xcoord = 0.2f;
            menuTexts[10].Ycoord = 0.27f;
            menuTexts[10].SelectAble = false;

            menuTexts[2].Content = "Windshield immunity:";
            menuTexts[2].textColour = Color.White;
            menuTexts[2].Xcoord = 0.2f;
            menuTexts[2].Ycoord = 0.3f;
            menuTexts[2].Attribute = wsImmunity.ToString();
            menuTexts[2].SelectAble = true;

            menuTexts[3].Content = "Drag-out immunity:";
            menuTexts[3].textColour = Color.White;
            menuTexts[3].Xcoord = 0.2f;
            menuTexts[3].Ycoord = 0.33f;
            menuTexts[3].Attribute = doImmunity.ToString();
            menuTexts[3].SelectAble = true;

            menuTexts[4].Content = "Fire immunity:";
            menuTexts[4].textColour = Color.White;
            menuTexts[4].Xcoord = 0.2f;
            menuTexts[4].Ycoord = 0.36f;
            menuTexts[4].Attribute = fireImmunity.ToString();
            menuTexts[4].SelectAble = true;

            menuTexts[5].Content = "Bike-fall immunity:";
            menuTexts[5].textColour = Color.White;
            menuTexts[5].Xcoord = 0.2f;
            menuTexts[5].Ycoord = 0.39f;
            menuTexts[5].Attribute = bikeImmunity.ToString();
            menuTexts[5].SelectAble = true;

            menuTexts[11].Content = "Keys";
            menuTexts[11].textColour = Color.Gray;
            menuTexts[11].Xcoord = 0.2f;
            menuTexts[11].Ycoord = 0.46f;
            menuTexts[11].SelectAble = false;

            menuTexts[6].Content = "Activation key:";
            menuTexts[6].textColour = Color.White;
            menuTexts[6].Xcoord = 0.2f;
            menuTexts[6].Ycoord = 0.49f;
            menuTexts[6].Attribute = activateKey.ToString();
            menuTexts[6].SelectAble = true;

            menuTexts[7].Content = "Secondary key:";
            menuTexts[7].textColour = Color.White;
            menuTexts[7].Xcoord = 0.2f;
            menuTexts[7].Ycoord = 0.52f;
            menuTexts[7].Attribute = secondaryKey.ToString();
            menuTexts[7].SelectAble = true;

            menuTexts[8].Content = "Save and quit";
            menuTexts[8].textColour = Color.White;
            menuTexts[8].Xcoord = 0.2f;
            menuTexts[8].Ycoord = 0.59f;
            menuTexts[8].SelectAble = true;

            menuTexts[9].Content = "Quit without saving";
            menuTexts[9].textColour = Color.White;
            menuTexts[9].Xcoord = 0.2f;
            menuTexts[9].Ycoord = 0.62f;
            menuTexts[9].SelectAble = true;                  
        }

        private void menuDown()
        {
            menuTexts[currentMenu].Selected = false;
            bool validEntry = false;

            while (!validEntry)
            {
                if ((currentMenu + 1) < menuTexts.Length)
                {
                    if (menuTexts[currentMenu + 1].SelectAble)
                    {
                        currentMenu += 1;
                        menuTexts[currentMenu].Selected = true;
                        validEntry = true;
                    }
                    else
                    {
                        currentMenu += 1;
                    }
                }
                else
                {
                    currentMenu = 0;
                }
            }
        }

        private void menuUp()
        {
            menuTexts[currentMenu].Selected = false;
            bool validEntry = false;

            while (!validEntry)
            {
                if ((currentMenu - 1) > 0)
                {
                    if (menuTexts[currentMenu - 1].SelectAble)
                    {
                        currentMenu -= 1;
                        menuTexts[currentMenu].Selected = true;                       
                        validEntry = true;
                    }
                    else
                    {
                        currentMenu -= 1;
                    }
                }
                else
                {
                    currentMenu = (menuTexts.Length - 1);
                }
            }            
        }

        private void KeyUpEvent(object sender, GTA.KeyEventArgs e) // gets executed when a key is released
        {
            if (e.Key == activateKey)
            {
                //activateKeyPressed = false;
            }

            if (e.Key == secondaryKey)
            {
                secondaryKeyPressed = false;
            }
        }
        private void modOnConsole(ParameterCollection Parameter)
        {
            switchMod();
        }

        private void openMenuConsole(ParameterCollection Parameter)
        {
            switchMenu();
        }
        private void KeyDownEvent(object sender, GTA.KeyEventArgs e) // gets executed when a key is pressed
        {
            if (menuEnabled) // keys handling in the menu
            {
                if (menuMode == 2) // select the activate key!
                {
                    if ((e.Key == Keys.Escape) || (e.Key == Keys.Enter))
                    {
                        menuMode = 1;
                        return;
                    }
                    else if ((e.Key == Keys.Left) || (e.Key == Keys.Right) || (e.Key == Keys.Up) || (e.Key == Keys.Down) || (e.Key == activateKeyTemp))
                    {
                    }
                    else
                    {
                        activateKeyTemp = e.Key;
                        menuMode = 1;
                        menuTexts[currentMenu].Selected = false;
                        menuTexts[1].Selected = true;
                        currentMenu = 1;
                        return;
                    }
                }
                else if (menuMode == 3)
                {
                    if ((e.Key == Keys.Escape) || (e.Key == Keys.Enter))
                    {
                        menuMode = 1;
                        return;
                    }
                    else if ((e.Key == Keys.Left) || (e.Key == Keys.Right) || (e.Key == Keys.Up) || (e.Key == Keys.Down) || (e.Key == activateKeyTemp))
                    {
                    }
                    else
                    {
                        secondaryKeyTemp = e.Key;
                        menuMode = 1;
                        menuTexts[currentMenu].Selected = false;
                        menuTexts[1].Selected = true;
                        currentMenu = 1;
                        return;
                    }
                } 

                if ((e.Key == Keys.Right) || (e.Key == Keys.Left)) // adjusting variables....
                {
                    if (menuTexts[1].Selected)
                    {
                        modEnabledTemp = !modEnabledTemp;
                    }
                    else if (menuTexts[2].Selected)
                    {
                        wsImmunityTemp = !wsImmunityTemp;
                    }
                    else if (menuTexts[3].Selected)
                    {
                        doImmunityTemp = !doImmunityTemp;
                    }
                    else if (menuTexts[4].Selected)
                    {
                        fireImmunityTemp = !fireImmunityTemp;
                    }
                    else if (menuTexts[5].Selected)
                    {
                        bikeImmunityTemp = !bikeImmunityTemp;
                    }                   
                }
                else if (e.Key == Keys.Enter) // enter key is pressed
                {
                    if (menuTexts[1].Selected)
                    {
                        modEnabledTemp = !modEnabledTemp;
                    }
                    else if (menuTexts[2].Selected)
                    {
                        wsImmunityTemp = !wsImmunityTemp;
                    }
                    else if (menuTexts[3].Selected)
                    {
                        doImmunityTemp = !doImmunityTemp;
                    }
                    else if (menuTexts[4].Selected)
                    {
                        fireImmunityTemp = !fireImmunityTemp;
                    }
                    else if (menuTexts[5].Selected)
                    {
                        bikeImmunityTemp = !bikeImmunityTemp;
                    }
                    else if (menuTexts[6].Selected) // cursor was on the "select activate" button
                    {
                        menuMode = 2;
                    }
                    else if (menuTexts[7].Selected) // cursor was on secondary button
                    {
                        menuMode = 3;
                    }
                    else if (menuTexts[8].Selected) // save and quit
                    {
                        menuMode = 1;
                        saveTempVars();
                        saveSettings();
                        switchMenu();

                        if (!modEnabled)
                        {
                            Player.MaxHealth = -100;
                            Player.Character.Invincible = false;
                            Player.Character.MakeProofTo(false, false, false, false, false);
                            Player.Character.CanBeDraggedOutOfVehicle = true;
                            Player.Character.CanBeKnockedOffBike = false;
                            Player.Character.WillFlyThroughWindscreen = true;
                        }
                    }
                    else if (menuTexts[9].Selected) // quit without saving
                    {
                        menuMode = 1;
                        switchMenu();
                    }
                }
                else if ((e.Key == Keys.Down) && (menuMode == 1)) // go one down in the menu
                {
                    menuDown();
                }
                else if ((e.Key == Keys.Up) && (menuMode == 1)) // go one up in the menu
                {
                    menuUp();
                }                               
            }

            if (e.Key == activateKey)
            {
                if (!menuEnabled)
                {
                    //activateKeyPressed = true;
                    if (!secondaryKeyPressed)
                    {
                        switchMod();
                    }
                    else // switch the menu, the two keys are both pressed.
                    {
                        switchMenu();
                    }
                }
            }

            if (e.Key == secondaryKey)
            {
                secondaryKeyPressed = true;
            }
        }

        private void switchMod()
        {
            int gameMode = GTA.Native.Function.Call<int>("NETWORK_GET_GAME_MODE");
            if ((gameMode != -1) && (gameMode != 8) && (gameMode != 16))
            {
                Game.DisplayText("[Godmode Plus]: game mode check failed! Are you *not* in singleplayer/partymode/freemode?", 10);
                Game.Console.Print("[GMP] blocked in multiplayer");
                GTA.Native.Function.Call("NETWORK_SEND_TEXT_CHAT", "test");
            }
            else
            {
                modEnabled = !modEnabled;
                if (modEnabled)
                {
                    Player.MaxHealth = maxHealth;
                    Game.SendChatMessage("[GMP]: I am using Godmode!");
                    Game.DisplayText("Godmode ON");
                    Game.Console.Print("[GMP] enabled in free or party mode");
                    GTA.Native.Function.Call("NETWORK_SEND_TEXT_CHAT", "test");
                }
                else
                {
                    Player.MaxHealth = -100;
                    Player.Character.Invincible = false;
                    Player.Character.MakeProofTo(false, false, false, false, false);
                    Player.Character.CanBeDraggedOutOfVehicle = true;
                    Player.Character.CanBeKnockedOffBike = false;
                    Player.Character.WillFlyThroughWindscreen = true;
                    Game.DisplayText("Godmode OFF");
                }
            }
        }

        private void switchMenu()
        {

            menuEnabled = !menuEnabled;
            if (menuEnabled)
            {
                Player.CanControlCharacter = false;
                Game.TimeScale = 0.0f;
                modEnabled = true;
                loadTempVars();
                menuTexts[1].Selected = true;
                currentMenu = 1;
            }
            else
            {
                Game.TimeScale = 1f;
                Player.CanControlCharacter = true;
                for (int i = 0; i < menuTexts.Length; i++)
                {
                    menuTexts[i].Selected = false;
                }
            }
        }
        
        private void DrawingExample_PerFrameDrawing(object sender, GraphicsEventArgs e) // Executes every GTA-frame
        {
            e.Graphics.Scaling = FontScaling.ScreenUnits;
            if (menuEnabled)
            {
                e.Graphics.DrawRectangle(new RectangleF(0.16f, 0.07f, 0.58f, 0.6f), Color.FromArgb(40, 255, 255, 255));                
                updateMenuAttributes();

                for (int i = 0; i < menuTexts.Length; i++)
                {
                    
                    if (menuTexts[i].Selected)
                    {
                        e.Graphics.DrawText(">", menuTexts[i].Xcoord - 0.013f, menuTexts[i].Ycoord, textColour, screenFont);
                        e.Graphics.DrawText(menuTexts[i].Content, menuTexts[i].Xcoord, menuTexts[i].Ycoord, textColour, screenFont);
                        e.Graphics.DrawText(menuTexts[i].Attribute, menuTexts[i].Xcoord + 0.25f, menuTexts[i].Ycoord, textColour, screenFont);
                    }
                    else
                    {
                        e.Graphics.DrawText(menuTexts[i].Content, menuTexts[i].Xcoord, menuTexts[i].Ycoord, menuTexts[i].textColour, screenFont);
                        e.Graphics.DrawText(menuTexts[i].Attribute, menuTexts[i].Xcoord + 0.25f, menuTexts[i].Ycoord, menuTexts[i].textColour, screenFont);
                    }
                }
            }
            
            if (modEnabled)
            {
                if (DEBUG)
                {
                    e.Graphics.DrawText("Player health: " + Player.Character.Health.ToString(), 0.5f, 0.1f);
                    Player[] plist = Game.PlayerList;
                    
                    e.Graphics.DrawText("Players: " + plist.Length, 0.5f, 0.2f);
                    for (int i = 0; i < plist.Length; i++)
                    {
                        e.Graphics.DrawText(plist[i].Name.ToString(), 0.5f, 0.3f + 0.1f * i);
                    }
                }

                int gameMode = GTA.Native.Function.Call<int>("NETWORK_GET_GAME_MODE");
                if ((gameMode == -1) || (gameMode == 8) || (gameMode == 16) || DEBUG)
                {
                    //e.Graphics.DrawText("Max health: " + Player.Character.Health.ToString(), 0.5f, 0.5f);
                    //e.Graphics.DrawText("Minigame?: " + GTA.Game.isMinigameInProgress.ToString(), 0.5f, 0.6f);

                    //e.Graphics.DrawText("On screen?: " + tempBool.ToString(), 0.5f, 0.7f);
                    if (Game.isMinigameInProgress)
                    {
                        Player.Character.Invincible = true;
                    }
                    else if (Player.Character.isInVehicle()) // is the player in a vehicle?
                    {
                        
                        if (GTA.Native.Function.Call<bool>("IS_CHAR_ON_ANY_BIKE", Player.Character)) // is the player on a bike?
                        {
                            Player.Character.Invincible = false;
                            if (Player.Character.Health != (maxHealth + 200) * 100)
                            {
                                Player.MaxHealth = maxHealth * 100;
                            }
                            Player.Character.MakeProofTo(false, fireImmunity, false, false, false);
                            Player.Character.WillFlyThroughWindscreen = true;
                            Player.Character.CanBeKnockedOffBike = bikeImmunity;
                        }
                        else // the player is in a vehicle, but not a bike, so we can just do:
                        {
                            Player.Character.CanBeDraggedOutOfVehicle = !doImmunity;
                            Player.Character.CanBeKnockedOffBike = !bikeImmunity;
                            Player.Character.Invincible = true;
                            Player.Character.MakeProofTo(true, fireImmunity, false, true, true);
                            Player.Character.WillFlyThroughWindscreen = !wsImmunity;
                        }
                    }
                    else // the player is on foot
                    {
                        if (Player.Character.Health != maxHealth + 200)
                        {
                            Player.MaxHealth = maxHealth;
                        }
                        Player.Character.Armor = 0;
                        Player.Character.MakeProofTo(false, fireImmunity, false, true, false);
                        Player.Character.Invincible = false;
                        Player.Character.WillFlyThroughWindscreen = true;
                        Player.Character.CanBeDraggedOutOfVehicle = true;
                        Player.Character.CanBeKnockedOffBike = true;
                    }

                }
            }
        }

    }
}

//e.Graphics.DrawText("Time:" + ((float)((float)(currentTimeLimit) / (float)(TIME_LIMIT))).ToString(), 0.89f, 0.85f, Color.Blue, screenFont);
//e.Graphics.DrawText("hits:" + (getIntStat((int)gtaStats.STAT_BULLETS_HIT) - shotsHitStart).ToString(), 0.25f, 0.85f, Color.Blue, screenFont);
//e.Graphics.DrawText("%:" + (Player.Character.Health / MAX_HEALTH).ToString(), 0.35f, 0.85f, Color.White, screenFont);
//e.Graphics.DrawText("rundown:" + (getIntStat((int)gtaStats.STAT_PEOPLE_RUN_DOWN) - rundownStart).ToString(), 0.45f, 0.85f, Color.Black, screenFont);
//e.Graphics.DrawText("hp:" + Player.Character.Health.ToString(), 0.25f, 0.95f, Color.Red, screenFont);
//e.Graphics.DrawText(((int)currentTimeLimit).ToString(), 0.4f, 0.95f, Color.Yellow, screenFont);