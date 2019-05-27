using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

using System.Diagnostics;
public class Hacker : MonoBehaviour
{
   // Start is called before the first frame update
    enum Screen { MainMenu ,LevelInstruction,Level,Win,Pass}; 
    Screen game_state;
    LevelEngine level_engine;
    static string menu = @"
    							   Alien's In Town v2.0

								  _ ,--=--._
								,'      _   `.
								-     _(_)_o  -
							____'    /_  _/]    `____
					-=====::(+):::::::::::::::::(+)::=====-
							(+)."""""""""""""""""""""""""",(+)
								.           ,
								  ` - =  - '

			You need to upload a video before the aliens get you !

			Choose A Wifi Network 

			The more encrypted the network the more faster connection 
			only that it is harder to crack

						Name Of Network 	        Encryption

			Press   1   Directex_5a45 	        WEP       (Lowest Encryption)
			Press   2   Neel-816 	            WPA+TKIP
			Press   3   TP-Link_E3DA 	        WPA+AES
			Press   4   DAMI3 	                WPA2+AES  (Highest Encrpytion)

----------------------------------------------------------------------------------------

";
   public static string level =@"							{0}
		
		Unscramble : {1} :

        Other Commands:
        menu    to go to MainMenu
		pass    to skip this word (your score will remain unchanged)
		scramble if you did not get the word 
		(hint) You can keep pressing Enter to scramble the word

Word: {2} / 5
----------------------------------------------------------------------------------------";
static string level_instructions = @"        You chose Level {0} , Network {1} 
		You will be given 5 scrambled words
		You need to unscramble them to pass this level
		More instructions will be on the level screen

		{2}
		
        Commands:
        menu    to go to MainMenu
		done	to proceed into this level 
		All The Best

----------------------------------------------------------------------------------------

";
static string win =@"			{0} {1} !
			You have taken {2} to complete this level
			
			{3} 
			
			{4}
			Your Score: {5}
----------------------------------------------------------------------------------------
					Hit Enter to return to MainMenu
";
    void Start()
    {
       
        game_state = Screen.MainMenu;
        ShowMainMenu();
    }
    void ShowMainMenu()
    {
        Terminal.ClearScreen();
        Terminal.WriteLine(menu);
        Terminal.Write("Your Choice: ");
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    bool ValidMenu(string input)
    {
       // Debug.Log(input + " Rec valid menu");
        int[] a = new int[] { 1, 2, 3, 4 };
        bool b_ = false;
        try
        {
           
            int s = int.Parse(input);
            foreach (int a_ in a)
            {
                if (a_ == s)
                {
                    b_ = true;
                    break;
                }
              
            }
        }
        catch (System.Exception e)
        {
         //   Debug.Log(e);
        }
        return b_;
    }

    void ShowLevelInstructions()
    {
        Terminal.ClearScreen();
        
        
        Terminal.WriteLine(string.Format(level_instructions,level_engine.level,LevelEngine.networks[level_engine.level-1],LevelEngine.aliens[1]));
        Terminal.Write("->: ");
    }
    void OnUserInput(string input)
    {
        if (input == "menu")
        {
            game_state = Screen.MainMenu;
            ShowMainMenu();

        }
        else
        {
            if (level_engine!=null && level_engine.completed_cnt == 6)
            {
                level_engine.st.Stop();
                game_state = Screen.Win;
                ShowWinScreen();
                level_engine = null;

            }
            else if (game_state == Screen.MainMenu)
            {
                game_state = Screen.LevelInstruction;
                level_engine = new LevelEngine(int.Parse(input));
                ShowLevelInstructions();
            }
            else if (game_state == Screen.Level)
            {
                
                if (input == "scramble")
                {
                    level_engine.ShowLevelData(1);
                }
                else
                {
                    level_engine.ShowLevelData(0);
                }
            }
            else if (game_state == Screen.LevelInstruction)
            {
              game_state = Screen.Level;
              level_engine.ShowLevelData(0);
               
            }
            else if (game_state==Screen.Pass)
            {
                level_engine.ShowLevelData(0);
                game_state = Screen.Level;
            }
            else
            {
                game_state = Screen.MainMenu;
                ShowMainMenu();
            }
            
        }
    }

    void ShowWinScreen()
    {
       
        Terminal.ClearScreen();
        System.TimeSpan ts = level_engine.st.Elapsed;
        string time_taken = string.Format("{0} minutes {1} seconds", ts.Minutes,ts.Seconds);
        string message ="";
        string heading = "";
        if ((level_engine.score > 100) && (ts.Seconds < 600))
        {
            message = "Your video will reach its destination in Time! Well Done";
            heading = "You have succesfully completed level ";
        }
        else
        {
            heading = "You have reached the end of level ";

        }
        
        Terminal.WriteLine(string.Format(win,heading,level_engine.level,time_taken,LevelEngine.aliens[0],message,level_engine.score));
        Terminal.Write(": ");
    }

    public bool CheckInput(string s)
    {
        bool ret = true;
        if (s != "menu")
        {
            switch (game_state)
            {
                case Screen.MainMenu:
                    ret = ValidMenu(s);
                    break;

                case Screen.Level:
                    if (s == "pass")
                    {
                        level_engine.completed_cnt += 1;
                        game_state = Screen.Pass;
                        Terminal.ClearCurrentInputLine();
                        Terminal.Write("The word was: " + LevelEngine.words[level_engine.level - 1, level_engine.curr_word_int]);
                        ret = false;
                    }
                    else
                    {
                        ret = level_engine.ValidateWord(s);
                    }
                    break;

                case Screen.LevelInstruction:
                    if (s != "done")
                    {
                        ret = false;
                    }
                    break;

                case Screen.Pass:
                    ret = true;
                    break;
            }

        }

        return ret;
    }

    public string getError() {
        string ret_error = "";
        switch (game_state)
        {
            case Screen.MainMenu:
                ret_error = "(Enter Valid Level) Your Choice: ";
                break;
            case Screen.Level:
                ret_error = "(Try Again) Word: ";
                break;
            case Screen.LevelInstruction:
                ret_error = "(Invalid Command) ->: ";
                break;
            case Screen.Pass:
                ret_error = "The word was: " + LevelEngine.words[level_engine.level - 1, level_engine.curr_word_int];
                break;
          
               

        }
        return ret_error;
    }
}

public class LevelEngine
{
    public int level;
    public int completed_cnt = 1;
    string template;
    public int score = 0;
    public static string[,] words = new string[4, 5]
    {
        { "birds", "monkey" ,"plenty" ,"side","supply" },
        { "asia", "battery" ,"inactive","xerox", "school"  },
        { "vagility", "macaroni", "amputation", "laboratory" ,"impossible"  },
        { "zephyr", "sabretooth", "dwelling" ,"athena" ,"secretary " }
    };
    public static string[] aliens = new string[5] {

    @"        __.,,------.._
     ,'""   _      _   ""`.
    /.__, ._  -=- _""`    Y
   (.____.-.`      """"`   j
    VvvvvvV`.Y,.    _.,-'       ,     ,     ,
        Y    ||,   '""\         ,/    ,/    ./
        |   ,'  ,     `-..,'_,'/___,'/   ,'/   ,
   ..  ,;,,',-'""\,'  ,  .     '     ' """"' '--,/    .. ..
 ,'. `.`---'     `, /  , Y -=-    ,'   ,   ,. .`-..||_|| ..
ff\\`. `._        /f ,'j j , ,' ,   , f ,  \=\ Y   || ||`||_..
l` \` `.`.""`-..,-' j  /./ /, , / , / /l \   \=\l   || `' || ||...
 `  `   `-._ `-.,-/ ,' /`""/-/-/-/-""'''""`.`.  `'.\--`'--..`'_`' || ,
            ""`-_,',  ,'  f    ,   /      `._    ``._     ,  `-.`'//         ,
          ,-""'' _.,-'    l_,-'_,,'          ""`-._ . ""`. /|     `.'\ ,       |
        ,',.,-'""          \=) ,`-.         ,    `-'._`.V |       \ // .. . /j
        |f\\               `._ )-.""`.     /|         `.| |        `.`-||-\\/
        l` \`                 ""`._   ""`--' j          j' j          `-`---'
         `  `                     ""`_,-','/       ,-'""  /
                                 ,'"",__,-'       /,, ,-'",

@"o
 \_/\o
( Oo)                    \|/
(_=-)  .===O-  ~~Z~A~P~~ -O-
/   \_/U'                /|\
||  |_/
\\  |
{K ||
 | PP
 | ||
 (__\\",

 @"        o o
        | |
       _L_L_
    }\/__-__\/{
    }(|~o.o~|){
    }/ \`-'/ \{
      _/`U'\_
     ( .   . )
    / /     \ \
    \ |  ,  | /
     \|=====|/
      |_.^._|
      | |""| |
      ( ) ( )
      |_| |_|
  _.-' _j L_ '-._
 (___.'     '.___)",

 @"   /\        
   ||        ----
   ||       /(o) \
   ||      (  <   )
   ||       \ -- /
/|_||_|\__(--====--)
  (|_______\======/\ \[[/
   ||        (--) \ \/ /
             /  \  \_-/
            |====|                  
           (  /\  )      
           |  )(  |    
           [  ][  ]
           _||  ||_
          (   ][   )",

          @"      \\ \/ //
       \\/\//
  \\\  ( '' )  ///
   )))  \__/  (((
  (((.'__||__'.)))
   \\  )    (  //
    \\/.'  '.\//
     \/ |,,| \/
        |  |
        //\\
       //  \\
      ||    ||
      ||    ||
   _'_))    ((_'_
"
    };
    public static string[] networks = new string[4] {
        "Directex_5a45" ,"Neel-816" ,"TP-Link_E3DA", "DAMI3 "
    };
    int[] used_words = new int[5];
    public int curr_word_int;
    public string curr_word_str;
    int curr_alien;
    public Stopwatch st;

    public LevelEngine(int level_)
    {
        st = new Stopwatch();
        st.Start();
        level = level_;
    }
   

    public int ChooseWord()
    {
        int t_ = Random.Range(0, 5);
        while(used_words[t_]==1)
        {
            t_ = Random.Range(0, 5);
        }
        used_words[t_] = 1;
        return t_;
    }
    public bool ValidateWord(string input)
    {
        bool ret = false;
        if (input== words[level - 1, curr_word_int])
        {
            score += (level * 20);
            completed_cnt += 1;
            ret = true;
        }
       
        else if(input=="scramble")
        {
            Scramble(curr_word_str);
            ret = true;
        }
        else
        {
            ShowLevelData(1);
       
        }
        return ret;
    }

    public void ShowLevelData(int s)
    {
        Terminal.ClearScreen();
        string alien;
        if (s == 0)
        {
            curr_word_int = ChooseWord();
            int t_= Random.Range(1, 5);
            while (t_ == curr_alien)
            {
                t_ = Random.Range(1, 5);
            }
            curr_alien = t_;
            Scramble(words[level - 1, curr_word_int]);
           alien =aliens[curr_alien];
            
        }
        else
        {
            Scramble(curr_word_str);
            alien = aliens[curr_alien];
         }
        Terminal.WriteLine(string.Format(Hacker.level, alien, curr_word_str, completed_cnt));
        Terminal.Write("Word: ");

    }

    public void Scramble(string s)
    {
        curr_word_str =s.Anagram( words[level-1,curr_word_int]);
    }
}
