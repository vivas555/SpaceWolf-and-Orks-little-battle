using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StateMachineGame.Actors.Tank
{
    internal class Landraider : Soldier
    {
        public static Texture2D LandraiderTexture;

        public Landraider(Texture2D image, Vector2 position, float rotation, float hp, List<Actor> enemyList,
            Actor[] forest, Actor headquarter)
            : base(image, position, rotation, hp, enemyList, forest, headquarter)
        {
            this.hp = 520;
            morale = 500;
            minDamage = 90;
            maxDamage = 92;
            baseMorale = 500;
            maxHp = 520;
            range = Image.Height*2;
        }
    }
}