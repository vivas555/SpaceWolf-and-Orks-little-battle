using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StateMachineGame.Actors.Walker
{
    class Nobz : Soldier
    {


        public static Texture2D NobzTexture;
        public Nobz(Texture2D image, Vector2 position, float rotation, float hp, List<Actor> enemyList, Actor[] forest,Actor headquarter)
            : base(image, position, rotation, hp, enemyList, forest,headquarter)
        {
            this.hp = 100;
            this.morale = 150;
            this.minDamage = 35; 
            this.maxDamage = 55;
            baseMorale = 150;
            maxHp = 100;
            range = (float)(this.Image.Height * 2.5);
        }
    }
}
