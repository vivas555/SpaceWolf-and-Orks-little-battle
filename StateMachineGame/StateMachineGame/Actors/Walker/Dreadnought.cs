using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StateMachineGame.Actors.Walker
{
    class Dreadnought: Soldier
    {


        public static Texture2D DreadnoughTexture;
        public Dreadnought(Texture2D image, Vector2 position, float rotation, float hp, List<Actor> enemyList, Actor[] forest,Actor headquarter)
            : base(image, position, rotation, hp, enemyList, forest,headquarter)
        {
            this.hp = 200;
            this.morale = 150;
            this.minDamage = 40;
            this.maxDamage = 60;
            baseMorale = 150;
            maxHp = 200;
            range = (float)(this.Image.Height * 2.5);
        }
    }
}
