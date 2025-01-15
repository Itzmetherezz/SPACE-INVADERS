using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SPACE_INVADER
{
    public partial class MainWindow : Window
    {
        bool goLeft, goRight;

        List<Rectangle> itemsToRemove = new List<Rectangle>();

        int enemyimages = 0;
        int bulletTimer = 0;
        int bulletTimerLimit = 50;
        int totalEnemies = 0;
        int enemySpeed = 8;
        bool gameOver = false;

        DispatcherTimer gameTimer = new DispatcherTimer();
        ImageBrush playerSkin = new ImageBrush();

        public MainWindow()
        {
            InitializeComponent();
            gameTimer.Tick += GameLoop;
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            gameTimer.Start();

            playerSkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/ASSETS/ship_6.png"));
            player.Fill = playerSkin;

            ImageBrush backgroundBrush = new ImageBrush();
            backgroundBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/ASSETS/hi.jpg"));
            MYCANVAS.Background = backgroundBrush;

            MYCANVAS.Focus();

            makeEnemies(30);
            VICTORYSCREEN.Visibility= Visibility.Collapsed;
            
        }

        private void GameLoop(object? sender, EventArgs e)
        {
            Rect playerHitBox = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Height, player.Width);

            InvadersLeft.Content = "Enemies Left: " + totalEnemies;
            if (goLeft == true && Canvas.GetLeft(player) > 0)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) - 10);
            }
            if (goRight == true && Canvas.GetLeft(player) + 80 < Application.Current.MainWindow.Width)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) + 10);
            }

            bulletTimer -= 3;

            if (bulletTimer < 0)
            {
                enemyBulletMaker(Canvas.GetLeft(player) + 20, 10);

                bulletTimer = bulletTimerLimit;
            }

            foreach (var x in MYCANVAS.Children.OfType<Rectangle>())
            {
                if (x is Rectangle && (string)x.Tag == "bullet")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) - 20);

                    if (Canvas.GetTop(x) < 10)
                    {
                        itemsToRemove.Add(x);
                    }
                    Rect bullet = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                    foreach (var y in MYCANVAS.Children.OfType<Rectangle>())
                    {
                        if (y is Rectangle && (string)y.Tag == "enemy")
                        {

                            Rect enemyHit = new Rect(Canvas.GetLeft(y), Canvas.GetTop(y), y.Width, y.Height);

                            if (bullet.IntersectsWith(enemyHit))
                            {
                                itemsToRemove.Add(x);
                                itemsToRemove.Add(y);
                                totalEnemies -= 1;

                                 
                            }
                        }
                    }


                      

                }

                
                if (x is Rectangle && (string)x.Tag == "enemy")
                {
                    Canvas.SetLeft(x, Canvas.GetLeft(x) + enemySpeed);

                    if (Canvas.GetLeft(x) > 820)
                    {
                        Canvas.SetLeft(x, -80);
                        Canvas.SetTop(x, Canvas.GetTop(x) + (x.Height + 10));

                    }
                    Rect enemyHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    if (playerHitBox.IntersectsWith(enemyHitBox))
                    {
                        showGameOver("You were killed by the invaders!!");
                    }

                }
                if (x is Rectangle && (string)x.Tag == "enemyBullet")
                {

                    Canvas.SetTop(x, Canvas.GetTop(x) + 10);

                    if(Canvas.GetTop(x) > 480)
                    {
                        itemsToRemove.Add(x);
                    }
                    Rect enemyBulletHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    if (playerHitBox.IntersectsWith(enemyBulletHitBox))
                    {
                        showGameOver("You were killed by the invaders!!");
                    }
                }
            }
            foreach (Rectangle i in itemsToRemove)
            {
                MYCANVAS.Children.Remove(i);
            }

            if (totalEnemies<30)
            {
                enemySpeed = 10;
            }
            if (totalEnemies<20)
            {
                enemySpeed = 12;
            }

            if (totalEnemies<10)
            {
                enemySpeed = 13;
            }

            if (totalEnemies < 1)
            {
                VICTORYSCREEN.Visibility= Visibility.Visible;
                gameTimer.Stop();
            }
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Left)
            {
                goLeft = true;
            }
            if(e.Key == Key.Right)
            {
                goRight = true;
            }
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                goLeft = false;
            }
            if (e.Key == Key.Right)
            {
                goRight = false;
            }

            if(e.Key == Key.Space)
            {
                Rectangle newBullet = new Rectangle
                {
                    Tag = "bullet",
                    Height = 20,
                    Width = 5,
                    Fill = Brushes.White,
                    Stroke = Brushes.Red,

                };
                Canvas.SetTop(newBullet, Canvas.GetTop(player)- newBullet.Height);
                Canvas.SetLeft(newBullet, Canvas.GetLeft(player) + player.Width / 2);

                MYCANVAS.Children.Add(newBullet);
            }
            if (e.Key == Key.Enter && gameOver == true)
            {
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }
        }

        private void enemyBulletMaker(double x, double y)
        {

            Rectangle enemyBullet = new Rectangle
            {
                Tag = "enemyBullet",
                Height = 34,
                Width = 7,
                Fill = Brushes.Red,
                Stroke = Brushes.Black,
                StrokeThickness = 2,

            };
            Canvas.SetTop(enemyBullet, y);
            Canvas.SetLeft(enemyBullet, x);

            MYCANVAS.Children.Add(enemyBullet);

        }

        private void makeEnemies(int limit)
        {
            int left = 0;

            totalEnemies = limit;

            for (int i = 0; i < limit; i++)
            {

                ImageBrush enemySkin = new ImageBrush();
                Rectangle newEnemy = new Rectangle
                {
                    Tag = "enemy",
                    Height = 45,
                    Width = 45,
                    Fill = enemySkin
                };
                Canvas.SetTop(newEnemy, 30);
                Canvas.SetLeft(newEnemy, left);
                MYCANVAS.Children.Add(newEnemy);
                left -= 60;

                enemyimages++;

                if(enemyimages>5)
                {
                    enemyimages = 1;
                }

                switch (enemyimages)
                {
                    case 1:
                        enemySkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/ASSETS/ship_1.png"));
                        break;
                    case 2:
                        enemySkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/ASSETS/ship_2.png"));
                        break;
                    case 3:
                        enemySkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/ASSETS/ship_3.png"));
                        break;
                    case 4:
                        enemySkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/ASSETS/ship_4.png"));
                        break;
                    case 5:
                        enemySkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/ASSETS/ship_5.png"));
                        break;  
                    
                }

                

            }

        }

        private void showGameOver(string msg)

        {

            gameOver = true;
            gameTimer.Stop();
            GameEnd.Visibility= Visibility.Visible;

        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            ResetGame();
        }

        private void ResetGame()
        {
            
            goLeft = false;
            goRight = false;
            itemsToRemove.Clear();
            enemyimages = 0;
            bulletTimer = 0;
            totalEnemies = 0;
            enemySpeed = 8;
            gameOver = false;

           
            MYCANVAS.Children.Clear();

            
            Canvas.SetLeft(player, 100);
            Canvas.SetTop(player, 400);
            MYCANVAS.Children.Add(player);

           
            ImageBrush backgroundBrush = new ImageBrush();
            backgroundBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/ASSETS/hi.jpg"));
            MYCANVAS.Background = backgroundBrush;

          
            GameEnd.Visibility = Visibility.Hidden;

            
            makeEnemies(30);

            
            gameTimer.Start();

            MYCANVAS.Focus();

            
        }
    }
}