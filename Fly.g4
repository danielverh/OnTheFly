grammar Fly;
program: statement+;
statement: (
		varAssignment
		| importStatement
		| methodCall
		| expression
	) ';'
	| forLoop
	| methodDefinition
	| ifElse;
varAssignment: ID '=' expression;
importStatement: 'import' ID (',' ID)*;
ifElse:
	'if' ifExpr = expression '{' (if += statement)* '}' (
		'elif' elifExpr += expression (elifSb+=statementBlock)
	)* ( | 'else' '{' (else += statement)* '}');
forLoop: 'for' (ID 'in' expression) '{' statement* '}';
statementBlock: '{'  statement* '}';
methodDefinition:
	'box ' ID '(' (ID (',' ID)* |) ')' '{' statement* '}';
expression:
	methodCall
	| array
	| NIL
	| INT
	| FLOAT
	| BOOL
	| STRING
	| ID
	| unary='!' right=expression
	| left = expression 'is' right = expression
	| left = expression comp = (EQ | NEQ | SM | LG | SMEQ | LGEQ) right = expression
	| left = expression op = (MUL | DIV) right = expression
	| left = expression op = (ADD | SUB) right = expression;
methodCall:
	ID ('.' ID)* '(' (expression (',' expression)* |) ')';
array: '[' ( | expression (',' expression)*) ']';
NIL: 'nil';
FLOAT: [0-9]* '.' [0-9]+;
INT: [0-9]+;
BOOL: 'true' | 'false';
STRING: '"' ~('\'' | '\\' | '\n' | '\r' | '"')+ '"';
ID: [a-zA-Z_]+ [a-zA-Z0-9_]*;
ADD: '+';
SUB: '-';
MUL: '*';
DIV: '/';

EQ: '==';
NEQ: '!=';
SM: '<';
LG: '>';
SMEQ: '<=';
LGEQ: '>=';

// MOD: '%';

WS: (' ' | '\r' | '\n' | '\t') -> skip;
LINE_COMMENT: '//' ~[\r\n]* -> skip;
COMMENT: '/*' .*? '*/' -> skip;