﻿using System.Globalization;
using ScriptEngine.Machine.Contexts;
using ScriptEngine.HostedScript.Library.ValueTable;
using ScriptEngine.Machine;
using ScriptEngine.HostedScript.Library;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data.Common;
using ScriptEngine.HostedScript.Library.Binary;


namespace OScriptSql
{
    /// <summary>
    /// Содержит результат выполнения запроса. Предназначен для хранения и обработки полученных данных.
    /// </summary>
    [ContextClass("РезультатЗапроса", "QueryResult")]
    public class QueryResult : AutoContext<QueryResult>
    {
        private DbDataReader _reader;

        public QueryResult()
        {
        }


        public QueryResult(DbDataReader reader)
        {
            _reader = reader;
        }

        /// <summary>
        /// Определяет, есть ли в результате записи 
        /// </summary>
        /// <returns>Булево. Истина - нет ни одной записи; Ложь - если есть записи.</returns>
        [ContextMethod("Пустой", "IsEmpty")]
        public bool IsEmpty()
        {
            return !_reader.HasRows;

        }

        /// <summary>
        /// Создает таблицу значений и копирует в нее все записи набора.
        /// </summary>
        /// <returns>ТаблицаЗначений</returns>
        [ContextMethod("Выгрузить", "Unload")]
        public ValueTable Unload()
        {

            ValueTable resultTable = new ValueTable();

            for (int ColIdx = 0; ColIdx < _reader.FieldCount; ColIdx++)
            {
                resultTable.Columns.Add(_reader.GetName(ColIdx));
            }

            foreach (DbDataRecord record in _reader)
            {
                ValueTableRow row = resultTable.Add();

                for (int ColIdx = 0; ColIdx < _reader.FieldCount; ColIdx++)
                {
                    if (record.IsDBNull(ColIdx))
                    {
                        row.Set(ColIdx, ValueFactory.Create());
                        continue;
                    }
                    
                    
                    //Console.WriteLine("queryresult-col-type:" + _reader.GetName(ColIdx) + " = " +  record.GetFieldType(ColIdx).ToString() + "::" + record.GetDataTypeName(ColIdx));

                    

                    if (record.GetFieldType(ColIdx) == typeof(SByte))
                    {
                        row.Set(ColIdx, ValueFactory.Create((int)record.GetValue(ColIdx)));
                    }
                    else if (record.GetFieldType(ColIdx) == typeof(Int32))
                    {
                        row.Set(ColIdx, ValueFactory.Create((int)record.GetValue(ColIdx)));
                    }
                    else if (record.GetFieldType(ColIdx) == typeof(Int64))
                    {
                        row.Set(ColIdx, ValueFactory.Create(record.GetInt64(ColIdx)));
                    }
                    else if (record.GetFieldType(ColIdx) == typeof(Boolean))
                    {
                        row.Set(ColIdx, ValueFactory.Create(record.GetBoolean(ColIdx)));
                    }
                    else if (record.GetFieldType(ColIdx) == typeof(UInt64))
                    {
                        row.Set(ColIdx, ValueFactory.Create(record.GetValue(ColIdx).ToString()));
                    }
                    else if (record.GetFieldType(ColIdx).ToString() == "System.Double")
                    {
                        double val = record.GetDouble(ColIdx);
                        row.Set(ColIdx, ValueFactory.Create(val.ToString()));
                    }
                    else if (record.GetFieldType(ColIdx) == typeof(Single))
                    {
                        float val = record.GetFloat(ColIdx);
                        row.Set(ColIdx, ValueFactory.Create(val.ToString()));
                    }
                    else if (record.GetFieldType(ColIdx) == typeof(Decimal))
                    {
                        row.Set(ColIdx, ValueFactory.Create(record.GetDecimal(ColIdx)));
                    }
                    else if (record.GetFieldType(ColIdx).ToString() == "System.String")
                    {
                        row.Set(ColIdx, ValueFactory.Create(record.GetString(ColIdx)));
                    }
                    else if (record.GetFieldType(ColIdx).ToString() == "System.DateTime")
                    {
                        row.Set(ColIdx, ValueFactory.Create(record.GetDateTime(ColIdx)));
                    }
                    else if (record.GetFieldType(ColIdx).ToString() == "System.Byte[]")
                    {
                        var data = (byte[])record[ColIdx];
                        var newData = new BinaryDataContext(data);
                        row.Set(ColIdx, ValueFactory.Create(newData));
                    }
                    else if (record.GetDataTypeName(ColIdx) == "uniqueidentifier")
                    {
                        row.Set(ColIdx, ValueFactory.Create(record.GetValue(ColIdx).ToString()));
                    }
                    else if (record.GetDataTypeName(ColIdx) == "tinyint")
                    {
                        row.Set(ColIdx, ValueFactory.Create((int)record.GetByte(ColIdx)));
                    }
                    else
                    {
                        row.Set(ColIdx, ValueFactory.Create(record.GetString(ColIdx)));
                    }

                }
            }
            _reader.Close();
            return resultTable;
        }
    }
}
