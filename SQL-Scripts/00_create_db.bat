echo off

set logfile=create_db.log
set servername=localhost
rem set servername=DIGITALMAN-PC\SQLEXPRESS
set username=sa
set passwd=123user!
set database=Meets

set automode=0

if /I .%1 == .AUTO set automode=1

echo > %logfile%
call :showmsg .
call :showmsg "starting database creation %TIME%"
call :showmsg .

call :docmd "try dropping database" 01_drop_db.sql
call :docmd "try creating database" 02_create_db.sql
call :docmddb "try creating tables" 03_create_table.sql
call :docmddb "try creating primary keys" 04_create_pk.sql
call :docmddb "try creating foreign keys" 05_create_fk.sql
call :docmddb "try creating index" 06_create_index.sql

call :docmddb "try creating function" 07_create_function.sql
rem call :docmddb "try creating procedure" 08_create_procedure.sql

rem call :docmddb "try creating checks" 09_create_check.sql

call :docmddb "try inserting default data" 10_insert_dummy.sql

call :showmsg .
call :showmsg "stopping database creation %TIME%"
call :showmsg .

if %automode% == 1  goto :eof
pause

goto :eof

:docmd
	call :showmsg %1
	osql -S %servername% -U %username% -P %passwd% < %2 >> %logfile%
	echo. >> %logfile%
	echo. >> %logfile%
	goto :eof
:docmddb
	call :showmsg %1
	osql -S %servername% -U %username% -P %passwd% -d %database% < %2 >> %logfile%
	echo. >> %logfile%
	echo. >> %logfile%
	goto :eof
:showmsg
	echo %1
	echo %1 >> %logfile%

:eof