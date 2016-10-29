using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StateMachineGame.Actors.Footman
{
    class Marine : Soldier
    {


        public static Texture2D MarineTexture;
        public Marine(Texture2D image, Vector2 position, float rotation, float hp, List<Actor> enemyList, Actor[] forest, Actor headquarter)
            : base(image, position, rotation, hp, enemyList, forest,headquarter)
        {
            this.hp = 50;
            this.morale = 60;
            this.minDamage = 10;
            this.maxDamage = 15;
            baseMorale = 60;
            maxHp = 50;
            range = (float)(this.Image.Height * 2.5);
        }
    }
}
