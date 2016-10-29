using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using StateMachineGame.Actors;
using StateMachineGame.Actors.Footman;
using StateMachineGame.Actors.Tank;
using StateMachineGame.Actors.Walker;

namespace StateMachineGame
{
   static class SoldierFabric
    {
        public enum soldierType
        {
            marine,
            boyz,
            dreadnought,
            nobz,
            landraider,
            loottruck
        }

        static public Soldier creatSoldier(soldierType token, Vector2 spawnPosition, List<Actor> enemyList, Actor[] forest,Actor headquarter)
       {
           switch (token)
           {
                   case soldierType.boyz:
                   return new Boyz(Boyz.boyzTexture,spawnPosition,0,0,enemyList,forest,headquarter);
                   break;

                   case soldierType.dreadnought:
                   return new Dreadnought(Dreadnought.DreadnoughTexture, spawnPosition, 0, 0, enemyList, forest, headquarter);
                   break;

                   case soldierType.landraider:
                   return new Landraider(Landraider.LandraiderTexture, spawnPosition, 0, 0, enemyList, forest, headquarter);
                   break;

                   case soldierType.loottruck:
                   return new Loottruck(Loottruck.LoottruckTexture, spawnPosition, 0, 0, enemyList, forest, headquarter);
                   break;

                   case soldierType.marine:
                   return new Marine(Marine.MarineTexture, spawnPosition, 0, 0, enemyList, forest, headquarter);
                   break;

                   case soldierType.nobz:
                   return new Nobz(Nobz.NobzTexture, spawnPosition, 0, 0, enemyList, forest, headquarter);
                   break;

                   

           }
           return null;
       }
    }
}
