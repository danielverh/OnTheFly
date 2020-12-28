grammar Fly;

program: statement+;
statement: (
		varAssignment
		| importStatement
		| methodCall
		| expression
		| returnStmt
		| breakStmt
	) ';'
	| forLoop
	| methodDefinition
	| ifElse;
returnStmt: 'return' expression;
breakStmt: 'break';
varAssignment:
	ID (|'[' index=expression ']') (op = (MUL | DIV |ADD | SUB)|)'=' value=expression;
importStatement: 'import' package (',' package)*;
package: ID (|'.' package);
ifElse:
	'if' ifExpr = expression '{' (if += statement)* '}' (
		'elif' elifExpr += expression (elifSb += statementBlock)
	)* ( | 'else' '{' (else += statement)* '}');
forLoop: 'for' (var=ID 'in' expression|expression) '{' statement* '}';
statementBlock: '{' statement* '}';
methodDefinition:
	'box ' name = ID '(' (args += ID (',' args += ID)* |) ')' '{' statement* '}';
expression:
	methodCall
	| array
	| NIL
	| INT
	| FLOAT
	| BOOL
	| STRING
	| target=expression '.' callOn=expression
	| ID '[' index=expression ']'
	| ID
	| '(' parenExp = expression ')'
	| unary = '!' right = expression
	| left = expression comp = (EQ | NEQ | SM | LG | SMEQ | LGEQ) right = expression
	| left = expression op = (MUL | DIV) right = expression
	| left = expression op = (ADD | SUB | MOD) right = expression;
methodCall:
	ID '(' (expression (',' expression)* |) ')';
array:
	'[' ( | items+=expression (',' items+=expression)*) ']' (|'(' size=expression (',' addSize=expression) ')')
	| var=ID '[' spliceStart = expression ':' (|spliceEnd = expression) ']';
NIL: 'nil';
FLOAT: [0-9]* '.' [0-9]+;
INT: [0-9]+;
BOOL: 'true' | 'false';
STRING: '"' ~('\'' | '\\' | '\n' | '\r' | '"')+ '"' | '""';
ID: [a-zA-Z_]+ [a-zA-Z0-9_]*;
ADD: '+';
SUB: '-';
MUL: '*';
DIV: '/';
MOD: '%';

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