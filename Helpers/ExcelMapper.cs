using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Generic.Helpers
{
	public class ExcelMapper
	{
		public static IEnumerable<T> GetRows<T>(string filePath, string sheetName, Dictionary<string, string> mapping = null)
		{
			if (mapping == null)
			{
				mapping = new Dictionary<string, string>();
			}
			if (Path.GetExtension(filePath) == ".xls")
				return GetOldVersionRows<T>(filePath, sheetName, mapping);
			else
				return GetNewVersionRows<T>(filePath, sheetName, mapping);

		}

		private static IEnumerable<T> GetOldVersionRows<T>(string filePath, string sheetName, Dictionary<string, string> mapping)
		{
			List<T> result = new List<T>();
			HSSFWorkbook hssfwb;
			//   using (FileStream file = new FileStream(@"D:\test.xls", FileMode.Open, FileAccess.Read))

			using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
			{
				hssfwb = new HSSFWorkbook(file);
				ISheet sheet = hssfwb.GetSheet("sheet1");
				var columnNames = new Dictionary<int, string>();
				for (int row = 0; row <= sheet.LastRowNum; row++)
				{
					T obj = Activator.CreateInstance<T>();
					var properties = obj.GetType().GetProperties();
					if (sheet.GetRow(row) != null) //null is when the row only contains empty cells 
					{
						for (int cell = 0; cell <= sheet.GetRow(row).LastCellNum; cell++)
						{
							var cellObj = sheet.GetRow(row).GetCell(cell);
							if (cellObj != null)
							{
								string cellValue = cellObj.ToString();

								if (row == 0)
								{
									columnNames.Add(cell, cellValue);
								}
								else
								{
									PropertyInfo property = null;
									if (mapping.ContainsKey(columnNames[cell]))
										property = properties.FirstOrDefault(o => o.Name.ToLower() == mapping[columnNames[cell]].ToLower());
									else property = properties.FirstOrDefault(o => o.Name.ToLower() == columnNames[cell].ToLower());
									if (property != null)
										SetValue(property, cellValue, obj);
								}
							}
						}
						if (row != 0)
							result.Add(obj);
						//sheet.GetRow(row).
						//mailids.Add(sheet.GetRow(row).GetCell(0).ToString());
					}
				}
			}


			//return mailids;
			return result;
		}
		private static IEnumerable<T> GetNewVersionRows<T>(string filePath, string sheetName, Dictionary<string, string> mapping)
		{
			List<T> result = new List<T>();
			using (ExcelPackage pck = new ExcelPackage(new FileInfo(filePath)))
			{
				var ws = pck.Workbook.Worksheets[sheetName];
				var start = ws.Dimension.Start;
				var end = ws.Dimension.End;
				var columnNames = new Dictionary<int, string>();
				for (int row = start.Row; row <= end.Row; row++)
				{ // Row by row...
					T obj = Activator.CreateInstance<T>();
					var properties = obj.GetType().GetProperties();
					for (int col = start.Column; col <= end.Column; col++)
					{ // ... Cell by cell...
						object cellValue = ws.Cells[row, col].Text; // This got me the actual value I needed.
						if (row == start.Row)
						{
							columnNames.Add(col, cellValue.ToString());
						}
						else
						{
							PropertyInfo property = null;
							if (mapping.ContainsKey(columnNames[col]))
								property = properties.FirstOrDefault(o => o.Name.ToLower() == mapping[columnNames[col]].ToLower());
							else property = properties.FirstOrDefault(o => o.Name.ToLower() == columnNames[col].ToLower());
							if (property != null)
								SetValue(property, cellValue, obj);
							//property.SetValue(obj, cellValue);
						}
					}
					if (row != start.Row)
					{
						result.Add(obj);
					}
				}
			}
			return result;
		}

		private static void SetValue<T>(PropertyInfo property, object value, T obj)
		{
			if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				switch (property.PropertyType.GetGenericArguments()[0].Name)
				{
					case "Int32":
						int i = 0;
						if (int.TryParse(value.ToString(), out i))
							property.SetValue(obj, (int?)i);
						break;
					case "Int16":
						short shi = 0;
						if (short.TryParse(value.ToString(), out shi))
							property.SetValue(obj, (short?)shi);
						break;
					case "Boolean":
						bool bi = false;
						if (bool.TryParse(value.ToString(), out bi))
							property.SetValue(obj, (bool?)bi);
						break;
					case "DateTime":
						DateTime dti = DateTime.Now;
						if (DateTime.TryParse(value.ToString(), out dti))
							property.SetValue(obj, (DateTime?)dti);
						break;
					case "Int64":
						long li = 0;
						if (long.TryParse(value.ToString(), out li))
							property.SetValue(obj, (long?)li);
						break;
				}
			}
			else
				if (value != null && !string.IsNullOrEmpty(value.ToString()))
					switch (property.PropertyType.Name)
					{
						case "Int32":
							property.SetValue(obj, int.Parse(value.ToString()));
							break;
						case "Int16": property.SetValue(obj, short.Parse(value.ToString())); break;
						case "Boolean": property.SetValue(obj, bool.Parse(value.ToString())); break;
						case "DateTime": property.SetValue(obj, DateTime.Parse(value.ToString())); break;
						case "Int64": property.SetValue(obj, long.Parse(value.ToString())); break;
						default:
							property.SetValue(obj, value);
							break;
					}
		}
	}
}