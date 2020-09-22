grammar MegaScrypt;

/* Parser Rules */
program: (statement | block)* ;

statement:
	(declaration | assignment | increment | decrement | invocation | returnStmt | compoundIdentifier) ';' | ifStmt 
;

declaration:	'var' Id ('=' (expression | compoundIdentifier | instantiation))?;
assignment:		compoundIdentifier ('=' | '+=' | '-=' | '*=' | '/=') (expression | instantiation) ;
funcDeclaration:	'function' '(' varList? ')' '{' statement* '}' ;
invocation:		compoundIdentifier '(' paramList? ')' ;
paramList:		expression (',' expression)* ;
varList:		'var' Id (',' 'var' Id)* ;
instantiation:	'{' keyValuePairs '}' ;
keyValuePairs:	keyValuePair (',' keyValuePair)*;
keyValuePair:	Id ':' (expression | instantiation) ;

compoundIdentifier:	Id ('.' Id)* ;
returnStmt:		'return' expression? ;


block:		'{' statement* '}' | statement;
elseStmt:	'else' block;
ifStmt:		'if' '(' expression ')' block ('else if' '(' expression ')' block)* (elseStmt)?;

increment:		compoundIdentifier '++' | '++' compoundIdentifier;
decrement:		compoundIdentifier '--' | '--' compoundIdentifier;

expression:
	Number | Id | 'true' | 'false' | Null | String | invocation | funcDeclaration | compoundIdentifier | increment | decrement |
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
Number:				Digit+ ( ('.')? | ('.' Digit*)? );
String:				'"' (~["\r\n] | '""')* '"';

Whitespace:			[ \t\r\n]+ -> skip;
LineComment:		'//' ~('\r' | '\n' )* -> skip;
BlockComment:		'/*' .*? '*/' -> skip;