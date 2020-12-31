grammar Fly;

program: statement+;
statement: (
		varAssignment
		| varMultiAssignment
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
varMultiAssignment:
	arrOrVar (COMMA arrOrVar)+ '=' values+=expression (COMMA values+=expression)*;
importStatement: 'import' package (COMMA package)*;
package: ID (|DOT package);
ifElse:
	'if' ifExpr = expression '{' (if += statement)* '}' (
		'elif' elifExpr += expression (elifSb += statementBlock)
	)* ( | 'else' '{' (else += statement)* '}');
forLoop: 'for' (var=ID 'in' expression|expression) '{' statement* '}';
statementBlock: '{' statement* '}';
methodDefinition:
	'box ' name = ID '(' (args += ID (COMMA args += ID)* |) ')' '{' statement* '}';
anonymousMethodDefinition:
	'box ' '(' (args += ID (COMMA args += ID)* |) ')' '{' statement* '}';

arrOrVar:  ID (|'[' index=expression ']');
expression:
	methodCall
	| array
	| NIL
	| INT
	| FLOAT
	| BOOL
	| STRING
	| target=expression DOT callOn=expression
	| ID '[' index=expression ']'
	| ID
	| '(' parenExp = expression ')'
	| unary = ('!'|'-') right = expression
	| left = expression op = (MUL | DIV) right = expression
	| left = expression op = (ADD | SUB | MOD) right = expression
	| left = expression comp = (EQ | NEQ | SM | LG | SMEQ | LGEQ) right = expression
	| varAssignment
	| anonymousMethodDefinition;
methodCall:
	ID '(' (expression (COMMA expression)* |) ')';
array:
	'[' ( | items+=expression (COMMA items+=expression)*) ']' (|'(' size=expression (COMMA addSize=expression) ')')
	| var=ID '[' spliceStart = expression ':' (|spliceEnd = expression) ']';
NIL: 'nil';
FLOAT: [0-9]* DOT [0-9]+;
INT: [0-9]+;
BOOL: 'true' | 'false';
STRING: '"' ~('\'' | '\\' | '\n' | '\r' | '"')+ '"' | '""';
ID: [a-zA-Z_]+ [a-zA-Z0-9_]*;
ADD: '+';
SUB: '-';
MUL: '*';
DIV: '/';
MOD: '%';

COMMA: ',';
DOT: '.';

EQ: '==';
NEQ: '!=';
SM: '<';
LG: '>';
SMEQ: '<=';
LGEQ: '>=';

// MOD: '%';

WS: (' ' | '\r' | '\n' | '\t') -> channel(HIDDEN);
LINE_COMMENT: '//' ~[\r\n]* -> skip;
COMMENT: '/*' .*? '*/' -> skip;