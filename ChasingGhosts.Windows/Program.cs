using System;

using ChasingGhosts.Windows.Scenes;

using Microsoft.Xna.Framework;

using Sharp2D.Engine.Common;
using Sharp2D.Engine.Infrastructure;
using Sharp2D.Windows;

namespace ChasingGhosts.Windows
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var app = new MainApp(new NinjectResolver());
            app.Start();
        }
    }

    public class MainApp : Sharp2DWindowsApp
    {
        public MainApp(IResolver resolver)
            : base(resolver)
        {

        }

        public override IGameHost CreateGame(SharpGameManager gameManager)
        {
            gameManager.BackgroundColor = Color.Gray;
            var game = base.CreateGame(gameManager);
            SharpGameManager.ContentLoaded += (sender, args) => gameManager.StartScene = new GameScene(this.Resolver);
            return game;
        }

        public override void SetupGraphics()
        {
            GameManager.SetVirtualResolution(1280, 800);
            GameManager.SetWindowResolution(1280, 800);
            GameManager.ApplyResolutionChanges();
        }
    }
#endif
}
