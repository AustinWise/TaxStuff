@echo off
setlocal

cd %~dp0

REM Asssume there is a antlr command in the PATH
antlr Expression.g4 -Dlanguage=CSharp -package TaxTest.ExpressionParsing -visitor
