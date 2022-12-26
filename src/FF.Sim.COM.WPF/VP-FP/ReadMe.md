# Flipping Flips - Add Game Guide
---

- Make sure you have the `FlippingFlipsScore.vbs` in scripts folder
- Make sure you have a API_Key file `flippingflips_key.txt`
- Registered controller

## Existing Tables

- Download the original file url and make a copy of this with the added name `-(Flips1.0)`. This isn't a naming convention but just so you know what it is.
- This will be used so you can create a patch against the original table so users can download the original and patch the game to be compatible

## Backglass Translite overlays controls

- `FlipsUserinterface-BackGlass.vpx`

- Copy textboxes to backglass or create your own using same data available for players

- FlipsBackgroundTextbox = BACKGROUND - Draw to back
- HeaderTextBox = TITLEINFO
- OptionsTextBox = OPTIONS
- AvailablePlayersTextBox = PLAYERS
- SelectedPlayersTextBox = SELECTED PLAYERS
- FlipsStatusTextBox
- ServerScoresTextBox = Show scores from server
- SelectedPlayersOverlay


## Table script

### Initiliaze Player Menu and game
---

Add under the `Option Explicit: Randomize`

```
const UseVPMNVRAM = true ' Flips PinMame game
Dim NvGameStart : NvGameStart = 7639 ' Value for game started, is called 4 times if add 4 players
Dim NvGameEnd : NvGameEnd = 7563     ' Value for game finished or gamesplayed
' 'Load flips score with game id and api key. Both flippers to activate menu
ExecuteGlobal GetTextFile ("FlippingFlipsScore.vbs")
' GameId
LoadFlipsScore "89559b4c-08e0-4f03-a74b-a6ba4c7414dd", Table1.FileName ' GameId, FileName
'Get latest scores from server. true = Include Other Machines, true = Get Top scores or Latest
GetScores
On Error Goto 0
```

### Keyboard controls
---

- Both flippers at same time activate player menu and calls to server

- Update the keyboard Up & Down handler sub routines. Add above any other keyboard controls.

```
Sub table1_KeyDown(ByVal keycode)
	MenuControlsKeyDown(keycode) '
	if FlipsMenuVisible then exit sub ' don't process any game controls in menu
End Sub
```

```
Sub table1_KeyUp(ByVal keycode)
	MenuControlsKeyUp(keycode) '
	if FlipsMenuVisible then exit sub ' don't process any game controls in menu
'End Sub
```

## Originals & EM tables

### Start Game 
---

- Find where your game is started in the script and add the setup

```SetupGame(5) ' Sets up a 5 ball game...uses the player selections from menu...tells server your game in progress```

### End Game 
---

- Find the end game to post the scores

```
'Dim postResult : postResult = False
postResult = PostScore(PlayerRealScore(0), Nothing, Nothing, Nothing)
```

Or by using array of scores...

```
postResult = PostScoreArray(Scores)
```

- Some games are multiplayer, so you can change Nothing to PlayerRealScore(1), etc

## Pinmame tables

Add variables for NVRAM top of script or just above the flips init sub routine

```
const UseVPMNVRAM = true ' Flips PinMame game
Dim NvGameStart : NvGameStart = 7639 ' Value for game started, is called 4 times if add 4 players
Dim NvGameEnd : NvGameEnd     = 7563     ' Value for game finished or gamesplayed
```

Create and init a callback method

```
Set NVRAMCallback = GetRef("OnNvCallBackChanged")
' listens for changes in nvram
Sub OnNvCallBackChanged(VPMNVRAM)
	For I = 0 To UBound(VPMNVRAM)
		if VPMNVRAM(I,0) = NvGameEnd Then ' Game Over
			PostScoreNv(Controller.NVRAM) 'Post score to server
		end if
	Next
End Sub
```