'---------------------------------------------------------------------------------
' ---------- Flipping Flips Score UI -----------
' UI Script for machine overlay
' 
' 1. Activate menu with both flippers
' 2. Add players to server with initials+Name
'    DUB+Your name
' 3. Select players for game, up to 4 players
' 
' - If no selections game posting is disabled
' Notes: //msgbox "" & FormatDateTime(now, 0) ' get this system date from vbs
'---------------------------------------------------------------------------------
Option Explicit
Dim FlipsControl
Dim FlipsEnabled, FlipMenu, FlipSelectedMenu, FlipSelectedMenuId, FlipPlayerMenuOn, FlipsJustExitedMenu
Dim FlipPlayers, FlipSelectedPlayers, FlipSelectedPlayerIds, FlipSelectedPlayerNames, FlipScores
Dim FlipsGameStarted, FlipsMenuVisible, FlipsInitialized, TableGameStarted, ScorePosted
Dim LFlipDown, RFlipDown : LFlipDown = 0 :  RFlipDown = 0 'Hold flipper states to open menu
Dim vpVer : vpVer = VersionMajor & "." & VersionMinor & "." & VersionRevision
FlipsMenuVisible = 0
FlipSelectedPlayers = Array() :FlipSelectedPlayerIds = array(0,0,0,0) : FlipSelectedPlayerNames = array("","","","")
FlipScores = Array()

Sub LoadFlipsScore(id, filename)
	Dim keyFile, obj : Set obj = CreateObject("Scripting.FileSystemObject")
	Set FlipsControl=CreateObject("FF.Sim.COM.Controller")
	If Err Then MsgBox "Flipping Flips Controller is not registered"
	Dim path: path = obj.GetAbsolutePathName(".")
	FlipsControl.Run id, path & "\" & filename & ".vpx" ' Sets up controller service
	InitFlipsDisplay ' Reset values in the flips UI
	On Error Goto 0
End Sub

Sub InitMainMenu
	FlipMenu =	Array("Create Player", "Select Players", "Back to table")
	FlipSelectedMenuId = 0
End Sub

' Handle controls to show menu items
Sub MenuControlsKeyDown(keycode)

	' don't show if game is running
	if TableGameStarted Then : Exit Sub

	'Enable menu with flippers only if not already visible
	if FlipsMenuVisible = 0 Then
		'Set flipper keys down to enable the menu
		If Keycode = LeftFlipperKey Then 
			LFlipDown = 1		
		elseIf Keycode = RightFlipperKey Then
			RFlipDown = 1		
		End If			
		' both not pressed? return
		if LFlipDown = 0 then Exit Sub 
		if RFlipDown = 0 then Exit Sub		
		LFlipDown = 0 : RFlipDown = 0
		
		'Get players on the first open of the menu
		if FlipsInitialized = 0 then GetPlayers : FlipsInitialized = 1 : end if
		if len(FlipsControl.GetLastError) > 0 then 
			MsgBox FlipsControl.GetLastError
		else			    
			FlipsMenuVisible = 1
			ToggleFlipsMenu							
		end If
	End If
	
	' TOGGLE MENU	
	
	if FlipsMenuVisible = 0 Then : Exit Sub

	'MENU CONTROLS
    If keycode = LeftFlipperKey Then
		FlipSelectedMenuId = FlipSelectedMenuId-1

		if FlipPlayerMenuOn = 0 Then
			if FlipSelectedMenuId < LBOUND(FlipMenu) Then
				FlipSelectedMenuId = UBOUND(FlipMenu)
			End If
		else
			if FlipSelectedMenuId < LBOUND(FlipPlayers) Then
				FlipSelectedMenuId = UBOUND(FlipPlayers)+2
			End If
		end if
		
		UpdateMenuText
	End If

	If keycode = RightFlipperKey Then

		FlipSelectedMenuId = FlipSelectedMenuId+1

		if FlipPlayerMenuOn = 0 Then
			if FlipSelectedMenuId > UBOUND(FlipMenu) Then
				FlipSelectedMenuId = 0
			End If
		else
			if FlipSelectedMenuId > UBOUND(FlipPlayers)+2 Then
				FlipSelectedMenuId = 0
			End If
		End if		

		UpdateMenuText
	End If

	If keycode = StartGameKey Then

		if FlipPlayerMenuOn = 1 Then
			If FlipSelectedMenuId > UBOUND(FlipPlayers) then
				If FlipSelectedMenuId - UBOUND(FlipPlayers) = 2 then
					FlipPlayerMenuOn = 0
					InitMainMenu  : UpdateMenuText : exit sub
				else
					ClearSelections
				end if
			else 
				Dim id, initials
				id = FlipPlayers(FlipSelectedMenuId, 0) : initials  = FlipPlayers(FlipSelectedMenuId, 1)
				AddSelectedPlayer id, initials
				UpdateSelectedPlayersText
			end if
		End if
			
		Select case FlipSelectedMenu
			case "Create Player"
				ShowCreatePlayer
			case "Select Players"				
				FlipPlayerMenuOn = 1 : FlipSelectedMenuId = 0					
				UpdateMenuText
			case "Back to table"
				FlipsMenuVisible=0 : ToggleFlipsMenu
				FlipsJustExitedMenu = 1
		End Select
	End If
End Sub

' Handle controls to show menu items
Sub MenuControlsKeyUp(keycode)
	' don't show if game is running
	if TableGameStarted Then : Exit Sub
	if FlipsMenuVisible Then : Exit Sub
	
	'Show / Hide the menu for flips
	If Keycode = LeftFlipperKey Then		
		LFlipDown = 0			
	elseIf Keycode = RightFlipperKey Then		
		RFlipDown = 0			
	End If
End Sub

private Sub InitFlipsDisplay			
	Dim InitText
	On Error Resume Next	
		ToggleFlipsMenu ' hide all menu textboxes
		FlipsBackgroundTextbox.Text = ""
		InitText = "FLIPPING FLIPS - VPX:" & vpVer & vbCrLf & "L & R flippers, Start to select" ' VP Version
		HeaderTextBox.Text = InitText			
		UpdateMenuText			
		InitMainMenu
	On Error Goto 0
End Sub

'Server calls
Sub GetPlayers		
	' Get players from server. Loads into FlipPlayers	
	On Error Resume Next	
		dim players
		debug.print "Getting players from server"
		players = FlipsControl.AvailablePlayers	'server call		
		if Err <> 0 Then debug.print "An error occured:" &	FlipsControl.GetLastError
		FlipPlayers = players		
		UpdatePlayersText
	On Error Goto 0
End Sub

'Displays an input box to enter player initials and name
private Sub ShowCreatePlayer	
	Dim strUserIn, spli, playerStr
    strUserIn = InputBox("Join Initials and name with + . Initials can be 4 chars long", "Player Initials and Name", "")
	
	If Len(strUserIn) > 0 Then
		spli = Split(strUserIn,"+")	
		If UBOUND(spli) = 0 Then
			msgbox "Error creating player from: " & strUserIn				
			ShowCreatePlayer
		Else
			Dim intButton
			playerStr =  "Initials:" &  spli(0) & ", Name:" & spli(1)			
			intButton = MsgBox("Add " & playerStr, 3, "Add player to server?")
			If intButton = 6 Then 'Add to server FlipsControl				
				Dim addPlayerResult
				addPlayerResult = FlipsControl.CreatePlayer(spli(0), spli(1)) ' server call to create player by initials and id
				If addPlayerResult Then GetPlayers  : Else msgbox FlipsControl.GetLastError : End If
			End If				
		End If	
	End If
End Sub

Sub GetScores	
	' Get scores from server. Loads into FlipScores	
	On Error Resume Next	
		debug.print "Getting scores from server"
		FlipScores = FlipsControl.GetScores()	'server call		
		if Err <> 0 Then msgbox FlipsControl.GetLastError
		UpdateServerScoresText
	On Error Goto 0
End Sub

Sub Quit
	Set FlipsControl = Null
End Sub

'***********************
'* Menus / Textboxes
'***********************
Sub AddSelectedPlayer(id, initials)
	Dim i ' AVOID MULTIPLE SAME PLAYER IDs
	
	for i = 0 to 3
		if FlipSelectedPlayerIds(i) = id then Exit sub
	next
	
	for i = 0 to 3
		if FlipSelectedPlayerIds(i)  < 1 then 
			FlipSelectedPlayerIds(i) = id : FlipSelectedPlayerNames(i) = initials
			exit sub
		end if
	next

End Sub

Sub ClearSelections
    FlipSelectedPlayerIds = array(0,0,0,0) : FlipSelectedPlayerNames = array("","","","")
	SelectedPlayersTextBox.Text = ""
	SelectedPlayersOverlay.Text = "" : SelectedPlayersOverlay.Visible = false
	UpdateFlipsStatus("") : FlipsStatusTextBox.Visible = false
End Sub

Sub ToggleFlipsMenu
	If FlipsMenuVisible Then SelectedPlayersOverlay.Visible = 0 Else SelectedPlayersOverlay.Visible = 1
	FlipsBackgroundTextbox.Visible = FlipsMenuVisible
	HeaderTextBox.Visible = FlipsMenuVisible
	OptionsTextBox.Visible = FlipsMenuVisible
	SelectedPlayersTextBox.Visible = FlipsMenuVisible
	AvailablePlayersTextBox.Visible = FlipsMenuVisible
End Sub

'Updates the current menu and player menus
Sub UpdateMenuText		
	if FlipPlayerMenuOn = 0 Then 
		FlipSelectedMenu = FlipMenu(FlipSelectedMenuId)
	else
		dim i : i = UBOUND(FlipPlayers)
		if FlipSelectedMenuId > i then 
			if FlipSelectedMenuId - i = 1 then 
				FlipSelectedMenu = "--CLEAR--"
			else
				FlipSelectedMenu = "--BACK--"	
			end if
		else
			FlipSelectedMenu = FlipPlayers(FlipSelectedMenuId, 1)
		end if
	end if
	OptionsTextBox.Text = FlipSelectedMenu
End Sub

Sub UpdatePlayersText
	Dim txt, i
	txt = "Available Players:" & vbcrLf & GetAvailablePlayersString
	AvailablePlayersTextBox.Text = txt	
End Sub

Sub UpdateServerScoresText
	Dim txt
	txt = "Scores:" & vbcrLf & GetScoresString
	ServerScoresTextBox.Text = txt	
End Sub

Sub UpdateSelectedPlayersText
	'updates SelectedPlayersTextBox
	Dim txt, i
	txt = "Selected Players:" & vbcrLf & GetSelectedPlayersString
	SelectedPlayersTextBox.Text = txt
	SelectedPlayersOverlay.Text = txt
End Sub

Sub UpdateFlipsStatus(status)
	FlipsStatusTextBox.Text = status
End Sub

'**************************
' FUNCTIONS
'**************************
Function GetAvailablePlayersString
	'returns a list of available players on the machine
	Dim i, txt
	For i = LBOUND(FlipPlayers) to UBOUND(FlipPlayers)
		txt = txt & FlipPlayers(i,0) & " - " & FlipPlayers(i, 1) & " - " & FlipPlayers(i,2) & vbcrLf
	next	
	GetAvailablePlayersString = txt
End Function

Function GetSelectedPlayersString
	'returns a list of selected players from a menu
	Dim i, txt
	For i = LBOUND(FlipSelectedPlayerIds) to UBOUND(FlipSelectedPlayerIds)
		txt = txt & FlipSelectedPlayerIds(i)  & " - " & FlipSelectedPlayerNames(i) & vbcrLf ' & " - " & FlipSelectedPlayers(i,2) & vbcrLf
	next	
	GetSelectedPlayersString = txt
End Function

Function GetScoresString
	'returns a score list string.
	'Array[0,6] = Points, Initials, UserName, Balls, Desktop, SimVersion
	Debug.Print "Getting scores. Scores available: " & UBOUND(FlipScores)
	Dim i, txt
	For i = LBOUND(FlipScores) to UBOUND(FlipScores)
		txt = txt & FlipScores(i,0) & " - " & FlipScores(i, 1) & " - " & FlipScores(i,2) & vbcrLf
	next	
	GetScoresString = txt
End Function

Function SetupGame(ByVal ballsPerGame)
	Dim result : result = false
	if FlipSelectedPlayerIds(0) > 0 Then
		'On Error Resume Next		
		debug.print "Flips setting GIP:" & ballsPerGame & " " & FlipSelectedPlayerIds(0) & " " & FlipSelectedPlayerIds(1) & " " & FlipSelectedPlayerIds(2) & " " & FlipSelectedPlayerIds(3)
		result = FlipsControl.StartGame (ShowDT, ballsPerGame, FlipSelectedPlayerIds(0), FlipSelectedPlayerIds(1),FlipSelectedPlayerIds(2),FlipSelectedPlayerIds(3), vpVer)			
	End if		

	debug.print "Flips game in progress success?:" & result
	if not result then debug.print FlipsControl.GetLastError : else UpdateFlipsStatus("FFlips In Play") : end if
	TableGameStarted = 1 : FlipsGameStarted = result
	SetupGame = result
End function

Function PostScore(p1Score, p2Score, p3Score, p4Score)	
	TableGameStarted = 0
	'Posts scores to the server	
	debug.print "Flips sending scores:" & p1Score & " " & p2Score & " " & p3Score & " " & p4Score
	if FlipsGameStarted = false then
		ScorePosted = false
	else		
		UpdateFlipsStatus("Sending scores..")
		ScorePosted = FlipsControl.EndGame(p1Score, p2Score, p3Score, p4Score)
		if ScorePosted then UpdateFlipsStatus("Scores sent") else UpdateFlipsStatus("Scores fail") End if
		FlipsGameStarted = false 'set game not started to enable menu again
	End if	
	if ScorePosted then debug.print "Flips scores sent" else debug.print "Error: " + FlipsControl.GetLastError end if	
End function

Function PostScoreArray(scoreArray)	
	TableGameStarted = 0
	'Posts a 4 player score array to to the server
	if FlipsGameStarted = false then
		ScorePosted = false
	else		
		UpdateFlipsStatus("Sending scores..")
		ScorePosted = FlipsControl.EndGame(scoreArray(0), scoreArray(1), scoreArray(2), scoreArray(3))
		if ScorePosted then UpdateFlipsStatus("Scores sent") else UpdateFlipsStatus("Scores fail") End if
		FlipsGameStarted = false 'set game not started to enable menu again
	End if	
	if ScorePosted then debug.print "Flips scores sent" else debug.print "Error: " + FlipsControl.GetLastError end if	
End function

Function PostScoreNv(VPNVRAM)	
	TableGameStarted = 0
	'Posts a pinmame game to the server
	if FlipsGameStarted = false then
		ScorePosted = false
	else		
		debug.Print "posting score to server"
		UpdateFlipsStatus("Sending scores..")
		FlipsControl.NvRam = VPNVRAM 'byte[]
		ScorePosted = FlipsControl.EndGameNv()
		if ScorePosted then UpdateFlipsStatus("Scores sent") else UpdateFlipsStatus("Scores fail") End if
		FlipsGameStarted = false 'set game not started to enable menu again
	End if	
	if ScorePosted then debug.print "Flips scores sent" else debug.print "Error: " + FlipsControl.GetLastError end if	
End function

Function PostScoreNvNoLastScoreSupport(VPNVRAM, scoreArray)	
	TableGameStarted = 0
	'Posts a pinmame game to the server
	if FlipsGameStarted = false then
		ScorePosted = false
	else		
		debug.Print "posting score to server"
		UpdateFlipsStatus("Sending scores..")
		FlipsControl.NvRam = VPNVRAM 'byte[]
		ScorePosted = FlipsControl.EndGameNvNoLastScoreSupport(scoreArray(0), scoreArray(1), scoreArray(2), scoreArray(3))
		if ScorePosted then UpdateFlipsStatus("Scores sent") else UpdateFlipsStatus("Scores fail") End if
		FlipsGameStarted = false 'set game not started to enable menu again
	End if	
	if ScorePosted then debug.print "Flips scores sent" else debug.print "Error: " + FlipsControl.GetLastError end if	
End function
