﻿//
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

namespace AIChallengeFramework
{
	/// <summary>
	/// The state of the game is the most important source of information for the
	/// bot. All information provided by the game engine gets processed and stored.
	/// </summary>
	public class State
	{
		/// <summary>
		/// Your bot.
		/// </summary>
		/// <value>Your bot.</value>
		public Bot MyBot { get; set; }

		/// <summary>
		/// The enemy's bot.
		/// </summary>
		/// <value>The enemy's bot.</value>
		public Bot EnemyBot { get; set; }

		/// <summary>
		/// This map contains all information that is available.
		/// </summary>
		/// <value>The complete map.</value>
		public Map CompleteMap { get; set; }

		/// <summary>
		/// This map only contains the part that is visible to the user.
		/// </summary>
		/// <value>The visible map.</value>
		public Map VisibleMap { get; set; }

		public State ()
		{
			MyBot = new Bot (this);
			EnemyBot = new Bot (this);
			CompleteMap = new Map ();
			VisibleMap = new Map ();
			MyBot.ArmiesPerTurn = 5;
			EnemyBot.ArmiesPerTurn = 5;

			Logger.Info ("State:\tInitialized.");
		}

		/// <summary>
		/// Check if a player currently is entitled to rewards for holding a
		/// country. Also keep track of changes to correctly calculate the
		/// number of armies the users are entitled to per turn.
		/// </summary>
		public void CheckRewards ()
		{
			string owner;

			foreach (Continent c in VisibleMap.Continents) {
				owner = c.OwnedBy ();

				if (owner.Equals ("unknown")) {
					if (MyBot.OwnsContinent (c)) {
						MyBot.LoseContinent (c);
					}

					if (EnemyBot.OwnsContinent (c)) {
						EnemyBot.LoseContinent (c);
					}
				}

				if (owner.Equals (MyBot.Name)) {
					if (!MyBot.OwnsContinent (c)) {
						MyBot.GainContinent (c);

						if (EnemyBot.OwnsContinent (c)) {
							EnemyBot.LoseContinent (c);
						}
					}
				}

				if (owner.Equals (EnemyBot.Name)) {
					if (!EnemyBot.OwnsContinent (c)) {
						EnemyBot.GainContinent (c);

						if (MyBot.OwnsContinent (c)) {
							MyBot.LoseContinent (c);
						}
					}
				}
			}

			Logger.Info ("State:\tChecked rewards.");
		}

		/// <summary>
		/// Process a AttackTransferMove, and updates the number of armies
		/// in the source and target region.
		/// </summary>
		/// <param name="move">Move.</param>
		public void ProcessAttackTransferMove (AttackTransferMove move)
		{
			int sourceId = move.SourceRegion.Id;
			int targetId = move.TargetRegion.Id;

			if (VisibleMap.Regions.ContainsKey (sourceId)) {
				Region sourceRegion = VisibleMap.Regions [move.SourceRegion.Id];
				sourceRegion.Armies -= move.Armies;
			}

			if (VisibleMap.Regions.ContainsKey (targetId)) {
				Region targetRegion = VisibleMap.Regions [move.TargetRegion.Id];
				targetRegion.Armies += move.Armies;
			}

			if (Logger.IsDebug ()) {
				Logger.Debug (string.Format("State:\tProcessed attack/transfer from {0} to {1} with {2} armies.",
					sourceId, targetId, move.Armies));
			}
		}

		/// <summary>
		/// Processes a PlaceArmiesMove, and updates the number of armies
		/// in the target region.
		/// </summary>
		/// <param name="move">Move.</param>
		public void ProcessPlaceArmiesMove (PlaceArmiesMove move)
		{
			Region region = VisibleMap.Regions [move.Region.Id];
			region.Armies += move.Armies;
			
			if (Logger.IsDebug ()) {
				Logger.Debug (string.Format("State:\tProcessed placement of {0} armies into {1}.",
					move.Region.Id, move.Armies));
			}
		}

		/// <summary>
		/// Updates the map.
		/// </summary>
		/// <param name="player">Player.</param>
		/// <param name="regionId">Region identifier.</param>
		/// <param name="armies">Armies.</param>
		public void UpdateMap (int regionId, string player, int armies)
		{
			if (VisibleMap.Regions.ContainsKey (regionId)) {
				Region originalRegion = VisibleMap.Regions [regionId];
				originalRegion = VisibleMap.Regions [regionId];
				originalRegion.Owner = player;
				originalRegion.Armies = armies;
			} else {
				Region originalRegion = CompleteMap.Regions [regionId];
				Region newRegion;
				Continent continent;

				if (!VisibleMap.Continents.Contains (originalRegion.Continent)) {
					Continent originalContinent = originalRegion.Continent;
					Continent newContinent = new Continent (originalContinent.Id, originalContinent.Reward);
					VisibleMap.Continents.Add (newContinent);
				}

				continent = VisibleMap.ContinentForId (originalRegion.Continent.Id);
				newRegion = new Region (originalRegion.Id, continent);

				foreach (Region n in originalRegion.Neighbors) {
					if (VisibleMap.Regions.ContainsKey (n.Id)) {
						newRegion.AddNeighbor (VisibleMap.Regions[n.Id]);
					}
				}

				newRegion.Owner = player;
				newRegion.Armies = armies;

				VisibleMap.AddRegion (newRegion);
			}
				
			if (Logger.IsDebug ()) {
				Logger.Debug (string.Format("State:\tUpdated region {0} with owner {1} and {2} armies.",
					regionId, player, armies));
			}
		}
	}
}

