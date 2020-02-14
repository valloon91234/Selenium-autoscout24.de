using Microsoft.VisualBasic.FileIO;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScout24
{
    class Car
    {
        public String DirectoryName { get; set; }
        public String Make { get; set; }
        public String Model { get; set; }
        public String Type { get; set; }
        public String Month { get; set; }
        public String Year { get; set; }
        public String KW { get; set; }
        public String Fuel { get; set; }
        public String Color { get; set; }
        public String Mileage { get; set; }
        public String PriceInEuro { get; set; }
        public String Description { get; set; }
        public String Pictures { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String Street { get; set; }
        public String ZIP { get; set; }
        public String City { get; set; }
        public String Area { get; set; }
        public String Tel { get; set; }

        public static String CheckNull(String input)
        {
            if (String.IsNullOrWhiteSpace(input)) return null;
            return input;
        }

        public static Car[] ReadData(String excelFilename)
        {
            String ext = Path.GetExtension(excelFilename);
            if (ext == "xlsx") return ReadDataFromXlsx(excelFilename);
            if (ext == "csv") return ReadDataFromCSV(excelFilename);
            return ReadDataFromTxt(excelFilename);
        }

        public static Car[] ReadDataFromXlsx(String excelFilename)
        {
            List<Car> list = new List<Car>();
            using (ExcelPackage excelPackage = new ExcelPackage(new FileInfo(excelFilename)))
            {
                var myWorksheet = excelPackage.Workbook.Worksheets.First(); //select sheet here
                var totalRows = myWorksheet.Dimension.End.Row;
                var totalColumns = myWorksheet.Dimension.End.Column;
                var cells = myWorksheet.Cells;
                for (int rowNum = 2; rowNum <= totalRows; rowNum++) //select starting row here
                {
                    //var row = myWorksheet.Cells[rowNum, 1, rowNum, totalColumns].Select(c => c.Value == null ? string.Empty : c.Value.ToString());
                    String[] fields = new String[totalColumns];
                    for (int colIndex = 0; colIndex < totalColumns; colIndex++)
                    {
                        var value = cells[rowNum, colIndex + 1].Value;
                        if (value != null)
                            fields[colIndex] = value.ToString();
                    }
                    Car car = new Car();
                    int i = 0;
                    car.DirectoryName = CheckNull(fields[i++]);
                    car.Make = CheckNull(fields[i++]);
                    car.Model = CheckNull(fields[i++]);
                    car.Type = CheckNull(fields[i++]);
                    car.Month = CheckNull(fields[i++]);
                    car.Year = CheckNull(fields[i++]);
                    car.KW = CheckNull(fields[i++]);
                    car.Fuel = CheckNull(fields[i++]);
                    car.Color = CheckNull(fields[i++]);
                    car.Mileage = CheckNull(fields[i++]);
                    car.PriceInEuro = CheckNull(fields[i++]);
                    car.Description = CheckNull(fields[i++]);
                    car.Pictures = CheckNull(fields[i++]);
                    car.FirstName = CheckNull(fields[i++]);
                    car.LastName = CheckNull(fields[i++]);
                    car.Street = CheckNull(fields[i++]);
                    String zipAndCity = CheckNull(fields[i++]);
                    String areaAndTel = CheckNull(fields[i++]);
                    car.ZIP = zipAndCity.Split('-')[0].Trim();
                    car.City = zipAndCity.Split('-')[1].Trim();
                    car.Area = areaAndTel.Split('-')[0].Trim();
                    car.Tel = areaAndTel.Split('-')[1].Trim();
                    list.Add(car);
                }
            }
            return list.ToArray();
        }

        public static Car[] ReadDataFromCSV(String csvFilename)
        {
            List<Car> list = new List<Car>();
            using (TextFieldParser parser = new TextFieldParser(csvFilename))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                parser.ReadFields();
                while (!parser.EndOfData)
                {
                    Car car = new Car();
                    string[] fields = parser.ReadFields();
                    int i = 0;
                    car.DirectoryName = CheckNull(fields[i++]);
                    car.Make = CheckNull(fields[i++]);
                    car.Model = CheckNull(fields[i++]);
                    car.Type = CheckNull(fields[i++]);
                    car.Month = CheckNull(fields[i++]);
                    car.Year = CheckNull(fields[i++]);
                    car.KW = CheckNull(fields[i++]);
                    car.Fuel = CheckNull(fields[i++]);
                    car.Color = CheckNull(fields[i++]);
                    car.Mileage = CheckNull(fields[i++]);
                    car.PriceInEuro = CheckNull(fields[i++]);
                    car.Description = CheckNull(fields[i++]);
                    car.Pictures = CheckNull(fields[i++]);
                    car.FirstName = CheckNull(fields[i++]);
                    car.LastName = CheckNull(fields[i++]);
                    car.Street = CheckNull(fields[i++]);
                    String zipAndCity = CheckNull(fields[i++]);
                    String areaAndTel = CheckNull(fields[i++]);
                    car.ZIP = zipAndCity.Split(new char[] { '-', ',' }, StringSplitOptions.RemoveEmptyEntries)[0].Trim();
                    car.City = zipAndCity.Split(new char[] { '-', ',' }, StringSplitOptions.RemoveEmptyEntries)[1].Trim();
                    car.Area = areaAndTel.Split(new char[] { '-', ',' }, StringSplitOptions.RemoveEmptyEntries)[0].Trim();
                    car.Tel = areaAndTel.Split(new char[] { '-', ',' }, StringSplitOptions.RemoveEmptyEntries)[1].Trim();
                    list.Add(car);
                }
            }
            return list.ToArray();
        }

        public static Car[] ReadDataFromTxt(String excelFilename)
        {
            List<Car> list = new List<Car>();
            String[] lines = File.ReadAllLines(excelFilename, Encoding.UTF8);
            int lineCount = lines.Length;
            String name = null, value = null;
            Car car = null;
            for (int i = 0; i < lineCount; i++)
            {
                String line = lines[i];
                if (line.StartsWith("#") || line.StartsWith("//")) continue;
                try
                {
                    String[] array = line.Split(new Char[] { '=' }, 2);
                    if (name != null && value != null)
                    {
                        int quoteCount = line.Split('\"').Length;
                        if (quoteCount > 1)
                        {
                            value += "\r\n" + line.Replace("\"", "");
                        }
                        else
                        {
                            value += "\r\n" + line;
                            continue;
                        }
                    }
                    else if (String.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }
                    else
                    {
                        name = array[0].Trim().ToLower();
                        value = array.Length > 1 ? array[1].Trim() : null;
                        if (String.IsNullOrWhiteSpace(value)) value = null;
                        if (value != null)
                        {
                            int quoteCount = value.Split('\"').Length;
                            if (quoteCount == 2)
                            {
                                value = value.Replace("\"", "");
                                continue;
                            }
                            else if (quoteCount == 3)
                            {
                                value = value.Replace("\"", "");
                            }
                        }
                    }
                    if (value != null) value = value.Trim();
                    if (name == "folder")
                    {
                        car = new Car();
                        car.DirectoryName = value;
                    }
                    else if (name == "make")
                    {
                        car.Make = value;
                    }
                    else if (name == "model")
                    {
                        car.Model = value;
                    }
                    else if (name == "type")
                    {
                        car.Type = value;
                    }
                    else if (name == "month")
                    {
                        car.Month = value;
                    }
                    else if (name == "year")
                    {
                        car.Year = value;
                    }
                    else if (name == "kw")
                    {
                        car.KW = value;
                    }
                    else if (name == "fuel")
                    {
                        car.Fuel = value;
                    }
                    else if (name == "color")
                    {
                        car.Color = value;
                    }
                    else if (name == "mileage")
                    {
                        car.Mileage = value;
                    }
                    else if (name == "price")
                    {
                        car.PriceInEuro = value;
                    }
                    else if (name == "description")
                    {
                        car.Description = value;
                    }
                    else if (name == "pictures")
                    {
                        car.Pictures = value;
                    }
                    else if (name == "firstname")
                    {
                        car.FirstName = value;
                    }
                    else if (name == "lastname")
                    {
                        car.LastName = value;
                    }
                    else if (name == "street")
                    {
                        car.Street = value;
                    }
                    else if (name == "zip")
                    {
                        car.ZIP = value;
                    }
                    else if (name == "city")
                    {
                        car.City = value;
                    }
                    else if (name == "area")
                    {
                        car.Area = value;
                    }
                    else if (name == "tel")
                    {
                        car.Tel = value;
                    }
                    else if (name == "end")
                    {
                        if (car != null) list.Add(car);
                    }
                    else
                    {
                        Scraper.Print($"\tUnknown KEY : {name} on line {i + 1} = {line}");
                    }
                    name = null; value = null;
                }
                catch
                {
                    Scraper.Print($"\tError on line {i + 1} = {line}");
                }
            }
            return list.ToArray();
        }

    }
}
