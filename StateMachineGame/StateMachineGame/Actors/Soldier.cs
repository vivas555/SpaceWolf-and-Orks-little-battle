using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StateMachineGame.Actors
{
    class Soldier: Actor
    {
   
        protected float morale;
        protected float movementSpeed;
        protected Actor target;
        protected float range;
        protected float minDamage;
        protected float maxDamage;
        Actor[] forest = new Actor[4];
        protected float xp;
        protected State currentState;
        public float baseMorale;      
        protected RulesOfEngagement CurrentRulesOfEngagement ;
        protected bool isInCombat;
        protected List<Actor> enemyList;
        protected Actor headquarter;
        protected bool canMove;


        public Soldier(Texture2D image, Vector2 position, float rotation, float hp , List<Actor> _enemyList, Actor[] forest, Actor headQuarter) : base(image, position, rotation,hp)
        {

            movementSpeed = 1;
            this.target = _enemyList[0];
            xp = 0;
            this.forest = forest;
            CurrentRulesOfEngagement = RulesOfEngagement.Defensive;
            isInCombat = false;
            this.enemyList = _enemyList;
            this.headquarter = headQuarter;
            currentState = new NormalState(this);

        }

		//Propriétés C# de la classe pour avoir accès à ses valeurs
		#region
        public float Hp
        {
            get { return hp; }
            set { hp = value; }
        }

		 public Actor Headquarter
        {
            get { return headquarter; }
            set { headquarter = value; }
        }

        public float MinDamage
        {
            get { return minDamage; }
        }

        public float MaxDamage
        {
            get { return maxDamage; }
        }
		
		public float Morale
        {
            get { return morale; }
            set { morale = value; }
        }

        public Actor Target
        {
            get { return target; }
            set { target = value; }
        }

        public float Xp
        {
            get { return xp; }
            set { xp = value; }
        }

        public override State CurrentState
        {
            get { return currentState; }
            set { currentState = value; }
        }

        public RulesOfEngagement CurrentRulesOfEngagement1
        {
            get { return CurrentRulesOfEngagement; }
            set { CurrentRulesOfEngagement = value; }
        }

        public bool IsInCombat
        {
            get { return isInCombat; }
            set { isInCombat = value; }
        }
		
		 public float Range
        {
            get { return range; }
            set { range = value; }
        }
		
		#endregion
		
        /// <summary>
        /// Algorithme de déplacement standard.  L'acteur détecte sa cible et détermine quel est l'angle pour s'orienter vers la cible.
        /// Si la distance entre l'acteur et sa cible est plus petite que la vitesse, on le place sur la cible, sinon il se déplace de 
        /// sa vitesse vers elle.
        /// </summary>
        public override void Move()
        {
			//Il ne peux pas se déplacer en combat
            if (this.isInCombat == false)
            {
                //Recherche de la distance
                float distanceX = target.Position.X - position.X;
                float distanceY = target.Position.Y - position.Y;
                float distance = (float) Math.Sqrt(distanceX*distanceX + distanceY*distanceY);

                //Si la distance entre l'acteur et sa cible est plus petite que la vitesse, on le place sur la cible

                if (distance <3)
                {
                    return;
                }
                
				//Ce if lui permet de "rentrer dans la tour ou dans une foret"
                if (enemyList.Contains(this.target))
                {
                    if (distance < movementSpeed)
                    {
                        Deplacement(distanceX, distanceY);
                        return;
                    }
                }

                //Sinon on calcul l'angle d'orientation de l'acteur vers la cible
                rotation = (float) Math.Atan((target.Position.Y - position.Y)/(target.Position.X - position.X));

                //Nécessaire car si la clible est plus petite en x, l'acteur fera dos à la cible
                if (target.Position.X < position.X)
                {
                    rotation += (float) (Math.PI);
                }
                ajustSpeedInForest();
                //On déplace le personnage
                Deplacement(movementSpeed*(float) Math.Cos(rotation), movementSpeed*(float) Math.Sin(rotation));
            }
        }

		//TODO: Mettre deplacement private
		
        /// <summary>
        /// Déplacements de l'acteur.  On le fait à l'intérieur du cadre de ses limites (les limites nulles ne sont pas encore codées)
        /// On déplace aussi la boite et la sphère de collision, qui ne suivent pas automatiquement
        /// 
        /// Méthode toujours utilisé par mouvement.
        /// </summary>
        /// <param name="deplacementX">Le déplacement selon l'axe des X</param>
        /// <param name="deplacementY">Le déplacement selon l'axe des Y</param>
        public void Deplacement(float deplacementX, float deplacementY)
        {
            //Déplacement à l'intérieur des limites en X
            if (position.X + deplacementX < upperLeftLimit.X)
            {
                position.X = upperLeftLimit.X;
            }
            else if (position.X + deplacementX > lowerRightLimit.X)
            {
                this.position.X = lowerRightLimit.X;
            }
            else
            {
                this.position.X += deplacementX;
            }

            //Déplacement à l'intérieur des limites en Y
            if (this.position.Y + deplacementY < upperLeftLimit.Y)
            {
                this.position.Y = upperLeftLimit.Y;
            }
            else if (this.position.Y + deplacementY > lowerRightLimit.Y)
            {
                this.position.Y = lowerRightLimit.Y;
            }
            else
            {
                this.position.Y += deplacementY;
            }

            //Déplacement de la boite de collision
            colisionBox.Min.X = position.X - centrageDuPivot.X;
            colisionBox.Min.Y = position.Y - centrageDuPivot.Y;
            colisionBox.Max.X = position.X + centrageDuPivot.X;
            colisionBox.Max.Y = position.Y + centrageDuPivot.Y;

            //Déplacement de la sphère de collision
            colisionSphere.Center.X = position.X;
            colisionSphere.Center.Y = position.Y;
        }

        private void ajustSpeedInForest()
        {
            if (checkIfIsInForest())
            {
                movementSpeed = 0.25f;
            }

            else
            {
                movementSpeed = 1;
            }
        }

        public bool checkIfIsInForest()
        {
			//Un for serait probablement plus performant, mais pour une raison inconnue avec un for il ne detecte rien jamais
            if (this.colisionBox.Intersects(forest[0].ColisionBox))
            {
                return true;
            }

            else if (this.colisionBox.Intersects(forest[1].ColisionBox))
            {
                return true;
            }

            else if (this.colisionBox.Intersects(forest[2].ColisionBox))
            {
                return true;
            }

            else if (this.colisionBox.Intersects(forest[3].ColisionBox))
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        public override void recevingDamage(float damage, Actor attacker)
        {
            if (!(this.CurrentState is SafetyState))
            {
                inflictingHpDamage(damage);
                inflictingMoraleDamage(damage);
				
				if (this.CurrentRulesOfEngagement == RulesOfEngagement.Defensive)
				{               
                changingTarget(attacker);				
				}
            }
        }

       

        public void heal(double amount)
        {
            if (hp+amount < maxHp)
            {
                this.hp += (float) amount;
            }
            this.morale += (float) amount;
        }

       

        private void changingTarget(Actor newTarget)
        {
                this.target = newTarget;
        }

        private void inflictingHpDamage(float damage)
        {
            float damageToInflict = damage;
            if (checkIfIsInForest())
            {
                damageToInflict = (float)(damageToInflict * 0.5);
            }

            if (CurrentRulesOfEngagement == RulesOfEngagement.FeelNoPain)
            {
                damageToInflict = damageToInflict * 2;
            }          
            hp -= damageToInflict;
        }

        private void inflictingMoraleDamage(float damage)
        {
            float damages = (float) (damage);
            morale -= damages;
        }

        public Actor getClosestForest()
        {
            int closestForest = 0;
            float distanceToClosestForest = 100000;

            for (int i = 0; i < forest.Length; i++)
            {
                float distanceX = forest[i].Position.X - position.X;
                float distanceY = forest[i].Position.Y - position.Y;
                float distance = (float)Math.Sqrt(distanceX * distanceX + distanceY * distanceY);
                if (distance < distanceToClosestForest)
                {
                    distanceToClosestForest = distance;
                    closestForest = i;
                }
            }

            return forest[closestForest];
        }

        

        private float getDistanceWithEnemyBase()
        {

            float distanceX = enemyList[0].Position.X - position.X;
            float distanceY = enemyList[0].Position.Y - position.Y;
            float distance = (float)Math.Sqrt(distanceX * distanceX + distanceY * distanceY);
            return distance;
        }

        public Actor getClosestEnemyActor()
        {
             int closestEnemy = 0;
            float distanceToClosestEnemy = getDistanceWithEnemyBase();

            for (int i = 0; i < enemyList.LongCount(); i++)
            {
                float distanceX = enemyList[i].Position.X - position.X;
                float distanceY = enemyList[i].Position.Y - position.Y;
                float distance = (float)Math.Sqrt(distanceX * distanceX + distanceY * distanceY);
                if (distance < distanceToClosestEnemy)
                {
                    distanceToClosestEnemy = distance;
                    closestEnemy = i;
                }
            }

            return enemyList[closestEnemy];
        }

       

        public float getDistanceBetweenTarget()
        {
            float distanceX = target.Position.X - position.X;
            float distanceY = target.Position.Y - position.Y;
           return (float)Math.Sqrt(distanceX * distanceX + distanceY * distanceY);
        }
      }

    }

