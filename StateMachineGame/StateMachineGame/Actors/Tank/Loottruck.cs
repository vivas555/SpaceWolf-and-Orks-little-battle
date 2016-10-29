using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StateMachineGame.Actors.Tank
{
    class Loottruck: Soldier
    {

        public static Texture2D LoottruckTexture;
        public Loottruck(Texture2D image, Vector2 position, float rotation, float hp, List<Actor> enemyList, Actor[] forest,Actor headquarter)
            : base(image, position, rotation, hp, enemyList, forest,headquarter)
        {
            this.hp = 260;
            this.morale = 200;
            this.minDamage = 75;
            this.maxDamage = 90;
            baseMorale = 200;
            maxHp = 260;
            range = (float)(this.Image.Height * 2.5);
        }
    }
}
