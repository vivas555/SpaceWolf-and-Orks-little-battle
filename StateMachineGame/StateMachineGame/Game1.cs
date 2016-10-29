using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using StateMachineGame.Actors;
using StateMachineGame.Actors.Footman;

namespace StateMachineGame
{

    public partial class Game1 : Microsoft.Xna.Framework.Game
    {
        /// <summary>
        /// À la base, les inputs dans XNA sont gérés au début de la méthode input
        /// Mais juste pour respecter parfaitement le cycle entrées/traitement/affichage
        /// on ajoute cette méthode
        /// </summary>
      
        private void ManageInputs()
        {
            GamePadState padOneState = GamePad.GetState(PlayerIndex.One);
            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape) || (padOneState.Buttons.Back == ButtonState.Pressed))
            {
                this.Exit();
            }

            if (keyboardState.IsKeyDown(Keys.Enter))
            {
                gameIsStarted = true;
                MediaPlayer.IsRepeating = true;
				//Pas besoin d'arréter la musique d'intro
                MediaPlayer.Play(emperorVictory);
            }
        }

		
        protected override void Update(GameTime gameTime)
        {
            ManageInputs();
            if (gameIsStarted)
            {
                gameTick++;

				//Spawn nouvelles instances de soldiers
                #region

                if (gameTick == SPAWN_DELAY)
                {
                    gameTick = 0;
                    Random r = new Random();


                    int whatToSpawnForOrk = r.Next(1, 100 + 1);


                    if (whatToSpawnForOrk < 51)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            orksActorsList.Add(SoldierFabric.creatSoldier(SoldierFabric.soldierType.boyz,
                                orksActorsList[0].Position, marineActorsList, forest, orksActorsList[0]));
                        }
                    }

                    else if (whatToSpawnForOrk > 85)
                    {
                        orksActorsList.Add(SoldierFabric.creatSoldier(SoldierFabric.soldierType.loottruck,
                            orksActorsList[0].Position, marineActorsList, forest, orksActorsList[0]));

                    }

                    else
                    {
                        orksActorsList.Add(SoldierFabric.creatSoldier(SoldierFabric.soldierType.nobz,
                            orksActorsList[0].Position, marineActorsList, forest, orksActorsList[0]));
                        orksActorsList.Add(SoldierFabric.creatSoldier(SoldierFabric.soldierType.nobz,
                            orksActorsList[0].Position, marineActorsList, forest, orksActorsList[0]));

                    }



                    int whatToSpawnForMarine = r.Next(1, 100 + 1);

                    if (whatToSpawnForMarine < 51)
                    {

                        marineActorsList.Add(SoldierFabric.creatSoldier(SoldierFabric.soldierType.marine,
                            marineActorsList[0].Position, orksActorsList, forest, marineActorsList[0]));
                    }

                    else if (whatToSpawnForMarine > 85)
                    {
                        marineActorsList.Add(SoldierFabric.creatSoldier(SoldierFabric.soldierType.landraider,
                            marineActorsList[0].Position, orksActorsList, forest, marineActorsList[0]));

                    }

                    else
                    {
                        marineActorsList.Add(SoldierFabric.creatSoldier(SoldierFabric.soldierType.dreadnought,
                            marineActorsList[0].Position, orksActorsList, forest, marineActorsList[0]));

                    }

                }

                #endregion

				#region
					//update les soldiers en jeu
                    for (int i = 1; i < marineActorsList.LongCount(); i++)
                    {
                        marineActorsList[i].CurrentState.update(gameTick);
                    }
                    for (int i = 1; i < orksActorsList.LongCount(); i++)
                    {
                        orksActorsList[i].CurrentState.update(gameTick);
                    }
                
					//deux boucles comme les listes on pas la même taille
                    for (int i = 1; i < marineActorsList.Count; i++)
                    {
                        marineActorsList[i].Move();
                        if (marineActorsList[i].CurrentState is DeadState)
                        {
                            marineActorsList.RemoveAt(i);
                        }
                    }
                    
					
                    for (int i = 1; i < orksActorsList.Count; i++)
                    {
                        orksActorsList[i].Move();
                        if (orksActorsList[i].CurrentState is DeadState)
                        {
                            orksActorsList.RemoveAt(i);
                        }
                    }
					
				#endregion
						//retire les morts
                    if (orksActorsList[0].Hp <= 0 || marineActorsList[0].Hp <= 0)
                    {
                        marineActorsList.Clear();
                        orksActorsList.Clear();
                        Exit();

                    }
                }
            
            base.Update(gameTime);
        }

        /// <summary>
        /// Dessine les acteurs en jeu
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        ///<remarks>
        /// Remarquez que chaque objet 2D du projet a la responsabilité de se dessiner soit même
        /// à l'aide du spritebatch.  Ce qui est dessiné à la fin est dessiné par dessus ce qui fut dessiné avant
        /// </remarks>
        protected override void Draw(GameTime gameTime)
        {
                   
                GraphicsDevice.Clear(Color.LawnGreen);

                spriteBatch.Begin(); 
            
            if (!gameIsStarted)
            {
                titleScreen.Dessiner(spriteBatch);
            }
            if (gameIsStarted)
            {
                battlefield.Dessiner(spriteBatch);

                for (int i = 0; i < 4; i++)
                {
                    forest[i].Dessiner(spriteBatch);
                }


                for (int i = 1; i < marineActorsList.Count; i++)
                {
                    marineActorsList[i].Dessiner(spriteBatch);
                }

                for (int i = 1; i < orksActorsList.Count; i++)
                {
                    orksActorsList[i].Dessiner(spriteBatch);
                }

                marineActorsList[0].Dessiner(spriteBatch);
                orksActorsList[0].Dessiner(spriteBatch);
	
				//Dessine la state ainsi que la vie des soldiers
                spriteBatch.DrawString(mainFont, orksActorsList[0].Hp.ToString(), orksActorsList[0].Position,
                    Color.Black);
                spriteBatch.DrawString(mainFont, marineActorsList[0].Hp.ToString(), marineActorsList[0].Position,
                    Color.Black);
                for (int i = 1; i < orksActorsList.Count; i++)
                {
                    spriteBatch.DrawString(mainFont, orksActorsList[i].Hp.ToString(),
                        orksActorsList[i].Position + offSetHp, Color.Black);
                    spriteBatch.DrawString(mainFont, orksActorsList[i].CurrentState.Name,
                        orksActorsList[i].Position + offSetState, Color.Black);
                }

                for (int i = 1; i < marineActorsList.Count; i++)
                {
                    spriteBatch.DrawString(mainFont, marineActorsList[i].Hp.ToString(),
                        marineActorsList[i].Position + offSetHp, Color.Black);
                    spriteBatch.DrawString(mainFont, marineActorsList[i].CurrentState.Name,
                        marineActorsList[i].Position + offSetState, Color.Black);

                }

               
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
