using System;
using System.Collections.Generic;
using System.Text;

namespace Hodilka
{   /* Ходилка по пустому пространству, но с вероятностью нахождения монеток и магазина для покупок.
     * Каждый шаг увеличивает счётчик шагов, найденные монетки собираются в кошель.
     * В магазине за 1 монетку можно купить 1 предмет: одежда, еду или оружие
     * w - Вперёд
     * a - Влево
     * d - Вправо
     * s - Назад
     * e - взять/использовать
     * Esc - выйти из игры 
     * q - Опиши что я вижу, не реализовано
     * 
     * 
     * Legend:
     * M - money
     * T - Thief
     * S - Shop
     * x - block
     * E - enemy
     */

    class Program
    {




        // Статические определения
        static int winmsgrow = 19;
        static int status = 20; // номер строки статуса
        static int msgrow = 21; // номер строки сообщения 
        static int warnrow = 22;// номер строки предупреждения
        // Вывод сообщения в строке msgrow
        static void Message(string msg)
        {
            Warning("");
            Console.SetCursorPosition(0, msgrow);
            for (int i = 0; i < 120; i++) Console.Write(' ');
            Console.SetCursorPosition(0, msgrow);
            Console.ForegroundColor = System.ConsoleColor.Green;
            Console.Write(msg);
        }

        //Сообщение в конце игры
        static void WinMessage(string msg)
        {
            Warning("");
            Console.SetCursorPosition(0, winmsgrow);
            for (int i = 0; i < 79; i++) Console.Write(' ');
            Console.SetCursorPosition(0, winmsgrow);
            Console.ForegroundColor = System.ConsoleColor.Yellow;
            Console.Write(msg);
        }
        // Вывод предупреждения в строке warnrow
        static void Warning(string msg)
        {
            Console.SetCursorPosition(0, warnrow);
            for (int i = 0; i < 79; i++) Console.Write(' ');
            Console.SetCursorPosition(0, warnrow);
            Console.ForegroundColor = System.ConsoleColor.Red;
            Console.Write(msg);
        }
        // Вывод статуса в строке status
        static void Status(int score, int eat, int dress, int weapon, int Kill, bool mod)
        {   // стереть строку на экране в позиции вывода
            Console.SetCursorPosition(0, status); for (int i = 0; i < 79; i++) Console.Write(' ');
            Console.SetCursorPosition(0, status);
            Console.ForegroundColor = System.ConsoleColor.White;
            Console.Write($"счёт:{score} ");
            Console.ForegroundColor = System.ConsoleColor.Yellow;
            Console.Write($" еды:{eat} ");
            Console.ForegroundColor = System.ConsoleColor.Cyan;
            Console.Write($" одежды:{dress} ");
            Console.ForegroundColor = System.ConsoleColor.Green;
            Console.Write($" оружия:{weapon} ");

            //Проверка на покупку мода
            if (mod == true) //Если есть, то сообщить, что есть
            {
                Console.ForegroundColor = System.ConsoleColor.DarkCyan;
                Console.Write($" Наличие мода: Есть");
            }
            else if (mod == false)//Если нет, то сообщить, что нет
            {
                Console.ForegroundColor = System.ConsoleColor.DarkCyan;
                Console.Write($" Наличие мода: Отсутствует");
            }

            //Количество убийств за игру
            Console.ForegroundColor = System.ConsoleColor.Red;
            Console.Write($" Убийств:{Kill} ");
        }
        // Вывод раскрашенного символа на цветном заднике, задник по умолчанию черный 
        static void ColorCharacter(char s, ConsoleColor c, ConsoleColor b = System.ConsoleColor.Black)
        {
            Console.ForegroundColor = c;
            Console.BackgroundColor = b;
            Console.Write(s);
            Console.BackgroundColor = System.ConsoleColor.Black;
        }
        static void Main(string[] args)
        {









            // Определим тип кодировки для ввода и вывода через консоль 
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;

            Random rnd = new Random(); // включаем генератор случайных чисел
            ConsoleKeyInfo action;     // обьект нажатой клавиши(действия)
            // Макет карты игрового поля
            // массив строк !! содержимое строк нельзя поменять
            string[] premap = {
                    "░░░░░░░░W░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░",
                    "░░░░░░░░  ░░░░░░░  E     S░░░  ?░░░░      M░░░░░        K░░░",
                    "░   M░░░░ E ░░░░░ ░░░░░░░░░S  ░░░M░░ ░░░░░░    E ░░░░░░░░░░░",
                    "░ ░░░░░░░░░ ░░░░░ ░░          ░░░    ░░     ░░░░░░░░S░░░░░░░",
                    "░ ░    ░░░░ ░░░░░ ░░ ░░░░░░M  ░░░ ░░░░░ ░░░░ ░░░M░░░     ░░░",
                    "░   ░░ ░░░░   M         E░░   ░M   ░░░░   ░░ ░MM M░░ ░░░T░░░",
                    "░░░░░░ ░░░░░░░░░░░░░░░░░M░░   ░░░░ ░░░░░░ T░ ░░░E░M░ ░M░ ░░░",
                    "         M     E    ░░░░░░░   ░░ T   ░░░░░   ░░░T░ ░ ░T░ ░░░",
                    "░░M░░░░░░░░░ ░░░░░░ ░░░   ░   ░░ ░░░░░░░░░░░ ░░T E   ░   T░░",
                    "░░░░░    T     ░░ ░  T  ░░░   ░░ ░░   E      ░░░░░░░ ░░░ ░░░",
                    "░░░  ░░ ░░░░░░ ░░ T ░░░ ░░░   ░░ ░░░░ ░░░░░░ ░░░░░░░ ░░░ ░S░",
                    "░░░ ░░░ ░░░░░░ ░░░░░░░░ ░░░   ░░ E       ░░          ░T    ░",
                    "░T    T ░░░ E        ░░TM░░   ░░░░░░░░░░░░░░░░░░░░░░ ░░░░░░░",
                    "░ ░░░░░░░░░░░░░░░░░░ ░░░░░░   M        M    M            ░░░",
                    "WT░░S                               E                    S░░",
                    "░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░"
            };
            int ymax = premap.Length;    // вычисляем размер макета по высоте
            int xmax = premap[0].Length; // и по ширине
            int y = ymax - 3; int x = 28; // определяем начальную позицию игрока
            // Создаём 2-х мерный массив символов нашей карты с возможностью удаления элементов на карте
            // с размерами макета
            char[,] map = new char[ymax, xmax];
            // Копируем из макета карты содержимое на карту посимвольно
            for (int i = 0; i < ymax; i++)
                for (int j = 0; j < xmax; j++)
                    map[i, j] = premap[i][j];

            int damage = 30; // standart person damage
            int WithGunDamage = 50; // standart gun damage
            int WithModDamage = 80; // gun damage with mod
            double crit = 1.5; // crit
            int enemyDmg = 20; // Урон от существ

            double HP = 100; // ХП Игрока
            double defence = 0.5; // Защита одежды от урона, урон множется на это число

            int map_offset = 3; // смещение поля игры по вертикали
            int weapon_price = 4; // цена в монетах за единицу оружия
            int dress_price = 2; // цена в монетах за одежду
            int mod_price = 3; // цена в монетах за усиление оружия
            int steps = 0;  // счётчик шагов
            int score = 0;  // кошелёк для монеток
            int dress = 0;  // количество купленной одежды
            int food = 0;   // количество купленной еды
            int weapon = 0; // количество купленного оружия
            int mod = 0; // количество купленного усиления
            int kills = 0; // количество убийств
            int TeloportX = 0; // Координаты для телепортации
            int Teloporty = 0; // Координаты для телепортации

            bool have_key = false; // Есть ли ключ
            bool mod_have = false; // Имеется ли мод у человека
            bool noway = false; // флажок "ход невозможен"




            void FoodBuy() // Добавление хп при покупке еды
            {
                if (HP >= 50 & HP + 30 <= 100) { HP += 30; } //если хп больше половины и не будет больше 100, то добавить 30
                if (HP >= 50 & HP + 30 > 100) { HP = 100; } //если хп больше половины и хп + 30 больше 100, то хп = 100
                else { HP += 50; }//если хп меньше половины, то еда восстанавливает 50 хп
                Message($"Теперь у вас " + HP + " ХП");
                Console.ReadKey(true);
            }



            Console.CursorVisible = false; // Убрать курсор

            Console.WriteLine("Бесконечная пошаговая бродилка с поиском монеток и покупкой вещей. Клавишы:\n" +
                    "↑ вперёд, ← влево, → вправо, ↓ назад, пробел взять/использовать, " +
                    "Esc выход /nЧтобы выбраться из лабиринта, вам нужен ключ");

            // считываем с клавиатуры символ очередного хода без отображения символа на экране
            // проверяем символ и итерируем до тех пор пока не нажата клавиша Escape
            while ((action = Console.ReadKey(true)).Key != ConsoleKey.Escape)
            {
                noway = false;
                switch (action.Key)
                {
                    case ConsoleKey.UpArrow:
                        Message("Вперёд");
                        if (y > 0)
                        {

                            if (map[y - 1, x] != '░') { y = y - 1; steps++; }
                            else noway = true;
                        }
                        else noway = true;
                        break;
                    case ConsoleKey.DownArrow:
                        Message("Назад");
                        if (y + 1 < ymax)
                        {
                            if (map[y + 1, x] != '░') { y = y + 1; steps++; }
                            else noway = true;
                        }
                        else noway = true;
                        break;
                    case ConsoleKey.LeftArrow:
                        Message("Влево");
                        if (x > 0)
                        {
                            if (map[y, x - 1] != '░') { x = x - 1; steps++; }
                            else noway = true;
                        }
                        else noway = true;
                        break;
                    case ConsoleKey.RightArrow:
                        Message("Вправо");
                        if (x + 1 < xmax)
                        {
                            if (map[y, x + 1] != '░') { x = x + 1; steps++; }
                            else noway = true;
                        }
                        else noway = true;
                        break;
                    default:
                        noway = true;
                        break;
                }

                if (noway)
                {
                    Warning("Ударились об стену"); noway = false;
                    Console.Beep();
                }
                for (int i = 0; i < ymax; i++)
                {
                    Console.SetCursorPosition(0, i + map_offset);
                    Console.ForegroundColor = System.ConsoleColor.Cyan;
                    for (int j = 0; j < xmax; j++)
                    {
                        switch (map[i, j])
                        {
                            case '░': ColorCharacter('█', System.ConsoleColor.Gray); break;
                            case 'T': ColorCharacter(map[i, j], System.ConsoleColor.Blue); break;
                            case 'S': ColorCharacter(map[i, j], System.ConsoleColor.Green); break;
                            case 'E': ColorCharacter(map[i, j], System.ConsoleColor.Red); break;
                            case 'M': ColorCharacter(map[i, j], System.ConsoleColor.Yellow); break;
                            case '?': ColorCharacter(map[i, j], System.ConsoleColor.Yellow); break;
                            case 'W': ColorCharacter(map[i, j], System.ConsoleColor.DarkYellow); break; //Win
                            case 'K': ColorCharacter(map[i, j], System.ConsoleColor.White); break; //key
                            default: Console.Write(map[i, j]); break;
                        }
                    }
                }
                Status(score, food, dress, weapon, kills, mod_have);
                Console.SetCursorPosition(x, y + map_offset);
                ColorCharacter('☻', System.ConsoleColor.Cyan); // показываем лицо на чорном фоне
                Console.SetCursorPosition(1, 1 + map_offset);

                if (map[y, x] == 'M') // если найдена монета, вы можете её забрать, нажав пробел
                {
                    Console.SetCursorPosition(x, y + map_offset);
                    ColorCharacter('☻', System.ConsoleColor.Cyan, System.ConsoleColor.Yellow); // лицо на жёлтом фоне(на монетке)

                    Message(" Ураа! Вы можете забрать монетку!!! нажмите пробел чтобы забрать");
                    action = Console.ReadKey(true); // вводим клавишу
                    if (action.Key == ConsoleKey.Spacebar) // если нажат пробел
                    {
                        Console.SetCursorPosition(x, y + map_offset);
                        ColorCharacter('☻', System.ConsoleColor.Cyan); // монетку забрали, показываем лицо на чорном фоне

                        Message("Взяли монетку");
                        map[y, x] = ' '; // удаляем эту монетку с карты
                        score++; // монеток на одну больше
                        steps++; // шагов тоже
                    }
                    else
                    {
                        Message("Не взяли монетку");
                        continue; // переходим к следующему циклу
                    }
                }
                else if (map[y, x] == 'E') //Столкновение с существом
                {
                    double EnemyHp = 100; // Создание здоровья существа



                    int flag = 0; //создание флага на котором будет работать цикл битвы с существом

                    Message("Вы встретили существо, у вас " + HP + " HP, хотите выстрелить в него? \ny-Да n-Нет");
                    while (flag < 1)
                    {
                        Message("Вы встретили существо, у вас " + HP + " HP, хотите выстрелить в него? \ny-Да n-Нет");



                        action = Console.ReadKey(); // Получаем выбор игрока
                        switch (action.KeyChar)
                        {
                            case 'y':
                                while (true) // Начало битвы
                                {
                                    int luck = rnd.Next(1, 10); // Создание числа удачи


                                    //Если удача больше семи, при определённых условиях наличия оружия и мода наносится критический урон
                                    if (luck > 7 & weapon == 0) { EnemyHp -= damage * crit; }
                                    else if (luck > 7 & weapon != 0 & mod_have == false) { EnemyHp -= WithGunDamage * crit; }
                                    else if (luck > 7 & weapon != 0 & mod_have == true) { EnemyHp -= WithModDamage * crit; }
                                    //Если удача больше семи, при определённых условиях наличия оружия и мода наносится обычный урон
                                    else if (luck <= 7 & weapon == 0) { EnemyHp -= damage; }
                                    else if (luck <= 7 & weapon != 0 & mod_have == false) { EnemyHp -= WithGunDamage; }
                                    else if (luck <= 7 & weapon != 0 & mod_have == true) { EnemyHp -= WithModDamage; }


                                    // Проверка аличие одежды, если есть, то урон уменьшается
                                    if (dress == 0) { HP -= enemyDmg; }
                                    else if (dress > 0) { HP -= enemyDmg * defence; }



                                    //Проверка на победу/проигрыш
                                    if (HP <= 0)
                                    {

                                        WinMessage("Game Over");
                                        break;
                                    }
                                    else if (EnemyHp <= 0)
                                    {
                                        kills++;
                                        score += 3;
                                        map[y, x] = ' ';
                                        Message("У вас осталось " + HP + " ХП, Вы выиграли");
                                        break;
                                    }


                                    // Если проверка на победу/проигрыш ничего не сделала, битва продолжается или заканчивается
                                    Message("У вас осталось " + HP + " ХП, у противника осталось " + EnemyHp + " ХП. Хотите продолжить? y-Да n-Нет");
                                    action = Console.ReadKey(true); // Получение ответа от игрока
                                    if (action.KeyChar == 'y') { } //да - продолжить игру
                                    else if (action.KeyChar == 'n') { break; } // нет - Закончить битву, игрок уходит
                                }
                                break;


                            case 'n': //нет - уйти от битвы
                                Message("Вы решили не стрелять");
                                flag = 2; break; //флаг заканчивает процесс
                                                 //DamageMath(EnemyHp, luck, damage, WithModDamage, mod_have, crit);
                        }


                        if (HP <= 0)
                        {
                            break;
                        }
                        else if (EnemyHp <= 0)
                        {
                            //Message("У вас осталось " + HP + " ХП, Вы выиграли");
                            break;
                        }
                    } //while over
                    if (HP <= 0) //Если игрок проиграл, закончить игру
                    {
                        break;
                    }
                }
                else if (map[y, x] == 'T') // если гангстер, он забирает все монетки и убегает
                {
                    Console.SetCursorPosition(x, y + map_offset);
                    ColorCharacter('☻', System.ConsoleColor.Cyan, System.ConsoleColor.Blue); // лицо на синем фоне(на гангстере)

                    // звуковой SOS
                    Console.Beep(); Console.Beep(); Console.Beep();
                    System.Threading.Thread.Sleep(500);
                    Console.Beep(800, 500); Console.Beep(800, 500); Console.Beep(800, 500);
                    Console.Beep(); Console.Beep(); Console.Beep();

                    Console.SetCursorPosition(x, y + map_offset);
                    ColorCharacter('☻', System.ConsoleColor.Cyan); // Гангстер убежал, показываем лицо на чорном фоне

                    Message(" Ой! Гангстер забрал монетки и убежал!!!");
                    map[y, x] = ' '; // удаляем гангстера с карты
                    score = 0; // монеток нет
                    steps++; // шагов на один больше
                    action = Console.ReadKey(true); // вводим клавишу
                }
                else if (map[y, x] == 'S') // если найден магазин, то можно за монету купить вещь
                {
                    Message($"Пришли в магазин. у вас {score} монеток !!! ");
                    if (score > 0) // если кошелёк не пуст
                    {
                        Console.SetCursorPosition(x, y + map_offset);
                        ColorCharacter('☻', System.ConsoleColor.Black, System.ConsoleColor.Green);
                        Message($"Можете купить нажав 1-одежда, 2-еда, 3-оружие, 4-усиление для оружия");
                        action = Console.ReadKey(true); // считываем символ определяющий покупку без отображения символа на экране
                        switch (action.KeyChar)
                        {
                            case '1':
                                if (score >= dress_price)
                                {
                                    Message("Купили одежду"); score -= dress_price; dress++;
                                }
                                else Warning("К сожалению у вас не хватает монеток!");
                                break;
                            case '2': Message("Купили еду"); score--; food++; FoodBuy(); break;
                            case '3': //armor_price
                                if (score >= weapon_price)
                                {
                                    Message("Купили оружие"); score -= weapon_price; weapon++;
                                }
                                else Warning("К сожалению у вас не хватает монеток!");
                                break;
                            case '4':
                                if (score >= mod_price)
                                {
                                    Message("Купили Мод"); score -= mod_price; mod++; mod_have = true;
                                }
                                else Warning("К сожалению у вас не хватает монеток!");
                                break;
                            default: Warning("Нет такого предмета!"); break;
                        }
                        Console.ReadKey();
                        Message($"После покупки у вас осталось монеток {score}");
                    }  // если же кошелёк пуст, предупредить
                    else Warning("К сожалениюу у вас нет монеток!");
                }
                else if (map[y, x] == '?') // Встреча с новым персонажем
                {
                    Message("Вы смотрите в тень и видете очертание человека");
                    Console.ReadKey();
                    Message("Незнакомец: Здравствуй путник, как продвигается твой путь? \ny-Всё хорошо n-Всё плохо");

                    while (true)
                    {

                        action = Console.ReadKey(true);
                        switch (action.KeyChar)
                        {
                            case 'y':
                                Message("Незнакомец: Вот и хорошо, держи немного монет на счастье");
                                Console.ReadKey(true);
                                score += 5;
                                Message("Вы получили 5 монет");

                                break;

                            case 'n':
                                Message("Незнакомец: Правда?");
                                Console.ReadKey(true);
                                Message("Незнакомец: В таком случае я могу попробовать тебе помочь, ты не будешь против если я испробую на тебе одно заклинание? \ny-Да n-Нет");

                                action = Console.ReadKey(true); // Получение ответа от игрока
                                switch (action.KeyChar)
                                {
                                    case 'y': // Если игрок согласен, телепортировать в случайное место

                                        while (true) //цикл телепортации
                                        {

                                            //Создание координат
                                            TeloportX = rnd.Next(1, xmax);
                                            Teloporty = rnd.Next(1, ymax);

                                            if (map[Teloporty, TeloportX] == ' ') //Если координаты на пустом блоке, то телепортировать игрока
                                            {
                                                x = TeloportX;// Присвоить старым координатам новые
                                                y = Teloporty;// Присвоить старым координатам новые

                                                //Нарисовать персонажа
                                                Console.SetCursorPosition(x, y + map_offset);
                                                ColorCharacter('☻', System.ConsoleColor.Cyan);

                                                Message("Вы оказались в непонятном месте");
                                                break;
                                            }
                                        }
                                        break;

                                    case 'n': //Если игрок не согласен, то дать 5 монет и закончить диалог
                                        Message("В таком случае возьми эти деньги, я думаю, что они тебе нужнее, прощай");
                                        Console.ReadKey(true);
                                        score += 5;
                                        Message("Вы получили 5 монет");
                                        break;

                                }
                                break;
                            default: Warning("Неправильна кнопка!"); break;



                        }
                        break;
                    }
                }

                else if (map[y, x] == 'K') // Поднять ключ
                {
                    Message("Вы подняли ключ");
                    have_key = true;
                    map[y, x] = ' ';
                    steps++;
                }
                else if (map[y, x] == 'W') //Если ступил на поле с ключём, то выиграл
                {
                    if (have_key == true)
                    {
                        WinMessage("Вы Выиграли!");
                        break;
                    }
                    else
                    {
                        Warning("Вам нужен ключ, чтобы выйти!");
                    }



                }

            }
            Message($"Ты прошёл шагов {steps} шт и заработал монет {score} шт, \n" +
    $"сьел еды {food} шт, приобрёл одежды {dress} шт, оружия {weapon} шт, убил {kills} монстров\n");
            Console.WriteLine("Нажми любую клавишу");
            Console.ReadKey();
        }
    }
}

