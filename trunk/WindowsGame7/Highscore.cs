using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Data;

namespace XNAUfoAttack
{
    /// <summary>
    /// Class representing single highscore with static methods to update and receive highscores from server
    /// </summary>
    public class Highscore
    {
        static String server = "http://vserver1.xgx.at/livesites/ufo-attack/";      // server storing the highscore scripts

        public int score;       // value of highscore
        public String name;     // name of user
        public DateTime date;   // date of highscore

        /// <summary>
        /// Constructor
        /// </summary>
        public Highscore()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of user</param>
        /// <param name="score">Score of user</param>
        public Highscore(String name, int score)
        {
            this.name = name;
            this.score = score;
        }

        /// <summary>
        /// Submits the given score to the server
        /// </summary>
        /// <param name="h">Highscore to submit</param>
        public static void setHighscore(Highscore h)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(server + "submitScore.php?name=" + h.name + "&score=" + h.score);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            }
            catch (Exception e)
            {
            }
        }

        /// <summary>
        /// Loads the scores from the server
        /// </summary>
        /// <returns>The list of the highest scores</returns>
        public static List<Highscore> loadHighscores()
        {
            List<Highscore> scores = new List<Highscore>();

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(server + "getScores.php");

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader input = new StreamReader(response.GetResponseStream());

                DataSet dsTest = new DataSet();
                dsTest.ReadXml(input);

                int varTotCol = dsTest.Tables[0].Columns.Count;
                int varTotRow = dsTest.Tables[0].Rows.Count;

                for (int j = 0; j < varTotRow; j++)
                {
                    Highscore h = new Highscore();

                    h.score = Int32.Parse((String) dsTest.Tables[0].Rows[j][0]);
                    h.name = (String) dsTest.Tables[0].Rows[j][1];

                    String date = (String)dsTest.Tables[0].Rows[j][2];

                    String[] dateArr = date.Split(new char[]{'.', ' ', ':'});

                    h.date = new DateTime(Int32.Parse(dateArr[2]), Int32.Parse(dateArr[1]), Int32.Parse(dateArr[0]), Int32.Parse(dateArr[3]), Int32.Parse(dateArr[4]), 0);

                    scores.Add(h);
                }

            }
            catch (Exception ex)
            {}

            return scores;
        }
    }
}
