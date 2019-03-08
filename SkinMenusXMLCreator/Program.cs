using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SkinMenusXMLCreator
{
    /// <summary>
    /// The image files have to be organized in folders inside the /Skins folder
    /// Then for each folder, a series of xml layouts will be created
    /// </summary>

    class Program
    {
        private const string FILEPATH = "/Skins";
        private const string OUTPUTPATH = "/XMLOutput";
        private static int curr_id = 0;
        private static int aux_id = 1;

        static void Main(string[] args)
        {
            string[] folders = GetFolderNames();

            foreach(string folderPath in folders)
            {
                List<string> files = GetFileNames(folderPath); //get a list of the file names

                string folder_name = Path.GetFileName(folderPath);

                //for the emojis
                if(folder_name.Equals("1_Emojis"))
                {
                    files = files.OrderBy(q => int.Parse(q.Split('_')[1])).ToList();
                }

                List<List<List<string>>> files_batches = SplitList(SplitList(files)); //split in batches for each page

                string projectDirectory = Environment.CurrentDirectory;
                CreateLayouts(files_batches, projectDirectory + "/../../.." + OUTPUTPATH);
            }
        }

        /// <summary>
        /// Returns the full path for each image folder
        /// </summary>
        /// <returns></returns>
        static string[] GetFolderNames()
        {
            string projectDirectory = Environment.CurrentDirectory;

            return Directory.GetDirectories(projectDirectory + "/../../.." + FILEPATH)
                            .Select(Path.GetFullPath)
                            .ToArray();
        }

        /// <summary>
        /// Gets a list of the .png file names in a directory
        /// </summary>
        /// <returns></returns>
        static List<string> GetFileNames(string path)
        {
            DirectoryInfo d = new DirectoryInfo(path);
            FileInfo[] Files = d.GetFiles("*.png");
            List<string> result = new List<string>();
            foreach(FileInfo file in Files)
                result.Add(file.Name.Split('.')[0]);

            return result;
        }

        /// <summary>
        /// Returns a list with the files in batches of 5
        /// </summary>
        /// <param name="files"></param>
        /// <param name="nSize"></param>
        /// <returns></returns>
        static List<List<string>> SplitList(List<string> files, int nSize = 5)
        {
            var list = new List<List<string>>();

            for(int i = 0; i < files.Count; i += nSize)
            {
                list.Add(files.GetRange(i, Math.Min(nSize, files.Count - i)));
            }

            return list;
        }

        /// <summary>
        /// Returns a list with the previous lists in batches of 4
        /// </summary>
        /// <param name="files"></param>
        /// <param name="nSize"></param>
        /// <returns></returns>
        static List<List<List<string>>> SplitList(List<List<string>> files, int nSize = 4)
        {
            var list = new List<List<List<string>>>();

            for(int i = 0; i < files.Count; i += nSize)
            {
                list.Add(files.GetRange(i, Math.Min(nSize, files.Count - i)));
            }

            return list;
        }

        /// <summary>
        /// Creates the layout xml files
        /// </summary>
        /// <param name="files"></param>
        /// <param name="out_path"></param>
        static void CreateLayouts(List<List<List<string>>> files, string path)
        {
            string aux_path = "";

            //for each page
            foreach(List<List<string>> batch_1 in files)
            {
                aux_path = path + "/fragment_skin_shop_" + ++curr_id + ".xml";

                // Open/create a file to write to.
                using(StreamWriter sw = File.CreateText(aux_path))
                {
                    WriteHeader(sw);

                    //for each row
                    foreach(List<string> batch_2 in batch_1)
                    {
                        //write the buttons layout
                        WriteLinearLayoutHeader(sw, curr_id, aux_id++);

                        //for each file
                        foreach(string s in batch_2)
                        {
                            WriteImageButton(sw, s);
                        }

                        WriteLinearLayoutFooter(sw);

                        //write the labels layout
                        WriteLinearLayoutHeader(sw, curr_id, aux_id++);

                        //for each file
                        foreach(string s in batch_2)
                        {
                            WriteTextView(sw, s);
                        }

                        WriteLinearLayoutFooter(sw);
                    }

                    WriteFooter(sw);
                }
            }
        }

        /// <summary>
        /// Writes the header for the xml file
        /// </summary>
        /// <param name="sw"></param>
        static void WriteHeader(StreamWriter sw)
        {
            sw.WriteLine(@"<?xml version=""1.0"" encoding=""utf-8""?>
<android.support.constraint.ConstraintLayout
    xmlns:android=""http://schemas.android.com/apk/res/android""
    xmlns:app=""http://schemas.android.com/apk/res-auto""
    android:layout_width=""match_parent""
    android:background=""#424242""
    android:layout_height=""match_parent"" >

    <ScrollView
        android:layout_width=""match_parent""
        android:layout_height=""match_parent"" >

        <LinearLayout
            android:layout_width=""match_parent""
            android:layout_height=""wrap_content""
            android:orientation=""vertical"" > ");
        }

        /// <summary>
        /// Writes the footer for the xml file
        /// </summary>
        /// <param name="sw"></param>
        static void WriteFooter(StreamWriter sw)
        {
            sw.WriteLine(@"        </LinearLayout>
    </ScrollView>

</android.support.constraint.ConstraintLayout>");
        }

        /// <summary>
        /// Writes the header for the linear layout
        /// </summary>
        /// <param name="sw"></param>
        static void WriteLinearLayoutHeader(StreamWriter sw, int page_id, int layout_id)
        {
            sw.WriteLine(@"            <LinearLayout
                android:id=""@+id/linearLayoutSkins{0}_{1}""
                android:layout_width=""match_parent""
                android:layout_height=""wrap_content""
                android:layout_marginStart=""8dp""
                android:layout_marginLeft=""8dp""
                android:layout_marginTop=""8dp""
                android:layout_marginEnd=""8dp""
                android:layout_marginRight=""8dp""
                android:gravity=""center""
                android:orientation=""horizontal""
                android:padding=""8dp"" > ", page_id, layout_id);
        }

        /// <summary>
        /// Writes the footer for the linear layout
        /// </summary>
        /// <param name="sw"></param>
        static void WriteLinearLayoutFooter(StreamWriter sw)
        {
            sw.WriteLine(@"            </LinearLayout>");
        }

        /// <summary>
        /// Writes the ImaegButton xml code
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="s"></param>
        static void WriteImageButton(StreamWriter sw, string s)
        {
            sw.WriteLine(@"                <ImageButton
                    android:id=""@+id/{0}_Button""
                    android:layout_width=""50dp""
                    android:layout_height=""50dp""
                    android:layout_weight=""1""
                    android:background=""#424242""
                    android:onClick=""onClickSkin""
                    android:scaleType=""fitCenter""
                    android:src=""@drawable/{1}"" />", s, s);
        }

        /// <summary>
        /// Writes the TextView xml code
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="s"></param>
        static void WriteTextView(StreamWriter sw, string s)
        {
            sw.WriteLine(@"                <TextView
                    android:id=""@+id/{0}_Label""
                    android:layout_width=""0dp""
                    android:layout_height=""wrap_content""
                    android:layout_weight=""1""
                    android:onClick=""onClickSkin""
                    android:text=""80""
                    android:textAlignment=""center"" /> ", s);
        }
    }
}
