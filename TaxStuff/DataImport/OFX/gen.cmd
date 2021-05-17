@echo off
setlocal

cd %~dp0

xsd /nologo /classes /namespace:TaxStuff.DataImport.OFX MyOfxWrapper.xsd
