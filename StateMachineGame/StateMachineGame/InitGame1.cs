using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using StateMachineGame.Actors;
using StateMachineGame.Actors.Footman;
using StateMachineGame.Actors.Tank;
using StateMachineGame.Actors.Walker;

namespace StateMachineGame
{
    /// <summary>
    /// L'équivalent du main pour un jeu XNA
    /// 
    /// Normalement, toutes les classes "systèmes" de XNA sont dans un seul et même fichier
    /// Mais j'aime bien séparer tout ce qui a trait à l'initialisation du jeu de la 
    /// boucle de jeu
    /// </summary>
    public partial class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont mainFont;
        
        private Vector2 offSetHp = new Vector2(10,10);
        private Vector2 offSetState = new Vector2(10,-5);
        public bool gameIsStarted = false;
        

        //Modifier ces constantes pour changer la résolution de votre fenêtre de jeu
        //ceci dit, par soucis de rapidité et de simplicité, les sprites ont été pensés pour cette résolution
        const int SCREEN_WIDTH = 1280;
        const int SCREEN_HEIGHT = 720;

        //Les images de fond, le champ et les quatres petites forêts
        Objet2D battlefield;
        private Objet2D titleScreen;
        Actor[] forest = new Actor[4];
        public Song emperorVictory;
        public Song intro;
        List<Actor> marineActorsList = new List<Actor>();
        List<Actor> orksActorsList = new List<Actor>();       
        //La texture des soldiers sera static dans chaque classe directement


        //Nombre de cycles update-draw avant de créer un nouveau personnage
        //(60 cycles =  une seconde)
        int gameTick = 0;
        const int SPAWN_DELAY = 300;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Initialise
        /// </summary>
        protected override void Initialize()
        {
            InitGraphicsMode(SCREEN_WIDTH, SCREEN_HEIGHT, false);
            base.Initialize();
        }

        private bool InitGraphicsMode(int width, int height, bool fullScreen)
        {
            // Si on est pas en plein écran, la taille de la fenêtre peut
            // être de n'importe qulle taille plus petite que la surface de l'écran
            if (fullScreen == false)
            {
                if ((width <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width)
                    && (height <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height))
                {
                    graphics.PreferredBackBufferWidth = width;
                    graphics.PreferredBackBufferHeight = height;
                    graphics.IsFullScreen = fullScreen;
                    graphics.ApplyChanges();
                    return true;
                }
            }
            else
            {
                //En plein écran il faut que la résolution qu'on esssai de prendre soit supportée
                foreach (DisplayMode dm in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
                {
                    //Si le format est supporté, on fait l'ajustement à l'écran et on retourne vrai.
                    if ((dm.Width == width) && (dm.Height == height))
                    {
                        graphics.PreferredBackBufferWidth = width;
                        graphics.PreferredBackBufferHeight = height;
                        graphics.IsFullScreen = fullScreen;
                        graphics.ApplyChanges();
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Changement de la font, pour l'écriture
            mainFont = Content.Load<SpriteFont>("Fonts\\FontPrincipale");

            //Champ de bataille: le fond et title screen
            titleScreen = new Objet2D(Content.Load<Texture2D>("sprites\\titleScreen"),Vector2.Zero);
            battlefield = new Objet2D(Content.Load<Texture2D>("Sprites\\neige"), Vector2.Zero);

            //Charge la foret dans la liste.
            Texture2D imageForest = Content.Load<Texture2D>("Sprites\\forest");
            forest[0] = new Actor(imageForest, new Vector2(300, 100),0,0);
            forest[1] = new Actor(imageForest, new Vector2(300, SCREEN_HEIGHT - imageForest.Height - 100), 0, 0);
            forest[2] = new Actor(imageForest, new Vector2(SCREEN_WIDTH - imageForest.Width - 300, 100), 0, 0);
            forest[3] = new Actor(imageForest, new Vector2(SCREEN_WIDTH - imageForest.Width - 300, SCREEN_HEIGHT - imageForest.Height - 100), 0, 0);

			//Charge les deux musiques
            emperorVictory = Content.Load<Song>("songs\\game_music");
            intro = Content.Load<Song>("songs\\title_screen_music");

			//Ici on charge les texture des actors
            Texture2D imageTour = Content.Load<Texture2D>("Sprites\\Tour");
            Marine.MarineTexture = Content.Load<Texture2D>("Sprites\\marine");
            Dreadnought.DreadnoughTexture = Content.Load<Texture2D>("Sprites\\dreadnought");
            Landraider.LandraiderTexture =Content.Load<Texture2D>("Sprites\\landraider");
            Boyz.boyzTexture = Content.Load<Texture2D>("Sprites\\boyz");
            Nobz.NobzTexture = Content.Load<Texture2D>("Sprites\\nobz");
            Loottruck.LoottruckTexture = Content.Load<Texture2D>("Sprites\\truck");

            //Ajout des tours dans leur liste respective
            marineActorsList.Add(new Actor(imageTour, new Vector2(50, SCREEN_HEIGHT / 2), 0, 1000));
            orksActorsList.Add(new Actor(imageTour, new Vector2(SCREEN_WIDTH - 50, SCREEN_HEIGHT / 2), 0, 1000));

			//Séparation de certaines génération pour mieux lire.
            generateTitleScreenMusic();
            GenererPersos();            
        }

        private void GenererPersos()
        {
            Random r =  new Random();

            int baseX = Marine.MarineTexture.Width / 2;
            int baseY =  Marine.MarineTexture.Height / 2;
            int limiteX = (SCREEN_WIDTH / 2) -  Marine.MarineTexture.Width;
            int limiteY = SCREEN_HEIGHT - Marine.MarineTexture.Height;

			//Generation des soldiers selon leur type et allignement
#region
            for (int i = 0; i<15;i++)
            {
                marineActorsList.Add(SoldierFabric.creatSoldier(SoldierFabric.soldierType.marine,new Vector2(baseX + r.Next(limiteX), baseY + r.Next(limiteY)), orksActorsList, forest,marineActorsList[0]));
            }

            for (int i = 0; i < 60; i++)
            {
                orksActorsList.Add(SoldierFabric.creatSoldier(SoldierFabric.soldierType.boyz, new Vector2(SCREEN_WIDTH / 2 + baseX + r.Next(limiteX), baseY + r.Next(limiteY)), marineActorsList, forest, orksActorsList[0]));
  
            }

           int j = r.Next(2, 4) + 1;

            for (int i = 0; i < j; i++)
            {
                marineActorsList.Add(SoldierFabric.creatSoldier(SoldierFabric.soldierType.dreadnought, new Vector2(baseX + r.Next(limiteX), baseY + r.Next(limiteY)), orksActorsList, forest,
                    marineActorsList[0]));

                orksActorsList.Add(SoldierFabric.creatSoldier(SoldierFabric.soldierType.nobz, new Vector2(SCREEN_WIDTH / 2 + baseX + r.Next(limiteX), baseY + r.Next(limiteY)), marineActorsList, forest, orksActorsList[0]));           
                orksActorsList.Add(SoldierFabric.creatSoldier(SoldierFabric.soldierType.nobz, new Vector2(SCREEN_WIDTH / 2 + baseX + r.Next(limiteX), baseY + r.Next(limiteY)), marineActorsList, forest, orksActorsList[0]));
            }

            int k = r.Next(1, 2) + 1;

            for (int i = 0; i < k; i++)
            {
                marineActorsList.Add(SoldierFabric.creatSoldier(SoldierFabric.soldierType.landraider, new Vector2(baseX + r.Next(limiteX), baseY + r.Next(limiteY)), orksActorsList, forest,
                    marineActorsList[0]));

                orksActorsList.Add(SoldierFabric.creatSoldier(SoldierFabric.soldierType.loottruck, new Vector2(SCREEN_WIDTH / 2 + baseX + r.Next(limiteX), baseY + r.Next(limiteY)), marineActorsList, forest, orksActorsList[0]));
                orksActorsList.Add(SoldierFabric.creatSoldier(SoldierFabric.soldierType.loottruck, new Vector2(SCREEN_WIDTH / 2 + baseX + r.Next(limiteX), baseY + r.Next(limiteY)), marineActorsList, forest, orksActorsList[0]));
           
            }
			#endregion
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {

        }

		//Genere et lance la musique d'intro
        private void generateTitleScreenMusic()
        {
            if (!gameIsStarted)
            {
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Play(intro);
            }
        }
    }
}
