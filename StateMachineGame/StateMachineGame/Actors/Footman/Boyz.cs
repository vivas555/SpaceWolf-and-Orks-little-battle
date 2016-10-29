using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StateMachineGame.Actors.Footman
{
    class Boyz:Soldier
    {
        public static Texture2D boyzTexture;

        public Boyz(Texture2D image, Vector2 position, float rotation, float hp, List<Actor> enemyList, Actor[] forest, Actor headquarter)
            : base(image, position, rotation, hp, enemyList, forest,headquarter)
        {
            this.hp = 25;
            this.morale = 20;
           this. baseMorale = 20;
            this.maxHp = 25;
            this.minDamage = 10;
            this.maxDamage = 15;
            this.range = this.image.Width/2;


        }
    }
}
