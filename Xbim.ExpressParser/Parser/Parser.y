%{
	
%}
%namespace Xbim.ExpressParser
%partial   
%parsertype Parser
%output=Parser.cs
%visibility internal

%using System.Linq.Expressions
%using Xbim.ExpressParser.SDAI


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
%token	LOGICAL	
%token  BINARY	
%token	REAL		
%token	ARRAY
%token	LIST
%token  SET
%token  BAG
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
	: SCHEMA IDENTIFIER ';' definitions END_SCHEMA ';'											{ Model.Schema.Name = $2.strVal; Model.Schema.Identification = $2.strVal; Finish();}
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
	: TYPE IDENTIFIER '=' identifier_or_type ';' END_TYPE ';'									{ CreateType($2.strVal); }
	| TYPE IDENTIFIER '=' enumerable OF identifier_or_type ';' END_TYPE ';'						{ CreateType($2.strVal); }
	| TYPE IDENTIFIER '=' identifier_or_type ';' where_section END_TYPE ';'						{ CreateType($2.strVal); }
	| TYPE IDENTIFIER '=' enumerable OF identifier_or_type ';' where_section END_TYPE ';'		{ CreateType($2.strVal); }
	;

enumeration
	: TYPE IDENTIFIER '=' ENUMERATION_OF identifier_list ';' END_TYPE ';'						{ CreateEnumeration($2.strVal, (List<string>)($5.val)); }
	;

select_type 
	: TYPE IDENTIFIER '=' SELECT  identifier_list ';' END_TYPE ';'								{ CreateSelectType($2.strVal, (List<string>)($5.val)); }
	; 

entity
	: ENTITY IDENTIFIER sections END_ENTITY ';'													{ CreateEntity($2.strVal, $3.val as List<ValueType>); }
	| ENTITY IDENTIFIER ';' sections END_ENTITY ';'												{ CreateEntity($2.strVal, $4.val as List<ValueType>); }
	;

identifier_list
	: '(' identifiers ')'				{ $$.val = $2.val; }
	;

identifiers
	: IDENTIFIER						{ $$.val = new List<string>(){$1.strVal}; }
	| identifiers ',' IDENTIFIER		{ var list = (List<string>)($1.val); list.Add($3.strVal); $$.val = list; }
	;

type
	: REAL									{ $$.val = Model.PredefinedSimpleTypes.RealType; }
	| BOOLEAN								{ $$.val = Model.PredefinedSimpleTypes.BooleanType; }
	| BINARY								{ $$.val = Model.PredefinedSimpleTypes.BinaryType; }
	| STRING								{ $$.val = Model.PredefinedSimpleTypes.StringType; }
	| INTEGER								{ $$.val = Model.PredefinedSimpleTypes.IntegerType; }
	| LOGICAL								{ $$.val = Model.PredefinedSimpleTypes.LogicalType; }
	| type '(' INTEGER ')'					{ $$.val = Model.New<ArrayType>(t => {t.ElementType = $1.val as BaseType; t.UpperIndex = $3.intVal;}); }
	| type '(' INTEGER ')' FIXED			{ $$.val = Model.New<ArrayType>(t => {t.ElementType = $1.val as BaseType; t.UpperIndex = $3.intVal;}); }
	;

identifier_or_type							
	: IDENTIFIER							{ $$.strVal = $1.strVal; $$.tokVal = Tokens.IDENTIFIER; }
	| type									{ $$.val = $1.val; $$.tokVal = Tokens.TYPE; }
	;

number
	: INTEGER
	| REAL
	;

sections																		
	: section																	{ $$.val = new List<ValueType>{(ValueType)$1}; }
	| sections section															{ ($1.val as List<ValueType>).Add((ValueType)$2); $$.val = $1.val;}
	;
	
section																			
	: parameter_section															{ $$.val = $1.val; $$.tokVal = Tokens.SELF; }
	| where_section																{ $$.val = $1.val; $$.tokVal = Tokens.WHERE; }
	| unique_section															{ $$.val = $1.val; $$.tokVal = Tokens.UNIQUE; }
	| inverse_section															{ $$.val = $1.val; $$.tokVal = Tokens.INVERSE; }
	| derive_section															{ $$.val = $1.val; $$.tokVal = Tokens.DERIVE; }
	| inheritance_section ';'													{ $$.val = $1.val; $$.tokVal = Tokens.ABSTRACT; }
	;

parameter_section
	: parameter_definition														{ $$.val = new List<ExplicitAttribute>{ $1.val as ExplicitAttribute }; }
	| parameter_section parameter_definition 									{ ($1.val as List<ExplicitAttribute>).Add($2.val as ExplicitAttribute); $$.val = $1.val; }
	;

parameter_definition
	: IDENTIFIER ':' parameter_definition_right ';'								{ $$.val = NameAttribute((ExplicitAttribute)($3.val), $1.strVal, false); }
	| IDENTIFIER ':' OPTIONAL parameter_definition_right ';'					{ $$.val = NameAttribute((ExplicitAttribute)($4.val), $1.strVal, true); }
	;

parameter_definition_right
	: identifier_or_type														{ $$.val = CreateSimpleAttribute($1); }
	| enumerable OF identifier_or_type											{  }
	| enumerable OF UNIQUE identifier_or_type									{  }
	| enumerable OF enumerable OF identifier_or_type							{  }
	| enumerable OF UNIQUE enumerable OF identifier_or_type						{  }
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
	: SET '[' INTEGER ':' optional_integer ']'				{ $$.val = Model.New<SetType>(); }
	| LIST '[' INTEGER ':' optional_integer ']'				{ $$.val = Model.New<ListType>(); }
	| ARRAY '[' INTEGER ':' optional_integer ']'			{ $$.val = Model.New<ArrayType>(); }
	| BAG '[' INTEGER ':' optional_integer ']'				{ $$.val = Model.New<BagType>(); }
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
