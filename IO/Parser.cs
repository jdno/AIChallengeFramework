//
//  Copyright 2014  jdno
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace AIChallengeFramework
{
	/// <summary>
	/// The parser takes care of the communicaton with the game engine. It parses
	/// the information and instructions presented by the engine, and triggers the
	/// appropriate actions, as well as converting the moves by the bot back into
	/// a format the engine understands.
	/// </summary>
	public class Parser
	{
		/// <summary>
		/// Your bot.
		/// </summary>
		/// <value>The bot.</value>
		public IBot Bot { get; private set; }

		/// <summary>
		/// The framework keeps track of the state of the game, which provides useful
		/// information to you.
		/// </summary>
		/// <value>The state.</value>
		public State State { get; private set; }

		public Parser (IBot bot)
		{
			Logger.Initialize ();

			this.Bot = bot;
			State = new State ();

			Logger.Info ("Parser:\tInitialized.");
		}

		public Parser (IBot bot, State state)
		{
			Logger.Initialize ();

			this.Bot = bot;
			this.State = state;

			Logger.Info ("Parser:\tInitialized.");
		}

		/// <summary>
		/// This is the main method if this framework, as it manages the interaction
		/// with the game engine. It contains a while-loop that constantly tries to
		/// read from the console until the program exists.
		/// </summary>
		public void Run ()
		{
			Logger.Info ("Parser:\tStarting loop.");

			try {
				Console.SetIn (new StreamReader (Console.OpenStandardInput (8192)));
			} catch (Exception e) {
				Logger.Error (string.Format (
					"Parser:\tCaught exception of type {0} while configuring the console.", e.GetType ().Name));
			}

			string command;

			while (true) {
				command = Console.ReadLine ();

				// Shut down gracefully when no more commands arrive.
				if (command.Equals (null)) {
					Logger.Info ("Parser:\tStopping loop.");
					break;
				}

				command = command.Trim ();

				// Wait for next command
				if (command.Length == 0) {
					continue;
				}

				try {
					ParseLine (command);
				} catch (Exception e) {
					Logger.Error (string.Format (
						"Parser:\tCaught exception of type {0} while parsing '{1}'.", e.GetType ().Name, command));
					Logger.Error (e.StackTrace);
				}
			}
		}

		/// <summary>
		/// This method interprets the first part of the command and uses
		/// a bunch of auxiliary methods to actually parse the command.
		/// </summary>
		/// <param name="commandLine">Command line.</param>
		public void ParseLine (string commandLine)
		{
			string[] commandParts = commandLine.Split (' ');

			switch (commandParts [0]) {
			case "go":
				Go (commandParts);
				break;
			
			case "opponent_moves":
				OpponentMoves (commandLine);
				break;
			
			case "update_map":
				UpdateMap (commandParts);
				break;
			
			case "pick_starting_regions":
				PickStartingRegions (commandParts);
				break;

			case "settings":
				Settings (commandParts);
				break;

			case "setup_map":
				SetupMap (commandParts);
				break;

			default:
				printErrorUnknownCommand (commandParts[0]);
				break;
			}
		}

		/// <summary>
		/// This method parses all commands that the bot has to react to.
		/// </summary>
		/// <param name="commandParts">Command parts.</param>
		public void Go (string[] commandParts)
		{
			if (commandParts.Length == 3) {

				switch (commandParts [1]) {
				case "place_armies":
					PlaceArmies (State.MyArmiesPerTurn);
					break;

				case "attack/transfer":
					AttackTransfer ();
					break;

				default:
					printErrorUnknownCommand ("go " + commandParts [1]);
					break;
				}

			} else {
				printErrorArgumentCount ("go", 3, commandParts.Length);
			}
		}

		/// <summary>
		/// This method takes care of moves the opponent made, as far
		/// as they are provided by the game engine.
		/// </summary>
		/// <param name="commandLine">Command line.</param>
		public void OpponentMoves (string commandLine)
		{
			commandLine = commandLine.Remove (0, 14);
			commandLine = commandLine.Trim ();
			string[] moves = commandLine.Split (',');
			string[] parts;

			List<Move> opponentMoves = new List<Move> ();
			Region sourceRegion;
			Region targetRegion;

			foreach (string move in moves) {
				parts = move.Split (' ');

				if (parts.Length == 4 && parts[1].Equals ("place_armies")) {
					sourceRegion = State.CompleteMap.Regions[int.Parse(parts[2])];
					PlaceArmiesMove placeArmiesMove = new PlaceArmiesMove (parts[0], sourceRegion, int.Parse(parts[3]));
					opponentMoves.Add (placeArmiesMove);
					State.ProcessPlaceArmiesMove (placeArmiesMove);
				}

				if (parts.Length == 5 && parts [1].Equals ("attack/transfer")) {
					sourceRegion = State.CompleteMap.Regions [int.Parse (parts [2])];
					targetRegion = State.CompleteMap.Regions [int.Parse (parts [3])];
					AttackTransferMove attackTransferMove =
						new AttackTransferMove (parts [0], sourceRegion, targetRegion, int.Parse (parts [4]));
					opponentMoves.Add (attackTransferMove);
					State.ProcessAttackTransferMove (attackTransferMove);
				}
			}

			State.EnemyMoves = opponentMoves;

			if (Logger.IsDebug ()) {
				Logger.Debug ("Parser:\tProcessed the opponent's moves.");
			}
		}

		/// <summary>
		/// This method updates the map whenever new information about visible
		/// countries exist. It is also responsible for verifying that the rewards
		/// are still valid.
		/// </summary>
		/// <param name="commandParts">Command parts.</param>
		public void UpdateMap (string[] commandParts)
		{
			for (int i = 1; i < commandParts.Length; i += 3) {
				State.UpdateMap (int.Parse(commandParts[i]), commandParts[i+1], int.Parse(commandParts[i+2]));
			}

			State.CheckRewards ();

			if (Logger.IsDebug ()) {
				Logger.Debug ("Parser:\tUpdated the map.");
			}
		}

		/// <summary>
		/// Presents a list of possible starting regions to the client and returns
		/// his choices to the game engine.
		/// </summary>
		/// <param name="commandParts">Command parts.</param>
		public void PickStartingRegions (string[] commandParts)
		{
			List<Region> startingRegions = new List<Region> ();
			Region region;

			for (int i = 2; i < commandParts.Length; i++) {
				try {
					region = State.CompleteMap.Regions [int.Parse(commandParts [i])];
					startingRegions.Add (region);
				} catch (Exception) {
					Logger.Error (string.Format("Parser:\tException while looking up region {0}.", commandParts [i]));
				}
			}

			startingRegions = Bot.PreferredStartingRegions (startingRegions);
			StringBuilder sb = new StringBuilder ();

			for (int i = 0; i < startingRegions.Count && i < 6; i++) {
				sb.Append (startingRegions [i].Id).Append (' ');
			}

			if (startingRegions.Count != 6) {
				Logger.Error (string.Format("Not enough starting regions picked: {0} of 6", startingRegions.Count));
			}

			sb.Remove (sb.Length - 1, 1);

			printResponse (sb.ToString ());
		}

		/// <summary>
		/// This method configures the game state, and is only used when
		/// the game gets initialized.
		/// </summary>
		/// <param name="commandParts">Command parts.</param>
		public void Settings (string[] commandParts)
		{
			if (commandParts.Length == 3) {

				switch (commandParts [1]) {
				case "your_bot":
					State.MyName = commandParts [2];
					break;

				case "opponent_bot":
					State.EnemyName = commandParts [2];
					break;

				case "starting_armies":
					int startingArmies = int.Parse (commandParts [2]);
					State.MyArmiesPerTurn = startingArmies;
					break;

				default:
					printErrorUnknownCommand ("settings " + commandParts [1]);
					break;
				}

			} else {
				printErrorArgumentCount ("settings", 3, commandParts.Length);
			}
		}

		/// <summary>
		/// This method sets up the map, and is only used when the game gets
		/// initialized.
		/// </summary>
		/// <param name="commandParts">Command parts.</param>
		public void SetupMap (string[] commandParts)
		{
			if (commandParts.Length > 3) {

				switch (commandParts [1]) {
				case "super_regions":
					SuperRegions (commandParts);
					break;

				case "regions":
					Regions (commandParts);
					break;

				case "neighbors":
					Neighbors (commandParts);
					break;

				default:
					printErrorUnknownCommand("setup_map " + commandParts[1]);
					break;
				}

			} else {
				printErrorArgumentCount ("setup_map", 3, commandParts.Length);
			}
		}

		/// <summary>
		/// This method asks the bot to place his armies, and returns the moves
		/// back to the game engine.
		/// </summary>
		/// <param name="armies">Armies.</param>
		public void PlaceArmies (int armies)
		{
			List<PlaceArmiesMove> moves = Bot.PlaceArmies (armies);

			StringBuilder sb = new StringBuilder ();

			foreach (PlaceArmiesMove move in moves) {
				State.ProcessPlaceArmiesMove (move);
				sb.Append (move.Parse ()).Append (", ");
			}

			if (sb.Length > 0) {
				sb.Remove (sb.Length - 2, 2);
				printResponse (sb.ToString ());
			} else {
				printResponse ("No moves");
			}
		}

		/// <summary>
		/// This method asks the bot to attack or transfer, and returns the
		/// moves back to the game engine.
		/// </summary>
		public void AttackTransfer ()
		{
			List<AttackTransferMove> moves = Bot.AttackOrTransfer ();

			StringBuilder sb = new StringBuilder ();

			foreach (AttackTransferMove move in moves) {
				State.ProcessAttackTransferMove (move);
				sb.Append (move.Parse ()).Append (", ");
			}

			if (moves.Count > 0) {
				sb.Remove (sb.Length - 2, 2);
				printResponse (sb.ToString ());
			} else {
				printResponse ("No moves");
			}
		}

		/// <summary>
		/// This method sets up the continents, and is only used when the
		/// game gets initialized.
		/// </summary>
		/// <param name="commandParts">Command parts.</param>
		public void SuperRegions (string[] commandParts)
		{
			int id, reward;
			Continent continent;

			for (int i = 2; i < commandParts.Length; i += 2) {
				id = int.Parse(commandParts [i]);
				reward = int.Parse(commandParts [i + 1]);
				continent = new Continent (id, reward);

				State.CompleteMap.AddContinent (continent);
			}

			if (Logger.IsDebug ()) {
				Logger.Debug ("Parser:\tSet up continents.");
			}
		}

		/// <summary>
		/// This method sets up the regions, and is only used when the game
		/// gets initialized.
		/// </summary>
		/// <param name="commandParts">Command parts.</param>
		public void Regions (string[] commandParts)
		{
			int id, continentId;
			Region region;
			Continent continent;

			for (int i = 2; i < commandParts.Length; i += 2) {
				id = int.Parse (commandParts [i]);
				continentId = int.Parse (commandParts [i + 1]);

				continent = State.CompleteMap.ContinentForId (continentId);

				if (continent.Equals (null)) {
					Logger.Info (string.Format("Parser:\tFailed to add region {0} with the unkown continent {1}.",
						id, continentId));
					continue;
				} else {
					region = new Region (id, continent);
				}
					
				State.CompleteMap.AddRegion (region);
			}

			if (Logger.IsDebug ()) {
				Logger.Debug ("Parser:\tSet up regions.");
			}
		}

		/// <summary>
		/// This method sets up the neighbor relationships between regions, and
		/// is only used when the game gets initialized.
		/// </summary>
		/// <param name="commandParts">Command parts.</param>
		public void Neighbors (string[] commandParts)
		{
			int id, neighborId;
			string[] neighborStrings;
			Region region, neighbor;

			for (int i = 2; i < commandParts.Length; i += 2) {
				id = int.Parse (commandParts [i]);
				region = State.CompleteMap.Regions [id];

				neighborStrings = commandParts [i + 1].Split (',');

				foreach (string s in neighborStrings) {
					neighborId = int.Parse (s);
					neighbor = State.CompleteMap.Regions [neighborId];
					region.AddNeighbor (neighbor);
				}
			}

			if (Logger.IsDebug ()) {
				Logger.Debug ("Parser:\tSet up neighbor relationships.");
			}
		}

		/// <summary>
		/// This auxiliary method prints the responses on the console.
		/// </summary>
		/// <param name="response">Response.</param>
		private void printResponse (string response)
		{
			Console.WriteLine (response);
		}

		/// <summary>
		/// This auxiliary method is used to print an error that occured because the
		/// number of expected arguments did not match the number of existing ones.
		/// </summary>
		/// <param name="command">Command.</param>
		/// <param name="expectedCount">Expected count.</param>
		/// <param name="isCount">Is count.</param>
		private void printErrorArgumentCount (string command, int expectedCount, int isCount)
		{
			string text = "Wrong argument count with command {0}: expected {1}, was {2}";
			Logger.Error (string.Format(text, command, expectedCount, isCount));
		}

		/// <summary>
		/// This auxiliary method is used to print an error that occured because a
		/// command was not understood.
		/// </summary>
		/// <param name="command">Command.</param>
		private void printErrorUnknownCommand (string command)
		{
			Logger.Error (string.Format("Unknown command received from game engine: {0}", command));
		}
	}
}

