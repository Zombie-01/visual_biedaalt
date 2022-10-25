using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Tetris
{
    public partial class MainWindow : Form
    {
        Control[] activePiece = { null, null, null, null };
        Control[] activePiece2 = { null, null, null, null };
        Control[] nextPiece = { null, null, null, null };
        Control[] savedPiece = { null, null, null, null };
        Control[] Ghost = { null, null, null, null };
        List<int> PieceSequence = new List<int>();
        int timeElapsed = 0;
        int currentPiece;
        int nextPieceInt;
        int savedPieceInt = -1;
        int rotations = 0;
        Color pieceColor = Color.White;
        Color savedPieceColor = Color.White;
        int combo = 0;
        int score = 0;
        int clears = 0;
        int level = 0;
        bool gameOver = false;
        int PieceSequenceIteration = 0;

        readonly Color[] colorList = 
        {  
            Color.Cyan,     // I piece
            Color.Orange,   // L piece
            Color.Blue,     // J piece
            Color.Green,    // S piece
            Color.Red,      // Z piece
            Color.Yellow,   // O piece
            Color.Purple    // T piece
        };

        public MainWindow()      
        {
            InitializeComponent();

            ScoreUpdateLabel.Text = "";
            SpeedTimer.Start();
            GameTimer.Start();

            activePiece2[0] = box1;
            activePiece2[1] = box2;
            activePiece2[2] = box3;
            activePiece2[3] = box4;

            System.Random random = new System.Random();
            while (PieceSequence.Count < 7)
            {
                int x = random.Next(7);
                if (!PieceSequence.Contains(x))
                {
                    PieceSequence.Add(x);
                }
            }

            nextPieceInt = PieceSequence[0];
            PieceSequenceIteration++;

            DropNewPiece();
        }

        public void DropNewPiece()
        {
            rotations = 0;

            currentPiece = nextPieceInt;

            if (PieceSequenceIteration == 7)
            {
                PieceSequenceIteration = 0;

                PieceSequence.Clear();
                System.Random random = new System.Random();
                while (PieceSequence.Count < 7)
                {
                    int x = random.Next(7);
                    if (!PieceSequence.Contains(x))
                    {
                        PieceSequence.Add(x);
                    }
                }
            }
            nextPieceInt = PieceSequence[PieceSequenceIteration];
            PieceSequenceIteration++;
            if (nextPiece.Contains(null) == false)
            {
                foreach (Control x in nextPiece)
                {
                    x.BackColor = Color.White;
                }
            }

            Control[,] nextPieceArray = 
            {
                { box203, box207, box211, box215 }, // I piece
                { box202, box206, box210, box211 }, // L piece
                { box203, box207, box211, box210 }, // J piece
                { box206, box207, box203, box204 }, // S piece
                { box202, box203, box207, box208 }, // Z piece
                { box206, box207, box210, box211 }, // O piece
                { box207, box210, box211, box212 }  // T piece
            };

            // Retrieve layout for next piece
            for (int x = 0; x < 4; x++)
            {
                nextPiece[x] = nextPieceArray[nextPieceInt,x];
            }

            foreach (Control square in nextPiece)
            {
                square.BackColor = colorList[nextPieceInt];
            }

            Control[,] activePieceArray =
            {
                { box6, box16, box26, box36 }, // I piece
                { box4, box14, box24, box25 }, // L piece
                { box5, box15, box25, box24 }, // J piece
                { box14, box15, box5, box6 },  // S piece
                { box5, box6, box16, box17 },  // Z piece
                { box5, box6, box15, box16 },  // O piece
                { box6, box15, box16, box17 }  // T piece
            };

            for (int x = 0; x < 4; x++)
            {
                activePiece[x] = activePieceArray[currentPiece, x];
            }

            for (int x = 0; x < 4; x++)
            {
                activePiece2[x] = activePieceArray[currentPiece, x];
            }

            foreach (Control box in activePiece)
            {
                if (box.BackColor != Color.White & box.BackColor != Color.LightGray)
                {
                    SpeedTimer.Stop();
                    GameTimer.Stop();
                    gameOver = true;
                    MessageBox.Show("Game over!");
                    return;
                }
            }

            DrawGhost();

            foreach (Control square in activePiece)
            {
                square.BackColor = colorList[currentPiece];
            }
        }

        public bool TestMove(string direction)
        {
            int currentHighRow = 21;
            int currentLowRow = 0;
            int currentLeftCol = 9;
            int currentRightCol = 0;

            int nextSquare = 0;

            Control newSquare = new Control();

            foreach (Control square in activePiece)
            {
                if (grid.GetRow(square) < currentHighRow)
                {
                    currentHighRow = grid.GetRow(square);
                }
                if (grid.GetRow(square) > currentLowRow)
                {
                    currentLowRow = grid.GetRow(square);
                }
                if (grid.GetColumn(square) < currentLeftCol)
                {
                    currentLeftCol = grid.GetColumn(square);
                }
                if (grid.GetColumn(square) > currentRightCol)
                {
                    currentRightCol = grid.GetColumn(square);
                }
            }

            foreach (Control square in activePiece)
            {
                int squareRow = grid.GetRow(square);
                int squareCol = grid.GetColumn(square);

                if (direction == "left" & squareCol > 0)
                {
                    newSquare = grid.GetControlFromPosition(squareCol - 1, squareRow);
                    nextSquare = currentLeftCol;
                }
                else if (direction == "left" & squareCol == 0)
                {
                    return false;
                }

                else if (direction == "right" & squareCol < 9)
                {
                    newSquare = grid.GetControlFromPosition(squareCol + 1, squareRow);
                    nextSquare = currentRightCol;
                }
                else if (direction == "right" & squareCol == 9)
                {
                    return false;
                }

                else if (direction == "down" & squareRow < 21)
                {
                    newSquare = grid.GetControlFromPosition(squareCol, squareRow + 1);
                    nextSquare = currentLowRow;
                }
                else if (direction == "down" & squareRow == 21)
                {
                    return false;
                }

                if ((newSquare.BackColor != Color.White & newSquare.BackColor != Color.LightGray) & activePiece.Contains(newSquare) == false & nextSquare > 0)
                {
                    return false;
                }

            }

            return true;
        }

        public void MovePiece(string direction)
        {
            
            int x = 0;
            foreach (PictureBox square in activePiece)
            {
                square.BackColor = Color.White;
                int squareRow = grid.GetRow(square);
                int squareCol = grid.GetColumn(square);
                int newSquareRow = 0;
                int newSquareCol = 0;
                if (direction == "left")
                {
                    newSquareCol = squareCol - 1;
                    newSquareRow = squareRow;
                }
                else if (direction == "right")
                {
                    newSquareCol = squareCol + 1;
                    newSquareRow = squareRow;
                }
                else if (direction == "down")
                {
                    newSquareCol = squareCol;
                    newSquareRow = squareRow + 1;
                }

                activePiece2[x] = grid.GetControlFromPosition(newSquareCol, newSquareRow);
                x++;
            }

            x = 0;
            foreach (PictureBox square in activePiece2)
            {

                activePiece[x] = square;
                x++;
            }

            // сүүдэр
            DrawGhost();

            x = 0;
            foreach (PictureBox square in activePiece2)
            {
                square.BackColor = colorList[currentPiece];
                x++;
            }
        }

        private bool TestOverlap()
        {
            foreach (PictureBox square in activePiece2)
            {
                if ((square.BackColor != Color.White & square.BackColor != Color.LightGray) & activePiece.Contains(square) == false)
                {
                    return false;
                }
            }
            return true;
        }
        
        private void SpeedTimer_Tick(object sender, EventArgs e)
        {
            if (CheckGameOver() == true)
            {
                SpeedTimer.Stop();
                GameTimer.Stop();
                MessageBox.Show("Game over!");
            }

            else
            {
                
                if (TestMove("down") == true)
                {
                    MovePiece("down");
                }
                else
                {
                    if (CheckGameOver() == true)
                    {
                        SpeedTimer.Stop();
                        GameTimer.Stop();
                        MessageBox.Show("Game over!");
                    }
                    if (CheckForCompleteRows() > -1)
                    {
                        ClearFullRow();
                    }
                    DropNewPiece();
                }
            }
        }

        // хугацаа
        private void GameTimer_Tick(object sender, EventArgs e)
        {
            timeElapsed++;
            TimeLabel.Text = "Хугацаа: " + timeElapsed.ToString();
        }

        // 
        private void ClearFullRow()
        {
            int completedRow = CheckForCompleteRows();

            //цагаан багана
            for (int x = 0; x <= 9; x++)
            {
                Control z = grid.GetControlFromPosition(x, completedRow);
                z.BackColor = Color.White;
            }

            //арилгассан мөрнөөс доош нь буулгах
            for (int x = completedRow - 1; x >= 0; x--) //арилгах
            {
                for (int y = 0; y <= 9; y++)
                {
                    //квадрат
                    Control z = grid.GetControlFromPosition(y, x);

                    Control zz = grid.GetControlFromPosition(y, x + 1);

                    zz.BackColor = z.BackColor;
                    z.BackColor = Color.White;
                }
            }

            UpdateScore();

            clears++;
            ClearsLabel.Text = "Арилгасан: " + clears;

            if (clears % 10 == 0)
            {
                LevelUp();
            }

            if (CheckForCompleteRows() > -1)
            {
                ClearFullRow();
            }
        }

        private void UpdateScore()
        {

            bool skipComboReset = false;

            // 1
            if (combo == 0)
            {
                score += 100;
                ScoreUpdateLabel.Text = "+100";
            }

            // 2
            else if (combo == 1)
            {
                score += 100;
                ScoreUpdateLabel.Text = "+200";
            }

            // 3
            else if (combo == 2)
            {
                score += 100;
                ScoreUpdateLabel.Text = "+300";
            }

            // 4
            else if (combo == 3)
            {
                score += 500;
                ScoreUpdateLabel.Text = "+800";
                skipComboReset = true;
            }

            // 1 мөр арилгах
            else if (combo > 3 && combo % 4 == 0)
            {
                score += 100;
                ScoreUpdateLabel.Text = "+100";
            }

            // 2 арилгах
            else if (combo > 3 && ((combo - 1) % 4 == 0))
            {
                score += 100;
                ScoreUpdateLabel.Text = "+200";
            }

            // 3 арилгах
            else if (combo > 3 && ((combo - 2) % 4 == 0))
            {
                score += 100;
                ScoreUpdateLabel.Text = "+300";
            }
            //4 арилгах
            else if (combo > 3 && ((combo - 3) % 4 == 0))
            {
                score += 900;
                ScoreUpdateLabel.Text = "+1200";
                skipComboReset = true;
            }

            if (CheckForCompleteRows() == -1 && skipComboReset == false)
            {
                combo = 0;
            }
            else
            {
                // Quad clear
                combo++;
            }

            ScoreLabel.Text = "Оноо: " + score.ToString();
            ScoreUpdateTimer.Start();
        }

        private int CheckForCompleteRows()
        {
            
            for (int x = 21; x >= 2; x--)
            {
                // бүх квадратыг тоолго
                for (int y = 0; y <= 9; y++)
                {
                    Control z = grid.GetControlFromPosition(y, x);
                    if (z.BackColor == Color.White)
                    {
                        break;
                    }
                    if (y == 9)
                    {
                        // row Тоолно
                        return x;
                    }
                }
            }
            return -1; // "null"
        }

        // хурд нэмнэ
        private void LevelUp()
        {
            level++;
            LevelLabel.Text = "Level: " + level.ToString();

            // хурдууд
            // Level 1 = 800 ms, level 2 = 716 ms 
            int[] levelSpeed =
            {
                800, 716, 633, 555, 466, 383, 300, 216, 133, 100, 083, 083, 083, 066, 066,
                066, 050, 050, 050, 033, 033, 033, 033, 033, 033, 033, 033, 033, 033, 016
            };

            // 29 level хүрээд хурд өөрчлөгдөхгүй
            if (level <= 29)
            {
                SpeedTimer.Interval = levelSpeed[level];
            }
        }

        // тоглоом дуусгах хэсэг
        private bool CheckGameOver()
        {
            Control[] topRow = { box1, box2, box3, box4, box5, box6, box7, box8, box9, box10 };

            foreach (Control box in topRow)
            {
                if ((box.BackColor != Color.White & box.BackColor != Color.LightGray) & !activePiece.Contains(box))
                {
                    //Тоглоом дуусах
                    return true;
                }
            }

            if (gameOver == true)
            {
                return true;
            }

            return false;
        }

        private void ScoreUpdateTimer_Tick(object sender, EventArgs e)
        {
                ScoreUpdateLabel.Text = "";
                ScoreUpdateTimer.Stop();
        }

        private void TetrisLabel_Click(object sender, EventArgs e)
        {

        }
    }   
}
