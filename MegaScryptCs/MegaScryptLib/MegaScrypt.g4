grammar MegaScrypt;

/* Parser Rules */
program: (statement | block)* ;

statement:
	(declaration | assignment | increment | decrement | instantiation | invocation) ';' | ifStmt 
;

declaration:	'var' Id ('=' (expression | compoundIdentifier))?;
assignment:		Id ('=' | '+=' | '-=' | '*=' | '/=') (compoundIdentifier | expression) ;
invocation:		Id '(' paramList? ')' ;
paramList:		expression (',' expression)* ;
instantiation:	'var' Id '{' keyValuePairs '}' ;
keyValuePairs:	keyValuePair (',' keyValuePair)*;
keyValuePair:	Id ':' expression ;
compoundIdentifier:	Id ('.' Id)* ;


block:		'{' statement* '}' | statement;
elseStmt:	'else' block;
ifStmt:		'if' '(' expression ')' block ('else if' '(' expression ')' block)* (elseStmt)?;

increment:		Id '++' | '++' Id;
decrement:		Id '--' | '--' Id;

expression:
	Number | Id | 'true' | 'false' | Null | String | invocation
	'(' expression ')' |
	'-' expression	|
	'!' expression	|

	expression ('*' | '/' | '%') expression |
	expression ('+' | '-') expression |

	expression ('<' | '>' | '<=' | '>=') expression |
	expression ('==' | '!=') expression |

	expression '&&' expression |
	expression '||' expression 
;

/* Lexer Rules */
fragment Digit:		[0-9];
fragment Letter:	[a-zA-Z];

True:				'true';
False:				'false';
Null:				'null';
Var:				'var';

Equals:				'=';
Underscore:			'_';
Plus:				'+';
Minus:				'-';
Multiply:			'*';
Divide:				'/';
Mod:				'%';

LeftParenthesis:	'(';
RightParenthesis:	')';
Dot:				'.';
Comma:				',';
Exclamation:		'!';
Colon:				':';

Smaller:			'<';
Greater:			'>';
SmallerEql:			'<=';
GreaterEql:			'>=';
DoubleAmp:			'&&';
DoubleVertical:		'||';
DoubleEquals:		'==';
NotEquals:			'!=';
DoublePlus:			'++';
DoubleMinus:		'--';
PlusEql:			'+=';
MinusEql:			'-=';
TimesEql:			'*=';
DivideEql:			'/=';

Id:					('_' | Letter)('_' | Letter | Digit)* ; 
Number:				Digit+ ('.' Digit*)?;
String:				'"' (~["\r\n] | '""')* '"';

Whitespace:			[ \t\r\n]+ -> skip;
LineComment:		'//' ~('\r' | '\n' )* -> skip;
BlockComment:		'/*' .*? '*/' -> skip;