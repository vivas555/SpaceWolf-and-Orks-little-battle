using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StateMachineGame.Actors;

namespace StateMachineGame
{
    abstract class State
    {
        protected Soldier soldier;
        private string name ;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        protected State(Soldier soldier)
        {
            this.soldier = soldier;
        }

        public abstract void update(int gameTick);
        protected abstract void setNextState();
    }


    class NormalState: State
    {
        public NormalState(Soldier soldier) : base(soldier)
        {
            this.soldier = soldier;
            Name = "Normale";
            this.soldier.CurrentRulesOfEngagement1 = RulesOfEngagement.Defensive;
        }

        Random r = new Random();
        public override void update(int gameTick)
        {
            if (soldier.getClosestEnemyActor() != soldier.Target)
            {
                soldier.Target = soldier.getClosestEnemyActor();
            }

            if (gameTick%30 == 0)
            {
                if (soldier.getDistanceBetweenTarget() <= soldier.Range)
                {
                    float damageDeal = r.Next((int) soldier.MinDamage, (int) soldier.MaxDamage);

                    soldier.Target.recevingDamage(damageDeal, this.soldier);
                    this.soldier.Morale += damageDeal;
                    this.soldier.IsInCombat = true;
                }
                else
                {
                    this.soldier.IsInCombat = false;
                    if (this.soldier.Hp < this.soldier.maxHp)
                                    {
                                       this.soldier.heal(0.5);
                                    }

                }
                
            }
            setNextState();
        }

		//Important de garder l'ordre pour les changements de states
        protected override void setNextState()
        {

            if (this.soldier.Hp <= 0)
            {
                this.soldier.CurrentState = new DeadState(this.soldier);
                this.soldier = null;
            }
         
            else if (this.soldier.Morale > 600)
            {
                this.soldier.CurrentState = new ConfidentState(this.soldier);
                this.soldier = null;
            }
            else  if (this.soldier.Hp < this.soldier.maxHp*0.2)
            {
                this.soldier.CurrentState = new DyingState(this.soldier);
                this.soldier = null;
            }

            else  if (this.soldier.Morale < (this.soldier.baseMorale/4))
            {
                this.soldier.CurrentState = new RoutingStateforForest(this.soldier);
                this.soldier = null;
            }
            
        }
    }

    class ConfidentState:State
    {
        Random r = new Random();

        public ConfidentState(Soldier soldier) : base(soldier)
        {
            this.soldier = soldier;
            Name = "Confident";
            this.soldier.CurrentRulesOfEngagement1 = RulesOfEngagement.Attack;
        }

        public override void update(int gameTick)
        {
            if (soldier.getClosestEnemyActor() != soldier.Target)
            {
                soldier.Target = soldier.getClosestEnemyActor();
            }

            if (gameTick%30 == 0)
            {
                
                this.soldier.heal(0.5);
                
                if (soldier.getDistanceBetweenTarget() <= soldier.Range)
                {
                    float damageDeal = r.Next((int) soldier.MinDamage, (int) soldier.MaxDamage);

                    soldier.Target.recevingDamage(damageDeal, this.soldier);
                    this.soldier.Morale += damageDeal;
                    this.soldier.IsInCombat = true;
                }
                else
                {
                    this.soldier.IsInCombat = false;
                }                  
            }
            setNextState();
        }

        protected override void setNextState()
        {

            if (this.soldier.Hp <= 0)
            {
                this.soldier.CurrentState = new DeadState(this.soldier);
                this.soldier = null;
            }            
            
            else if (this.soldier.Hp < this.soldier.maxHp * 0.2)
            {
                this.soldier.CurrentState = new DyingState(this.soldier);
                this.soldier = null;
            }
            else if (this.soldier.Morale < (this.soldier.baseMorale / 4))
            {
                this.soldier.CurrentState = new RoutingStateforForest(this.soldier);
                this.soldier = null;
            }
            
        }

        
    }

	
	//Deux state différentes pour quand le morale diminue trop. Une pour se sauver d'abord dans la foret
	//L'autre pour aller dans sa tour. Ça facilite le code ainsi que la logique
    class RoutingStateforForest: State
    {

        public RoutingStateforForest(Soldier soldier) : base(soldier)
        {
            this.soldier = soldier;
            Name = "Routing for forest";
        }

        public override void update(int gameTick)
        {
            if (this.soldier.Target != soldier.getClosestForest())
            {
                this.soldier.Target = soldier.getClosestForest();
            }

            setNextState();
        }

        protected override void setNextState()
        {
            

            if (this.soldier.Hp <= 0)
            {
                this.soldier.CurrentState = new DeadState(this.soldier);
                this.soldier = null;
            }

            else if (this.soldier.Hp < this.soldier.maxHp * 0.2)
            {
                this.soldier.CurrentState = new DyingState(this.soldier);
                this.soldier = null;
            }

            else if (this.soldier.checkIfIsInForest())
            {
                this.soldier.CurrentState = new HiddenState(this.soldier);
            }
        }

        
    }

    class HiddenState:State
    {
        public HiddenState(Soldier soldier) : base(soldier)
        {
            this.soldier = soldier;
            Name = "Hidden";
        }

        public override void update(int gameTick)
        {
            if (gameTick % 30 == 0 && this.soldier.Hp < this.soldier.maxHp)
            {
                this.soldier.heal(5);
            }
            setNextState();
        }

        protected override void setNextState()
        {
             if (this.soldier.Hp <= 0)
            {
                this.soldier.CurrentState = new DeadState(this.soldier);
                this.soldier = null;
            }

            else if (this.soldier.Hp < this.soldier.maxHp * 0.2)
            {
                this.soldier.CurrentState = new DyingState(this.soldier);
                this.soldier = null;
            }
            
            else if (this.soldier.Morale <= this.soldier.baseMorale*0.1)
            {
                this.soldier.CurrentState = new RoutingForTowerState(this.soldier);
                this.soldier = null;
            }
            else if (this.soldier.Morale>= this.soldier.baseMorale)
            {
                this.soldier.CurrentState = new NormalState(this.soldier);
                this.soldier = null;
            }

            
        }

        
    }

	class RoutingForTowerState:State
    {
        public RoutingForTowerState(Soldier soldier) : base(soldier)
        {
            this.soldier = soldier;
            Name = "Routing for Tower";
        }

        public override void update(int gameTick)
        {
            this.soldier.Target = this.soldier.Headquarter;
            setNextState();
        }

        protected override void setNextState()
        {
            if (soldier.Position == this.soldier.Target.Position)
            {
                this.soldier.CurrentState = new SafetyState(this.soldier);
                this.soldier = null;
            }

         else if (this.soldier.Hp <= 0)
            {
                this.soldier.CurrentState = new DeadState(this.soldier);
                this.soldier = null;
            }

          else  if (this.soldier.Hp < this.soldier.maxHp * 0.2)
            {
                this.soldier.CurrentState = new DyingState(this.soldier);
                this.soldier = null;
            }

           
        }
    }
	
    class SafetyState:State
    {
        Random r = new Random();

         public SafetyState(Soldier soldier) : base(soldier)
                {
                    this.soldier = soldier;
                    Name = "Safety";
                }
        public override void update(int gameTick)
        {
            if (gameTick % 30 == 0 && this.soldier.Hp < this.soldier.maxHp)
            {

                   this.soldier.heal(2);
             
            }
            setNextState();
        }

        protected override void setNextState()
        {
            if (Math.Abs(this.soldier.Hp - this.soldier.maxHp) < 1)
            {
                this.soldier.CurrentState = new NormalState(this.soldier);
                this.soldier = null;
            }
        }

       
    }

    class DyingState:State
    {
        Random r = new Random();
        
        public DyingState(Soldier soldier) : base(soldier)
        {
            this.soldier = soldier;
            Name = "Dying";
            this.soldier.CurrentRulesOfEngagement1 = RulesOfEngagement.FeelNoPain;
        }

        public override void update(int gameTick)
        {

            if (soldier.getClosestEnemyActor() != soldier.Target)
            {
                soldier.Target = soldier.getClosestEnemyActor();
            }

            if (gameTick%30==0)
            {
                if (soldier.getDistanceBetweenTarget() <= soldier.Range)
                {
                    float damageDeal = r.Next((int) soldier.MinDamage, (int) soldier.MaxDamage);

                    soldier.Target.recevingDamage(damageDeal, this.soldier);
                    this.soldier.Morale += damageDeal;
                    this.soldier.IsInCombat = true;
                }
                else
                {
                    this.soldier.IsInCombat = false;
                }
            }
            setNextState();
        }

        protected override void setNextState()
        {
            if (this.soldier.Hp <= 0)
            {
                this.soldier.CurrentState = new DeadState(this.soldier);
                this.soldier = null;
            }
        }

        
    }

    class DeadState:State
    {
        
        public DeadState(Soldier soldier) : base(soldier)
        {
            this.soldier = soldier;
            Name = "Dead";
        }
        public override void update(int gameTick)
        {
           
        }

        protected override void setNextState()
        {
            
        }

        
    }

    
}
