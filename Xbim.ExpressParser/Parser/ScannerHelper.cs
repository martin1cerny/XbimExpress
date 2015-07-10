using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Xbim.ExpressParser
{
    internal sealed partial class Scanner
    {
        /// <summary>
        /// function used by scanner to set values for value type tokens
        /// </summary>
        /// <param name="type">Value type. If no value type is specified 'STRING' is used by default</param>
        /// <returns>Token set by the function</returns>
        private Tokens SetValue(Tokens type)
        {
            yylval.strVal = yytext;

            switch (type)
            {
                case Tokens.STRING:
                    yylval.strVal = yytext.Trim('\'', '"');
                    return Tokens.STRING;
                case Tokens.INTEGER:
                    if (int.TryParse(yytext, out yylval.intVal))
                        return type;
                    break;
                case Tokens.REAL:
                    try
                    {
                        yylval.realVal = double.Parse(yytext, CultureInfo.InvariantCulture);
                        return type;
                    }
                    catch (Exception)
                    {
                        
                    }
                    break;
                case Tokens.BOOLEAN:
                    if (yytext.ToLower() == ".t.")
                    {
                        yylval.boolVal = true;
                        return type;
                    }
                    if (yytext.ToLower() == ".f.")
                    {
                        yylval.boolVal = false;
                        return type;
                    }
                    if (bool.TryParse(yytext, out yylval.boolVal))
                        return type;
                    break;
                default:
                    yylval.strVal = yytext.Trim('\'', '"');
                    return Tokens.STRING;
            }
            return Tokens.STRING;
        }


        /// <summary>
        /// List of errors
        /// </summary>
        public List<string> Errors = new List<string>();

        /// <summary>
        /// List of error locations
        /// </summary>
        public List<ErrorLocation> ErrorLocations = new List<ErrorLocation>();

        /// <summary>
        /// Overriden yyerror function for error reporting
        /// </summary>
        /// <param name="format">Formated error message</param>
        /// <param name="args">Error arguments</param>
        public override void yyerror(string format, params object[] args)
        {
            if (format.Contains("'{'"))
                format = format.Replace("'{'", "'{{'");
            if (format.Contains("'}'"))
                format = format.Replace("'}'", "'}}'");

            Errors.Add(String.Format(format, args) + String.Format("\n From line {0}, column {1} to line {2}, column {3}", yylloc.StartLine, yylloc.StartColumn, yylloc.EndLine, yylloc.EndColumn));
            ErrorLocations.Add(new ErrorLocation() { 
                StartLine = yylloc.StartLine, 
                EndLine = yylloc.EndLine, 
                StartColumn = yylloc.StartColumn, 
                EndColumn = yylloc.EndColumn,
                Message = String.Format(format, args)
            });

            base.yyerror(format, args);
        }

        public void DiscardLastError()
        {
            Errors.RemoveAt(Errors.Count - 1);
            ErrorLocations.RemoveAt(ErrorLocations.Count - 1);
        }
    }

    public struct ErrorLocation
	{
		public int StartLine;
        public int EndLine;
        public int StartColumn;
        public int EndColumn;
        public string Message;
	}
}
