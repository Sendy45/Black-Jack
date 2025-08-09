using System;
namespace blackjack
{
    class Program
    {
        //מחלקת קלף, לכל קלף מספר או אות, צורה וערך במשחק
        public class Card
        {
            private int cardnum;
            private int cardshape;
            private int cardvalue;
            public Card(int[,] prev_cards, int side)
            {
                //קלף נבחר בצורה אקראית
                Random r = new Random();
                this.cardnum = r.Next(13);
                this.cardshape = r.Next(4);
                //בדיקה בעזרת פונקציה קיימת שהקלף לא קיים כבר במשחק אחרת יווצר קלף חדש
                while (existing_card(this.cardnum,this.cardshape,prev_cards))
                {
                    this.cardnum = r.Next(13);
                    this.cardshape = r.Next(4);
                }
                //כאשר מגיעים לקלף שלא קיים מוסיפים אותו לקלפים קיימים
                prev_cards = addcard(this.cardnum, this.cardshape, side, prev_cards);
                //נתינת ערך לקלף
                if (this.cardnum < 9)
                    this.cardvalue = this.cardnum + 2;
                else
                {
                    if (this.cardnum == 12)
                    {
                        this.cardvalue = 11;
                    }
                    else
                        this.cardvalue = 10;
                }
            }
            //החזרת מספר הקלף
            public string getcardnum()
            {
                //שימוש במערך כדי להחזיר מספרים מתאימים
                string[] num_conv = new string[13] { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };
                return num_conv[this.cardnum];
            }
            //החזרת צורת הקלף
            public string getcardshape()
            {
                //שימוש במערך כדי להחזיר צורה מתאימה
                string[] shape_conv = new string[4] { "♣", "♥", "♦", "♠" };
                return shape_conv[this.cardshape];
            }
            //החזרת ערך הקלף
            public int getcardvalue()
            {
                return this.cardvalue;
            }
            //קביעת ערך הקלף
            public void setcardvalue(int newvalue)
            {
                this.cardvalue = newvalue;
            } 
        }
        //פעולה לבדיקת קיום של קלף במשחק
        static bool existing_card(int cardnum, int cardshape, int[,]prev_cards)
        {
            //מעבר על רשימת הקלפים הקיימים כדי לבדוק האם קיים הקלף הניתן בהם
            for(int i=0; i<prev_cards.GetLength(0);i++)
            {
                if (prev_cards[i,0] == cardnum && prev_cards[i, 1] == cardshape)
                {
                    return true;
                }
            }
            return false;
        }
        //פעולה להוספת קלף קיים לרשימת הקלפים הקיימים
        static int[,] addcard(int cardnum, int cardshape, int side, int[,] prev_cards)
        {
            int row = 0;
            while (prev_cards[row, 0] != -1)
                row++;
            prev_cards[row, 0] = cardnum;
            prev_cards[row, 1] = cardshape;
            prev_cards[row, 2] = side;
            return prev_cards;
        }
        //פעולה ליצירת קלף חדש
        static Card newcard(int value, int side, int[,] prev_cards, string[,] board)
        {
            Card c = new Card(prev_cards, side);
            //אם הערך הכולל של שחקן גדול מעשר אז ערך קלף האס יהיה 1 ולא 11
            if (c.getcardvalue() == 11 && value > 10)
                c.setcardvalue(1);
            //הדפסת הקלף החדש בלוח
            printcard(board, side, c);
            return c;
        }
        //פעולה להתחלת תור משחק
        static int turn(Card c, int value, int[,] prev_cards, int side, string[,] board)
        {
            //במקרה בו זה תור השחקן
            if (side==1)
            {
                Console.WriteLine("What would you like to do?");
                while (1 > 0)
                {
                    string key = Console.ReadLine();
                    //כדי לסיים משחק עבור השחקן Q
                    if (key == "q" || key == "Q")
                        return 0;
                    //כדי לקחת קלף מהקופה B
                    if (key == "b" || key == "B")
                    {
                        c = newcard(value, side, prev_cards, board);
                        value += c.getcardvalue();
                        return c.getcardvalue();
                    }
                }
            }
            //אם לא תור השחקן אז זה הוא תור המחשב
            //אם הערך הכולל של המחשב קטן מ12 אז יקח קלף כי אין לו מה להפסיד
            if (value<12)
            {
                c = newcard(value, side, prev_cards, board);
                value += c.getcardvalue();
                return c.getcardvalue();
            }
            //אם הערך הכולל של המחשב קטן מ17 וגדול מ12 אז יש סיכוי של 50% שיקח קלף
            if (value<17)
            {
                Random r = new Random();
                if (r.Next(2)==1)
                    return 0;
                c = newcard(value, side, prev_cards, board);
                value += c.getcardvalue();
                return c.getcardvalue();
            }
            //אם לא קרה אף אחד מהדברים האלה יסיים המחשב את משחקו
            return 0;

            
        }
        //פעולה להתחלת משחק
        static void game()
        {
            //יצירת הלוח
            string[,] board = new string[21, 120];
            //ערך ממוספר מסמל ערך כולל לכל צד, ערך תור מסמל ערך בכל תור חדש, מקסימום קלפים מסמל את הכמות המקסימלית של קלפים שניתן לקחת במשחק
            int value1 = 0, value2 = 0, c1_turn_value = 1, c2_turn_value = 1, max_cards = 0, row = 0;
            //יצירת המערך קלפים קודמים
            int[,] prev_cards = new int[52, 3];
            //מילוי הלוח ברווחים 
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                    board[i, j] = " ";
            }
            //מילוי המערך קלפים קודמים ב-1 כדי שלא יהיה בילבול עם אינדקס 0
            for (int i=0;i<prev_cards.GetLength(0);i++)
            {
                for (int j = 0; j < prev_cards.GetLength(1); j++)
                    prev_cards[i,j]=-1;
            }
            //יצירת שני קלפים ראשונים לכל צד
            Card c1 = newcard(value1, 1, prev_cards, board);
            value1 += c1.getcardvalue();
            c1 = newcard(value1, 1, prev_cards, board);
            value1 += c1.getcardvalue();
            Card c2 = newcard(value2, 2, prev_cards, board);
            value2 += c2.getcardvalue();
            c2 = newcard(value2, 2, prev_cards, board);
            value2 += c2.getcardvalue();
            printcard(board, 0, c2);
            printboard(board);
            //מהלך המשחק עד לעצירתו על ידי שני הצדדים
            while (c1_turn_value != 0 || c2_turn_value != 0)
            {
                if(c1_turn_value != 0)
                {
                    c1_turn_value = turn(c1, value1, prev_cards, 1, board);
                    value1 += c1_turn_value;
                    max_cards++;
                    if (max_cards == 8)
                        c1_turn_value = 0;
                }
                if (c2_turn_value != 0)
                {
                    c2_turn_value = turn(c2, value2, prev_cards, 2, board);
                    value2 += c2_turn_value;
                }
                printboard(board);
            }
            //מציאת קלפי היריב והדפסתם מחדש כדי לראות את ערכם בסיום המשחק
            while (prev_cards[row, 0] != -1)
            {
                if (prev_cards[row, 2] == 2)
                {
                    //שימוש במערכים לשם החזרת מספרי וצורות הקלפים הניתנים לפי אינדקס
                    string[] num_conv = new string[13] { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };
                    string[] shape_conv = new string[4] { "♣", "♥", "♦", "♠" };
                    reprintcard(board, num_conv[prev_cards[row, 0]], shape_conv[prev_cards[row, 1]]);
                    row++;
                }
                else
                    row++;
            }
            printboard(board);
            //בדיקת מנצח
            if (value1 <= 21 && value2 <= 21)
            {
                if (value2 > value1)
                    Console.WriteLine("Computer wins");
                else
                {
                    if (value1 > value2)
                        Console.WriteLine("You win");
                    else
                        Console.WriteLine("It's a tie");
                }
            }
            else
            {
                if (value1 > value2)
                    Console.WriteLine("Computer wins");
                else
                {
                    if (value2 > value1)
                        Console.WriteLine("You win");
                }
            }
        }
        //פעולה להדפסת קלף על הלוח
        static void printcard(string[,] board, int side, Card c)
        {
            //s - התחלה, e - סוף, row - שורה, col - עמודה
            int srow=0, erow=0, scol=0, ecol=0;
            string num = " ", shape = " ";
            //במקרה של צד 0 תודפס הקופה
            if (side == 0)
            {
                srow = 8;
                erow = srow + 4;
                scol = 50;
                ecol = scol + 10;
                board[10, 55] = "B";
            }
            else
            {
                int place = 0, a=0;
                //עבור צד 1 יודפסו פני הקלף והוא יודפס למטה, לכן המשנתה הנוסף ונתינת ערך למשתני מספר וצורה
                if (side == 1)
                {
                    a = 14;
                    num = c.getcardnum();
                    shape = c.getcardshape();
                } 
                //מציאת מקום פנוי על הלוח להדפסת קלף
                while (board[0+a,place]!=" ")
                {
                    if (place < 108)
                        place += 12;
                    else
                        break;
                }
                srow = 0+a;
                erow = srow + 6;
                scol = place;
                ecol = place + 10;
            }
            //מילוי הלוח בסימנים עבור ההדפסה
            for (int row = srow; row <= erow; row++)
            {
                board[row, ecol] = "║";
                board[row, scol] = "║";
            }
            for (int col = scol; col <= ecol; col++)
            {
                board[srow, col] = "═";
                board[erow, col] = "═";
            }
            board[srow, ecol] = "╗";
            board[srow, scol] = "╔";
            board[erow, ecol] = "╝";
            board[erow, scol] = "╚";
            board[srow + 3, scol + 5] = shape;
            //המספר היחידי שגודלו גדול ממשבצת אחת הוא 10 לכן בשבילו יש הדפסה מיוחדת
            if (num == "10")
            {
                num = "1";
                board[srow + 5, scol + 9] = "0";
                board[srow + 1, scol + 3] = "0";
            }
            board[srow + 1, scol + 2] = num;
            board[srow + 5, scol + 8] = num;
        }
        //פעולה להדפסת קלף מחדש כדי לחשוף את פניו בסיום המשחק
        static void reprintcard(string[,] board, string num, string shape)
        {
            int place = 5;
            while (board[3, place] != " ")
            {
                if (place < 108)
                    place += 12;
                else
                    break;
            }
            board[3, place] = shape;
            //המספר היחידי שגודלו גדול ממשבצת אחת הוא 10 לכן בשבילו יש הדפסה מיוחדת
            if (num == "10")
            {
                num = "1";
                board[1, place - 2] = "0";
                board[5, place + 4] = "0";
            }
            board[1, place - 3] = num;
            board[5, place + 3] = num;
        }
        //פעולה להדפסת הלוח
        static void printboard(string[,] board)
        {
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                    Console.Write(board[i, j]);
                Console.WriteLine("");
            }
        }
        static void Main(string[] args)
        {
            game();            
        }
    }
}