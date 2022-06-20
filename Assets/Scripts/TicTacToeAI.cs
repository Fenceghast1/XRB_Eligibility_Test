using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public enum TicTacToeState{none, cross, circle}

[System.Serializable]
public class WinnerEvent : UnityEvent<int>
{
}

public class TicTacToeAI : MonoBehaviour
{

	int _aiLevel;

	public TicTacToeState[,] boardState;

	[SerializeField]
	public bool _isPlayerTurn;

	[SerializeField]
	private int _gridSize = 3;

	private TicTacToeState playerState = TicTacToeState.circle;
	TicTacToeState aiState = TicTacToeState.cross;

	[SerializeField]
	private GameObject _xPrefab;

	[SerializeField]
	private GameObject _oPrefab;

	public UnityEvent onGameStarted;

	//Call This event with the player number to denote the winner
	public WinnerEvent onPlayerWin;

	ClickTrigger[,] _triggers;

	public int moveCount = 0;

	private void Awake(){
		if (onPlayerWin == null) {
			onPlayerWin = new WinnerEvent();
		}
	}

	public void StartAI(int AILevel) {
		_aiLevel = AILevel;
		StartGame();
	}

	public void RegisterTransform(int myCoordX, int myCoordY, ClickTrigger clickTrigger)
	{
		_triggers[myCoordX, myCoordY] = clickTrigger;
	}

	private void StartGame(){
		_triggers = new ClickTrigger[3, 3];
		onGameStarted.Invoke();
		boardState = new TicTacToeState[_gridSize, _gridSize];
		_isPlayerTurn = true;
	}

	public void PlayerSelects(int coordX, int coordY) {
		if (boardState[coordX, coordY] != TicTacToeState.none || !_isPlayerTurn){
			return;
		}
		StartCoroutine(PlayerSelects_CoRoutine(coordX, coordY));
	}

	IEnumerator PlayerSelects_CoRoutine(int coordX, int coordY){ // Adds delay before AI move is displayed, and prevents player from clicking during this time
		moveCount++;

		SetVisual(coordX, coordY, playerState);

		if (VictoryCheck(playerState) == true) {
			onPlayerWin.Invoke(1);
		}
		else if (moveCount == 9) {
			onPlayerWin.Invoke(-1);		
		}
		else{
			_isPlayerTurn = false;

			yield return new WaitForSeconds(.6f);
			Vector2 coordXY = AiDecision();
			AiSelects((int)coordXY.x, (int)coordXY.y);
		}		
	}
	
	public void AiSelects(int coordX, int coordY)
	{

		moveCount++;

		SetVisual(coordX, coordY, aiState);

		if (VictoryCheck(aiState) == true)
		{
			//EndMessage onGameEnded(1);
			onPlayerWin.Invoke(0);
		}
		else if (moveCount == 9)
		{
			onPlayerWin.Invoke(-1);
		}
		_isPlayerTurn = true;
	}

	public bool VictoryCheck(TicTacToeState state)
	{
		if (boardState[0, 0] == state && boardState[0, 1] == state && boardState[0, 2] == state)
		{
			return true;
		}
		if (boardState[1, 0] == state && boardState[1, 1] == state && boardState[1, 2] == state)
		{
			return true;
		}

		if (boardState[2, 0] == state && boardState[2, 1] == state && boardState[2, 2] == state)
		{
			return true;
		}
		if (boardState[0, 0] == state && boardState[1, 0] == state && boardState[2, 0] == state)
		{
			return true;
		}
		if (boardState[0, 1] == state && boardState[1, 1] == state && boardState[2, 1] == state)
		{
			return true;
		}

		if (boardState[0, 2] == state && boardState[1, 2] == state && boardState[2, 2] == state)
		{
			return true;
		}

		//checks for diagnols
		if (boardState[0, 0] == state && boardState[1, 1] == state && boardState[2, 2] == state)
		{
			return true;
		}
		if (boardState[2, 0] == state && boardState[1, 1] == state && boardState[0, 2] == state)
		{
			return true;
		}
		return false;
	}

	/*public void EndGame(int winType) {
        switch (winType)
        {
			case '0':
				Debug.Log("Stalemate!");
				break;
			case '1':
				Debug.Log("You Win!");
				break;
			case '2':
				Debug.Log("You Lost!");
				break;
		}
		moveCount = 0;
	}*/

    public Vector2 AiDecision()
	{
		//Check if player selected Easy Mode (random AI placement) or Hard Mode (AI watches the board to deny victory and recognize when it can win.
		bool foundEmptySpot = false;
		if (_aiLevel == 0) // Easy Mode, AI randomly selects squares, does not ever check the board.
		{

			while (foundEmptySpot == false)
			{
				int randomNumeral1 = UnityEngine.Random.Range(0, 3);
				int randomNumeral2 = UnityEngine.Random.Range(0, 3);

				if (boardState[randomNumeral1, randomNumeral2] == TicTacToeState.none)
				{
					foundEmptySpot = true;
					return new Vector2(randomNumeral1, randomNumeral2);
				}
			} 
		}
		else  // AI HARDMODE  - Automatically checks for 2 pieces in any position of any row/column, and blocks the 3rd spot, denying player victory and securing AI victory
		{	  // After these checks, AI looks for possible guarunteed victories 1 move ahead based on current board configurations
			//Beggining of every turn: Begin check if AI is currently able to win, if not, check if player is currently able to win

			if (boardState[0, 0] == TicTacToeState.cross && boardState[0, 1] == TicTacToeState.cross && boardState[0, 2] == TicTacToeState.none)
			{
				return new Vector2(0, 2);
			}
			if (boardState[0, 1] == TicTacToeState.cross && boardState[0, 2] == TicTacToeState.cross && boardState[0, 0] == TicTacToeState.none)
			{
				return new Vector2(0, 0);
			}
			if (boardState[0, 0] == TicTacToeState.cross && boardState[0, 2] == TicTacToeState.cross && boardState[0, 1] == TicTacToeState.none)
			{
				return new Vector2(0, 1);
			}


			//Row 2 check
			if (boardState[1, 0] == TicTacToeState.cross && boardState[1, 1] == TicTacToeState.cross && boardState[1, 2] == TicTacToeState.none)
			{
				return new Vector2(1, 2);
			}
			if (boardState[1, 1] == TicTacToeState.cross && boardState[1, 2] == TicTacToeState.cross && boardState[1, 0] == TicTacToeState.none)
			{
				return new Vector2(1, 0);
			}
			if (boardState[1, 2] == TicTacToeState.cross && boardState[1, 0] == TicTacToeState.cross && boardState[1, 1] == TicTacToeState.none)
			{
				return new Vector2(1, 1);
			}


			//Row 3 check
			if (boardState[2, 0] == TicTacToeState.cross && boardState[2, 1] == TicTacToeState.cross && boardState[2, 2] == TicTacToeState.none)
			{
				return new Vector2(2, 2);
			}
			if (boardState[2, 1] == TicTacToeState.cross && boardState[2, 2] == TicTacToeState.cross && boardState[2, 0] == TicTacToeState.none)
			{
				return new Vector2(2, 0);
			}
			if (boardState[2, 0] == TicTacToeState.cross && boardState[2, 2] == TicTacToeState.cross && boardState[2, 1] == TicTacToeState.none)
			{
				return new Vector2(2, 1);
			}


			//Column 1 check
			if (boardState[0, 0] == TicTacToeState.cross && boardState[1, 0] == TicTacToeState.cross && boardState[2, 0] == TicTacToeState.none)
			{
				return new Vector2(2, 0);
			}
			if (boardState[1, 0] == TicTacToeState.cross && boardState[2, 0] == TicTacToeState.cross && boardState[0, 0] == TicTacToeState.none)
			{
				return new Vector2(0, 0);
			}
			if (boardState[0, 0] == TicTacToeState.cross && boardState[2, 0] == TicTacToeState.cross && boardState[1, 0] == TicTacToeState.none)
			{
				return new Vector2(1, 0);
			}


			//Column 2 check
			if (boardState[0, 1] == TicTacToeState.cross && boardState[1, 1] == TicTacToeState.cross && boardState[2, 1] == TicTacToeState.none)
			{
				return new Vector2(2, 1);
			}
			if (boardState[1, 1] == TicTacToeState.cross && boardState[2, 1] == TicTacToeState.cross && boardState[0, 1] == TicTacToeState.none)
			{
				return new Vector2(0, 1);
			}
			if (boardState[0, 1] == TicTacToeState.cross && boardState[2, 1] == TicTacToeState.cross && boardState[1, 1] == TicTacToeState.none)
			{
				return new Vector2(1, 1);
			}


			//Column 3 check
			if (boardState[0, 2] == TicTacToeState.cross && boardState[1, 2] == TicTacToeState.cross && boardState[2, 2] == TicTacToeState.none)
			{
				return new Vector2(2, 2);
			}
			if (boardState[1, 2] == TicTacToeState.cross && boardState[2, 2] == TicTacToeState.cross && boardState[0, 2] == TicTacToeState.none)
			{
				return new Vector2(0, 2);
			}
			if (boardState[0, 2] == TicTacToeState.cross && boardState[2, 2] == TicTacToeState.cross && boardState[1, 2] == TicTacToeState.none)
			{
				return new Vector2(1, 2);
			}


			//Diagnol 1 check
			if (boardState[0, 0] == TicTacToeState.cross && boardState[1, 1] == TicTacToeState.cross && boardState[2, 2] == TicTacToeState.none)
			{
				return new Vector2(2, 2);
			}
			if (boardState[1, 1] == TicTacToeState.cross && boardState[2, 2] == TicTacToeState.cross && boardState[0, 0] == TicTacToeState.none)
			{
				return new Vector2(0, 0);
			}
			if (boardState[2, 2] == TicTacToeState.cross && boardState[0, 0] == TicTacToeState.cross && boardState[1, 1] == TicTacToeState.none)
			{
				return new Vector2(1, 1);
			}


			//Diagnol 2 check
			if (boardState[2, 0] == TicTacToeState.cross && boardState[1, 1] == TicTacToeState.cross && boardState[0, 2] == TicTacToeState.none)
			{
				return new Vector2(0, 2);
			}
			if (boardState[0, 2] == TicTacToeState.cross && boardState[1, 1] == TicTacToeState.cross && boardState[2, 0] == TicTacToeState.none)
			{
				return new Vector2(2, 0);
			}
			if (boardState[2, 0] == TicTacToeState.cross && boardState[0, 2] == TicTacToeState.cross && boardState[1, 1] == TicTacToeState.none)
			{
				return new Vector2(1, 1);
			}


			//If code reaches this point (AI cannot win this turn), begin to check if it is possible to stop player from winning after this AI's move (2 in a row)

			//Row 1 check
			if (boardState[0, 0] == TicTacToeState.circle && boardState[0, 1] == TicTacToeState.circle && boardState[0, 2] == TicTacToeState.none) {
				return new Vector2(0, 2);
			}
			if (boardState[0, 1] == TicTacToeState.circle && boardState[0, 2] == TicTacToeState.circle && boardState[0, 0] == TicTacToeState.none) {
				return new Vector2(0, 0);
			}
			if (boardState[0, 0] == TicTacToeState.circle && boardState[0, 2] == TicTacToeState.circle && boardState[0, 1] == TicTacToeState.none) {
				return new Vector2(0, 1);
			}


			//Row 2 check
			if (boardState[1, 0] == TicTacToeState.circle && boardState[1, 1] == TicTacToeState.circle && boardState[1, 2] == TicTacToeState.none) {
				return new Vector2(1, 2);
			}
			if (boardState[1, 1] == TicTacToeState.circle && boardState[1, 2] == TicTacToeState.circle && boardState[1, 0] == TicTacToeState.none) {
				return new Vector2(1, 0);
			}
			if (boardState[1, 2] == TicTacToeState.circle && boardState[1, 0] == TicTacToeState.circle && boardState[1, 1] == TicTacToeState.none) {
				return new Vector2(1, 1);
			}


			//Row 3 check
			if (boardState[2, 0] == TicTacToeState.circle && boardState[2, 1] == TicTacToeState.circle && boardState[2, 2] == TicTacToeState.none) {
				return new Vector2(2, 2);
			}
			if (boardState[2, 1] == TicTacToeState.circle && boardState[2, 2] == TicTacToeState.circle && boardState[2, 0] == TicTacToeState.none) {
				return new Vector2(2, 0);
			}
			if (boardState[2, 0] == TicTacToeState.circle && boardState[2, 2] == TicTacToeState.circle && boardState[2, 1] == TicTacToeState.none) {
				return new Vector2(2, 1);
			}


			//Column 1 check
			if (boardState[0, 0] == TicTacToeState.circle && boardState[1, 0] == TicTacToeState.circle && boardState[2, 0] == TicTacToeState.none) {
				return new Vector2(2, 0);
			}
			if (boardState[1, 0] == TicTacToeState.circle && boardState[2, 0] == TicTacToeState.circle && boardState[0, 0] == TicTacToeState.none) {
				return new Vector2(0, 0);
			}
			if (boardState[0, 0] == TicTacToeState.circle && boardState[2, 0] == TicTacToeState.circle && boardState[1, 0] == TicTacToeState.none) {
				return new Vector2(1, 0);
			}


			//Column 2 check
			if (boardState[0, 1] == TicTacToeState.circle && boardState[1, 1] == TicTacToeState.circle && boardState[2, 1] == TicTacToeState.none) {
				return new Vector2(2, 1);
			}
			if (boardState[1, 1] == TicTacToeState.circle && boardState[2, 1] == TicTacToeState.circle && boardState[0, 1] == TicTacToeState.none) {
				return new Vector2(0, 1);
			}
			if (boardState[0, 1] == TicTacToeState.circle && boardState[2, 1] == TicTacToeState.circle && boardState[1, 1] == TicTacToeState.none) {
				return new Vector2(1, 1);
			}


			//Column 3 check
			if (boardState[0, 2] == TicTacToeState.circle && boardState[1, 2] == TicTacToeState.circle && boardState[2, 2] == TicTacToeState.none) {
				return new Vector2(2, 2);
			}
			if (boardState[1, 2] == TicTacToeState.circle && boardState[2, 2] == TicTacToeState.circle && boardState[0, 2] == TicTacToeState.none) {
				return new Vector2(0, 2);
			}
			if (boardState[0, 2] == TicTacToeState.circle && boardState[2, 2] == TicTacToeState.circle && boardState[1, 2] == TicTacToeState.none) {
				return new Vector2(1, 2);
			}


			//Diagnol 1 check
			if (boardState[0, 0] == TicTacToeState.circle && boardState[1, 1] == TicTacToeState.circle && boardState[2, 2] == TicTacToeState.none) {
				return new Vector2(2, 2);
			}
			if (boardState[1, 1] == TicTacToeState.circle && boardState[2, 2] == TicTacToeState.circle && boardState[0, 0] == TicTacToeState.none) {
				return new Vector2(0, 0);
			}
			if (boardState[2, 2] == TicTacToeState.circle && boardState[0, 0] == TicTacToeState.circle && boardState[1, 1] == TicTacToeState.none) {
				return new Vector2(1, 1);
			}


			//Diagnol 2 check
			if (boardState[2, 0] == TicTacToeState.circle && boardState[1, 1] == TicTacToeState.circle && boardState[0, 2] == TicTacToeState.none) {
				return new Vector2(0, 2);
			}
			if (boardState[0, 2] == TicTacToeState.circle && boardState[1, 1] == TicTacToeState.circle && boardState[2, 0] == TicTacToeState.none) {
				return new Vector2(2, 0);
			}
			if (boardState[2, 0] == TicTacToeState.circle && boardState[0, 2] == TicTacToeState.circle && boardState[1, 1] == TicTacToeState.none) {
				return new Vector2(1, 1);
			}

			//End of victory deny/secure checks

			//Begin check for player guarunteed victory 1 move in advance, place piece in corner to block player from executing the same guarunteed victory combo AI uses below
			if (boardState[0, 1] == TicTacToeState.circle && boardState[1, 0] == TicTacToeState.circle && boardState[0, 0] == TicTacToeState.none && boardState[2, 0] == TicTacToeState.none && boardState[0, 2] == TicTacToeState.none)
			{
				return new Vector2(0, 0);
			}
			if (boardState[0, 1] == TicTacToeState.circle && boardState[1, 2] == TicTacToeState.circle && boardState[0, 2] == TicTacToeState.none && boardState[0, 0] == TicTacToeState.none && boardState[2, 2] == TicTacToeState.none)
			{
				return new Vector2(0, 2);
			}
			if (boardState[1, 0] == TicTacToeState.circle && boardState[2, 1] == TicTacToeState.circle && boardState[2, 0] == TicTacToeState.none && boardState[0, 0] == TicTacToeState.none && boardState[2, 2] == TicTacToeState.none)
			{
				return new Vector2(2, 0);
			}
			if (boardState[1, 2] == TicTacToeState.circle && boardState[2, 1] == TicTacToeState.circle && boardState[2, 2] == TicTacToeState.none && boardState[2, 0] == TicTacToeState.none && boardState[0, 2] == TicTacToeState.none)
			{
				return new Vector2(2, 2);
			}

			//Begin check for AI guarunteed victory 1 move in advance, search for opportunity "corner" move that will requires two turns from player to stop

			if (boardState[0, 1] == TicTacToeState.cross && boardState[1, 0] == TicTacToeState.cross && boardState[0, 0] == TicTacToeState.none && boardState[2, 0] == TicTacToeState.none && boardState[0, 2] == TicTacToeState.none)
			{
				return new Vector2(0, 0);
			}
			if (boardState[0, 1] == TicTacToeState.cross && boardState[1, 2] == TicTacToeState.cross && boardState[0, 2] == TicTacToeState.none && boardState[0, 0] == TicTacToeState.none && boardState[2, 2] == TicTacToeState.none)
			{
				return new Vector2(0, 2);
			}
			if (boardState[1, 0] == TicTacToeState.cross && boardState[2, 1] == TicTacToeState.cross && boardState[2, 0] == TicTacToeState.none && boardState[0, 0] == TicTacToeState.none && boardState[2, 2] == TicTacToeState.none)
			{
				return new Vector2(2, 0);
			}
			if (boardState[1, 2] == TicTacToeState.cross && boardState[2, 1] == TicTacToeState.cross && boardState[2, 2] == TicTacToeState.none && boardState[2, 0] == TicTacToeState.none && boardState[0, 2] == TicTacToeState.none)
			{
				return new Vector2(2, 2);
			}
			


		}//End of guarunteed victory (1 move in advance) search for opportunity alternative move that requires two turns to stop

		while (foundEmptySpot == false) // if AI cannot find any favorable moves, it will randomly select a box.
			{
			if (boardState[1, 1] == TicTacToeState.none) { // occupies middle piece for best chance of survival
				return new Vector2(1, 1);
            } // by default, assume the middle square if the player has not
			if (boardState[1, 1] == TicTacToeState.circle) { // if player placed circle in middle square, place piece in an empty corner
				if (boardState[0, 0] == TicTacToeState.none)
                {
					return new Vector2 (0,0);
                }
				if (boardState[0, 2] == TicTacToeState.none)
				{
					return new Vector2(0, 2);
				}
				if (boardState[2, 0] == TicTacToeState.none)
				{
					return new Vector2(2, 0);
				}
				if (boardState[2, 2] == TicTacToeState.none)
				{
					return new Vector2(2, 2);
				}
			}
			//Check if Bottom Right, Upper Left are circles (winning move combo for player)
			if (boardState[0, 0] == TicTacToeState.circle && boardState[2, 2] == TicTacToeState.circle && boardState[0, 1] == TicTacToeState.none) {
				return new Vector2(0, 1);
            }
			if (boardState[0, 0] == TicTacToeState.circle && boardState[2, 2] == TicTacToeState.circle && boardState[1, 0] == TicTacToeState.none)
			{
				return new Vector2(1, 0);
			}
			if (boardState[0, 0] == TicTacToeState.circle && boardState[2, 2] == TicTacToeState.circle && boardState[1, 2] == TicTacToeState.none)
			{
				return new Vector2(1, 2);
			}
			if (boardState[0, 0] == TicTacToeState.circle && boardState[2, 2] == TicTacToeState.circle && boardState[2, 1] == TicTacToeState.none)
			{
				return new Vector2(2, 1);
			}
			//Check if Bottom Left, Upper Right are circles (winning move combo for player)
			if (boardState[2, 0] == TicTacToeState.circle && boardState[0, 2] == TicTacToeState.circle && boardState[0, 1] == TicTacToeState.none)
			{
				return new Vector2(0, 1);
			}
			if (boardState[2, 0] == TicTacToeState.circle && boardState[0, 2] == TicTacToeState.circle && boardState[1, 0] == TicTacToeState.none)
			{
				return new Vector2(1, 0);
			}
			if (boardState[2, 0] == TicTacToeState.circle && boardState[0, 2] == TicTacToeState.circle && boardState[1, 2] == TicTacToeState.none)
			{
				return new Vector2(1, 2);
			}
			if (boardState[2, 0] == TicTacToeState.circle && boardState[0, 2] == TicTacToeState.circle && boardState[2, 1] == TicTacToeState.none)
			{
				return new Vector2(2, 1);
			}


			int randomNumeral1 = UnityEngine.Random.Range(0, 3);
				int randomNumeral2 = UnityEngine.Random.Range(0, 3);

				if (boardState[randomNumeral1, randomNumeral2] == TicTacToeState.none)
				{
					foundEmptySpot = true;
				Debug.Log("AI HAD TO RANDOMIZE MOVE");
					return new Vector2(randomNumeral1, randomNumeral2);
				}
            
            
			}
		Debug.LogWarning("Reached outer end of AIDecision Script");
		return Vector2.zero;
		
	}

	private void SetVisual(int coordX, int coordY, TicTacToeState targetState)
	{
		Instantiate(
			targetState == TicTacToeState.circle ? _oPrefab : _xPrefab,
			_triggers[coordX, coordY].transform.position,
			Quaternion.identity);

		boardState[coordX, coordY] = targetState;
		Debug.Log(targetState);
	}
}