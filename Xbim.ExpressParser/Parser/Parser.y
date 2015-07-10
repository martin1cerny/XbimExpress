%{
	
%}
%namespace Xbim.ExpressParser
%partial   
%parsertype Parser
%output=Parser.cs
%visibility internal

%using System.Linq.Expressions


%start schema_definition

%union{
		public string strVal;
		public int intVal;
		public double realVal;
		public bool boolVal;
		public object val;
		public Tokens tokVal;
	  }


%token	INTEGER	
%token	STRING	
%token	BOOLEAN	
%token  BINARY	
%token	REAL		
%token	ARRAY
%token	LIST
%token  SET
%token  IDENTIFIER

%token  OF
%token  FOR

%token  FIXED
%token  UNIQUE
%token  ONEOF
%token  INVERSE
%token  OPTIONAL
	    
%token  SCHEMA
%token  END_SCHEMA
%token  TYPE
%token  END_TYPE
%token  ENUMERATION_OF
%token  SELECT
%token  ENTITY
%token  END_ENTITY
%token  SUBTYPE_OF
%token  SUPERTYPE_OF
%token  ABSTRACT
%token  NON_ABSTRACT
%token  DERIVE

%token  FUNCTION
%token  END_FUNCTION

%token  RULE
%token  END_RULE
	  
	    
%token  WHERE
%token  SELF
%token  IN
%token  AND
%token  OR
%token  XOR
%token  NOT
%token  EXISTS
%token  SIZEOF
%token  QUERY

%token ASSIGNMENT
%token GT
%token LT
%token GTE
%token LTE
%token NEQ
%token BACKSLASH

%%
schema_definition 
	: SCHEMA IDENTIFIER ';' definitions END_SCHEMA ';'											{ Finished(); }
	;

definitions
	: definition
	| definitions definition
	;

definition
	: type_definition
	| enumeration
	| select_type
	| entity
	| function
	| rule
	;

type_definition 
	: TYPE IDENTIFIER '=' identifier_or_type ';' END_TYPE ';'									{ CheckTypeExists($2.strVal); }
	| TYPE IDENTIFIER '=' enumerable OF identifier_or_type ';' END_TYPE ';'						{ CheckTypeExists($2.strVal); }
	| TYPE IDENTIFIER '=' identifier_or_type ';' where_section END_TYPE ';'						{ CheckTypeExists($2.strVal); }
	| TYPE IDENTIFIER '=' enumerable OF identifier_or_type ';' where_section END_TYPE ';'		{ CheckTypeExists($2.strVal); }
	;

enumeration
	: TYPE IDENTIFIER '=' ENUMERATION_OF identifier_list ';' END_TYPE ';'						{ CreateEnumeration($2.strVal, (List<string>)($5.val));  CheckTypeExists($2.strVal); }
	;

select_type 
	: TYPE IDENTIFIER '=' SELECT  identifier_list ';' END_TYPE ';'								{ CreateSelectInterface($2.strVal, (List<string>)($5.val));  CheckTypeExists($2.strVal); }
	; 

entity
	: ENTITY IDENTIFIER sections END_ENTITY ';'													{ CheckTypeExists($2.strVal); }
	| ENTITY IDENTIFIER ';' sections END_ENTITY ';'												{ CheckTypeExists($2.strVal); }
	;

identifier_list
	: '(' identifiers ')'				{ $$.val = $2.val; }
	;

identifiers
	: IDENTIFIER						{ $$.val = new List<string>(){$1.strVal}; }
	| identifiers ',' IDENTIFIER		{ var list = (List<string>)($1.val); list.Add($3.strVal); $$.val = list; }
	;

type
	: REAL
	| BOOLEAN
	| BINARY
	| STRING
	| INTEGER
	| type '(' INTEGER ')'
	| type '(' INTEGER ')' FIXED
	;

identifier_or_type
	: IDENTIFIER
	| type
	;

number
	: INTEGER
	| REAL
	;

sections
	: section
	| sections section
	;
	
section
	: parameter_section
	| where_section
	| unique_section
	| inverse_section
	| derive_section
	| inheritance_section ';'
	;

parameter_section
	: parameter_definition
	| parameter_section parameter_definition 
	;

parameter_definition
	: IDENTIFIER ':' parameter_definition_right ';'
	| IDENTIFIER ':' OPTIONAL parameter_definition_right ';'
	;

parameter_definition_right
	: identifier_or_type
	| enumerable OF identifier_or_type
	| enumerable OF UNIQUE identifier_or_type
	| enumerable OF enumerable OF identifier_or_type
	| enumerable OF UNIQUE enumerable OF identifier_or_type
	;

where_section
	: WHERE where_rules
	;

where_rules
	: where_rule
	| where_rules where_rule 
	;

where_rule
	: IDENTIFIER ':' error ';'												{ yyerrok(); }
	| IDENTIFIER ':' SELF IN string_array ';'
	| IDENTIFIER ':' SELF comparer number ';'
	| IDENTIFIER ':' SELF comparer IDENTIFIER ';'
	| IDENTIFIER ':' accessor comparer number ';'
	| IDENTIFIER ':' accessor comparer IDENTIFIER ';'
	| IDENTIFIER ':' '{' number comparer SELF comparer number '}' ';'
	;

comparer
	: GT
	| LT
	| GTE
	| LTE
	| NEQ
	| '='
	;

string_array
	: '[' strings ']'			{ $$.val = $2.val; }
	;

strings
	: STRING					{ $$.val = new List<string>() { $1.strVal }; }
	| strings ',' STRING		{ var list = (List<string>)($1.val); list.Add($3.strVal); $$.val = list; }
	;

unique_section
	: UNIQUE unique_rules
	;

unique_rule
	: IDENTIFIER ':' IDENTIFIER ';'
	| IDENTIFIER ':' identifiers ';'
	;
	
unique_rules 
	: unique_rule
	| unique_rules unique_rule 
	;

inverse_section
	: INVERSE inverse_rules
	;

inverse_rules
	: inverse_rule
	| inverse_rules inverse_rule 
	;

inverse_rule
	: IDENTIFIER ':' enumerable OF IDENTIFIER FOR IDENTIFIER ';'
	| IDENTIFIER ':' IDENTIFIER FOR IDENTIFIER ';'
	;

derive_section
	: DERIVE derive_rules
	;

derive_rules
	: derive_rule
	| derive_rules derive_rule 
	;

derive_rule
	: IDENTIFIER ':' identifier_or_type ASSIGNMENT error ';'									{ yyerrok(); }
	| IDENTIFIER ':' enumerable  OF identifier_or_type ASSIGNMENT error ';'						{ yyerrok(); }
	| IDENTIFIER ':' enumerable  OF enumerable  OF identifier_or_type ASSIGNMENT error ';'		{ yyerrok(); }
	| accessor ':' identifier_or_type ASSIGNMENT error ';'										{ yyerrok(); }
	;

optional_integer
	: INTEGER
	| IDENTIFIER
	| '?'
	;

enumerable
	: SET '[' INTEGER ':' optional_integer ']'
	| LIST '[' INTEGER ':' optional_integer ']'
	| ARRAY '[' INTEGER ':' optional_integer ']'
	;

inheritance_section
	: inheritance_definition									{$$.val = $1.val; $$.tokVal = $1.tokVal; }
	| inheritance_section inheritance_definition				{$$.val = $1.val ?? $2.val; if ($1.tokVal == Tokens.ABSTRACT || $2.tokVal == Tokens.ABSTRACT) $$.tokVal = Tokens.ABSTRACT; else $$.tokVal = Tokens.NON_ABSTRACT;}
	;

inheritance_definition
	: SUBTYPE_OF identifier_list								{ $$.val = $2.val; $$.tokVal = Tokens.NON_ABSTRACT; }
	| SUPERTYPE_OF '(' ONEOF identifier_list ')'				{ $$.tokVal = Tokens.NON_ABSTRACT;  }
	| ABSTRACT SUPERTYPE_OF '(' ONEOF identifier_list ')'		{ $$.tokVal = Tokens.ABSTRACT;  }
	;

function
	: FUNCTION IDENTIFIER error END_FUNCTION ';'					{ yyerrok(); }
	;


rule
	: RULE  IDENTIFIER  FOR error END_RULE ';'							{ yyerrok(); }
	;

accessor
	: IDENTIFIER '.' IDENTIFIER		{ $$.val = new List<string>(){$1.strVal, $2.strVal}; }
	| accessor '.' IDENTIFIER		{ var list = (List<string>)($1.val); list.Add($3.strVal); $$.val = list; }
	| SELF BACKSLASH accessor		{ $$.val = $3.val; }
	;

%%
