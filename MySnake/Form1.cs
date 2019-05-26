using System;
using System.Media;
using System.Drawing;
using System.Windows.Forms;

namespace MySnake
{
    public partial class Form1 : Form
    {
        private int r1, r3;   //Переменные для рандомного появления фрукта
        private PictureBox fruit; //PictureBox для фрукта
        private PictureBox[] snake = new PictureBox[400]; //Массив для змеейки
        private Label Labelscore; //"Счёт: "
        private int dirX, dirY; //Настправления для движения змейки
        private int _width = 900; //Ширина формы
        private int _height = 800; //Высота формы
        private int _sizeOfSides = 40; //Сторона квадрата клетки
        private int score = 0; //Начальный счёт
        private SoundPlayer SP; //Муз/ плейр

        //Элементы формы Правила 
        private Form fr_rules; //Форма правил
        private Label lbl_Title;
        private TextBox tb_Rules;
        private Label lbl_Title_des;
        private TextBox tb_des;
        private PictureBox pb_rules;
        private Button btn_Rules_ok;

        public Form1() //Инициализация формы
        {
            InitializeComponent();
            this.Width = _width;
            this.Height = _height;
            dirX = 1; //Изначальное движение змейки
            dirY = 0;
            Labelscore = new Label();
            Labelscore.Text = "Счёт: 0"; //Начальный счёт
            Labelscore.Location = new Point(810, 10);
            this.Controls.Add(Labelscore);
            snake[0] = new PictureBox();
            snake[0].Location = new Point(201, 201); //Начальное положение головы змейки
            snake[0].Size = new Size(_sizeOfSides-1, _sizeOfSides-1); //Размер головы по размеру клетки
            snake[0].BackColor = Color.Red;
            this.Controls.Add(snake[0]);
            timer1.Tick += new EventHandler(_update); 
            timer1.Interval = 350;
            timer1.Start();
            fruit = new PictureBox();
            _generationmap(); //Генерация сетки карты
            _generationfruit(); //Генерация фрукта
            fruit.BackColor = Color.Green;
            fr_rules = new Form(); //Создание формы "Правила"
            fr_rules.Text = "Правила";
            //Размер формы
            fr_rules.Width = 400;
            fr_rules.Height = 320;
            fr_rules.StartPosition = FormStartPosition.CenterScreen;
            fruit.Size = new Size(_sizeOfSides, _sizeOfSides); //Размер "фрукта"
            SP = new SoundPlayer(Application.StartupPath + "//untitled.wav");
            SP.PlayLooping();
            this.Click += new EventHandler(правилаToolStripMenuItem_Click);
            this.KeyDown += new KeyEventHandler(OKP);
        }

        //Контекстное меню Правила
        private void правилаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            fr_rules.Show();
            Rules_Form_Fill();
        }

        //Заполнение формы "Правила"
        private void Rules_Form_Fill()
        {
            //Загoловок "Правила"
            lbl_Title = new Label();
            lbl_Title.Location = new Point(100, 5);
            lbl_Title.Text = "Правила игры Змейка";
            fr_rules.Controls.Add(lbl_Title);
            //Тело правил
            tb_Rules = new TextBox();
            tb_Rules.Location = new Point(10, 30);
            tb_Rules.Width = 270;
            tb_Rules.Height = 110;
            tb_Rules.ReadOnly = true;
            tb_Rules.Multiline = true;
            tb_Rules.Text = "В начале игры от змейки есть только голова. Она передвигается с постоянной скоростью. " +
                "Игрок управляет направлением её движения и кормит змейку, направляя её к еде. Змейка удлиняется на одну секцию, когда ест." +
                "Когда змейка выходит за форму она уменьшается для трёх секций и начинает двигаться в обратном направлении, когда кусает сама себя, "+
                "«откусанная» часть пропадает и змейка продолжает движение.";
            fr_rules.Controls.Add(tb_Rules);
            //Заголовок "Описание"
            lbl_Title_des = new Label();
            lbl_Title_des.Location = new Point(100, 150);
            lbl_Title_des.Text = "История игры";
            fr_rules.Controls.Add(lbl_Title_des);
            //Тело Описания
            tb_des = new TextBox();
            tb_des.Multiline = true;
            tb_des.ReadOnly = true;
            tb_des.Location = new Point(10, 180);
            tb_des.Width = 270;
            tb_des.Height = 70;
            tb_des.Text = "Оригинальная «Змейка» (Snake) от Nokia появилась в 1997 году благодаря стараниями " +
                "разработчика Танели Орманто. В том же году компания выпустила первый телефон с этой игрой — Nokia 6110. "+
                "Уже тогда игра была многопользовательской: телефоны общались через ИК-порты.";
            fr_rules.Controls.Add(tb_des);            
            //PictureBox иконка
            pb_rules = new PictureBox();
            pb_rules.Location = new Point(290, 100);
            pb_rules.Image = Properties.Resources.ico1;
            pb_rules.SizeMode = PictureBoxSizeMode.Zoom;
            pb_rules.Width = 90;
            pb_rules.Height = 90;
            fr_rules.Controls.Add(pb_rules);
            //Кнопка OK
            btn_Rules_ok = new Button();
            btn_Rules_ok.Location = new Point(150, 250);
            btn_Rules_ok.Text = "Понял";
            btn_Rules_ok.Click += new EventHandler(btn_Rules_Click_ok);
            fr_rules.Controls.Add(btn_Rules_ok);
        }
        //Кнопка ОК в MessageBox на форме "Правила"
        private void btn_Rules_Click_ok(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            fr_rules.Close();
        }
        //Генерация фрукта
        private void _generationfruit()
        {
            Random r = new Random();
            r1 = r.Next(0, _height - _sizeOfSides);
            int temp1 = r1 % _sizeOfSides;
            r1 -= temp1;
            r3 = r.Next(0, _height - _sizeOfSides);
            int temp3 = r3 % _sizeOfSides;
            r3 -= temp3;
            r1++;
            r3++;
            fruit.Location = new Point(r1, r3);
            this.Controls.Add(fruit);
        }

        private void _checkBorders() //Проверка выхода за рамки формы
        {
            if (snake[0].Location.X < 0)        //Если змейка уходит влево
            {
                for (int _i = 1; _i <= score; _i++)
                {
                    this.Controls.Remove(snake[_i]);
                }
                score = 0;
                Labelscore.Text = "Счёт: " + score;
                dirX = 1;
            }

            if (snake[0].Location.X > _width-140)        //Если змейка уходит вправо
            {
                for (int _i = 1; _i <= score; _i++)
                {
                    this.Controls.Remove(snake[_i]);
                }
                score = 0;
                Labelscore.Text = "Счёт: " + score;
                dirX = -1;
            }

            if (snake[0].Location.Y < 0)        //Если змейка уходит ввехр
            {
                for (int _i = 1; _i <= score; _i++)
                {
                    this.Controls.Remove(snake[_i]);
                }
                score = 0;
                Labelscore.Text = "Счёт: " + score;
                dirY = 1;
            }

            if (snake[0].Location.Y > _height)        //Если змейка уходит вниз
            {
                for (int _i = 1; _i <= score; _i++)
                {
                    this.Controls.Remove(snake[_i]);
                }
                score = 0;
                Labelscore.Text = "Счёт: " + score;
                dirY = -1;
            }
        }
        private void _eatself() //Событие происходит, когда змейка ест сама себя
        {
            for (int _i = 1; _i<score; _i++)
            {
                if(snake[0].Location == snake[_i].Location)
                {
                    for (int _j = _i; _j <= score; _j++)
                        this.Controls.Remove(snake[_j]);
                    score = score - (score - _i + 1);
                }
            }
        }
        private void _eatfruit() //Событие, когда змейка ест фрукт
        {
            if (snake[0].Location.X == r1 && snake[0].Location.Y == r3)
            {
                Labelscore.Text = "Счёт: " + ++score;
                snake[score] = new PictureBox();
                snake[score].Location = new Point(snake[score - 1].Location.X + 40 * dirX, snake[score - 1].Location.Y - 40 * dirY);
                snake[score].Size = new Size(_sizeOfSides-1, _sizeOfSides-1); 
                snake[score].BackColor = Color.Red;
                this.Controls.Add(snake[score]);
                if (score > 0)
                    if (score % 5 == 0) timer1.Interval -= 30;
                _generationfruit();
            }
        }
        private void _generationmap() //Генерация сетки карты
        {
            for (int i = 0; i < _width / _sizeOfSides; i++) //Рисование плоских PictureBox по вертикали
            {
                PictureBox pic = new PictureBox();
                pic.BackColor = Color.Black;
                pic.Location = new Point(0, _sizeOfSides * i);
                pic.Size = new Size(_width - 100, 1);
                this.Controls.Add(pic);
            }
            for (int i = 0; i <= _height / _sizeOfSides; i++) //Рисование плоских PictureBox по горизонтали
            {
                PictureBox pic = new PictureBox();
                pic.BackColor = Color.Black;
                pic.Location = new Point(_sizeOfSides * i, 0);
                pic.Size = new Size(1, _height);
                this.Controls.Add(pic);
            }
        }

        private void _moveSnake() //Движение змейки и наращивание змейки
        {
            for (int i = score; i >= 1; i--)
            {
                snake[i].Location = snake[i - 1].Location;
            }
            snake[0].Location = new Point(snake[0].Location.X + dirX * (_sizeOfSides), snake[0].Location.Y + dirY * (_sizeOfSides));
            _eatself();
        }

        private void _update(object sender, EventArgs eventargs) //Обновление змейки при поедании себя, выходе за края и движения змейки
        {
            _checkBorders();
            _eatfruit();
            _moveSnake();
        }
        private void OKP(object sender, KeyEventArgs e) //onKeyPress для изменения направления движения змейки
        {
            switch (e.KeyCode.ToString())
            {
                case "Right":
                    dirX = 1;
                    dirY = 0;
                    break;
                case "Left":
                    dirX = -1;
                    dirY = 0;
                    break;
                case "Up":
                    dirY = -1;
                    dirX = 0;
                    break;
                case "Down":
                    dirY = 1;
                    dirX = 0;
                    break;
            }
        }
    }
}
