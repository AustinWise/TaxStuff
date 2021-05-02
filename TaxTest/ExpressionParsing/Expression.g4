
grammar Expression;

LPAREN   :  '('   ;
RPAREN   :  ')'   ;
DOT      :  '.'   ;
PLUS     :  '+'   ;
MINUS    :  '-'   ;
TIMES    :  '*'   ;
DIVIDE   :  '/'   ;

INTEGER : ('0'..'9') ('0'..'9')*;

fragment VALID_ID_START
   : ('a' .. 'z') | ('A' .. 'Z')
   ;

fragment VALID_ID_CHAR
   : VALID_ID_START | ('0' .. '9') | '-' | DOT
   ;

IDENTIFIER
   : VALID_ID_START VALID_ID_CHAR*
   ;

WS       :   [ ]+ -> skip ;

identifier
   : IDENTIFIER
   ;

simple
   :  term ((PLUS | MINUS) term)*
   ;
term
   :  unary ((TIMES | DIVIDE) unary)*
   ;
unary
   :  MINUS factor
   |  factor
   ;
factor
    : LPAREN simple RPAREN
    | float_num
    | identifier
    ;
float_num
    : INTEGER (DOT INTEGER)?
    ;

complete_expression
    : simple EOF
    ;