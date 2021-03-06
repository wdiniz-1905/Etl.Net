using OfficeOpenXml;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace Paillave.Etl.ExcelFile.Core
{
    public class ExcelFilePropertySetter
    {
        private readonly CultureInfo _cultureInfo;
        private readonly TypeConverter _typeConverter;
        public PropertyInfo PropertyInfo { get; }
        public string ColumnName { get; }
        public ExcelFilePropertySetter(PropertyInfo propertyInfo, CultureInfo cultureInfo, string columnName)
        {
            this.ColumnName = columnName;
            this.PropertyInfo = propertyInfo;
            this._typeConverter = TypeDescriptor.GetConverter(this.PropertyInfo.PropertyType);
            this._cultureInfo = cultureInfo;
        }

        private object Deserialize(string text)
        {
            //TODO: Better exception handleling here
            if (PropertyInfo.PropertyType == typeof(DateTime))
            {
                return DateTime.ParseExact(text.Trim(), _cultureInfo.DateTimeFormat.LongDatePattern, _cultureInfo);
            }
            if (PropertyInfo.PropertyType == typeof(DateTime?))
            {
                if (string.IsNullOrWhiteSpace(text)) return (DateTime?)null;
                return DateTime.ParseExact(text.Trim(), _cultureInfo.DateTimeFormat.LongDatePattern, _cultureInfo);
            }
            return _typeConverter.ConvertFromString(null, _cultureInfo, text.Trim());
        }

        public object ParsedValue { get; private set; } = null;

        public bool SetValue(ExcelWorksheet excelWorksheet, int row, int column)
        {
            return SetValue(excelWorksheet.GetValue(row, column));
        }
        private bool SetValue(object value)
        {
            if (value != null)
            {
                if (PropertyInfo.PropertyType != value.GetType())
                {
                    var tmpVal = value.ToString();
                    if (string.IsNullOrEmpty(tmpVal)) return false;
                    SetValue(tmpVal);
                }
                else
                {
                    this.ParsedValue = value;
                }
            }
            else
                return false;
            return true;
        }

        private void SetValue(string text)
        {
            this.ParsedValue = Deserialize(text);
        }
    }
}
