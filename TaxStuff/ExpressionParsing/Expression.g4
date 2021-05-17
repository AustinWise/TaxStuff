
grammar Expression;

LPAREN   :  '('   ;
RPAREN   :  ')'   ;
LBRACKET :  '['   ;
RBRACKET :  ']'   ;
DOT      :  '.'   ;
PLUS     :  '+'   ;
MINUS    :  '-'   ;
TIMES    :  '*'   ;
DIVIDE   :  '/'   ;
COMMA    :  ','   ;
EQUAL    :  '=='  ;
NEQUAL   :  '!='  ;

INTEGER : ('0'..'9') ('0'..'9')*;

fragment VALID_ID_START
   : ('a' .. 'z') | ('A' .. 'Z')
   ;

fragment VALID_ID_CHAR
   : VALID_ID_START | ('0' .. '9') | '-'
   ;

IDENTIFIER
   : VALID_ID_START VALID_ID_CHAR*
   ;

WS       :   [ ]+ -> skip ;

identifier
   : IDENTIFIER
   ;
simple
   :  plusMinus ((EQUAL | NEQUAL) plusMinus)*
   ;
plusMinus
   :  term ((PLUS | MINUS) term)*
   ;
term
   :  unary ((TIMES | DIVIDE) unary)*
   ;
unary
   :  MINUS factor
   |  factor
   ;
parameter_list
   :  simple (COMMA simple)*
   ;
selector
   : identifier
   | functionInvoke
   | selector DOT identifier
   | selector LBRACKET simple RBRACKET
   ;
functionInvoke
   : identifier LPAREN parameter_list? RPAREN
   ;
factor
   : LPAREN simple RPAREN
   | floatNum
   | selector
   ;
floatNum
   : INTEGER (DOT INTEGER)?
   ;
completeExpression
   : simple EOF
   ;