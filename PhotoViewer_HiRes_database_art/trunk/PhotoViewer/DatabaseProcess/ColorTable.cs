using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MySql.Data.MySqlClient;
using System.IO;
using PhotoViewer.PhotoInfo;
using PhotoViewer.PhotoInfo.Tag;

namespace PhotoViewer.Database.Table
{
    class ColorTable
    {
        //Vector3[] feature_ = null;
        //double variance_ = 0d;
        DBConnect db = new DBConnect();
        public Dictionary<string, Photo.colorFeature> select(List<string> fileName)
        {
            #region read from xml
            StreamReader reader = new StreamReader("color.xml");
            var d = reader.ReadToEnd();

            return ArtworksTag.FromColorXml(d);
            #endregion

            Dictionary<string, Photo.colorFeature> colorFeatures = new Dictionary<string, Photo.colorFeature>();
            if (fileName.Count == 0)
                return colorFeatures;

            
            string queryColor = "SELECT * FROM colorinfo WHERE fileName = '" + fileName[0] + "'";
            for (int i = 1; i < fileName.Count; i++)
            {
                queryColor += " or fileName = '" + fileName[i] + "'";
            }
            queryColor = queryColor.Replace(@"\", @"\\");
            if (db.OpenConnection() == true)
            {
                MySqlCommand cmdColor = new MySqlCommand(queryColor, db.connection);
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmdColor.ExecuteReader();
                while (dataReader.Read())
                {
                    Photo.colorFeature feature = new Photo.colorFeature();
                    for (int i = 1; i <= 75; i += 3)
                    {
                        Vector3 one = new Vector3();
                        one.X = (float)dataReader["f" + i.ToString()];
                        one.Y = (float)dataReader["f" + (i + 1).ToString()];
                        one.Z = (float)dataReader["f" + (i + 2).ToString()];
                        feature.feature_[i / 3] = one;
                    }
                    feature.variance_ = (float)dataReader["variance"];
                    colorFeatures[dataReader["fileName"].ToString()] = feature;
                }
                dataReader.Close();
                db.CloseConnection();
            }

            #region write to xml
            //var xml = ArtworksTag.ExportColorXml(colorFeatures);
            //StreamWriter writer = new StreamWriter("color.xml");
            //writer.Write(xml);
            //writer.Close();
            #endregion

            return colorFeatures;
        }

        public void insert(Dictionary<string, Photo.colorFeature> colors)
        {
            if (colors.Count == 0)
                return;
            string query = "insert into colorInfo values ";
            foreach (var name in colors.Keys)
            {
                query += " ( '" + name + "', ";
                for (int i = 0; i < 25; i++)
                {
                    query += colors[name].feature_[i].X.ToString() + ", ";
                    query += colors[name].feature_[i].Y.ToString() + ", ";
                    query += colors[name].feature_[i].Z.ToString() + ", ";
                }
                query += colors[name].variance_.ToString() + "),";
            }
            char[] charToTrim = {','};
            query = query.TrimEnd(charToTrim);
            query = query.Replace(@"\", @"\\");
            if (db.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, db.connection);
                //Create a data reader and Execute the command
                cmd.ExecuteReader();
                db.CloseConnection();
            }
        }
    }
}
