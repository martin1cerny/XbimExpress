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
%token	NUMBER	
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
%token  ANDOR
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

%token CONSTANT
%token END_CONSTANT

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
	| constant_definition
	;

constant_definition
	: CONSTANT IDENTIFIER ':' IDENTIFIER ASSIGNMENT IDENTIFIER error END_CONSTANT ';'						{ yyerrok(); }
	;

type_definition 
	: TYPE IDENTIFIER '=' identifier_or_type ';' END_TYPE ';'									{ CreateType($2.strVal, $4, null); }
	| TYPE IDENTIFIER '=' enumerable OF identifier_or_type ';' END_TYPE ';'						{ CreateTypeEnumerable($2.strVal, $4.val as AggregationType, $6, null); }
	| TYPE IDENTIFIER '=' identifier_or_type ';' where_section END_TYPE ';'						{ CreateType($2.strVal, $4, $6.val as List<WhereRule>); }
	| TYPE IDENTIFIER '=' enumerable OF identifier_or_type ';' where_section END_TYPE ';'		{ CreateTypeEnumerable($2.strVal, $4.val as AggregationType, $6, $8.val as List<WhereRule>); }
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
	| ENTITY IDENTIFIER ';' END_ENTITY ';'														{ CreateEntity($2.strVal, new ValueType[]{}); }
	;

identifier_list
	: identifiers								{ $$.val = $1.val; }
	| '(' identifier_list ')'					{ $$.val = $2.val; }
	| ONEOF identifier_list 					{ $$.val = $2.val; }
	| identifier_list ANDOR identifier_list		{ var list = (List<string>)($1.val); list.AddRange($3.val as List<string>); $$.val = list; }
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
	| NUMBER								{ $$.val = Model.PredefinedSimpleTypes.NumberType; }
	| LOGICAL								{ $$.val = Model.PredefinedSimpleTypes.LogicalType; }
	| type '(' INTEGER ')'					{ $$.val = CreateArrayType($1.val as BaseType, $3.intVal); }
	| type '(' INTEGER ')' FIXED			{ $$.val = CreateArrayType($1.val as BaseType, $3.intVal); }
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
	| inheritance_section ';'													{ $$.val = $1.val; $$.tokVal = Tokens.ABSTRACT; $$.boolVal = ($1.tokVal == Tokens.ABSTRACT);}
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
	| enumerable OF identifier_or_type											{ $$.val = CreateEnumerableAttribute($1.val as AggregationType, $3, false); }
	| enumerable OF UNIQUE identifier_or_type									{ $$.val = CreateEnumerableAttribute($1.val as AggregationType, $4, true); }
	| enumerable OF OPTIONAL identifier_or_type									{ $$.val = CreateEnumerableAttribute($1.val as AggregationType, $4, true); }
	| enumerable OF enumerable OF identifier_or_type							{ $$.val = CreateEnumerableOfEnumerableAttribute($1.val as AggregationType, $3.val as AggregationType, $5, false); }
	| enumerable OF UNIQUE enumerable OF identifier_or_type						{ $$.val = CreateEnumerableOfEnumerableAttribute($1.val as AggregationType, $4.val as AggregationType, $6, true); }
	| enumerable OF enumerable OF enumerable OF identifier_or_type				{ $$.val = CreateEnumerableOfEnumerableAttribute($1.val as AggregationType, $3.val as AggregationType, $5.val as AggregationType, $7, false); }
	;

where_section																
	: WHERE where_rules														{ $$.val = $2.val; }
	;

where_rules
	: where_rule															{ $$.val = new List<WhereRule>{ $1.val as WhereRule }; }
	| where_rules where_rule 												{ ($1.val as List<WhereRule>).Add($2.val as WhereRule); $$.val = $1.val; }
	;

where_rule
	: IDENTIFIER ':' error ';'												{ $$.val = CreateWhereRule($1.strVal); yyerrok(); }
	| IDENTIFIER ':' IDENTIFIER ':' comparer ':' '(' SELF ')' ';'			{ $$.val = CreateWhereRule($1.strVal); }
	| IDENTIFIER ':' IDENTIFIER ':' comparer ':' IDENTIFIER ';'				{ $$.val = CreateWhereRule($1.strVal); }
	| IDENTIFIER ':' SELF IN string_array ';'								{ $$.val = CreateWhereRule($1.strVal); }
	| IDENTIFIER ':' SELF comparer number ';'								{ $$.val = CreateWhereRule($1.strVal); }
	| IDENTIFIER ':' SELF comparer IDENTIFIER ';'							{ $$.val = CreateWhereRule($1.strVal); }
	| IDENTIFIER ':' accessor comparer number ';'							{ $$.val = CreateWhereRule($1.strVal); }
	| IDENTIFIER ':' accessor comparer accessor ';'							{ $$.val = CreateWhereRule($1.strVal); }
	| IDENTIFIER ':' accessor comparer IDENTIFIER ';'						{ $$.val = CreateWhereRule($1.strVal); }
	| IDENTIFIER ':' '{' number comparer SELF comparer number '}' ';'		{ $$.val = CreateWhereRule($1.strVal); }
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
	: UNIQUE unique_rules					{ $$.val = $2.val; }
	;

unique_rule
	: IDENTIFIER ':' IDENTIFIER ';'			{ $$.val = CreateUniquenessRule($1.strVal, new [] {$3.strVal} ); }
	| IDENTIFIER ':' identifiers ';'		{ $$.val = CreateUniquenessRule($1.strVal, $3.val as List<string>); }
	| IDENTIFIER ':' error ';'				{ $$.val = CreateUniquenessRule($1.strVal, new string[]{} ); yyerrok();}
	;
	
unique_rules 
	: unique_rule							{ $$.val = new List<UniquenessRule>{ $1.val as UniquenessRule }; }
	| unique_rules unique_rule 				{ ($1.val as List<UniquenessRule>).Add($2.val as UniquenessRule); $$.val = $1.val; }
	;

inverse_section
	: INVERSE inverse_rules														{ $$.val = $2.val; }
	;

inverse_rules
	: inverse_rule																{ $$.val = new List<InverseAttribute>{ $1.val as InverseAttribute }; }
	| inverse_rules inverse_rule 												{ ($1.val as List<InverseAttribute>).Add($2.val as InverseAttribute); $$.val = $1.val; }
	;

inverse_rule
	: IDENTIFIER ':' enumerable OF IDENTIFIER FOR IDENTIFIER ';'				{ $$.val = CreateInverseAtribute($1.strVal, $5.strVal, $7.strVal); }
	| IDENTIFIER ':' IDENTIFIER FOR IDENTIFIER ';'								{ $$.val = CreateInverseAtribute($1.strVal, $3.strVal, $5.strVal); }
	;

derive_section
	: DERIVE derive_rules																		{$$.val = $2.val;}
	;

derive_rules
	: derive_rule																				{ $$.val = new List<DerivedAttribute>{$1.val as DerivedAttribute}; }
	| derive_rules derive_rule 																	{ ($1.val as List<DerivedAttribute>).Add($2.val as DerivedAttribute); $$.val = $1.val;}
	;

derive_rule
	: IDENTIFIER ':' identifier_or_type ASSIGNMENT error ';'												{ $$.val = CreateDerivedAttribute($1.strVal); yyerrok(); }
	| IDENTIFIER ':' error ';'																				{ $$.val = CreateDerivedAttribute($1.strVal); yyerrok(); }
	| IDENTIFIER ':' enumerable  OF identifier_or_type ASSIGNMENT error ';'									{ $$.val = CreateDerivedAttribute($1.strVal); yyerrok(); }
	| IDENTIFIER ':' enumerable  OF enumerable  OF identifier_or_type ASSIGNMENT error ';'					{ $$.val = CreateDerivedAttribute($1.strVal); yyerrok(); }
	| IDENTIFIER ':' enumerable  OF enumerable  OF enumerable  OF identifier_or_type ASSIGNMENT error ';'	{ $$.val = CreateDerivedAttribute($1.strVal); yyerrok(); }
	| accessor ':' identifier_or_type ASSIGNMENT error ';'													{ $$.val = CreateDerivedAttribute($1.val as List<string>); yyerrok(); }
	| accessor ':' error ';'																				{ $$.val = CreateDerivedAttribute($1.val as List<string>); yyerrok(); }
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
	| SUPERTYPE_OF identifier_list								{ $$.val = null; $$.tokVal = Tokens.NON_ABSTRACT;  }
	| ABSTRACT SUPERTYPE_OF identifier_list						{ $$.val = null; $$.tokVal = Tokens.ABSTRACT;  }
	;

function
	: FUNCTION IDENTIFIER error END_FUNCTION ';'					{ yyerrok(); }
	;


rule
	: RULE  IDENTIFIER  FOR identifier_list error END_RULE ';'							{CreateGlobalRule($2.strVal, $4.val as List<string>); yyerrok(); }
	;

accessor
	: IDENTIFIER '.' IDENTIFIER		{ $$.val = new List<string>(){$1.strVal, $3.strVal}; }
	| accessor '.' IDENTIFIER		{ var list = (List<string>)($1.val); list.Add($3.strVal); $$.val = list; }
	| SELF BACKSLASH accessor		{ $$.val = $3.val; }
	;

%%
