@echo off
setlocal

cd /d %~dp0

xsd /nologo /classes /namespace:TaxStuff.DataImport.OFX MyOfxWrapper.xsd
