﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace StateMachineGame
{
    /// <summary>
    /// Classe simple qui réunit le minimum afin de pouvoir afficher un sprite à l'écran
    /// </summary>
    /// <remarks>
    /// La classe Acteur hérite de Objet2D
    /// </remarks>
    class Objet2D
    {
        protected Texture2D image;
        protected Vector2 position;

        public Texture2D Image
        {
            get
            {
                return image;
            }
            set
            {
                image = value;
            }
        }
        public Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }
        public Objet2D(Texture2D image, Vector2 position)
        {
            this.image = image;
            this.position = position;
        }

        /// <summary>
        /// Dessiners the specified sprite batch.
        /// </summary>
        /// <param name="spriteBatch">Batch d'écriture à l'écran en cours, on le passe en 
        /// référence afin de contrôler l'affichage du sprite directement dans la classe</param>
        public virtual void Dessiner(SpriteBatch spriteBatch)
        {
            //Ici la position représente le coin haut à gauche du sprite            
            //Color.White fait en sorte qu'on ne mat aucun filtre de couleur sur notre image
            spriteBatch.Draw(image, position, Color.White);
        }
    }
}
