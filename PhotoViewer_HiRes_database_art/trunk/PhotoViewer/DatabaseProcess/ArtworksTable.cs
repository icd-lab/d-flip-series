using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Microsoft.Xna.Framework;
using System.IO;
using PhotoViewer.PhotoInfo.Tag;

namespace PhotoViewer.Database.Table
{
    class ArtworksTable: TableProcessor
    {
        DBConnect db = new DBConnect();
        public Dictionary<string, PhotoTag> select(List<string> fileName)
        {
            Dictionary<string, PhotoTag> fileTags = new Dictionary<string, PhotoTag>();

            StreamReader reader = new StreamReader("data.xml");
            var d = reader.ReadToEnd();
            return ArtworksTag.FromXml(d);
            


            /////////////////////create table
            //string query = "create table colorInfo ( fileName varchar(45) NOT NULL PRIMARY KEY, f1 float";
            //for (int i = 2; i <= 75; i++)
            //{
            //    query += ", ";
            //    query += "f" + i.ToString() + " float";
            //}
            //query += ", variance float );";
            //if (db.OpenConnection() == true)
            //{
            //    //Create Command
            //    MySqlCommand cmd = new MySqlCommand(query, db.connection);
            //    //Create a data reader and Execute the command
            //    MySqlDataReader dataReader = cmd.ExecuteReader();
            //}
            /////////////////////////


            //if (fileName.Count == 0)
            //    return fileTags;

            string query = "SELECT * FROM artwork1 WHERE file_name = '" + fileName[0] + "'";
            for (int i = 1; i < fileName.Count; i++)
            {
                query += " or file_name = '" + fileName[i] + "'";
            }
            query = query.Replace(@"\", @"\\");
            //Open connection
            if (db.OpenConnection() == true)
            {
                //Create Command
                MySqlCommand cmd = new MySqlCommand(query, db.connection);
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //List<ArtworksTag> tagList = new List<ArtworksTag>();
                //Read the data and store them in the list
                while (dataReader.Read())
                {
                    List<String> tags = new List<String>();
                    int startYear = 0, endYear = 0;
                    //tags.Add(dataReader["fileName"] + "");
                    tags.Add(dataReader["original_title"] + "");
                    tags.Add(dataReader["japanese_title"] + "");
                    tags.Add(dataReader["artist_english"] + "");
                    tags.Add(dataReader["artist_japanese"] + "");
                    tags.Add(dataReader["possession_museum_english"] + "");
                    tags.Add(dataReader["possession_museum_japanese"] + "");
                    tags.Add(dataReader["possession_country_english"] + "");
                    tags.Add(dataReader["possession_country_japanese"] + "");
                    tags.Add(dataReader["periods_1_english"] + "");
                    tags.Add(dataReader["periods_1_japanese"] + "");
                    tags.Add(dataReader["periods_2_english"] + "");
                    tags.Add(dataReader["periods_2_japanese"] + "");
                    tags.Add(dataReader["school_of_painting_english"] + "");
                    tags.Add(dataReader["school_of_painting_japanese"] + "");
                    tags.Add(dataReader["genre_english"] + "");
                    tags.Add(dataReader["genre_japanese"] + "");
                    if (dataReader["date_created_from"] == DBNull.Value)
                        startYear = 0;
                    else
                        startYear = (int)dataReader["date_created_from"];
                    if (dataReader["date_created_to"] == DBNull.Value)
                        endYear = startYear;
                    else
                        endYear = (int)dataReader["date_created_to"];
                    //Console.WriteLine(tags[1]);


                    ArtworksTag log = new ArtworksTag(tags, startYear, endYear);

                    //tagList.Add(log);

                    //log.startYear = dataReader.GetInt32(dataReader.GetOrdinal("startYear"));
                    //log.startYear = dataReader.GetInt32(dataReader.GetOrdinal("endYear"));
                    // read color info
                    /*if (dataReader.GetFloat(dataReader.GetOrdinal("color_h")) != null && dataReader.GetFloat(dataReader.GetOrdinal("color_s")) != null && dataReader.GetFloat(dataReader.GetOrdinal("color_v")) != null && dataReader.GetFloat(dataReader.GetOrdinal("color_variance")) != null)
                    {
                        float h = dataReader.GetFloat(dataReader.GetOrdinal("color_h"));
                        float s = dataReader.GetFloat(dataReader.GetOrdinal("color_s"));
                        float v = dataReader.GetFloat(dataReader.GetOrdinal("color_v"));
                        float variance = dataReader.GetFloat(dataReader.GetOrdinal("variance"));
                        log.feature = new Vector3(h, s, v);
                        log.variance = variance;
                    }*/

                    fileTags[dataReader["file_name"] + ""] = log;

                }

                //close Data Reader
                dataReader.Close();

                //close Connection
                db.CloseConnection();
                var xml = ArtworksTag.ExportXml(fileTags);
                StreamWriter writer = new StreamWriter("data.xml");
                writer.Write(xml);
                writer.Close();
                
                //return list to be displayed
                return fileTags;
            }
            else
            {
                return fileTags;
            }
        }
    }
}
