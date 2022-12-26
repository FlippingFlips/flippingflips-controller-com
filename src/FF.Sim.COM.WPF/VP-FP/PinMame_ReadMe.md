# Guide for setting up a PinMame rom game
---

Flips basically works by finding a `GameStart` and `GameEnd`, so these will be different per each game. Some Williams maybe the same or similar. I made this guide while setting up BSD-Dracula.

## File Download
---

1. Download game, save link to where it came from so users can get the exact release

2. Get author names as comma separated list

## Visual Pinball
---

1. Put table in your tables folder. Get original vpx filename.

2. Get CRC32 of original VPX file, can use 7Zip windows context menu (right click) on the VPX file

3. Make a copy of it and add to the filename -(Flips1.0).vpx. This is the file you'll work on.

4. Find `romname` in VP script. Usually under `cGameName`

## Finding an NVRAM Game Start / End
---

1. Add to top of script. This where you'll save the address for game events

```
const UseVPMNVRAM = true ' Flips PinMame game
```

2. Add sub to find the address for game start and ending. I check for a value above 7000, has been good so far.

```
Set NVRAMCallback = GetRef("OnNvCallBackChanged")
Sub OnNvCallBackChanged(VPMNVRAM)
	For I = 0 To UBound(VPMNVRAM)
		if VPMNVRAM(I,0) > 7000 Then ' Game Over
			Debug.Print VPMNVRAM(I,0)
		end if
	Next
End Sub
```

3. Run the game and open the debug window and watch the values after the player is added. You may get a few. The following is from BSD (Dracula)

Game Started: `8072, 8073, 8074, 8075, 8076`

4. Watch for the Game Ended. I played this twice in single player

`7516,7519,7525,7533,7733`

`7516,7519,7525,7532,7733`

5. Use the values you've found to test a game with VP debug window and you should see Game Start and Game End in correct places

```
const UseVPMNVRAM = true ' Flips PinMame game
Dim NvGameStart : NvGameStart = 8072 ' Game started address
Dim NvGameEnd   : NvGameEnd   = 7516 ' Game ended address
```

```
Set NVRAMCallback = GetRef("OnNvCallBackChanged")
Dim TableGameStarted
Sub OnNvCallBackChanged(VPMNVRAM)
	Dim I
	For I = 0 To UBound(VPMNVRAM)
		if VPMNVRAM(I,0) = NvGameEnd Then ' Game Over
			if TableGameStarted Then
				TableGameStarted = false
				Debug.print "NV:Game Ended"					
			End If			
		elseif VPMNVRAM(I,0) = NvGameStart Then
			if not TableGameStarted Then  'Start game in progress if not already started
				TableGameStarted = true
				Debug.print "NV:Game Setup"
			end if
		end if
	Next
End Sub
```


## Existing maps for scores

1. Check a mapping exists or you'll have to create one.

Rom mapping files: https://github.com/tomlogic/pinmame-nvram-maps

## Provide patched Diff file
---

This is so the end user can patch the table to be ready for scoring.

- Make patch notes. You may add something else other than the scripting updates.