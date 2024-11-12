@echo off

for /d /r %%i in (obj) do (
    if exist "%%i" (
        echo Deleting folder: "%%i"
        rmdir /s /q "%%i"
    )
)
for /d /r %%i in (bin) do (
    if exist "%%i" (
        echo Deleting folder: "%%i"
        rmdir /s /q "%%i"
    )
)
