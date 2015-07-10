%using QUT.Xbim.Gppg;

%namespace Xbim.ExpressParser

%option verbose, summary, caseinsensitive, noPersistBuffer, out:Scanner.cs
%visibility internal

%{
	//all the user code is in ScanerHelper

%}
 
%%

%{
		
%}
/* ************  skip white chars and line comments ************** */
"\t"					{}
" "						{}
[\n]					{} 
[\r]					{} 
[\0]+					{} 
\/\/[^\r\n]*			{}		/*One line comment*/
(\(\*)(.*(\r\n)*)*(\*\))		{}		/*Multiline comment*/


/* ********************** Operators ************************** */

">="	{return (int)Tokens.GTE ;}
"<="	{return (int)Tokens.LTE ;}
">"		{return (int)Tokens.GT ;}
"<"		{return (int)Tokens.LT ;}
"="		{ return '=';}
"<>"	{return (int)Tokens.NEQ ;}
"?"		{  return ('?'); }
":="	{return (int)Tokens.ASSIGNMENT ;}


/* ********************** Separators ************************** */

";"		{  return (';'); }
","		{  return (','); }
":"		{  return (':'); }
"("		{  return ('('); }
")"		{  return (')'); }
"{"		{  return ('{'); }
"}"		{  return ('}'); }
"["		{  return ('['); }
"]"		{  return (']'); }
"."		{  return ('.'); }
"\\"		{  return (int)Tokens.BACKSLASH; }
					 

/* ********************** Keywords ************************** */

"INTEGER"			{ return (int)(Tokens.INTEGER); }
"STRING"			{ return (int)(Tokens.STRING); }
"BOOLEAN"	 		{ return (int)(Tokens.BOOLEAN); }
"BINARY"	 		{ return (int)(Tokens.BINARY); }
"REAL"				{ return (int)(Tokens.REAL); }
"ARRAY"				{ return (int)(Tokens.ARRAY); }
"LIST"				{ return (int)(Tokens.LIST); }
"SET"				{ return (int)(Tokens.SET); }
       
"OF"				{ return (int)(Tokens.OF); }
"FOR"				{ return (int)(Tokens.FOR); }
       
"FIXED"				{ return (int)(Tokens.FIXED); }
"UNIQUE"			{ return (int)(Tokens.UNIQUE); }
"ONEOF"				{ return (int)(Tokens.ONEOF); }
"INVERSE"			{ return (int)(Tokens.INVERSE); }
"OPTIONAL"			{ return (int)(Tokens.OPTIONAL); }
       
"SCHEMA"			{ return (int)(Tokens.SCHEMA); }
"END_SCHEMA"		{ return (int)(Tokens.END_SCHEMA); }
"TYPE"				{ return (int)(Tokens.TYPE); }
"END_TYPE"			{ return (int)(Tokens.END_TYPE); }
"ENUMERATION OF"	{ return (int)(Tokens.ENUMERATION_OF); }
"SELECT"			{ return (int)(Tokens.SELECT); }
"ENTITY"			{ return (int)(Tokens.ENTITY); }
"END_ENTITY"		{ return (int)(Tokens.END_ENTITY); }
"SUBTYPE OF"		{ return (int)(Tokens.SUBTYPE_OF); }
"SUPERTYPE OF"		{ return (int)(Tokens.SUPERTYPE_OF); }
"ABSTRACT"			{ return (int)(Tokens.ABSTRACT); }
"DERIVE"			{ return (int)(Tokens.DERIVE); }
       
"FUNCTION"			{ return (int)(Tokens.FUNCTION); }
"END_FUNCTION"		{ return (int)(Tokens.END_FUNCTION); }
       
"RULE"				{ return (int)(Tokens.RULE); }
"END_RULE"			{ return (int)(Tokens.END_RULE); }
       
       
"WHERE"				{ return (int)(Tokens.WHERE); }
"SELF"				{ return (int)(Tokens.SELF); }
"IN"				{ return (int)(Tokens.IN); }
"AND"				{ return (int)(Tokens.AND); }
"OR"				{ return (int)(Tokens.OR); }
"XOR"				{ return (int)(Tokens.XOR); }
"NOT"				{ return (int)(Tokens.NOT); }
"EXISTS"			{ return (int)(Tokens.EXISTS); }
"SIZEOF"			{ return (int)(Tokens.SIZEOF); }
"QUERY"				{ return (int)(Tokens.QUERY); }





/* ********************     Values        ****************** */
[\-\+]?[0-9]+															{  return (int)SetValue(Tokens.INTEGER); }
[\-\+]?[0-9]*[\.][0-9]*	|
[\-\+\.0-9][\.0-9]+E[\-\+0-9][0-9]*										{ return (int)SetValue(Tokens.REAL); }
[\"]([\n]|[\000\011-\046\050-\176\201-\237\240-\377]|[\047][\047])*[\"]	{ return (int)SetValue(Tokens.STRING); }
[\']([\n]|[\000\011-\046\050-\176\201-\237\240-\377]|[\047][\047])*[\']	{ return (int)SetValue(Tokens.STRING); }
".T." |
".F." |
true |
false					{ return (int)SetValue(Tokens.BOOLEAN); }
[a-zA-Z0-9_]*		        { yylval.strVal = yytext; return (int)(Tokens.IDENTIFIER); }


/* -----------------------  Epilog ------------------- */
%{
	yylloc = new LexLocation(tokLin,tokCol,tokELin,tokECol);
%}
/* --------------------------------------------------- */
%%


