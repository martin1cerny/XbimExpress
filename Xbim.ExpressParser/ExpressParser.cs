﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xbim.ExpressParser.SDAI;

namespace Xbim.ExpressParser
{
    public class ExpressParser
    {
        private string _data;

        /// <summary>
        /// Check this for errors after you call Parse(). This only contains error messages without any other information.
        /// </summary>
        public IEnumerable<string> Errors { get; private set; }

        /// <summary>
        /// Check this for errors after you call Parse(). This contains error messages 
        /// alongside with location of the error so you can examine the original file.
        /// </summary>
        public IEnumerable<ErrorLocation> ErrorLocations { get; private set; }

        /// <summary>
        /// Instance of the schema definition. This represents an abstract model of the schema. It is usable for
        /// comparison of different schemas or for code generation. Schema is heavily inspired by SDAI schema.
        /// </summary>
        public SchemaModel SchemaInstance { get; private set; }

        /// <summary>
        /// This function starts parsing of the schema input. Function returns true if parser didn't crash. 
        /// It means that it didn't encounter any error or all errors were caught in parser. These errors might
        /// be due to complex schema structure.
        /// </summary>
        /// <param name="schema">Stream containing textual Express schema definition</param>
        /// <returns>True if parser finished regularly, False if it crashed unexpectedly</returns>
        public bool Parse(Stream schema)
        {
            if (schema.CanSeek)
            {
                var reader = new StreamReader(schema);
                _data = reader.ReadToEnd();
                schema.Seek(0, SeekOrigin.Begin);
            }
            
            var scanner = new Scanner(schema);
            return Parse(scanner);
        }

        /// <summary>
        /// This function starts parsing of the schema input. Function returns true if parser didn't crash. 
        /// It means that it didn't encounter any error or all errors were caught in parser. These errors might
        /// be due to complex schema structure.
        /// </summary>
        /// <param name="schemaData">String representation of the Express schema</param>
        /// <returns>True if parser finished regularly, False if it crashed unexpectedly</returns>
        public bool Parse(string schemaData)
        {
            _data = schemaData;
            var scanner = new Scanner();
            scanner.SetSource(schemaData, 0);
            return Parse(scanner);
        }

        private bool Parse(Scanner scanner)
        {
            var parser = new Parser(scanner);
            var result = parser.Parse();

            Errors = scanner.Errors;
            ErrorLocations = scanner.ErrorLocations;
            SchemaInstance = parser.Model;

            //change where rules descriptions to actual content from the input data
            var lines = _data.Split('\n');
            var wrs = parser.Model.Get<WhereRule>();
            foreach (var rule in wrs)
            {
                var positionStr = rule.Description;
                if(String.IsNullOrWhiteSpace(positionStr)) continue;
                var parts = positionStr.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if(parts.Length != 2) throw new Exception("Wrong format.");

                var start = int.Parse(parts[0]);
                var end = int.Parse(parts[1]);

                var description = "";
                for (var i = start-1; i < end; i++)
                {
                    description += lines[i] + "\n";
                }
                description = description.TrimEnd();

                rule.Description = String.Format("{0}:{1}", rule.Label, description);
            }

            //clean for the next processing
            _data = null;

            return result;
        }
    }
}