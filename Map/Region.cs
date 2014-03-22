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

namespace AIChallengeFramework
{
	/// <summary>
	/// A region is the smallest organizational unit in the challenge. Regions
	/// themselves belong to continents, and continents belong to a map.
	/// 
	/// A region is closely defined by its parameters, most importantly its ID.
	/// Additionally, a region has neighbors, which get set during the
	/// initialization of the game board.
	/// </summary>
	public class Region
	{
		/// <summary>
		/// If the classic map is played, you can use these enums to map between
		/// readable names and the IDs in the system. Please note that, if for
		/// whatever reason, another map is played, those mappings are meaningless.
		/// </summary>
		public enum Name {
			Alaska = 1,
			NorthwestTerritory = 2,
			Greenland = 3,
			Alberta = 4,
			Ontario = 5,
			Quebec = 6,
			WesternUnitedStates = 7,
			EasternUnitedStates = 8,
			CentralAmerica = 9,
			Venezuela = 10,
			Peru = 11,
			Brazil = 12,
			Argentina = 13,
			Iceland = 14,
			GreatBritain = 15,
			Scandinavia = 16,
			Ukraine = 17,
			WesternEurope = 18,
			NorthernEurope = 19,
			SouthernEurope = 20,
			NorthAfrica = 21,
			Egypt = 22,
			EastAfrica = 23,
			Congo = 24,
			SouthAfrica = 25,
			Madagascar = 26,
			Ural = 27,
			Siberia = 28,
			Yakutsk = 29,
			Kamchatka = 30,
			Irkutsk = 31,
			Kazakhstan = 32,
			China = 33,
			Mongolia = 34,
			Japan = 35,
			MiddleEast = 36,
			India = 37,
			Siam = 38,
			Indonesia = 39,
			NewGuinea = 40,
			WesternAustralia = 41,
			EasternAustralia = 42
		}

		/// <summary>
		/// The identifier of the region.
		/// </summary>
		/// <value>The identifier.</value>
		public int Id { get; private set; }

		/// <summary>
		/// Every region has neighors, which are saved in this list.
		/// </summary>
		/// <value>The neighbors.</value>
		public List<Region> Neighbors { get; private set; }

		/// <summary>
		/// Every region is part of a continent.
		/// </summary>
		/// <value>The continent.</value>
		public Continent Continent { get; private set; }

		/// <summary>
		/// If a region has been conquered, a non-zero amount of armies
		/// holds it.
		/// </summary>
		/// <value>The armies.</value>
		public int Armies { get; set; }

		/// <summary>
		/// The owner of a region can either be the name of the owner
		/// or null, if the region has no owner.
		/// </summary>
		/// <value>The owner.</value>
		public string Owner { get; set; }

		public Region (int id, Continent continent)
		{
			this.Id = id;
			Neighbors = new List<Region> ();
			this.Continent = continent;
			continent.AddRegion (this);
			Armies = 2;
			Owner = "unknown";

			if (Logger.IsDebug ()) {
				Logger.Debug (string.Format("Region {0}:\tInitialized with continent {1}", id, continent.Id));
			} else {
				Logger.Info (string.Format("Region {0}:\tInitialized.", id));
			}
		}

		/// <summary>
		/// Adds a neighbor to this region's list of neighbors.
		/// </summary>
		/// <param name="region">Region.</param>
		public void AddNeighbor (Region region)
		{
			if (!region.Equals (this) && !Neighbors.Contains (region)) {
				Neighbors.Add (region);
				region.AddNeighbor (this);

				if (IsBorderRegion ()) {
					Continent.BorderRegions.Add (this);
				}

				if (Logger.IsDebug ()) {
					Logger.Debug (string.Format("Region {0}:\tAdded neighbor {1}.", Id, region.Id));
				}
			}
		}

		/// <summary>
		/// Determines whether this instance has enemy neighbors.
		/// </summary>
		/// <returns><c>true</c> if this instance has enemy neighbors; otherwise, <c>false</c>.</returns>
		public bool HasEnemyNeighbors () {
			if (NumberOfEnemyNeighbors () == 0) {
				return false;
			} else {
				return true;
			}
		}

		/// <summary>
		/// Determines whether this region has an owner.
		/// </summary>
		/// <returns><c>true</c> if this region has an owner; otherwise, <c>false</c>.</returns>
		public bool HasOwner ()
		{
			if (Owner.Equals (null)) {
				return false;
			} else {
				return true;
			}
		}

		/// <summary>
		/// Determines whether this instance is a border region.
		/// </summary>
		/// <returns><c>true</c> if this instance is a border region; otherwise, <c>false</c>.</returns>
		public bool IsBorderRegion ()
		{
			foreach (Region n in Neighbors) {
				if (!Continent.Equals (n.Continent)) {
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Calculates the number of enemy neighbors.
		/// </summary>
		/// <returns>The of enemy neighbors.</returns>
		public int NumberOfEnemyNeighbors () {
			int numberOfEnemyNeighbors = 0;

			foreach (Region r in Neighbors) {
				if (!r.Owner.Equals (Owner)) {
					numberOfEnemyNeighbors++;
				}
			}

			return numberOfEnemyNeighbors;
		}
	}
}

