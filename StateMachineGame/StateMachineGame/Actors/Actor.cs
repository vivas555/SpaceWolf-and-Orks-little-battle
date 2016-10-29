using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StateMachineGame
{

    /// <summary>
    /// Classe qui représente les acteurs de la partie.  
	/// </summary>
    /// <seealso cref="StateMachineGame.Objet2D" />
    class Actor : Objet2D
    {
        /// <summary>
        /// Rotation de l'acteur.  Il est normalement oreienté dans l'angle où il avance
        /// </summary>
        protected float rotation;

        /// <summary>
        /// vector de offset.  Le pivot d'un sprite est normalement en haut à gauche.  On veut le ramener au centre.
        /// </summary>        
        protected Vector2 centrageDuPivot;


        /// <summary>
        /// Voici deux objets de XNA qui permettent facilment de gérer les collisions.  Regardez les méthodes intersects, 
        /// elles peuvent tester les collisions avec pas mal toutes les formes possibles.  Une boite de collision avec une
        /// hauteur de zéro (axe des z) est une boite de collision et une sphère avec le pivot sur le plan zéro en z verra son rayon
        /// maximal être sur cette axe également, ce qui fait qu'on peut le traiter comme un cercle de collision.
        /// </summary>
        protected BoundingBox colisionBox;
        protected BoundingSphere colisionSphere;

        //Boite limite dans lequl un acteur peut évoluer.  Normalement il s'agit de l'écran.
        protected Vector2 upperLeftLimit;
        protected Vector2 lowerRightLimit;
        protected float hp;
        public float maxHp;

        public float Rotation
        {
            get
            {
                return rotation;
            }
            set
            {
                rotation = value;
            }
        }
        public Vector2 CentrageDuPivot
        {
            get
            {
                return centrageDuPivot;
            }
            set
            {
                centrageDuPivot = value;
            }
        }
        public BoundingBox ColisionBox
        {
            get
            {
                return colisionBox;
            }
            set
            {
                colisionBox = value;
            }
        }
        public BoundingSphere ColisionSphere
        {
            get
            {
                return colisionSphere;
            }
            set
            {
                colisionSphere = value;
            }
        }

        public virtual State CurrentState
        {
            get { return null; }
            set {}
        }


        public Actor(Texture2D image, Vector2 position, float rotation,  float hp) : base(image, position)
        {
            this.rotation = rotation;
            this.centrageDuPivot = new Vector2(image.Width / 2, image.Height / 2);
            this.colisionBox = new BoundingBox(new Vector3(position.X - centrageDuPivot.X, position.Y - centrageDuPivot.Y, 0), new Vector3(position.X + centrageDuPivot.X, position.Y + centrageDuPivot.Y, 0));
            this.colisionSphere = new BoundingSphere(new Vector3(position.X, position.Y, 0), centrageDuPivot.X);
            this.upperLeftLimit = new Vector2(image.Width / 2, image.Height / 2) + centrageDuPivot;
            this.lowerRightLimit = new Vector2(1280 - image.Width / 2, 720 -image.Height / 2) - centrageDuPivot;
            this.hp = hp;
            this.maxHp = hp;
            this.colisionSphere.Radius = 1;
        }

        public float Hp
        {
            get { return hp; }
            set { hp = value; }
        }

        public float MaxHp
        {
            get { return maxHp; }
            set { maxHp = value; }
        }


        public virtual void Move()
        {
            
        }

        public override void Dessiner(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                image,              //L'image à afficher
                position,           //sa position
                null,               //Rectangle pour un affichage partiel: null signifit qu'on affiche tout le sprite
                Color.White,        //couleur du filtre.  Blanc veut dire aucun filtre
                rotation,           //rotation de l'image
                centrageDuPivot,    //point de pivot.  0,0 par défaut
                1f,                 //Mise à l'échelle: 1 = taille de base
                SpriteEffects.None, //effets de mirroir
                0                   //Échelles de profondeur.  Si tout est égal, le dernier affiché dans l'ordre du
                );                  //code sera celui "le plus sur le dessus"
        }


		//Comme le tours et les soldiers reçoivent des dommages, mais agissent de manière différente. Cette méthode ce trouve ici et est override pour les soldiers
        public virtual void recevingDamage(float damage, Actor attacker)
        {
            this.hp -= damage;
        }
    }
}
