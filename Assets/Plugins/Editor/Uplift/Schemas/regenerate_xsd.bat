@echo off
REM Welcome to Windows where the xsd command doesn't accept the *.xsd wildcard
setlocal enabledelayedexpansion enableextensions
dir *.xsd /a:-d /b > search.txt
set LIST=
for /F "tokens=*" %%A in (search.txt) do (
    set LIST=!LIST! %%A
    )
del search.txt
xsd!LIST! /classes /namespace:Uplift.Schemas