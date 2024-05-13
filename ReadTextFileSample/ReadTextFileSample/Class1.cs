using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadTextFileSample
{
    public class Class1 {
        public static List<DataValues> GetValues(string path)
        {
            List<DataValues> valuesCollection = new List<DataValues>();
            string strDate = string.Empty;
            bool checkDate = false;
            string[] strArrayData;
            List<string> list = new List<string>();
            using (var f = new StreamReader(path))
            {
                string line = string.Empty;
                string data = string.Empty;
                while ((line = f.ReadLine()) != null)
                {
                    //Read date from first line 
                    if(!checkDate)
                    {
                        if (line.Trim().IndexOf('/') != -1)
                        {
                            int index = line.Trim().IndexOf('/');
                            strDate = $"20{ line.Trim().Substring(index - 2, 8).Replace("/","")}";
                        }
                        checkDate = true;
                    }
                    else
                    {
                        var parts = line.Split(',');
                        if (parts.Length > 2 && !string.IsNullOrEmpty(parts[2].Trim()))
                        {
                            string strCusip = string.Empty;                            
                            if (parts[1].Trim().Length > 1)
                            {
                                strCusip = parts[1].Trim();
                                //data += $"{strDate}|{strCusip}";

                                string[] dataArr = parts[2].Split(";");
                                int indexData = 5;
                                int col= Convert.ToInt16(dataArr[2].Trim());
                                int row = Convert.ToInt16(dataArr[3].Trim());

                                string[,] mutiDimentionArray = new string[col, row];

                                for (int i = 0; i < col; i++)
                                {
                                    for (int j = 0; j < row; j++)
                                    {                                        
                                        mutiDimentionArray[i, j] = dataArr[indexData];
                                        indexData += 2;
                                    }
                                }
                                int arrCol = mutiDimentionArray.GetLength(1); //Get Col
                                int arrRow = mutiDimentionArray.GetLength(0);//Get row
                                
                                List<SubCategoryData> listSubCategoryData = new List<SubCategoryData>();
                                for (int i = 0; i < arrRow; i++)
                                {
                                    var objSubCategoryData = new SubCategoryData();
                                    string subCategory = mutiDimentionArray[i, 0];
                                    decimal percentagevalue = Convert.ToDecimal(mutiDimentionArray[i, 1]);

                                    objSubCategoryData.SubType = subCategory;
                                    objSubCategoryData.Percentage = percentagevalue;

                                    listSubCategoryData.Add(objSubCategoryData);
                                }

                                string strMainCategory = listSubCategoryData.MaxBy(x => x.Percentage).SubType; //Highest percentage value

                                //StringBuilder sb = new StringBuilder();
                                
                                
                                String[] str = list.ToArray();
                                foreach (var subCategory in listSubCategoryData)
                                {
                                    list.Add($"{strDate}|{strCusip}|{strMainCategory}|{subCategory.SubType}|{subCategory.Percentage}");
                                    //sb.Append($"{strDate}|{strCusip}|{strMainCategory}|{subCategory.SubType}|{subCategory.Percentage}");
                                    valuesCollection.Add(new DataValues(strDate, strCusip, strMainCategory, subCategory.SubType, subCategory.Percentage.ToString()));
                                }
                            }
                        }
                        //valuesCollection.Add(new DataValues(Convert.ToDateTime(parts[0]), Convert.ToInt32(parts[1]), parts[2]));
                    }
                }
            }
            strArrayData = list.ToArray();
            GenerateTextFile(strArrayData);
            return valuesCollection;
        }


        private static void GenerateTextFile(string[] strArr)
        {
            string subPath = "OutPutFile.txt"; // Your code goes here

            bool exists = System.IO.Directory.Exists(@"D:\\projects\\console app");

            if (!exists)
                System.IO.Directory.CreateDirectory(@"D:\\projects\\console app");

            string strPath = $"D:\\projects\\console app\\{subPath}";
            if (File.Exists(strPath))
            {
                File.Delete(strPath);
            }
            //FileStream fs = File.Create(strPath, 1024, FileOptions.WriteThrough);
            
            using (FileStream fs = File.Create(strPath, 1024, FileOptions.WriteThrough)){}
            if (File.Exists(strPath))
            {
                //string[] lines = { "First line", "Second line", "Third line" };
                // WriteAllLines creates a file, writes a collection of strings to the file, 
                // and then closes the file.
                System.IO.File.WriteAllLines(strPath, strArr);
            }
        }
    }

    public class SubCategoryData
    {
        public string SubType { get; set; }
        public decimal Percentage { get; set; }
    }

    public class DataValues
    {
        public string EffectiveDate { get; set; }
        public string Cusip { get; set; }
        public string MainPropertyType { get; set; }
        public string SubPropertyType { get; set; }
        public string PercentageOfSecutities { get; set; }
        
        public DataValues(string effectiveDate, string cusip, string mainPropertyType, string subPropertyType, string percentageOfSecutities)
        {
            this.EffectiveDate = effectiveDate;
            this.Cusip = cusip;
            this.MainPropertyType = mainPropertyType;
            this.SubPropertyType = subPropertyType;
            this.PercentageOfSecutities = percentageOfSecutities;
        }
    }
    
}
